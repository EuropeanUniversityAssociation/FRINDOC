using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Security;
using Newtonsoft.Json;

namespace SelfEvalToolv2.Models
{
    public class Question
    {
        public Question()
        {
            this.Weight = 1;
        }

        public int QuestionID { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string GoalsText { get; set; }
        [Required]
        public string Type { get; set; }
        public string ExtraInfo1 { get; set; }
        public string ExtraInfo2 { get; set; }
        public string ExtraInfo3 { get; set; }
        public string ExtraInfo4 { get; set; }
        public string ExtraInfo5 { get; set; }

        public decimal Weight { get; set; }

        public virtual ICollection<UserAnswer> Answers { get; set; }

        //FK
        [ForeignKey("Category")]
        public virtual int CategoryID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Category Category { get; set; }

        //FK
        [ForeignKey("Survey")]
        public virtual int SurveyID { get; set; }

        // Navigation properties
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Survey Survey { get; set; }
    }
}
