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
        //protected IDbRepository<DataForToken> dataForToken;

        public LoginController()
        {
            this.phones = new DbRepository<Phone>(context);
            //this.dataForToken = new DbRepository<DataForToken>(context);
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

        //// DELETE api/login/logout?imei=xxxx
        //[Route("logout")]
        //public HttpResponseMessage Delete(string imei)
        //{
        //    var phone = this.phones.All().FirstOrDefault(p => p.IMEI == imei);
        //    if (phone != null)
        //    {
        //        this.phones.HardDelete(phone);
        //        this.phones.Save();

        //        return Request.CreateResponse(HttpStatusCode.OK, new { message = "You was loged out successfully." });
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Invalid IMEI number. Logout unsuccessful." });
        //}

        //// GEt api/login/byImei?imei=xxxx
        //[Route("byimei")]
        //[AllowAnonymous]
        //[HttpGet]
        //public async Task<HttpResponseMessage> LoginByIMEIAsync(string imei)
        //{
        //    bool imeiExist = this.phones.All().Any(p => p.IMEI == imei);

        //    if (imeiExist)
        //    {
        //        var dataForToken = this.dataForToken.All().FirstOrDefault();

        //        string token = await RequestHelper.GetTokenAsync(dataForToken.UserName, dataForToken.Password);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { access_token = token });
        //    }

        //    return Request.CreateResponse(HttpStatusCode.NoContent);

        //}
    }
}

