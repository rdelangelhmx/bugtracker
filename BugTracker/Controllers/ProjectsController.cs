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

		// GET: accounts/{accountId}/projects
        public ActionResult Index(string accountId)
        {
			var projects = db.AspNetUsers.FirstOrDefault(user => user.Id == accountId).Projects;

            List<ProjectViewModel> model = new List<ProjectViewModel>();
            foreach (var item in projects)
            {
                model.Add(new ProjectViewModel(item));
            }

            return View(model);
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

            DetailsProjectViewModel model = new DetailsProjectViewModel(project, projectTicketViewModels);

            return View(model);
        }

		// GET: accounts/{accountId}/projects/new
        public ActionResult New(string accountId)
		{
			IEnumerable<AspNetUser> users = db.AspNetUsers;

			AspNetUser user = users.FirstOrDefault(u => u.Id == accountId);

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

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string accountId, CreateProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
				// Get currently logged in user
				IEnumerable<AspNetUser> users = db.AspNetUsers;
				AspNetUser user = users.FirstOrDefault(u => u.Id == accountId);

				// Create new project from info contained in the CreatProjectViewModel
				Project project = new Project();
				project.Name = model.Name;
				project.Creator = user.UserName;
				project.Manager = model.Manager;

				// Add selected users to project list
				foreach (var item in model.SelectedUsers)
				{
					project.AspNetUsers.Add(users.FirstOrDefault(u => u.Id == item));
				}

				//// Add newly created project to users
				//user.Projects.Add(project);

				// Save changes to database
				db.Entry(user).State = EntityState.Modified;
                db.Projects.Add(project);
                db.SaveChanges();
				return RedirectToAction("Index", new { accountId = accountId });
            }

            return View("New", model);
        }

        // GET: users/{accountId}/projects/{id}
        public ActionResult Edit(string accountId, int? id)
        {
            if (accountId == null && id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

			IEnumerable<AspNetUser> users = db.AspNetUsers;

			AspNetUser user = users.FirstOrDefault(u => u.Id == accountId);
			ViewBag.UserId = accountId;

			Project project = user.Projects.FirstOrDefault(p => p.ID == id);
			if (project == null)
            {
                return HttpNotFound();
            }

			EditProjectViewModel model = new EditProjectViewModel(project);
			model.Users = users.Select(x => new SelectListItem
			{
				Value = x.Id,
				Text = x.UserName
			});

            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update(string accountId, EditProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
				List<AspNetUser> users = db.AspNetUsers.ToList();
				AspNetUser user = users.FirstOrDefault(u => u.Id == accountId);
				Project project = user.Projects.FirstOrDefault(p => p.ID == model.ID);
				project.Name = model.Name;
				project.Manager = model.Manager;

				project.AspNetUsers.Clear();
				foreach (var item in model.SelectedUsers)
				{
					project.AspNetUsers.Add(users.FirstOrDefault(u => u.Id == item));
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
