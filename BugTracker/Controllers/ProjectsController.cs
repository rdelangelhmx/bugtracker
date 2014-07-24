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
			AspNetUser user = db.AspNetUsers.Find(accountId);

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("New", project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPut]
        [ValidateAntiForgeryToken]
        public ActionResult Update([Bind(Include = "ID,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
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
