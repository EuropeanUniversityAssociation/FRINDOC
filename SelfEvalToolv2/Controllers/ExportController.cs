using HtmlAgilityPack;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SelfEvalToolv2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SelfEvalToolv2.Controllers
{
    public class ExportController : Controller
    {
        private EvalDataContext db = new EvalDataContext();
        private UsersContext dbu = new UsersContext();

        //
        // GET: /Export/

        public ActionResult Excel(string id)
        {
            //// very basic security
            //if (User.Identity.Name.ToLower() != "eua")
            //    return new HttpNotFoundResult();

            var fileDownloadName = "allResults.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var baseInstitutions = (from u in db.Users
                                    join b in db.Benchmarks on u.UserName equals b.User
                                    where SqlFunctions.StringConvert((double)b.BenchmarkID).Trim() == id
                                    select u);
            
            // add all subaccounts to institutions
            var institutions = ((from u in db.Users
                                 where baseInstitutions.Any(i => i.UserName == u.ParentUserName)
                                 where u.IsSubAccount == true
                                 select u).ToList());
            institutions = institutions.Union(baseInstitutions.ToList()).ToList();

            ExcelPackage pck = new ExcelPackage();
            foreach (var institute in institutions)
            {
                var data = new List<ExcelExportRecord>();
                string userDisplayName = string.IsNullOrWhiteSpace(institute.DisplayName) ? institute.UserName : institute.DisplayName + " (" + institute.UserName + ")";

                var b = db.Benchmarks.Where(x => x.User == institute.UserName).FirstOrDefault();
                // take the parent benchmark if this is a subaccount
                if(institute.IsSubAccount.GetValueOrDefault())
                    b = db.Benchmarks.Where(x => x.User == institute.ParentUserName).FirstOrDefault();

                foreach (var s in db.Surveys.ToList())
                {
                    var categories = db.Categories.Include("ChildCategories")
                                                  .Include("Questions")
                                                  .Include("Questions.Answers")
                                                  .Include("ChildCategories.Questions")
                                                  .Include("ChildCategories.Questions.Answers")
                                                  .Where(c => c.SurveyID == s.SurveyID).OrderBy(c => c.Order).ToList();
                    foreach (var c in categories)
                    {
                        processCategory(b, s, c, db, institute, ref data);
                        if (c.ChildCategories != null)
                        {
                            foreach (var cc in c.ChildCategories)
                            {
                                processCategory(b, s, cc, db, institute, ref data);
                            }
                        }
                    }
                }

                
                ListToExcel<ExcelExportRecord>(ref pck, data, userDisplayName);
            }

            var fileStream = new MemoryStream();
            pck.SaveAs(fileStream);
            fileStream.Position = 0;

            var fsr = new FileStreamResult(fileStream, contentType);
            fsr.FileDownloadName = fileDownloadName;

            return fsr;
        }

        public void processCategory(Benchmark b, Survey s, Category cat, EvalDataContext db, UserProfile user, ref List<ExcelExportRecord> data)
        {
            string userDisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName : user.DisplayName + " (" + user.UserName + ")";
            if (cat.Questions != null)
            {
                foreach (var q in cat.Questions)
                {
                    var ua = q.Answers.Where(a => a.User == user.UserName).FirstOrDefault();
                    if (ua != null)
                    {
                        data.Add(new ExcelExportRecord
                        {
                            Benchmark = parseHtmlValue(b.Clarification),
                            AlternativeBenchmark = parseHtmlValue(b.Alternative),
                            Category = cat.Title,
                            Country = b.Country,
                            Answer = int.Parse(ua.Answer),
                            Explanation = parseHtmlValue(ua.ExtraAnswer1),
                            Goal = int.Parse(ua.Goal),
                            GoalExplanation = parseHtmlValue(ua.ExtraAnswer2),
                            GoalInstruments = parseHtmlValue(ua.ExtraAnswer3),
                            Institute = b.InstituteName,
                            PersonResponsible = b.ResponsiblePersonName,
                            Question = q.Text,
                            User = userDisplayName
                        });
                    }
                }
            }
        }

        private string parseHtmlValue(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return string.Empty;

            if(!html.Contains("<html>"))
                html = "<html><head></head><body>" + html + "</body></html>";

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            html = doc.DocumentNode.SelectSingleNode("//body").InnerText;

            html = Regex.Replace(html, @"<[^>]*>", String.Empty);
            html = HttpUtility.HtmlDecode(html);

            return html;
        }

        public class ExcelExportRecord
        {
            public string Benchmark { get; set; }
            public string AlternativeBenchmark { get; set; }
            public string PersonResponsible { get; set; }
            public string ContextDefinition { get; set; }
            public string Country { get; set; }
            public string Institute { get; set; }
            public string Category { get; set; }
            public string Question { get; set; }
            public int Answer { get; set; }
            public string Explanation { get; set; }
            public int Goal { get; set; }
            public string GoalExplanation { get; set; }
            public string GoalInstruments { get; set; }
            public string User { get; set; }
        }

        public void ListToExcel<T>(ref ExcelPackage pck, List<T> query, string worksheetName)
        {
            //Create the worksheet
            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheetName);

            //get our column headings
            var t = typeof(T);
            var headings = t.GetProperties();
            for (int i = 0; i < headings.Count(); i++)
            {
                ws.Cells[1, i + 1].Value = headings[i].Name;
            }

            //populate our Data
            if (query.Count() > 0)
            {
                ws.Cells["A2"].LoadFromCollection(query);
            }

            //Format the header
            using (ExcelRange rng = ws.Cells["A1:BZ1"])
            {
                rng.Style.Font.Bold = true;
                rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
                rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
                rng.Style.Font.Color.SetColor(Color.White);
            }
        }
    }

    /// <summary>
    /// This should be in a separate class file but sorry, I can't be bothered.
    /// </summary>
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> SelectManyRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");

            return !source.Any() ? source :
                source.Concat(
                    source
                    .SelectMany(i => selector(i).EmptyIfNull())
                    .SelectManyRecursive(selector)
                );
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }
    }
}

