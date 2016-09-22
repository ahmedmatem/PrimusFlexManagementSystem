namespace PrimusFlex.WebServices.Controllers
{
    using System.Web.Http;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.IO;
    using System.Web;

    using Microsoft.AspNet.Identity;

    using Newtonsoft.Json.Linq;

    using PrimusFlex.Data;
    using PrimusFlex.Data.Common;

    using Common;
    using Models;
    using Data.Models;

    [Authorize]
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        protected ApplicationDbContext context = new ApplicationDbContext();

        protected IDbRepository<Phone> phones;

        public LoginController()
        {
            this.phones = new DbRepository<Phone>(context);
        }

        // POST api/login/savephone
        [Route("savephone")]
        public HttpResponseMessage Post(PhoneModel model)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var userName = User.Identity.GetUserName();
            var phone = new Phone()
            {
                Imei = model.Imei,
                AccessToken = model.AccessToken,
                UserName = userName
            };

            this.phones.Add(phone);
            this.phones.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Remove phone record from databse with given imei parameter.
        /// </summary>
        /// <param name="imei"></param>
        /// <returns></returns>

        // DELETE api/login/remove?imei=xxxx
        [Route("remove")]
        public HttpResponseMessage Remove(string imei)
        {
            var phone = this.phones.All().FirstOrDefault(p => p.Imei == imei);
            if (phone != null)
            {
                this.phones.HardDelete(phone);
                this.phones.Save();

                return Request.CreateResponse(HttpStatusCode.OK, new { message = "You removed imei login successfully." });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Invalid IMEI number. The operation was unsuccessful." });
        }

        /// <summary>
        /// This service is used to login by phone imei number.
        /// </summary>
        /// <param name="imei"></param>
        /// <returns>NoContent if access token expired or no registered phone with given imei parameter.
        /// Otherwise return status code OK.</returns>
        
        // GET api/login/byImei?imei
        [AllowAnonymous]
        [Route("byimei")]
        [HttpGet]
        public HttpResponseMessage LoginByIMEI(string imei)
        {
            var phone = this.phones.All().FirstOrDefault(p => p.Imei == imei);

            if (phone != null)
            {
                var totalDays = (DateTime.Now - phone.CreatedOn).TotalDays;
                if(totalDays > AppConfig.ACCESS_TOKEN_EXPIRE_TIME_SPAN)
                {
                    // Remove phone record, because access token is expired
                    this.phones.HardDelete(phone);
                    this.phones.Save();

                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                return Request.CreateResponse(HttpStatusCode.OK, new { access_token = phone.AccessToken, user_name = phone.UserName });
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);

        }
    }
}

