using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
	public class TicketCommentViewModel
	{
		private BugTrackerEntities1 db = new BugTrackerEntities1();

		public string Comment { get; set; }

		[Display(Name="By: ")]
		public string Author { get; set; }
		public string AuthorId { get; set; }

		public TicketCommentViewModel() { }

		public TicketCommentViewModel(TicketComment ticketComment) : this()
		{
			this.Comment = ticketComment.Comment;
			this.Author = ticketComment.AspNetUser.UserName;
			this.AuthorId = ticketComment.AspNetUser.Id;
		}
	}

	public class NewTicketCommentViewModel
	{
		[Required]
		[Display(Name = "Comment")]
		public string Comment { get; set; }

		[Required]
		[Display(Name = "Author")]
		public string Author { get; set; }

		public NewTicketCommentViewModel() { }
	}
}