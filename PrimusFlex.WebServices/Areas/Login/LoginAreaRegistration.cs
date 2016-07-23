using System.Web.Mvc;

namespace PrimusFlex.WebServices.Areas.Login
{
    public class LoginAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Login";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Login_default",
                "Login/{action}/{id}",
                new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}