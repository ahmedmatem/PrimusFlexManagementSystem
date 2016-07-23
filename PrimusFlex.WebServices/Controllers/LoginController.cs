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
        protected IDbRepository<TokenArgument> tokenArguments;

        public LoginController()
        {
            this.phones = new DbRepository<Phone>(context);
            this.tokenArguments = new DbRepository<TokenArgument>(context);
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
        public async Task<HttpResponseMessage> LoginByIMEIAsync(string imei)
        {
            bool imeiExist = this.phones.All().Any(p => p.Imei == imei);

            if (imeiExist)
            {
                var tokenArgument = this.tokenArguments.All().FirstOrDefault();
                var uri = Request.IsLocal() ? Constant.LOCAL_TOKEN_URI : Constant.REMOTE_TOKEN_URI;
                string token = await RequestHelpers.GetTokenAsync(tokenArgument.UserName, tokenArgument.Password, uri);

                return Request.CreateResponse(HttpStatusCode.OK, new { access_token = token });
            }

            return Request.CreateResponse(HttpStatusCode.NoContent);

        }
    }
}

