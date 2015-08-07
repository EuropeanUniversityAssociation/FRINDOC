using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frindoc.Web.Models
{
    public class Evaluation
    {
        public int EvaluationID { get; set; }
        public string User { get; set; }
        public string InstituteName { get; set; }
        public int FormatPresentationRating { get; set; }
        public int ResultPresentationRating { get; set; }
        public int UsefullnessRating { get; set; }
        public string WouldRecommendToOthers {get;set;}
        public string ImprovementSuggestions {get;set;}
        public string EvaluationComments {get;set;}
    }
}