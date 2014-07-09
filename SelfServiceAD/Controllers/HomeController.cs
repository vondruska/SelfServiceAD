using System;
using System.Web.Mvc;

namespace SelfServiceAD.Controllers
{
    using System.DirectoryServices.AccountManagement;

    using Helpers;

    using Models;

    using ViewModels;

    [ForceLogon]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var ad = new ActiveDirectory((string)Session["Username"]);

            var user = ad.GetUserPrincipal();

            if (user == null) return RedirectToAction("Logout", "Auth");

            var entry = (System.DirectoryServices.DirectoryEntry)user.GetUnderlyingObject();

            var native = (ActiveDs.IADsUser)entry.NativeObject;

            return
                View(
                    new UserViewModel
                    {
                        PasswordExpiration =
                            native.PasswordExpirationDate <= new DateTime(1980, 1, 1)
                                ? null
                                : (DateTime?)native.PasswordExpirationDate,
                        DisplayName = user.DisplayName,
                        EmailAddress = user.EmailAddress,
                        LastPasswordSet = user.LastPasswordSet,
                        PasswordNeverExpires = user.PasswordNeverExpires
                    });
        }

        public ActionResult ChangePassword()
        {
            // let's check to see if the user is allowed to change their password
            var ad = new ActiveDirectory((string)Session["Username"]);
            return ad.UserCannotChangePassword() ? View("UnableToChangePassword") : View(new ChangePasswordViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            // let's check to see if the user is allowed to change their password
            var ad = new ActiveDirectory((string)Session["Username"]);
            if (ad.UserCannotChangePassword()) return View("UnableToChangePassword");

            if (!ModelState.IsValid)
            {
                return View(new ChangePasswordViewModel());
            }
         
            try
            {
                ad.ChangePassword(model.OldPassword, model.NewPassword);
                TempData["Success"] = "Password successfully changed!";
                return RedirectToAction("Index");
            }
            catch (PasswordException)
            {
                ModelState.AddModelError("PasswordFailed", "Changing of your password failed. Please ensure the new password meets complexity requirements, hasn't been used previously, or if the old password does not match.");
            }

            return View(new ChangePasswordViewModel());
        }
    }
}