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
using AngularTutorial.Models;

namespace AngularTutorial.Controllers
{
    public class ToDoController : ApiController
    {
        private ToDoContext db = new ToDoContext();

        // GET api/ToDo
        public IEnumerable<Todo> GetTodos(string q = null, string sort = null, bool desc = false, int? limit = null, int offset = 0)
        {
            var list = ((IObjectContextAdapter)db).ObjectContext.CreateObjectSet<Todo>();

            IQueryable<Todo> items = string.IsNullOrEmpty(sort)
                ? list.OrderBy(o => o.Priority)
                : list.OrderBy(String.Format("it.{0} {1}", sort, desc ? "DESC" : "ASC"));

            if (!string.IsNullOrEmpty(q) && q != "undefined")
            {
                items = items.Where(t => t.Text.Contains(q));
            }

            if (offset > 0)
            {
                items = items.Skip(offset);
            }

            if (limit.HasValue)
            {
                items = items.Take(limit.Value);
            }

            return items;
        }

        // GET api/ToDo/5
        public Todo GetTodo(int id)
        {
            Todo todomodel = db.Todoes.Find(id);
            if (todomodel == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return todomodel;
        }

        // PUT api/ToDo/5
        public HttpResponseMessage PutTodo(int id, Todo todomodel)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != todomodel.Id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            db.Entry(todomodel).State = EntityState.Modified;

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

        // POST api/ToDo
        public HttpResponseMessage PostTodo(Todo todomodel)
        {
            if (ModelState.IsValid)
            {
                db.Todoes.Add(todomodel);
                db.SaveChanges();

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todomodel);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todomodel.Id }));
                return response;
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
        }

        // DELETE api/ToDo/5
        public HttpResponseMessage DeleteTodo(int id)
        {
            Todo todomodel = db.Todoes.Find(id);
            if (todomodel == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            db.Todoes.Remove(todomodel);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todomodel);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}