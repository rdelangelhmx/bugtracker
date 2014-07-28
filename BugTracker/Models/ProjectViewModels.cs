using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class ProjectViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "# Tickets")]
        public string NumberTickets { get; set; }

        [Display(Name = "# Members")]
        public string NumberMembers { get; set; }

        public ProjectViewModel() { }

        public ProjectViewModel(Project project) : this()
        {
            this.ID = project.ID;
            this.Name = project.Name;
            this.NumberTickets = project.Tickets.Count().ToString();
            this.NumberMembers = project.AspNetUsers.Count().ToString();
        }
    }

    public class DetailsProjectViewModel 
    {
        public int ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "# Tickets")]
        public string NumberTickets { get; set; }

        [Display(Name = "# Members")]
        public string NumberMembers { get; set; }

        [Display(Name = "Tickets")]
        public IEnumerable<TicketViewModel> ProjectTickets { get; set; }

        public DetailsProjectViewModel() { }

        public DetailsProjectViewModel(Project project, IEnumerable<TicketViewModel> tvm) : this ()
        {
            this.ID = project.ID;
            this.Name = project.Name;
            this.NumberTickets = project.Tickets.Count().ToString();
            this.NumberMembers = project.AspNetUsers.Count().ToString();
            this.ProjectTickets = tvm;
        }
    }

	public class CreateProjectViewModel
	{
		[Required]
		[Display(Name = "Project Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Project Manager")]
		public string Manager { get; set; }

		[Display(Name = "Users")]
		public IEnumerable<SelectListItem> Users { get; set; }

		[Display(Name = "Members")]
		public List<string> SelectedUsers { get; set; }

		public CreateProjectViewModel()
		{

		}
	}

	public class EditProjectViewModel
	{
		[Required]
		public int ID { get; set; }

		[Required]
		[Display(Name = "Project Name")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Project Manager")]
		public string Manager { get; set; }

		[Display(Name = "Users")]
		public IEnumerable<SelectListItem> Users { get; set; }

		[Display(Name = "Members")]
		public List<string> SelectedUsers { get; set; }

		public EditProjectViewModel()
		{
			this.Users = new List<SelectListItem>();
			this.SelectedUsers = new List<string>();
		}

		public EditProjectViewModel(Project project)
			: this()
		{
			this.ID = project.ID;
			this.Name = project.Name;
			this.Manager = project.Manager;
			this.SelectedUsers = project.AspNetUsers.Select(u => u.Id).ToList();
		}
	}

}