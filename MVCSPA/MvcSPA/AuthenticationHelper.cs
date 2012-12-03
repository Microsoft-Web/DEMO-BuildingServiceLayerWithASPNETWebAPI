using DotNetOpenAuth.AspNet.Clients;
using Microsoft.Web.WebPages.OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;

namespace MvcSPA
{
    public class AuthenticationHelper
    {
        /// <summary>
        /// Validate the auth code sent by client.
        /// </summary>
        /// <returns>Username if authentication succeeds.</returns>
        public static string ValidateAuthentication()
        {
            // Currently only Facebook is supported. But this method can be extended.
            var authClient = OAuthWebSecurity.RegisteredClientData.Where(c => c.DisplayName == "Facebook").ToList()[0].AuthenticationClient;
            Type type = authClient.GetType();
            if (type == typeof(FacebookClient))
            {
                FacebookClient oAuth2Client = (FacebookClient)authClient;
                string uri = "https://graph.facebook.com/me?access_token=" + HttpContext.Current.Request.QueryString["code"];
                HttpClient client = new HttpClient();
                var httpRequestTask = client.GetAsync(uri);
                httpRequestTask.Wait();
                HttpResponseMessage response = httpRequestTask.Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpResponseException(response);
                }
                StringContent content = response.Content as StringContent;
                var readResponseContentTask = response.Content.ReadAsStreamAsync();
                readResponseContentTask.Wait();
                Stream responseStream = readResponseContentTask.Result;
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseJson = reader.ReadToEnd();
                    dynamic responseObj = Json.Decode(responseJson);
                    string username = responseObj.name;
                    return username;
                }
            }
            return string.Empty;
        }

    }
}