using Frindoc.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Frindoc.Web.Controllers
{
    public class MembershipController : Controller
    {
        private EvalDataContext db = new EvalDataContext();

        //
        // GET: /Membership/

        public ActionResult Index()
        {
            var viewModel = new Models.MembershipOverview();
            viewModel.Users = db.Users.OrderBy(u=>u.UserName).ToList();
            viewModel.Roles = new List<string>(Roles.GetAllRoles());
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditUser(int UserId)
        {
            var model = db.Users.First(x => x.UserId == UserId);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var originalUser = db.Users.First(x => x.UserId == model.UserId);
                originalUser.UserName = model.UserName;
                db.SaveChanges();
                return View(EditUser(model.UserId));
            }
            return View(model);
        }

        // Get
        [HttpGet]
        public ActionResult AddUserRole()
        {
            return View(new AddUserRole());
        }

        [HttpPost]
        public ActionResult AddUserRole(AddUserRole model)
        {
            if (ModelState.IsValid)
            {
                Roles.AddUserToRole(model.UserName, model.NewRole);

                // return empty view
                return View(new AddUserRole());
            }
            // return errors on error
            return View(model);
        }

    }
}
