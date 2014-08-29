using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace BugTracker.Models
{
	public class TicketCommentViewModel
	{
		private BugTrackerEntities db = new BugTrackerEntities();

		public string Comment { get; set; }
		public string Author { get; set; }
		public string AuthorId { get; set; }
        public string AuthorPic { get; set; }
        public string DateCreated { get; set; }

		public TicketCommentViewModel() { }

		public TicketCommentViewModel(TicketComment ticketComment) : this()
		{
			this.Comment = ticketComment.Comment;
			this.Author = ticketComment.AspNetUser.UserName;
			this.AuthorId = ticketComment.AspNetUser.Id;
            this.AuthorPic = "/img/avatars/" + Path.GetFileName(ticketComment.AspNetUser.AvatarFilePath);
            this.DateCreated = ticketComment.Created.ToLocalTime().ToString("MMM\nd");
		}
	}

	public class NewTicketCommentViewModel
	{
		[Required(ErrorMessage="A Comment is Required!")]
        [MinLength(10, ErrorMessage="A Comment must be at least 10 characters in length!")]
		public string Comment { get; set; }

		public NewTicketCommentViewModel() { }
	}
}