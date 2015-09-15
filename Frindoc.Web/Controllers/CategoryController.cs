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
    public class CategoryController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        // GET api/Category
        public IEnumerable<Category> GetCategories()
        {
            var categories = db.Categories.Include(c => c.ParentCategory).Include(c=>c.Questions);
            return categories.AsEnumerable();
        }

        // GET api/Category/5
        public Category GetCategory(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }
            return category;
        }

        // PUT api/Category/5
        public HttpResponseMessage PutCategory(int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != category.CategoryID)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(category).State = EntityState.Modified;

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

        // POST api/Category
        public HttpResponseMessage PostCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // calculate the last order
                int lastOrder = 1;
                var parent = db.Categories.Include(c=>c.ChildCategories).FirstOrDefault(c=>c.CategoryID == category.ParentCategoryID);
                if (parent != null && parent.ChildCategories.Count > 0)
                    lastOrder = parent.ChildCategories.Max(c => c.Order) + 1;

                if (parent == null) {                    
                    var survey = db.Surveys.Include(s => s.Categories).Where(s => s.SurveyID == category.SurveyID).FirstOrDefault();
                    if (survey != null && survey.Categories.Count > 0)
                        lastOrder = survey.Categories.Max(c => c.Order) + 1;
                }

                category.Order = lastOrder;

                db.Categories.Add(category);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.CategoryID }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/Category/5
        public HttpResponseMessage DeleteCategory(int id)
        {
            Category category = db.Categories.Include(c=>c.ChildCategories.Select(cc=>cc.ChildCategories)).FirstOrDefault(c=>c.CategoryID == id);
            if (category == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Categories.Remove(category);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, category);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}