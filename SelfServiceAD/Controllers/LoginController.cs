using System.Web.Mvc;

namespace SelfServiceAD.Controllers
{
    using System.Net.Configuration;

    using Helpers;

    using Models;

    using ViewModels;

    public class LoginController : Controller
    {

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var logon = new ActiveDirectory(model.Username);
                var response = logon.Logon(model.Password);

                if (response == WindowsLogonResponse.Successful
                    || response == WindowsLogonResponse.PasswordChangeRequired) Session["Username"] = model.Username;

                if (response == WindowsLogonResponse.Successful) return RedirectToAction("Index", "Home");
                if (response == WindowsLogonResponse.PasswordChangeRequired) return RedirectToAction("ChangePassword", "Home");

                ModelState.AddModelError("Invalid", "Invalid username and/or password");
                return View();
            }

            return View();
        }
    }
}