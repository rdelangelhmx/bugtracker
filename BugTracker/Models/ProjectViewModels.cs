using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

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
		[Display(Name = "Name")]
		public string Name { get; set; }

		[Display(Name = "Project Manager")]
		public string Manager { get; set; }

		[Display(Name = "Name")]
		public string Creator { get; set; }

		[Display(Name = "Members")]
		public ICollection<AspNetUser> Members { get; set; }

		public CreateProjectViewModel()
		{

		}

		public Project GetProject() 
		{
			Project project = new Project() {
				Name = this.Name,
				Manager = this.Manager,
				Creator = this.Creator,
				AspNetUsers = this.Members
			};
			return project;
		}
	}
}