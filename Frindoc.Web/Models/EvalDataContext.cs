using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace Frindoc.Web.Models
{
    public class SelfEvalConfiguration : DbMigrationsConfiguration<EvalDataContext>
    {
        public SelfEvalConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(EvalDataContext context)
        {
            try
            {
                if (context.Surveys.Count() == 0)
                {
                    // TEST DATA
                    context.Surveys.Add(new Survey { Title = "EUA Evaluation tool" });
                    context.SaveChanges();

                    var s = context.Surveys.FirstOrDefault();
                    context.Surveys.First().Categories = new List<Category>() { 
                        new Category { Title="A - Research Capacity", Order = 1, ChildCategories = new List<Category> {
                            new Category { Title = "Staff capacity for research and supervision", Order = 1, Weight = 1, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the time and capacity of staff for research", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the time and capacity of your staff to conduct research?",
                                    ExtraInfo1="This corresponds to both the tangible resources available for research in terms of time and funding, as well as the capacity of staff in your institution to engage at the forefront of their disciplines." },
                                new Question { Survey = s, Text = "Please rate (0-5) the time and capacity of staff for supervision", Type="Numeric",
                                    GoalsText = "What rating could realistically be achieved regarding the time and capacity of your staff to engage in supervision?",
                                    ExtraInfo1="This corresponds to both the time available to research staff to supervise as well as their overall ability to deliver high-quality supervision." }
                            } },
                            new Category { Title = "Research productivity", Order = 2, Weight = 1, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the research output of your insitution (publications, patents and similar).", Type="Numeric",
                                    GoalsText = "What rating could realistically be achieved regarding the research output of your institution (publications, patents and similar)?",
                                    ExtraInfo1="Research output can be measured, for example, in bibliometric terms such as number of publications or impact. Respondents might also agree on more qualitative measures if this suits the particular institutional profile." },
                                new Question { Survey = s, Text = "Please rate (0-5) the output in terms of doctoral graduations.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the output of your institution in terms of doctoral graduations?",
                                    ExtraInfo1="The output of doctoral graduations should be considered quantitatively according to the context, whether national, global or otherwise defined. If, for example, the benchmark is national, a high rating would correspond to a high number of graduations in comparison with other institutions in the same country." }
                            } },
                            new Category { Title = "External funding for research", Order = 3, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) your capacity to attract competitive external funding.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the capacity of your institution to attract competitive external funding?",
                                    ExtraInfo1="This corresponds to the capacity of the institution as a whole to attract funding through external sources allocated through a competitive selection process." }
                            } },
                            new Category { Title = "Funding for doctoral candidates stipends/wages", Order = 4, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) your capacity to fund scholarships/salaries for doctoral candidates.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the capacity of your institution to fund scholarships/salaries for doctoral candidates?",
                                    ExtraInfo1="This corresponds to the financial capacity of the institution to provide funding for doctoral candidates either through their own funds or through their capacity to attract external funding." }
                            } }
                        } },
                        new Category { Title="B - International profile", Order = 2, ChildCategories = new List<Category> {
                            new Category { Title = "Institutional reputation", Order = 1, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the reputation/prestige of your own institution in terms of being an attractive partner.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the reputation/prestige of your institution in terms of being an attractive partner?",
                                    ExtraInfo1="This corresponds to how other institutions view your university in terms of prestige and/or reputation as a research partner." },
                            } },
                            new Category { Title = "International profile of staff", Order = 2, Weight = 1, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) your capacity to attract international staff.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding your institution’s capacity to attract international staff?",
                                    ExtraInfo1="The ability to attract international research staff could be measured by the proportion of international researchers at your institution compared to your chosen benchmark." },
                                new Question { Survey = s, Text = "Please rate (0-5)  the ability of your staff to engage in international networks.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the ability of your institution’s staff to engage in international networks?",
                                    ExtraInfo1="This corresponds to the ability of research staff to, for example, take part in international research teams, lead international consortia or be part of networks in other countries." }
                            } },
                            new Category { Title = "International profile of doctoral candidates", Order = 3, Weight = 0.6666M, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) your capacity to recruit doctoral candidates internationally.", Type="Numeric",
                                    GoalsText = "What rating could realistically be achieved regarding the capacity of your institution to recruit doctoral candidates internationally?", 
                                    ExtraInfo1="The ability to attract international doctoral candidates could be measured by the proportion of international doctoral candidates at your institution and/or the quality of international applicants for doctoral programmes compared to your chosen benchmark." },
                                new Question { Survey = s, Text = "Please rate (0-5) the international \"outlook\" of your doctoral candidates." , Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the international ‘outlook’ of the doctoral candidates in your institution?",
                                    ExtraInfo1="This corresponds to the degree to which your doctoral candidates are capable of taking part in international activities, taking into account for instance language skills, intercultural communication skills and motivation to engage internationally." },
                                new Question { Survey = s, Text = "Please rate (0-5) the international employability of your doctoral candidates.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the international employability of the doctoral candidates in your institution?",
                                    ExtraInfo1="The degree to which your doctoral candidates are equipped for the international labour market, academic or non-academic, for example in terms of their research competences, language and intercultural communication skills." }
                            } }
                        } },
                        new Category { Title="C - Institutional Framework", Order = 3, ChildCategories = new List<Category> {
                            new Category { Title = "Quality assurance system", Order = 1, Weight = 1, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the extent to which your internal QA-system enhances internationalisation in your institution?", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the extent to which your institution’s internal QA system enhances internationalisation in your institution?",
                                    ExtraInfo1="This corresponds to how helpful the internal procedures in the institution are to internationalisation. It could be considered whether these procedures promote internationalisation and facilitate collaborations or whether they present obstacles." },
                                new Question { Survey = s, Text = "Please rate (0-5) the extent to which external QA enhances internationalisation?", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the extent to which external QA enhances internationalisation in your institution?",
                                    ExtraInfo1="This corresponds to how helpful the external procedures of quality assurance agencies, research assessments and similar are for internationalisation. It could be considered if these agencies apply the necessary flexibility to facilitate internationalisation or if they present obstacles in terms of for example accreditation, recognition or cumbersome procedures." },
                            } },
                            new Category { Title = "Management capacity", Order = 2, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the capacity of the management structure to facilitate internationalisation of doctoral education?", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the capacity of your institution’s management structure to facilitate internationalisation of doctoral education?",
                                    ExtraInfo1="This corresponds to the overall management structure of your institution and whether it is capable of supporting internationalisation of doctoral education. You could consider, among other things, the ability of different offices to work together and/or the links between individual research projects and the institution as a whole." },
                            } },
                            new Category { Title = "Operational capacity", Order = 3, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the capacity of your institution to cope with the tasks related to internationalisation (visa applications, travel arrangements etc).", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the capacity of your institution to cope with the tasks related to internationalisation (visa applications, travel arrangements etc.)?",
                                    ExtraInfo1="This corresponds to the capacity of your institution to manage operational tasks related to internationalisation. You could consider, among other things, whether there is smooth and efficient handling of travel arrangements, visa applications, reimbursement of travel costs and similar." },
                            } },
                            new Category { Title = "National legal and administrative framework", Order = 4, Weight = 1, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the degree to which national the legal and administrative framework facilitate financial arrangements.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the degree to which the national, legal and administrative framework facilitate financial arrangements?",
                                    ExtraInfo1="This corresponds to whether funding is sufficiently available for international activities and if it is administered in a way that is not excessively burdensome for your institution." },
                                new Question { Survey = s, Text = "Please rate (0-5) degree to which national regulations facilitate internationalisation.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the degree to which national regulations facilitate internationalisation?",
                                    ExtraInfo1="This corresponds to the degree to which national regulations support internationalisation of doctoral education or make it more difficult for your institution. If, for example, there are problems related to recognition of joint and dual degrees, the rating should reflect this only if your institution would like to use dual or joint degrees." },
                            } },
                        } },
                        new Category { Title="D - Mobility", Order = 4, ChildCategories = new List<Category> {
                            new Category { Title = "Doctoral candidates' mobility", Order = 1, Weight = 0.6666M, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) how frequently your doctoral candidates engage in long-term mobility (more than 3 months).", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the frequency with which the doctoral candidates in your institution engage in long-term mobility (more than 3 months)?",
                                    ExtraInfo1="This corresponds to the number of doctoral candidates who are mobile for longer periods as well as the frequency of mobility among doctoral candidates in your institution as compared to your chosen benchmark" },
                                new Question { Survey = s, Text = "Please rate (0-5) how frequently your doctoral candidates engage in short-term mobility (less than 3 months).", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the frequency with which the doctoral candidates in your institution engage in short-term mobility (less than 3 months)?",
                                    ExtraInfo1="This corresponds to the number of doctoral candidates who are mobile for short periods (excluding very short trips to meetings or conferences), as well as the frequency of short-term mobility among doctoral candidates in your institution as compared to your chosen benchmark." },
                                new Question { Survey = s, Text = "Please rate (0-5) how frequently your doctoral candidates participate in international conference.", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the frequency with which the doctoral candidates in your institution participate in international conferences?",
                                    ExtraInfo1="This corresponds to the number of doctoral candidates from your institution who attend international conferences where they have the opportunity to network and engage with the international research community, as well as the frequency among your doctoral candidates of attending such conferences." },
                            } },
                            new Category { Title = "Staff mobility", Order = 2, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) how frequently your supervisors are mobile", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the frequency with which supervisors in your institution are mobile?",
                                    ExtraInfo1="This corresponds to the overall mobility of supervisors in your institution, including short-term and long-term mobility as well as participation in international conferences." },
                            } },
                            new Category { Title = "Funding for mobility", Order = 3, Weight = 2, Questions = new List<Question> {
                                new Question { Survey = s, Text = "Please rate (0-5) the availability of funding for mobility of doctoral candidates", Type="Numeric", 
                                    GoalsText = "What rating could realistically be achieved regarding the availability of funding for the mobility of doctoral candidates in your institution?",
                                    ExtraInfo1="This corresponds to the availability of sufficient funding for costs related to the mobility of doctoral candidates such as travel grants, personal travel budgets and similar." },
                            } }
                        } }
                    };

                    context.SaveChanges();
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }
    }

    public class SelfEvalInitializer : MigrateDatabaseToLatestVersion<EvalDataContext, SelfEvalConfiguration>
    {

    }

    public class EvalDataContext : DbContext
    {
        public EvalDataContext()
            : base("name=DefaultConnection")
        {
            //this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Benchmark> Benchmarks { get; set; }
        public DbSet<UserProfile> Users { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new CategoryConfiguration());
        }
    }
}