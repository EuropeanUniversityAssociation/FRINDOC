using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Security;
using Frindoc.Web.Models;

namespace Frindoc.Web.Controllers
{
    public class UserAnswerController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        // GET api/Answer
        public IEnumerable<UserAnswer> GetUserAnswers()
        {
            return db.UserAnswers.Where(ua => ua.User == this.User.Identity.Name).AsEnumerable();
        }

        // GET api/Answer/5
        public UserAnswer GetUserAnswer(int id)
        {
            UserAnswer useranswer = db.UserAnswers.Find(id);
            if (useranswer == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return useranswer;
        }

        // PUT api/Answer/5
        public HttpResponseMessage PutUserAnswer(int id, UserAnswer useranswer)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != useranswer.UserAnswerID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(useranswer).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/Answer
        public HttpResponseMessage PostUserAnswer(UserAnswer useranswer)
        {
            useranswer.User = this.User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.UserAnswers.Add(useranswer);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, useranswer);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = useranswer.UserAnswerID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Answer/5
        public HttpResponseMessage DeleteUserAnswer(int id)
        {
            UserAnswer useranswer = db.UserAnswers.Find(id);
            if (useranswer == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.UserAnswers.Remove(useranswer);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, useranswer);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}