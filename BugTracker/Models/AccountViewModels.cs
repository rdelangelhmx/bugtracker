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
		public ApplicationUser GetUser()
		{
			var user = new ApplicationUser()
			{
				UserName = this.UserName,
				Email = this.Email,
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

		public EditUserViewModel(ApplicationUser user)
			: this()
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
}
