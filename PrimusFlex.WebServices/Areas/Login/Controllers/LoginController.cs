namespace PrimusFlex.WebServices.Areas.Login.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using System.Threading.Tasks;

    using PrimusFlex.WebServices.Models;
    using Common;
    using Web.Infrastructure.Helpers;

    [Authorize]
    public class LoginController : Controller
    {
        // GET: Login/Index
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Login/Start
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Start(LoginBindingModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", model);
            }

            var uri = Request.IsLocal ? Constant.LOCAL_TOKEN_URI : Constant.REMOTE_TOKEN_URI;
            var token = await RequestHelpers.GetTokenAsync(model.Email, model.Password, uri);

            if(token != null)
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", model);
        }
    }
}