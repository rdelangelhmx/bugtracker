﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ListTicketsViewModel
    {
        public ListTicketsViewModel() { }

        public ListTicketsViewModel(Ticket ticket) : this()
        {
            var db = new ApplicationDbContext();

            this.ID = ticket.ID;
            var submitter = db.Users.First(m => m.Id == ticket.SubmitterID);
            this.Submitter = submitter.UserName;
            this.Title = ticket.Title;
            this.Description = ticket.Description;

            var assignee = db.Users.First(m => m.Id == ticket.AssigneeID);
            this.Assignee = (assignee != null) ? assignee.UserName : "Unassigned";

            this.Project = ticket.Project.Name;
            this.Priority = ticket.TicketPriority.Name;
            this.Status = ticket.TicketStatus.Name;
            this.Type = ticket.TicketType.Name;
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
    }

    public class CreateTicketViewModel
    {
        public CreateTicketViewModel() { }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Assignee")]
        public SelectList Assignees { get; set; }
        public string Assignee { get; set; }

        [Display(Name = "Project")]
        public SelectList Projects { get; set; }
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
    }
}