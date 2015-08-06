using Logland.Client;
using SelfEvalToolv2.Helpers;
using SelfEvalToolv2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using System.Net.Http;

namespace SelfEvalToolv2.Controllers
{
    public class UserProfileController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        [System.Web.Http.AuthorizeAttribute]
        // GET api/Survey
        public IEnumerable<UserProfile> GetSubUserProfiles()
        {
            var children = db.Users.Where(u=>u.ParentUserName == User.Identity.Name).AsEnumerable();
            return children;
        }

        [System.Web.Http.AuthorizeAttribute]
        public HttpResponseMessage CreateSubAccount(RegisterSubModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    if (!WebSecurity.UserExists(model.EmailAddress))
                    {
                        WebSecurity.CreateUserAndAccount(model.EmailAddress, "secretPassword", new { DisplayName= model.Username, ParentUserName = User.Identity.Name, IsSubAccount = true });
                        string token = WebSecurity.GeneratePasswordResetToken(model.EmailAddress, 7 * 24 * 60);
                        // send an email with the token to the created user
                        SecurityHelper.SendCreatedSubaccountEmail(token, model.EmailAddress, model.Username);
                        return Request.CreateResponse(HttpStatusCode.OK, "{}");
                    }
                    else {
                        ModelState.AddModelError("email", "A user with this emailadress already exists.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                catch (MembershipCreateUserException e)
                {
                    Logger.Log(e);
                    ModelState.AddModelError(e.Message, e.StatusCode.ToString());
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
                catch (Exception ex)
                {
                    try
                    {
                        ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(model.EmailAddress);
                        ((SimpleMembershipProvider)Membership.Provider).DeleteUser(model.EmailAddress, true);
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                    Logger.Log(ex);
                    ModelState.AddModelError("", "An unexpected error occured");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        [System.Web.Http.AuthorizeAttribute]
        [System.Web.Http.HttpDelete]
        public HttpResponseMessage DeleteSubAccount(RegisterSubModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    if (WebSecurity.UserExists(model.EmailAddress))
                    {
                        ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(model.EmailAddress);
                        ((SimpleMembershipProvider)Membership.Provider).DeleteUser(model.EmailAddress, true);
                        return Request.CreateResponse(HttpStatusCode.OK, "{}");
                    } else
                    {
                        ModelState.AddModelError("emailaddress", "Sorry, the user no longer exists.");
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                    ModelState.AddModelError("", "An unexpected error occured");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }
    }
}