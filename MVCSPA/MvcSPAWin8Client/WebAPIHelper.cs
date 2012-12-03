using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MvcSPAWin8Client.Model;
using Windows.Data.Json;
using Windows.Security.Authentication.Web;

namespace MvcSPAWin8Client
{
    class WebAPIHelper
    {
        private const string ApiRoot = "http://xinqiudemo1.azurewebsites.net/";
        public static string FacebookReturnedToken;

        public static bool IsAuthorized()
        {
            return !string.IsNullOrEmpty(FacebookReturnedToken);
        }
        
        private static CookieContainer CookieContainer = new CookieContainer();
        private static HttpClientHandler Handler = new HttpClientHandler();
        private static HttpClient _httpClient = null;

        private static bool IsUsingFormLogin = false;
        public static HttpClient GetHttpClient()
        {
            if (_httpClient == null)
            {
                _httpClient = new HttpClient(Handler);
                _httpClient.BaseAddress = new Uri(ApiRoot);
                // Limit the max buffer size for the response so we don't get overwhelmed
                _httpClient.MaxResponseContentBufferSize = 266000;
            }
            return _httpClient;
        }

        public static async Task<bool> LoginToFacebook()
        {
            string facebookCallbackUrl = "http://xinqiudemo1.azurewebsites.net/";
            string facebookClientID = "305651152867822";
            WebAPIHelper.FacebookReturnedToken = "";

            bool exceptionCaught = false;
            try
            {
                string facebookURL = "https://www.facebook.com/dialog/oauth?client_id=" + Uri.EscapeDataString(facebookClientID) + "&redirect_uri=" + Uri.EscapeDataString(facebookCallbackUrl) + "&scope=read_stream&display=popup&response_type=token";

                System.Uri StartUri = new Uri(facebookURL);
                System.Uri EndUri = new Uri(facebookCallbackUrl);

                WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        StartUri,
                                                        EndUri);
                if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
                {
                    string facebookReturnedToken = WebAuthenticationResult.ResponseData.ToString();
                    FacebookReturnedToken = facebookReturnedToken.Substring(facebookReturnedToken.IndexOf("=", 0) + 1);

                    IsUsingFormLogin = false;
                }
                else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
                {
                    return false;
                    //await ShowMessage("HTTP Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseErrorDetail.ToString());
                }
                else
                {
                    return false;
                    //await ShowMessage("Error returned by AuthenticateAsync() : " + WebAuthenticationResult.ResponseStatus.ToString());
                }

            }
            catch (Exception)
            {
                //
                // Bad Parameter, SSL/TLS Errors and Network Unavailable errors are to be handled here.
                //
                exceptionCaught = true;
            }

            if (exceptionCaught)
            {
                return false;
                //await ShowMessage("network error");
            }
            return true;
        }

        public static async Task<bool> Login(string userName, string password)
        {
            HttpClient httpClient = GetHttpClient();

            //If the Server JsonLogin has AntiForgery turned on
            //string token = "";
            //var response1 = await httpClient.GetAsync(ApiRoot);
            ////<input name="__RequestVerificationToken" type="hidden" value="MEzcLJ6G5dWseG7dh4mc0qSV2lxUTRbIqFlnOgFUCf4fNQawmoZx6IE3FmjLEhkn0Tzf-OxFlN2KFse8uxsPtGTKcLXu0myGwxCfDxSXLP01"/>
            //string pattern = "\"__RequestVerificationToken\"\\stype=\"hidden\"\\svalue=\".*?\"";
            //Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
            //Match myMatch = reg.Match(await response1.Content.ReadAsStringAsync());
            //while (myMatch.Success)
            //{
            //    token = myMatch.Groups[0].Value;
            //    token = token.Replace("\"__RequestVerificationToken\" type=\"hidden\" value=\"", "").Replace("\"", "");
            //    break;
            //}
            //string jsonData = String.Format("{{\"__RequestVerificationToken\":\"{0}\",\"Username\":\"{1}\",\"Password\":\"{2}\",\"RememberMe\":\"false\"}}",
            //    token, userName, password);

            string jsonData = String.Format("{{\"Username\":\"{0}\",\"Password\":\"{1}\",\"RememberMe\":\"false\"}}", userName, password);

            HttpContent content = new StringContent(jsonData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await httpClient.PostAsync("Account/JsonLogin", content);
            if (response.IsSuccessStatusCode)
            {
                IsUsingFormLogin = true;
            }

            return response.IsSuccessStatusCode;
        }

        public static string GetWebAPIExtension()
        {
            if (!IsUsingFormLogin)
            {
                return "external";
            }
            return "";
        }

        public static string GetActionExtension()
        {
            if (!IsUsingFormLogin)
            {
                return string.Format("?code={0}", FacebookReturnedToken);
            }
            return "";
        }

        #region Web API TodoList calls
        public static async Task<List<TodoList>> WebAPIGetTodoList()
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                var response = await httpClient.GetAsync(string.Format("api/todolist{0}{1}", GetWebAPIExtension(), GetActionExtension()));
                if (response.IsSuccessStatusCode)
                {
                    var todoList = await response.Content.ReadAsStringAsync();

                    var tList = JsonArray.Parse(todoList);
                    var myTodoList = (from m in tList
                                      select new TodoList
                                      {
                                          TodoListId = Convert.ToInt32(m.GetObject()["TodoListId"].GetNumber()),
                                          UserId = m.GetObject()["UserId"].GetString(),
                                          Title = m.GetObject()["Title"].GetString(),
                                          Todos = new ObservableCollection<TodoItem>(
                                              (from n in m.GetObject()["Todos"].GetArray()
                                               select new TodoItem
                                               {
                                                   IsDone = n.GetObject()["IsDone"].GetBoolean(),
                                                   Title = n.GetObject()["Title"].GetString(),
                                                   TodoItemId = Convert.ToInt32(n.GetObject()["TodoItemId"].GetNumber()),
                                                   TodoListId = Convert.ToInt32(n.GetObject()["TodoListId"].GetNumber())
                                               }).ToArray())
                                      }).ToList();
                    return myTodoList;
                }
                return null;
            }
        }

        public static async Task<bool> WebAPIUpdateTodoList(TodoList todoList)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                HttpContent content = new StringContent(todoList.SerializeToJsonObject());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PutAsync(string.Format("api/todolist{0}/{1}{2}", GetWebAPIExtension(), todoList.TodoListId, GetActionExtension()), content);
                return response.IsSuccessStatusCode;
            }
        }

        public static async Task<bool> WebAPIAddTodoList(TodoList todoList)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                HttpContent content = new StringContent(todoList.SerializeToJsonObjectWithoutId());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(string.Format("api/todolist{0}{1}", GetWebAPIExtension(), GetActionExtension()), content);

                if (response.IsSuccessStatusCode)
                {
                    string newTodoList = await response.Content.ReadAsStringAsync();
                    var tList = JsonObject.Parse(newTodoList);
                    todoList.TodoListId = Convert.ToInt32(tList.GetNamedNumber("TodoListId"));
                }
                return response.IsSuccessStatusCode;
            }
        }

        public static async Task<bool> WebAPIDeleteTodoList(int todoListId)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                var response = await httpClient.DeleteAsync(string.Format("api/todolist{0}/{1}{2}", GetWebAPIExtension(), todoListId, GetActionExtension()));
                return response.IsSuccessStatusCode;
            }
        }
        #endregion


        #region Web API TodoItem calls
        public static async Task<bool> WebAPIUpdateTodoItem(TodoItem todoItem)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                HttpContent content = new StringContent(todoItem.SerializeToJsonObject());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PutAsync(string.Format("api/Todo{0}/{1}{2}", GetWebAPIExtension(), todoItem.TodoItemId, GetActionExtension()), content);
                return response.IsSuccessStatusCode;
            }
        }

        public static async Task<bool> WebAPIAddTodoItem(TodoItem todoItem)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                HttpContent content = new StringContent(todoItem.SerializeToJsonObjectWithoutId());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(string.Format("api/Todo{0}{1}", GetWebAPIExtension(), GetActionExtension()), content);

                if (response.IsSuccessStatusCode)
                {
                    string newTodoItem = await response.Content.ReadAsStringAsync();
                    var tItem = JsonObject.Parse(newTodoItem);
                    todoItem.TodoItemId = Convert.ToInt32(tItem.GetNamedNumber("TodoItemId"));
                }
                return response.IsSuccessStatusCode;
            }
        }

        public static async Task<bool> WebAPIDeleteTodoItem(int todoItemId)
        {
            HttpClient httpClient = GetHttpClient();
            //using (HttpClient httpClient = GetHttpClient())
            {
                var response = await httpClient.DeleteAsync(string.Format("api/Todo{0}/{1}{2}", GetWebAPIExtension(), todoItemId, GetActionExtension()));
                return response.IsSuccessStatusCode;
            }
        }
        #endregion
    }
}
