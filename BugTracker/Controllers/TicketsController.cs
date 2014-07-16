using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        public TicketsController() { }

        private BugTrackerEntities1 db = new BugTrackerEntities1();
        private ApplicationDbContext Db = new ApplicationDbContext();
        private UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            List<ListTicketsViewModel> model = new List<ListTicketsViewModel>();
            foreach (var item in tickets)
            {
                model.Add(new ListTicketsViewModel(item));
            }
            return View(model);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {
            var model = new CreateTicketViewModel();

            //model.Assignees = new List<SelectListItem>();
            //foreach (var item in Db.Users)
            //{
            //    model.Assignees.Add(new SelectListItem() { Value = item.Id, Text = item.UserName });
            //}
            model.Assignees = new SelectList(Db.Users, "Id", "UserName");

            //model.Projects = new List<SelectListItem>();
            //foreach (var item in db.Projects)
            //{
            //    model.Projects.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Name });
            //}
            model.Projects = new SelectList(db.Projects, "ID", "Name");

            //model.Priorities = new List<SelectListItem>();
            //foreach (var item in db.TicketPriorities)
            //{
            //    model.Priorities.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Name });
            //}
            model.Priorities = new SelectList(db.TicketPriorities, "ID", "Name");

            //model.Statuses = new List<SelectListItem>();
            //foreach (var item in db.TicketStatuses)
            //{
            //    model.Statuses.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Name });
            //}
            model.Statuses = new SelectList(db.TicketStatuses, "ID", "Name");

            //model.Types = new List<SelectListItem>();
            //foreach (var item in db.TicketTypes)
            //{
            //    model.Types.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Name });
            //}
            model.Types = new SelectList(db.TicketTypes, "ID", "Name");

            return View(model);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                //ticket.SubmitterID = User.Identity.GetUserId();
                //ticket.AssigneeID = UserManager.FindByEmail(ticket.AssigneeID).Id;
                //ticket.Created = System.DateTime.Now;
                //db.Tickets.Add(ticket);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", ticket.ProjectID);
            //ViewBag.PriorityID = new SelectList(db.TicketPriorities, "ID", "Name", ticket.PriorityID);
            //ViewBag.StatusID = new SelectList(db.TicketStatuses, "ID", "Name", ticket.StatusID);
            //ViewBag.TypeID = new SelectList(db.TicketTypes, "ID", "Name", ticket.TypeID);
            //return View(ticket);
            return View(model);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", ticket.ProjectID);
            ViewBag.PriorityID = new SelectList(db.TicketPriorities, "ID", "Name", ticket.PriorityID);
            ViewBag.StatusID = new SelectList(db.TicketStatuses, "ID", "Name", ticket.StatusID);
            ViewBag.TypeID = new SelectList(db.TicketTypes, "ID", "Name", ticket.TypeID);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UserID,Title,Description,Created,Updated,AssignedToUserID,ProjectID,TypeID,PriorityID,StatusID")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectID = new SelectList(db.Projects, "ID", "Name", ticket.ProjectID);
            ViewBag.PriorityID = new SelectList(db.TicketPriorities, "ID", "Name", ticket.PriorityID);
            ViewBag.StatusID = new SelectList(db.TicketStatuses, "ID", "Name", ticket.StatusID);
            ViewBag.TypeID = new SelectList(db.TicketTypes, "ID", "Name", ticket.TypeID);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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
