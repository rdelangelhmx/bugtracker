using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker.Models;

namespace BugTracker.Controllers
{
	[Authorize]
    public class ProjectsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

		// GET: accounts/{accountUsername}/projects
        public ActionResult Index(string accountUsername)
        {
            ViewBag.AccountUserName = accountUsername;
            var users = db.AspNetUsers.Include("UserProjectRoles");
            var user = users.Where(u => u.UserName == accountUsername).Single();

            
            var projects = user.UserProjectRoles;
            List<ProjectViewModel> model = new List<ProjectViewModel>();
            foreach (var item in projects)
            {
                model.Add(new ProjectViewModel(item, user.Id));
            }

			if (Request.IsAjaxRequest())
			{
				return Json(model, JsonRequestBehavior.AllowGet);
			}
			else
			{
				return View(model);
			}
        }

        // GET: projects/5
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);

            if (project == null)
            {
                return HttpNotFound();
            }

            List<TicketViewModel> projectTicketViewModels = new List<TicketViewModel>();
            foreach (var item in project.Tickets.ToList())
            {
                projectTicketViewModels.Add(new TicketViewModel(item));
            }

			List<EditUserViewModel> projectMembers = new List<EditUserViewModel>();
            var userIDs = db.UserProjectRoles.Where(u => u.ProjectID == id).Select(u => u.UserID);
            var users = db.AspNetUsers;
			foreach (var item in userIDs.ToList())
			{
				projectMembers.Add(new EditUserViewModel(users.Find(item)));
			}

            DetailsProjectViewModel model = new DetailsProjectViewModel(project, projectTicketViewModels, projectMembers);

            return View(model);
        }

		// GET: accounts/{accountUsername}/projects/new
        public ActionResult New(string accountUsername)
		{
			IEnumerable<AspNetUser> users = db.AspNetUsers;

			AspNetUser user = users.FirstOrDefault(u => u.UserName == accountUsername);

			CreateProjectViewModel model = new CreateProjectViewModel();
			model.SelectedUsers = new List<string>();
            //model.SelectedUsers.Add(user.Id);

			model.Users = users.Select(u => new SelectListItem
			{
				Value = u.Id,
				Text = u.UserName
			});
            return View(model);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// POST: users/{accountUsername}/projects
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string accountUserName, CreateProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
				// Get currently logged in user
                IEnumerable<AspNetUser> users = db.AspNetUsers.Include(u => u.UserProjectRoles);
                IEnumerable<AspNetRole> roles = db.AspNetRoles;

				// Create new project from info contained in the CreatProjectViewModel
				Project project = new Project();
				project.Name = model.Name;
				project.Creator = User.Identity.GetUserName();
				project.Manager = model.Manager;
                db.Projects.Add(project);
                db.SaveChanges();

                // Persist Project Manager to UserProjectRoles table
                var projectManagerRoleID = roles.FirstOrDefault(r => r.Name == "Project Manager").Id;
                db.UserProjectRoles.Add(new UserProjectRole()
                {
                    UserID = users.FirstOrDefault(u => u.Id == project.Manager).Id,
                    ProjectID = project.ID,
                    RoleID = projectManagerRoleID
                });

				// Add selected users to project list
                var developerRoleID = roles.FirstOrDefault(r => r.Name == "Developer").Id;
				foreach (var item in model.SelectedUsers)
				{
                    db.UserProjectRoles.Add(new UserProjectRole()
                    {
                        UserID = item,
                        ProjectID = project.ID,
                        RoleID = developerRoleID
                    });
				}

				// Save changes to database
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
				return RedirectToAction("Index", new { accountId = accountUserName });
            }

            return View("New", model);
        }

        // GET: users/{accountUserName}/projects/{id}
        public ActionResult Edit(string accountUserName, int? id)
        {
			if (accountUserName == null && id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			IEnumerable<AspNetUser> users = db.AspNetUsers;
			ViewBag.UserName = accountUserName;

			EditProjectViewModel model = new EditProjectViewModel(db.Projects.FirstOrDefault(p=> p.ID == id));
			model.Users = users.Select(x => new SelectListItem
			{
				Value = x.Id,
				Text = x.UserName
			});

            return View(model);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// PUT: users/{accountUsername}/projects/{id}
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string accountUsername, int id, EditProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
				List<AspNetUser> users = db.AspNetUsers.ToList();
                List<AspNetRole> roles = db.AspNetRoles.ToList();
				AspNetUser user = users.FirstOrDefault(u => u.UserName == accountUsername);
				UserProjectRole userProjectRole = db.UserProjectRoles.FirstOrDefault(p => p.ProjectID == model.ID);
                Project project = db.Projects.FirstOrDefault(p => p.ID == userProjectRole.ProjectID);

                // Update Project Manager, if needed.
                if (project.Manager != model.Manager)
                {
                    project.Name = model.Name;
                    project.Manager = model.Manager;
                    var projectManagerRoleID = roles.FirstOrDefault(r => r.Name == "Project Manager").Id;
                    var projectManager = users.FirstOrDefault(u => u.Id == project.Manager);
                    
                    var projectManagerRoleToRemove = db.UserProjectRoles.Where(x => x.ProjectID == project.ID && x.RoleID == projectManagerRoleID);
                    db.UserProjectRoles.RemoveRange(projectManagerRoleToRemove);
                    db.UserProjectRoles.Add(new UserProjectRole
                    {
                        UserID = project.Manager,
                        ProjectID = project.ID,
                        RoleID = projectManagerRoleID
                    });
                }

                // Update Developers
                var projectDeveloperRoleID = roles.FirstOrDefault(r => r.Name == "Developer").Id;
                var projectDeveloperRolesToRemove = db.UserProjectRoles.Where(x => x.ProjectID == project.ID && x.RoleID == projectDeveloperRoleID);
                db.UserProjectRoles.RemoveRange(projectDeveloperRolesToRemove);
				foreach (var item in model.SelectedUsers)
				{
                    db.UserProjectRoles.Add(new UserProjectRole
                    {
                        UserID = item,
                        ProjectID = project.ID,
                        RoleID = projectDeveloperRoleID
                    });
				}

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Show", new { id = id });
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        [Route("projects/{id}/delete")]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public ActionResult Destroy(int id)
        {
            // Find the project to be deleted.
            Project project = db.Projects.Find(id);

            // Remove all UserProjectRoles
            db.UserProjectRoles.RemoveRange(project.UserProjectRoles);
            
            // Remove all tickets and their associated assets
            var projectTickets = project.Tickets;
            foreach (var ticket in projectTickets)
            {
                db.TicketComments.RemoveRange(ticket.TicketComments);
                db.TicketAttachments.RemoveRange(ticket.TicketAttachments);
            }
			db.Tickets.RemoveRange(project.Tickets);

            // Finally Remove Project.
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index", new { accountUsername = User.Identity.GetUserName() });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}