using iTextSharp.text;
using iTextSharp.text.pdf;
using SelfEvalToolv2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SelfEvalToolv2.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            var db = new EvalDataContext();
            var user = db.Users.Where(u=>u.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
                model.IsSubAccount = user.IsSubAccount.GetValueOrDefault(false);

            return View(model);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Export(ExportViewModel model)
        {
            var db = new EvalDataContext();
            model.Organisations = (from b in db.Benchmarks.ToList()
                                   join u in db.Users.ToList() on b.User equals u.UserName
                                   where !string.IsNullOrWhiteSpace(b.InstituteName)
                                   select new SelectListItem() {
                                       Text = (u.Created >= new DateTime(2015,01,08) ? "[OT] " : "[FT] ") + b.InstituteName, 
                                       Value = ((double)b.BenchmarkID).ToString().Trim() 
                                   })
                                   .Distinct()
                                   .OrderBy(b => b.Text)
                                   .ToList();

            model.SubAccounts = (from u in db.Users
                                 join b in db.Benchmarks on u.ParentUserName equals b.User
                                 where u.IsSubAccount == true
                                 where SqlFunctions.StringConvert((double)b.BenchmarkID).Trim() == model.SelectedBenchmarkID
                                 select new SelectListItem()
                                 {
                                     Text = u.DisplayName,
                                     Value = u.UserName
                                 });

            // Validate valid subaccount
            if(!string.IsNullOrWhiteSpace(model.SelectedSubAccount) && !model.SubAccounts.Any(x=>x.Value == model.SelectedSubAccount))
                model.SelectedSubAccount = null;

            if (string.IsNullOrEmpty(model.SelectedBenchmarkID))
                model.SelectedBenchmarkID = model.Organisations.First().Value;
            
            model.OrganisationName = model.Organisations.First(o => o.Value == model.SelectedBenchmarkID).Text;
            model.ApplicableBenchmarks = db.Benchmarks.Where(b => SqlFunctions.StringConvert((double)b.BenchmarkID).Trim() == model.SelectedBenchmarkID).ToList();

            if (!string.IsNullOrEmpty(model.SelectedBenchmarkID))
            {
                model.Categories = db.Categories.Include("Questions").Include("Questions.Answers").Select(c => c).ToList();
                model.HighchartsOptions = buildHighchartOptions(model, db);
            }
            return View(model);
        }

        private string buildHighchartOptions(ExportViewModel model, EvalDataContext db)
        {
            var users = (from u in db.Users
                        join b in db.Benchmarks on u.UserName equals b.User
                         where SqlFunctions.StringConvert((double)b.BenchmarkID).Trim() == model.SelectedBenchmarkID
                        select u.UserName).ToList();

            if (!string.IsNullOrWhiteSpace(model.SelectedSubAccount))
            {
                users = new List<string>() { model.SelectedSubAccount };
            }

            List<int> series0List = new List<int>();
            List<int> series1List = new List<int>();
            string categories = string.Join(",", model.Categories.Where(c => c.ChildCategories == null || c.ChildCategories.Count == 0).Select(x => "'" + x.Title.Replace("'", "´") + "'"));
            foreach (var x in model.Categories.Where(c => c.ChildCategories == null || c.ChildCategories.Count == 0))
            {
                int status = 0;
                int goal = 0;
                foreach (var q in x.Questions)
                {
                    var s = db.UserAnswers.Where(ua => users.Contains(ua.User) && ua.QuestionID == q.QuestionID).FirstOrDefault();
                    var g = db.UserAnswers.Where(ua => users.Contains(ua.User) && ua.QuestionID == q.QuestionID).FirstOrDefault();
                    status += s != null ? int.Parse(s.Answer) : 0;
                    goal += g != null ? int.Parse(g.Goal) : 0;
                }
                status = (int)(status * x.Weight);
                goal = (int)(goal * x.Weight);

                series0List.Add(status);
                series1List.Add(goal);
            }
            string series0 = string.Join(",", series0List);
            string series1 = string.Join(",", series1List);

            return @"
var chart = $('#highchart').highcharts();
if (typeof (chart) !== 'undefined') {
    // Set categories
    chart.xAxis[0].setCategories([" + categories + @"]);
            
    // Set colors
    chart.series[0].color = '#0066FF';
    chart.series[1].color = '#FF0000';
    chart.legend.colorizeItem(chart.series[0], chart.series[0].visible);
    chart.legend.colorizeItem(chart.series[1], chart.series[1].visible);

    // Set series data
    chart.series[0].setData([" + series0 + @"]);//status
    chart.series[1].setData([" + series1 + @"]);//goal


    canvg(document.getElementById('canvas'), chart.getSVG())

    var canvas = document.getElementById('canvas');
    var img = canvas.toDataURL('image/png');

    if ($('canvas').length > 0) {
        $('#highcharts-png').attr('src', img);
    }
}
            ";
        }
    }
}
