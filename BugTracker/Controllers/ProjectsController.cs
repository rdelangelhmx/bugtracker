using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
	[Authorize]
    public class ProjectsController : Controller
    {
        private BugTrackerEntities1 db = new BugTrackerEntities1();

		// GET: accounts/{accountUsername}/projects
        public ActionResult Index(string accountUsername)
        {
			var projects = db.AspNetUsers.FirstOrDefault(user => user.UserName == accountUsername).Projects;

            List<ProjectViewModel> model = new List<ProjectViewModel>();
            foreach (var item in projects)
            {
                model.Add(new ProjectViewModel(item));
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
			foreach (var item in project.AspNetUsers.ToList())
			{
				projectMembers.Add(new EditUserViewModel(item));
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
			model.SelectedUsers.Add(user.Id);

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
				IEnumerable<AspNetUser> users = db.AspNetUsers.Include(u => u.UserProjectRoles).Include(u => u.Projects);
                IEnumerable<AspNetRole> roles = db.AspNetRoles;

				AspNetUser user = users.FirstOrDefault(u => u.UserName == accountUserName);

				// Create new project from info contained in the CreatProjectViewModel
				Project project = new Project();
				project.Name = model.Name;
				project.Creator = user.UserName;
				project.Manager = model.Manager;

                // Persist Project Manager to UserProjectRoles table
                db.UserProjectRoles.Add(new UserProjectRole()
                {
                    UserID = users.FirstOrDefault(u => u.Id == project.Manager).Id,
                    ProjectID = project.ID,
                    RoleID = roles.FirstOrDefault(r => r.Name == "Project Manager").Id
                });

				// Add selected users to project list
				foreach (var item in model.SelectedUsers)
				{
                    project.AspNetUsers.Add(users.FirstOrDefault(u => u.Id == item));
                    db.UserProjectRoles.Add(new UserProjectRole()
                    {
                        UserID = users.FirstOrDefault(u => u.Id == item).Id,
                        ProjectID = project.ID,
                        RoleID = roles.FirstOrDefault(r => r.Name == "Developer").Id
                    });
				}

				//// Add newly created project to users
				//user.Projects.Add(project);

				// Save changes to database
				db.Entry(user).State = EntityState.Modified;
                db.Projects.Add(project);
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

			AspNetUser user = users.FirstOrDefault(u => u.UserName == accountUserName);
			ViewBag.UserName = accountUserName;

			Project project = user.Projects.FirstOrDefault(p => p.ID == id);
			if (project == null)
            {
                return HttpNotFound();
            }

            // Get all users assigned to this project.


			EditProjectViewModel model = new EditProjectViewModel(project);
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
				Project project = user.Projects.FirstOrDefault(p => p.ID == model.ID);

                // If a NEW project manager was assigned, 
                // reassign them on the domain model
                if (project.Manager != model.Manager)
                {
                    project.Name = model.Name;
                    project.Manager = model.Manager;
                    var projectManagerRoleID = roles.FirstOrDefault(r => r.Name == "Project Manager").Id;
                    project.UserProjectRoles.FirstOrDefault(r => r.RoleID == projectManagerRoleID).UserID = users.FirstOrDefault(u => u.Id == project.Manager).Id;
                }

                project.UserProjectRoles.Clear();
				project.AspNetUsers.Clear();
				foreach (var item in model.SelectedUsers)
				{
					project.AspNetUsers.Add(users.FirstOrDefault(u => u.Id == item));
                    db.UserProjectRoles.Add(new UserProjectRole()
                    {
                        UserID = users.FirstOrDefault(u => u.Id == item).Id,
                        ProjectID = project.ID,
                        RoleID = roles.FirstOrDefault(r => r.Name == "Developer").Id
                    });
				}

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
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
            Project project = db.Projects.Find(id);
			db.Tickets.RemoveRange(project.Tickets);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
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
