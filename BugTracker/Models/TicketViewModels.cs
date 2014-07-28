using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class TicketViewModel
    {
        private BugTrackerEntities1 db = new BugTrackerEntities1();
        private ApplicationDbContext Db = new ApplicationDbContext();

        public TicketViewModel() 
		{
			this.Comments = new List<TicketCommentViewModel>();
		}

        public TicketViewModel(Ticket ticket) : this()
        {
            this.ID = ticket.ID;
            var submitter = Db.Users.First(m => m.Id == ticket.SubmitterID);
            this.Submitter = submitter.UserName;
            this.Title = ticket.Title;
            this.Description = ticket.Description;

            var assignee = Db.Users.First(m => m.Id == ticket.AssigneeID);
            this.Assignee = (assignee != null) ? assignee.UserName : "Unassigned";

            this.Project = ticket.Project.Name;
            this.Priority = ticket.TicketPriority.Name;
            this.Status = ticket.TicketStatus.Name;
            this.Type = ticket.TicketType.Name;

			// Form a list of TicketComments
			foreach (var item in ticket.TicketComments)
			{
				this.Comments.Add(new TicketCommentViewModel(item));
			}
			
        }

        public int ID { get; set; }

        [Display(Name = "Submitter")]
        public string Submitter { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Assignee")]
        public string Assignee { get; set; }

        [Display(Name = "Project")]
        public string Project { get; set; }

        [Display(Name = "Priority")]
        public string Priority { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

		[Display(Name = "Comments")]
		public ICollection<TicketCommentViewModel> Comments { get; set; }
    }

    public class NewTicketViewModel
    {
        public NewTicketViewModel() { }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Assignee")]
        public SelectList Assignees { get; set; }
        public string Assignee { get; set; }

        [Display(Name = "Priority")]
        public SelectList Priorities { get; set; }
        public string Priority { get; set; }

        [Display(Name = "Status")]
        public SelectList Statuses { get; set; }
        public string Status { get; set; }

        [Display(Name = "Type")]
        public SelectList Types { get; set; }
        public string Type { get; set; }
    }

    public class EditTicketViewModel
    {
        private BugTrackerEntities1 db = new BugTrackerEntities1();
        private ApplicationDbContext Db = new ApplicationDbContext();

        public int ID { get; set; }

        [Display(Name = "Submitter")]
        public string Submitter { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Assignee")]
        public SelectList Assignees { get; set; }
        public string Assignee { get; set; }

		[Display(Name = "Project")]
		public string Project { get; set; }

        [Display(Name = "Priority")]
        public SelectList Priorities { get; set; }
        public string Priority { get; set; }

        [Display(Name = "Status")]
        public SelectList Statuses { get; set; }
        public string Status { get; set; }

        [Display(Name = "Type")]
        public SelectList Types { get; set; }
        public string Type { get; set; }

        public EditTicketViewModel() { }

        public EditTicketViewModel(Ticket ticket) : this()
        {
            this.ID = ticket.ID;
            this.Submitter = Db.Users.First(m => m.Id == ticket.SubmitterID).UserName;
            this.Title = ticket.Title;
            this.Description = ticket.Description;
            this.Assignees = new SelectList(Db.Users, "ID", "UserName", ticket.AssigneeID);
			this.Project = ticket.Project.Name;
            this.Priorities = new SelectList(db.TicketPriorities, "ID", "Name", ticket.PriorityID);
            this.Statuses = new SelectList(db.TicketStatuses, "ID", "Name", ticket.StatusID);
            this.Types = new SelectList(db.TicketTypes, "ID", "Name", ticket.TypeID);
        }

        public void UpdateTicket(Ticket ticket)
        {
            ticket.Title = this.Title;
            ticket.Description = this.Description;
            ticket.AssigneeID = this.Assignee;
            ticket.PriorityID = Int32.Parse(this.Priority);
            ticket.StatusID = Int32.Parse(this.Status);
            ticket.TypeID = Int32.Parse(this.Type);
        }

    }
}