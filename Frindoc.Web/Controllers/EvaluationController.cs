using Frindoc.Web.Models;
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

namespace Frindoc.Web.Controllers
{
    public class EvaluationController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        // GET api/Evaluation
        public Evaluation Get()
        {
            // get current user and see if he is a SubAccount
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();
            if (user == null) return null;

            var Evaluation = new Evaluation();
            if (user.IsSubAccount.GetValueOrDefault())
            {
                Evaluation = db.Evaluations.Where(b => b.User == user.ParentUserName).FirstOrDefault();
            }
            else
                Evaluation = db.Evaluations.Where(b => b.User == this.User.Identity.Name).FirstOrDefault();
            return Evaluation;
        }

        [Authorize]
        // GET api/Evaluation/5
        public Evaluation Get(int id)
        {
            // get current user and see if he is a SubAccount
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();
            if (user.IsSubAccount.GetValueOrDefault())
            {
                return db.Evaluations.Where(b => b.User == user.ParentUserName).FirstOrDefault();
            }
            else
                return db.Evaluations.Where(b => b.User == this.User.Identity.Name).FirstOrDefault();
        }

        // POST api/Evaluation
        public HttpResponseMessage Post(Evaluation viewModel)
        {
            viewModel.User = this.User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.Evaluations.Add(viewModel);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, viewModel);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = viewModel.EvaluationID }));
                return response;
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // PUT api/Evaluation/5
        public HttpResponseMessage Put(int id, Evaluation viewModel)
        {
            viewModel.User = this.User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != viewModel.EvaluationID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(viewModel).State = EntityState.Modified;

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

        // DELETE api/Evaluation/5
        public void Delete(int id)
        {
        }
    }
}