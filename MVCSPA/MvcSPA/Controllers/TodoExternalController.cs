using System;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MvcSPA.Models;

namespace MvcSPA.Controllers
{
    public class TodoExternalController : ApiController
    {
        private TodoItemContext db = new TodoItemContext();

        // PUT api/TodoExternal/5
        public HttpResponseMessage PutTodoItem(int id, TodoItemDto todoItemDto)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            if (ModelState.IsValid && id == todoItemDto.TodoItemId)
            {
                TodoItem todoItem = todoItemDto.ToEntity();
                TodoList todoList = db.TodoLists.Find(todoItem.TodoListId);
                if (todoList == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (todoList.UserId != userName)
                {
                    // Trying to modify a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                // Need to detach to avoid duplicate primary key exception when SaveChanges is called
                db.Entry(todoList).State = EntityState.Detached;
                db.Entry(todoItem).State = EntityState.Modified;

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

        // POST api/TodoExternal
        public HttpResponseMessage PostTodoItem(TodoItemDto todoItemDto)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            if (ModelState.IsValid)
            {
                TodoList todoList = db.TodoLists.Find(todoItemDto.TodoListId);
                if (todoList == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (todoList.UserId != userName)
                {
                    // Trying to add a record that does not belong to the user
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                }

                TodoItem todoItem = todoItemDto.ToEntity();

                // Need to detach to avoid loop reference exception during JSON serialization
                db.Entry(todoList).State = EntityState.Detached;
                db.TodoItems.Add(todoItem);
                db.SaveChanges();
                todoItemDto.TodoItemId = todoItem.TodoItemId;

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, todoItemDto);
                response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = todoItemDto.TodoItemId }));
                return response;
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        // DELETE api/TodoExternal/5
        public HttpResponseMessage DeleteTodoItem(int id)
        {
            string userName = AuthenticationHelper.ValidateAuthentication();
            TodoItem todoItem = db.TodoItems.Find(id);
            if (todoItem == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            if (db.Entry(todoItem.TodoList).Entity.UserId != userName)
            {
                // Trying to delete a record that does not belong to the user
                return Request.CreateResponse(HttpStatusCode.Unauthorized);
            }

            TodoItemDto todoItemDto = new TodoItemDto(todoItem);
            db.TodoItems.Remove(todoItem);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return Request.CreateResponse(HttpStatusCode.OK, todoItemDto);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}