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
		private BugTrackerEntities1 db = new BugTrackerEntities1();

		public string Comment { get; set; }
		public string Author { get; set; }
		public string AuthorId { get; set; }
        public string AuthorPic { get; set; }

		public TicketCommentViewModel() { }

		public TicketCommentViewModel(TicketComment ticketComment) : this()
		{
			this.Comment = ticketComment.Comment;
			this.Author = ticketComment.AspNetUser.UserName;
			this.AuthorId = ticketComment.AspNetUser.Id;
            this.AuthorPic = "/img/avatars/" + Path.GetFileName(ticketComment.AspNetUser.AvatarFilePath);
		}
	}

	public class NewTicketCommentViewModel
	{
		[Required]
		[Display(Name = "Comment")]
		public string Comment { get; set; }

		public NewTicketCommentViewModel() { }
	}
}