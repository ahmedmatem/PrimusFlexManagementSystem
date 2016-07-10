namespace PrimusFlex.WebServices.Controllers
{
    using System.Web.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Microsoft.AspNet.Identity;

    using PrimusFlex.Data;
    using PrimusFlex.Data.Common;
    using PrimusFlex.Data.Models;
    using ViewModels;
    using Newtonsoft.Json.Linq;
    using System.Threading.Tasks;
    using System.IO;
    using System.Web;
    using Common;
    using Newtonsoft.Json;
    [Authorize]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();
        protected IDbRepository<Phone> phones;
        protected IDbRepository<DataForToken> dataForToken;

        public LoginController()
        {
            this.phones = new DbRepository<Phone>(context);
            this.dataForToken = new DbRepository<DataForToken>(context);
        }

        // POST api/login/savephone
        [Route("savephone")]
        public HttpResponseMessage Post(PhoneViewModel model)
        {
            var userId = this.User.Identity.GetUserId();

            var phone = new Phone()
            {
                IMEI = model.IMEI,
                OwnerId = userId
            };

            this.phones.Add(phone);
            this.phones.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // GEt api/login/byImei
        [Route("byimei")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<HttpResponseMessage> LoginByIMEIAsync(string imei)
        {
            bool imeiExist = this.phones.All().Any(p => p.IMEI == imei);

            if (imeiExist)
            {
                var dataForToken = this.dataForToken.All().FirstOrDefault();

                string token = await GetTokenAsync(dataForToken.UserName, dataForToken.Password);
                return Request.CreateResponse(HttpStatusCode.OK, new { access_token = token});
            }

            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        private async Task<string> GetTokenAsync(string userName, string password)
        {
            var url = HttpContext.Current.Request.IsLocal ? Constant.LOCAL_TOKEN_URI : Constant.REMOTE_TOKEN_URI;
            
            var client = new HttpClient();

            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            string postString = string.Format("grant_type={0}&username={1}&password={2}", "password", userName, password);
            request.ContentLength = postString.Length;

            CookieContainer cookies = new CookieContainer();
            request.CookieContainer = cookies;

            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
            requestWriter.Write(postString);
            requestWriter.Close();

            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // extract access_token from content
            var token = ExtractAccessToken(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

            return token;
        }

        private string ExtractAccessToken(string responseFromServer)
        {
            var splitedResponse = responseFromServer.Split(new char[] { ',' });
            var token = splitedResponse[0];
            var value = token.Split(new char[] { ':', '\"' }, StringSplitOptions.RemoveEmptyEntries);
            return value[value.Length - 1];
        }
    }
}

