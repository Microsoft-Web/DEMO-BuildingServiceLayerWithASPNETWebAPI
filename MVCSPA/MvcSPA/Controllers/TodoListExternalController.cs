using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcSPA.Models;

namespace MvcSPA.Controllers
{
    public class TodoListExternalController : ApiController
    {
        private TodoItemContext db = new TodoItemContext();

        // GET api/TodoListExternal
        public IEnumerable<TodoListDto> GetTodoListsExternal()
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            return db.TodoLists.Include("Todos")
                .Where(u => u.UserId == userName)
                .OrderByDescending(u => u.TodoListId)
                .AsEnumerable()
                .Select(todoList => new TodoListDto(todoList));
        }

        // GET api/TodoListExternal/5
        public TodoListDto GetTodoListExternal(int id)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            TodoList todoList = db.TodoLists.Find(id);
            if (todoList == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            if (todoList.UserId != userName)
            {
                // Trying to modify a record that does not belong to the user
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            return new TodoListDto(todoList);
        }

        // PUT api/TodoListExternal/5
        public HttpResponseMessage PutTodoListExternal(int id, TodoListDto todoListDto)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            if (ModelState.IsValid && id == todoListDto.TodoListId)
            {
                TodoList todoList = todoListDto.ToEntity();
                if (db.Entry(todoList).Entity.UserId != userName)
                {
                    // Trying to modify a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                db.Entry(todoList).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // POST api/TodoListExternal
        public HttpResponseMessage PostTodoListExternal(TodoListDto todoListDto)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            if (ModelState.IsValid)
            {
                todoListDto.UserId = userName;
                TodoList todoList = todoListDto.ToEntity();
                db.TodoLists.Add(todoList);
                db.SaveChanges();
                todoListDto.TodoListId = todoList.TodoListId;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoListDto);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoListDto.TodoListId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/TodoListExternal/5
        public HttpResponseMessage DeleteTodoListExternal(int id)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            TodoList todoList = db.TodoLists.Find(id);
            if (todoList == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (db.Entry(todoList).Entity.UserId != userName)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoListDto todoListDto = new TodoListDto(todoList);
            db.TodoLists.Remove(todoList);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoListDto);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}