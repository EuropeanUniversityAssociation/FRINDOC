using Frindoc.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Frindoc.Web.Controllers
{
    public class BenchmarkController : ApiController
    {
        private EvalDataContext db = new EvalDataContext();

        // GET api/benchmark
        public Benchmark Get()
        {
            // get current user and see if he is a SubAccount
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();
            if (user == null) return null;

            var benchmark = new Benchmark();
            if(user.IsSubAccount.GetValueOrDefault()){
                benchmark = db.Benchmarks.Where(b => b.User == user.ParentUserName).FirstOrDefault();
            }
            else
                benchmark = db.Benchmarks.Where(b => b.User == this.User.Identity.Name).FirstOrDefault();
            return benchmark;
        }

        [Authorize]
        // GET api/benchmark/5
        public Benchmark Get(int id)
        {
            // get current user and see if he is a SubAccount
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();
            if(user.IsSubAccount.GetValueOrDefault()){
                return db.Benchmarks.Where(b => b.User == user.ParentUserName).FirstOrDefault();
            }
            else
                return db.Benchmarks.Where(b => b.User == this.User.Identity.Name).FirstOrDefault();            
        }

        // POST api/benchmark
        public HttpResponseMessage Post(Benchmark viewModel)
        {
            viewModel.User = this.User.Identity.Name;

            if (ModelState.IsValid)
            {
                db.Benchmarks.Add(viewModel);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, viewModel);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = viewModel.BenchmarkID }));
                return response;
            }
            else
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // PUT api/benchmark/5
        public HttpResponseMessage Put(int id, Benchmark viewModel)
        {
            viewModel.User = this.User.Identity.Name;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != viewModel.BenchmarkID)
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

        // DELETE api/benchmark/5
        public void Delete(int id)
        {
        }
    }
}
