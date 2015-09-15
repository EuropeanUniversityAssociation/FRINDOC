using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace Frindoc.Web.Models
{
    public class UserAnswer
    {
        public int UserAnswerID { get; set; }
        public string Answer { get; set; }
        public string Goal { get; set; }
        public string User { get; set; }
        public string ExtraAnswer1 { get; set; }
        public string ExtraAnswer2 { get; set; }
        public string ExtraAnswer3 { get; set; }
        public string ExtraAnswer4 { get; set; }
        public string ExtraAnswer5 { get; set; }

        [ForeignKey("Question")]
        public int QuestionID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Question Question { get; set; }
    }
}