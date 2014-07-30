using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class ExternalLoginListViewModel
	{
		public string Action { get; set; }
		public string ReturnUrl { get; set; }
	}

	public class ManageUserViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Username or Email")]
		public string UserName { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		public string UserId { get; set; }

		[Display(Name = "Username")]
		[RegularExpression(@"^[a-zA-Z0-9]*$")]
		public string UserName { get; set; }

		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		// Return a pre-populated instance of ApplicationUser;
		public AspNetUser GetUser()
		{
			var user = new AspNetUser()
			{
				UserName = this.UserName,
				FirstName = this.FirstName,
				LastName = this.LastName,
				Email = this.Email
			};
			this.UserId = user.Id;
			return user;
		}
	}

	public class ResetPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class EditUserViewModel
	{
		public EditUserViewModel() { }

		public EditUserViewModel(AspNetUser user) : this()
		{
			this.UserId = user.Id;
			this.UserName = user.UserName;
			this.FirstName = user.FirstName;
			this.LastName = user.LastName;
			this.Email = user.Email;
			this.Website = user.Website;
			this.About = user.About;
		}

		public string UserId { get; set; }

		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Display(Name = "Email Address")]
		[EmailAddress]
		public string Email { get; set; }

		[Display(Name = "Website")]
		public string Website { get; set; }

		[Display(Name = "About You")]
		public string About { get; set; }
	}

	public class RolesViewModel
	{
		public RolesViewModel() { }

		public RolesViewModel(IdentityRole role)
			: this()
		{
			this.RoleName = role.Name;
			this.RoleId = role.Id;
		}

		[Display(Name = "Role Name")]
		public string RoleName { get; set; }
		public string RoleId { get; set; }
	}

	public class UserProfileViewModel
	{
		public UserProfileViewModel() { }

		public UserProfileViewModel(ApplicationUser user) : this()
		{
			this.UserId = user.Id;
			this.UserName = user.UserName;
			this.Email = user.Email;
		}

		public string UserId { get; set; }

		[Display(Name = "Username")]
		public string UserName { get; set; }

		[Display(Name = "Email")]
		[EmailAddress]
		public string Email { get; set; }
	}

	public class UserSettingsViewModel
	{
		public EditUserViewModel EditUserViewModel { get; set; }
		public ManageUserViewModel ManageUserViewModel { get; set; }
	}
}
