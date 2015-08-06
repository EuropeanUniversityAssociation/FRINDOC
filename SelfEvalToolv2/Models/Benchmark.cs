using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SelfEvalToolv2.Models
{
    public class Benchmark
    {
        public int BenchmarkID { get; set; }
        public int Choice { get; set; }
        public string Clarification { get; set; }
        public string Alternative { get; set; }
        public string User { get; set; }
        public string ResponsiblePersonName { get; set; }
        public string InstituteName { get; set; }
        public string Country { get; set; }
        public string NrOfUndergraduates { get; set; }
        public string NrOfDoctoralCandidates { get; set; }
    }
}