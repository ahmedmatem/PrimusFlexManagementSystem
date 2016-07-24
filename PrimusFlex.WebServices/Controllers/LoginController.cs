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
    using Web.Infrastructure.Helpers;
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

            var userId = User.Identity.GetUserId();
            var phone = new Phone()
            {
                Imei = model.Imei,
                AccessToken = model.AccessToken,
                OwnerId = userId
            };

            this.phones.Add(phone);
            this.phones.Save();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

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

            return Request.CreateResponse(HttpStatusCode.OK, new { message = "Invalid IMEI number. Yhe operation was unsuccessful." });
        }

        // GEt api/login/byImei?imei=xxxx
        [AllowAnonymous]
        [Route("byimei")]
        [HttpGet]
        public HttpResponseMessage LoginByIMEIAsync(string imei)
        {
            var phone = this.phones.All().FirstOrDefault(p => p.Imei == imei);

            if (phone != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { access_token = phone.AccessToken });
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);

        }
    }
}

