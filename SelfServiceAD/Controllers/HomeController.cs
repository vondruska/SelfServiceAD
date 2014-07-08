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
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                var user = UserPrincipal.FindByIdentity(pc, (string)Session["Username"]);
                if (user == null)
                    return RedirectToAction("Index", "Login");

                var entry =
                    (System.DirectoryServices.DirectoryEntry)user.GetUnderlyingObject();

                var native = (ActiveDs.IADsUser)entry.NativeObject;
                DateTime? passwordExpiration = native.PasswordExpirationDate;

                if (passwordExpiration.Value <= new DateTime(1980, 1, 1)) passwordExpiration = null;

                return
                    View(
                        new UserViewModel
                        {
                            PasswordExpiration = passwordExpiration,
                            DisplayName = user.DisplayName,
                            EmailAddress = user.EmailAddress,
                            LastPasswordSet = user.LastPasswordSet,
                            PasswordNeverExpires = user.PasswordNeverExpires
                        });
            }
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(new ChangePasswordViewModel());
            }

            var ad = new ActiveDirectory((string)Session["Username"]);
         
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