using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace SelfEvalToolv2.Models
{
    public class AddUserRole
    {
        public string UserName { get; set; }
        public string NewRole { get; set; }
    }

    public class DeleteUserRole
    {
        public string UserName { get; set; }
        public string DeletedRole { get; set; }
    }

    public class MembershipOverview
    {
        public List<UserProfile> Users { get; set; }
        public List<string> Roles { get; set; }
    }
}