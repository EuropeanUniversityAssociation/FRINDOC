using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SelfEvalToolv2.Models
{
    public class Survey
    {
        public Survey() {
            Categories = new List<Category>();
        }

        public int SurveyID { get; set; }
        [Required]
        public string Title { get; set; }

        // Navigation property
        public virtual ICollection<Category> Categories { get; set; }
    }
}