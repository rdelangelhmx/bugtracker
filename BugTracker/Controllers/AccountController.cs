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

namespace BugTracker.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private ApplicationUserManager _userManager;

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
		public ActionResult Index()
		{
			var Db = new ApplicationDbContext();

			var users = Db.Users;
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
			ApplicationUser user = UserManager.FindByName(username);
			EditUserViewModel model = new EditUserViewModel(user);

			return View(model);
		}

		//
		// GET: /Account/Create
		public ActionResult New()
		{
			return View();
		}

		//
		// POST: /Account/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				ApplicationUser user = model.GetUser();
				if (user.UserName == null)
				{
					user.UserName = user.Email;
				}

				IdentityResult result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
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
		// GET: /Account/Edit/5
		public ActionResult Edit(string username)
		{
			//var Db = new ApplicationDbContext();
			////var user = Db.Users.FirstOrDefault(m => m.UserName == username);
			ApplicationUser user = UserManager.FindByName(username);

			return View(new EditUserViewModel(user));
		}

		//
		// POST: /Account/Edit/5
		[HttpPut]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Update(string UserId, EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByIdAsync(model.UserId);
				user.UserName = model.UserName;
				user.Email = model.Email;

				var result = await UserManager.UpdateAsync(user);
				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					AddErrors(result);
				}
			}
			var errors = ModelState.Values.SelectMany(v => v.Errors);
			// If we got this far, something failed, redisplay form
			TempData["message"] = "Something went wrong!";
			return View(model);
		}

		//
		// GET: /Account/Delete/:id
		public ActionResult Delete(string id)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.FirstOrDefault(m => m.Id == id);

			return View(new EditUserViewModel(user));
		}

		//
		// POST: /Account/Delete/:id
		[HttpDelete]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Destroy(EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByIdAsync(model.UserId);
				var result = await UserManager.DeleteAsync(user);

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay the view
			return View(model);
		}

		// GET: /Account/ListRoles
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

		//
		// GET: /Account/UserRoles/:id
		public ActionResult UserRoles(string id)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.FirstOrDefault(m => m.Id == id);


			return View(new AssignUsersViewModel(user));
		}

		//
		// POST: /Account/UserRoles/:id
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> UserRoles(AssignUsersViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindByIdAsync(model.Id);
				user.Roles.Clear();

				foreach (var item in model.ItemList) // iterate through the list of SelectUserViewModel objects
				{
					if (item.Selected)
					{
						IdentityUserRole userRole = new IdentityUserRole();
						userRole.UserId = user.Id;
						userRole.RoleId = item.Id;

						user.Roles.Add(userRole);
					}
				}

				var request = await UserManager.UpdateAsync(user);

				if (request.Succeeded)
				{
					return RedirectToAction("Index", "Account", new { id = user.Id });
				}
				else
				{
					AddErrors(request);
				}
			}

			// If we got this far, something failed, redisplay view
			return View(model);
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
				var user = model.GetUser(); // Gets user object created from submitted form data held in 'model'
				if (user.UserName == null)
				{
					user.UserName = user.Email;
				}
				IdentityResult result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					// Automatically signin newly registered user
					//await SignInAsync(user, isPersistent: false);

					// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
					// Send an email with this link
					string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
					var callbackUrl = Url.Action(
						"ConfirmEmail",
						"Account",
						new { userId = user.Id, code = code },
						protocol: Request.Url.Scheme);

					await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

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
		// POST: /Account/Disassociate
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
		{
			ManageMessageId? message = null;
			IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
			if (result.Succeeded)
			{
				var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
				await SignInAsync(user, isPersistent: false);
				message = ManageMessageId.RemoveLoginSuccess;
			}
			else
			{
				message = ManageMessageId.Error;
			}
			return RedirectToAction("Manage", new { Message = message });
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
			return View();
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
		// POST: /Account/ExternalLogin
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public ActionResult ExternalLogin(string provider, string returnUrl)
		{
			// Request a redirect to the external login provider
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
		}

		//
		// GET: /Account/ExternalLoginCallback
		[AllowAnonymous]
		public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
			if (loginInfo == null)
			{
				return RedirectToAction("Login");
			}

			// Sign in the user with this external login provider if the user already has a login
			var user = await UserManager.FindAsync(loginInfo.Login);
			if (user != null)
			{
				await SignInAsync(user, isPersistent: false);
				return RedirectToLocal(returnUrl);
			}
			else
			{
				// If the user does not have an account, then prompt the user to create an account
				ViewBag.ReturnUrl = returnUrl;
				ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
				return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
			}
		}

		//
		// POST: /Account/LinkLogin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LinkLogin(string provider)
		{
			// Request a redirect to the external login provider to link a login for the current user
			return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
		}

		//
		// GET: /Account/LinkLoginCallback
		public async Task<ActionResult> LinkLoginCallback()
		{
			var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
			if (loginInfo == null)
			{
				return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
			}
			IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
			if (result.Succeeded)
			{
				return RedirectToAction("Manage");
			}
			return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
		}

		//
		// POST: /Account/ExternalLoginConfirmation
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Manage");
			}

			if (ModelState.IsValid)
			{
				// Get the information about the user from the external login provider
				var info = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (info == null)
				{
					return View("ExternalLoginFailure");
				}
				var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
				IdentityResult result = await UserManager.CreateAsync(user);
				if (result.Succeeded)
				{
					result = await UserManager.AddLoginAsync(user.Id, info.Login);
					if (result.Succeeded)
					{
						await SignInAsync(user, isPersistent: false);

						// For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
						// Send an email with this link
						// string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
						// var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
						// SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

						return RedirectToLocal(returnUrl);
					}
				}
				AddErrors(result);
			}

			ViewBag.ReturnUrl = returnUrl;
			return View(model);
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("users/logout")]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		//
		// GET: /Account/ExternalLoginFailure
		[AllowAnonymous]
		public ActionResult ExternalLoginFailure()
		{
			return View();
		}

		[ChildActionOnly]
		public ActionResult RemoveAccountList()
		{
			var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
			ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
			return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
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
			AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
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