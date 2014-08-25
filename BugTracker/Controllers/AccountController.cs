using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using BugTracker.Models;
using System.Net;
using System.Data.Entity;
using System.IO;

namespace BugTracker.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private ApplicationUserManager _userManager;
		private BugTrackerEntities db = new BugTrackerEntities();

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/ListUsers
        [Route("admin/users", Name="adminListUsers")]
        [Authorize(Roles="Admin")]
		public ActionResult Index()
		{
			List<AspNetUser> users = db.AspNetUsers.ToList();

			var model = new List<EditUserViewModel>();
			foreach (var user in users)
			{
				var u = new EditUserViewModel(user);
				model.Add(u);
			}
			return View(model);
		}

		public ActionResult Show(string username)
		{
			//ApplicationUser user = UserManager.FindByName(username);
			AspNetUser user = db.AspNetUsers.FirstOrDefault(u => u.UserName == username);
			EditUserViewModel model = new EditUserViewModel(user);

			return View(model);
		}

		//
		// PUT: users/{username}
		[HttpPut]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Update(string username, EditUserViewModel model, HttpPostedFileBase AvatarFile)
		{
			if (ModelState.IsValid)
			{
				var user = db.AspNetUsers.FirstOrDefault(u => u.UserName == username);
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.Email = model.Email;
				user.Website = model.Website;
				user.About = model.About;

				if ((AvatarFile != null) && (AvatarFile.ContentLength > 0) && !String.IsNullOrEmpty(AvatarFile.FileName))
				{
					string FileName = AvatarFile.FileName;
					string AvatarFilePath = Path.Combine(Server.MapPath("~\\img\\avatars\\"), FileName);
					AvatarFile.SaveAs(AvatarFilePath);

					// Assign Avatar file path to user model
					user.AvatarFilePath = AvatarFilePath;
				}

				db.Entry(user).State = EntityState.Modified;
				await db.SaveChangesAsync();

				return RedirectToAction("Manage", new { username = username });
			}

			// If we got this far, something failed, redisplay form
			TempData["message"] = "Something went wrong!";
			return View("Manage", model);
		}

		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				// Check if User signed in with email or username
				ApplicationUser user = new ApplicationUser();
				if (model.UserName.Contains('@'))
				{
					// In this case, the user logged in with their email
					model.Email = model.UserName;
					var userByEmail = await UserManager.FindByEmailAsync(model.Email);
					user = await UserManager.FindAsync(userByEmail.UserName, model.Password);
				}
				else
				{
					// In this case, the user signed in with their username
					user = await UserManager.FindAsync(model.UserName, model.Password);
				}


				if (user != null)
				{
					await SignInAsync(user, model.RememberMe);
					returnUrl = returnUrl ?? "/users/" + user.UserName;
					return RedirectToLocal(returnUrl);
				}
				else
				{
					ModelState.AddModelError("", "Invalid username or password.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Register
		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser applicationUser = model.GetUser(); // Gets user object created from submitted form data held in 'model'

				IdentityResult result = await UserManager.CreateAsync(applicationUser, model.Password);
				if (result.Succeeded)
				{
					AspNetUser user = db.AspNetUsers.FirstOrDefault(u => u.Id == applicationUser.Id);
					user.FirstName = model.FirstName;
					user.LastName = model.LastName;
                    user.AvatarFilePath = Server.MapPath("~\\img\\avatars\\default-avatar.png");
					db.Entry(user).State = EntityState.Modified;
					db.SaveChanges();

					// Automatically signin newly registered user
					await SignInAsync(applicationUser, isPersistent: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					string code = await UserManager.GenerateEmailConfirmationTokenAsync(applicationUser.Id);
					var callbackUrl = Url.Action(
						"ConfirmEmail",
						"Account",
						new { userId = user.Id, code = code },
						protocol: Request.Url.Scheme);

					await UserManager.SendEmailAsync(applicationUser.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

					TempData["message"] = "Thank you for registratering!. Please confirm your account before signing in.";
					return RedirectToAction("Index", "Home");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ConfirmEmail
		[AllowAnonymous]
		public async Task<ActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return View("Error");
			}

			IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
			if (result.Succeeded)
			{
				return View("ConfirmEmail");
			}
			else
			{
				AddErrors(result);
				return View();
			}
		}

		//
		// GET: /Account/ForgotPassword
		[AllowAnonymous]
		public ActionResult ForgotPassword()
		{
			return View();
		}

		//
		// POST: /Account/ForgotPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByEmailAsync(model.Email);
				if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
				{
					ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
					return View();
				}

				// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
				// Send an email with this link
				string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
				var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
				//await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
				return RedirectToAction("ForgotPasswordConfirmation", "Account");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ForgotPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/ResetPassword
		[AllowAnonymous]
		public ActionResult ResetPassword(string code)
		{
			if (code == null)
			{
				return View("Error");
			}
			return View();
		}

		//
		// POST: /Account/ResetPassword
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					ModelState.AddModelError("", "No user found.");
					return View();
				}
				IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("ResetPasswordConfirmation", "Account");
				}
				else
				{
					AddErrors(result);
					return View();
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/ResetPasswordConfirmation
		[AllowAnonymous]
		public ActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		//
		// GET: /Account/Manage
		[Route("users/{username:alpha}/settings", Name="userSettings")]
		public ActionResult Manage(string username, ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ManageMessageId.Error ? "An error has occurred."
				: "";
			ViewBag.HasLocalPassword = HasPassword();
			ViewBag.ReturnUrl = Url.Action("Manage");

			var user = db.AspNetUsers.FirstOrDefault(u => u.UserName == username);
			EditUserViewModel userModel = new EditUserViewModel(user);
			ManageUserViewModel manageUserModel = new ManageUserViewModel();

			UserSettingsViewModel model = new UserSettingsViewModel(userModel, manageUserModel);

			return View(model);
		}

		//
		// POST: /Account/Manage
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			bool hasPassword = HasPassword();
			ViewBag.HasLocalPassword = hasPassword;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasPassword)
			{
				if (ModelState.IsValid)
				{
					IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
						await SignInAsync(user, isPersistent: false);
						return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
					}
					else
					{
						AddErrors(result);
					}
				}
			}
			else
			{
				// User does not have a password so remove any validation errors caused by a missing OldPassword field
				ModelState state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
					}
					else
					{
						AddErrors(result);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		//[Route("users/logout")]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

        /* 
         * 
         * ROLES!
         * 
         * */
        // GET: /Account/ListRoles
        [Route("admin/roles", Name="rolesList")]
        [Authorize(Roles="Admin")]
        public ActionResult ListRoles()
        {
            var Db = new ApplicationDbContext(); // Create database instance
            var roles = Db.Roles; // Return list of IdentityRole objects
            var model = new List<RolesViewModel>();

            foreach (var item in roles)
            {
                var r = new RolesViewModel(item);
                model.Add(r);
            }

            return View(model);
        }

        //
        // GET: /Account/CreateRole
        public ActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        //[Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRole(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                var result = rm.Create(new IdentityRole(model.RoleName));

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/EditRole/:id
        public ActionResult EditRole(string id)
        {
            var Db = new ApplicationDbContext();
            var role = Db.Roles.FirstOrDefault(m => m.Id == id);

            return View(new RolesViewModel(role));
        }

        //
        // POST: /Account/EditRole/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditRole(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

                var role = await rm.FindByIdAsync(model.RoleId);
                role.Name = model.RoleName;

                var result = await rm.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/DeleteRole/:id
        public ActionResult DeleteRole(string id)
        {
            var Db = new ApplicationDbContext();
            var role = Db.Roles.FirstOrDefault(m => m.Id == id);

            return View(new RolesViewModel(role));
        }

        //
        // POST: /Account/DeleteRole/:id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRole(RolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                var role = await rm.FindByIdAsync(model.RoleId);
                var result = await rm.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we get this far, something failed, redisplay form
            return View(model);
        }

		protected override void Dispose(bool disposing)
		{
			if (disposing && UserManager != null)
			{
				UserManager.Dispose();
				UserManager = null;
			}
			base.Dispose(disposing);
		}

		#region Helpers
		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await user.GenerateUserIdentityAsync(UserManager);

            AspNetUser userEntity = await db.AspNetUsers.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            string avatarPath = "/img/avatars/" + Path.GetFileName(userEntity.AvatarFilePath);

            identity.AddClaim(new Claim("AvatarImage", avatarPath));
			AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private bool HasPassword()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user != null)
			{
				return user.PasswordHash != null;
			}
			return false;
		}

		private void SendEmail(string email, string callbackUrl, string subject, string message)
		{
			// For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			Error
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		private class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}
		#endregion
	}
}