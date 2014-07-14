using System.Web.Mvc;

namespace SelfServiceAD.Controllers
{
    using System.Net.Configuration;

    using Helpers;

    using Models;

    using Views.Auth;

    public class AuthController : Controller
    {

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var logon = new ActiveDirectory(model.Username);
                var response = logon.Logon(model.Password);

                if (response == WindowsLogonResponse.Successful
                    || response == WindowsLogonResponse.PasswordChangeRequired) WebsiteUser.Login(model.Username);

                if (response == WindowsLogonResponse.Successful) return RedirectToAction("Index", "Home");
                if (response == WindowsLogonResponse.PasswordChangeRequired)
                {
                    TempData["Notice"] = "You are required to change your password.";
                    return RedirectToAction("ChangePassword", "Home");
                }

                ModelState.AddModelError("Invalid", "Invalid username and/or password");
                return View();
            }

            return View();
        }

        public ActionResult Logout()
        {
            WebsiteUser.Logout();
            TempData["Notice"] = "You have been successfully logged out. Close your browser for the best security.";
            return RedirectToAction("Login");
        }
    }
}