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
using Frindoc.Web.Models;

namespace Frindoc.Web.Controllers
{
    public class SurveyController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        // GET api/Survey
        public IEnumerable<Survey> GetSurveys()
        {
            var a = db.Surveys.Include(s => s.Categories.Select(c => c.ChildCategories))
                              .Include(s => s.Categories.Select(c => c.Questions.Select(q => q.Survey)))
                              .Include(s => s.Categories.Select(c => c.ChildCategories.Select(cc => cc.Questions))).AsEnumerable();
            return a;
        }

        // GET api/Survey/5
        public Survey GetSurvey(int id)
        {
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return survey;
        }

        // PUT api/Survey/5
        public HttpResponseMessage PutSurvey(int id, Survey survey)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != survey.SurveyID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(survey).State = EntityState.Modified;

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

        // POST api/Survey
        public HttpResponseMessage PostSurvey(Survey survey)
        {
            if (ModelState.IsValid)
            {
                db.Surveys.Add(survey);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, survey);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = survey.SurveyID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Survey/5
        public HttpResponseMessage DeleteSurvey(int id)
        {
            Survey survey = db.Surveys.Find(id);
            if (survey == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Surveys.Remove(survey);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, survey);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}