using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Newtonsoft.Json;

namespace SelfEvalToolv2.Models
{
    public class Category
    {
        public Category() {
            Weight = 1;
        }
        //public string sqdqsd { get; set; }

        public int CategoryID { get; set; }
        public string Title { get; set; }
        public decimal Weight { get; set; }
        public int Order { get; set; }

        [ForeignKey("ParentCategory")]
        public int? ParentCategoryID { get; set; }

        public int? SurveyID { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Survey Survey { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> ChildCategories { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }

}