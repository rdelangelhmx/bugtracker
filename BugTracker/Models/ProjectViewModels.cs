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

        private BugTrackerEntities _db = new BugTrackerEntities();

        public int ID { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "# Tickets")]
        public string NumberTickets { get; set; }

        [Display(Name = "# Members")]
        public string NumberMembers { get; set; }

        [Display(Name = "Role")]
        public string ProjectMemberRole { get; set; }

        [Display(Name = "Project Manager")]
        public string ProjectManager { get; set; }

        public ProjectViewModel() { }

        public ProjectViewModel(UserProjectRole userProjectRole, string userId) : this()
        {
            var project = _db.Projects.FirstOrDefault(p => p.ID == userProjectRole.ProjectID);

            this.ID = project.ID;
            this.Name = project.Name;
            this.ProjectManager = project.Manager;
            this.NumberTickets = project.Tickets.Count().ToString();
            this.NumberMembers = project.UserProjectRoles.Count().ToString();

            this.ProjectMemberRole = _db.AspNetRoles.FirstOrDefault(r => r.Id == userProjectRole.RoleID).Name;
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

        public string ProjectManager { get; set; }

		public IEnumerable<EditUserViewModel> Members { get; set; }

        [Display(Name = "Tickets")]
        public IEnumerable<TicketViewModel> ProjectTickets { get; set; }

        public DetailsProjectViewModel() { }

        public DetailsProjectViewModel(Project project, IEnumerable<TicketViewModel> tvm, IEnumerable<EditUserViewModel> editUserViewModel) : this ()
        {
            this.ID = project.ID;
            this.Name = project.Name;
            this.ProjectManager = project.Manager;
            this.NumberTickets = project.Tickets.Count().ToString();
            this.NumberMembers = project.UserProjectRoles.Count().ToString();
            this.ProjectTickets = tvm;
			this.Members = editUserViewModel;
        }

        public bool isProjectManager(string userID)
        {
            return ProjectManager == userID;
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

        private BugTrackerEntities _db = new BugTrackerEntities();

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
            var developerRoleID = _db.AspNetRoles.FirstOrDefault(r => r.Name == "Developer").Id;
			this.SelectedUsers = project.UserProjectRoles.Where(u => u.RoleID == developerRoleID).Select(u => u.UserID).ToList();
		}
	}

}