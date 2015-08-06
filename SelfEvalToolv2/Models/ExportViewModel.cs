using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SelfEvalToolv2.Models
{
    public class ExportViewModel
    {
        public string SelectedBenchmarkID { get; set; }
        public string OrganisationName { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<SelectListItem> Organisations { get; set; }
        public IEnumerable<Benchmark> ApplicableBenchmarks { get; set; }
        public string HighchartsOptions { get; set; }
        public IEnumerable<SelectListItem> SubAccounts { get; set; }
        public string SelectedSubAccount { get; set; }
    }
}