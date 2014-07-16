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
    [Authorize]
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

            model.Assignees = new SelectList(Db.Users, "Id", "UserName");
            model.Projects = new SelectList(db.Projects, "ID", "Name");
            model.Priorities = new SelectList(db.TicketPriorities, "ID", "Name");
            model.Statuses = new SelectList(db.TicketStatuses, "ID", "Name");
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
                Ticket ticket = new Ticket();
                ticket.SubmitterID = User.Identity.GetUserId();

                ticket.Title = model.Title;
                ticket.Description = model.Description;
                ticket.AssigneeID = UserManager.FindById(model.Assignee).Id;
                ticket.ProjectID = Int32.Parse(model.Project);
                ticket.TypeID = Int32.Parse(model.Type);
                ticket.PriorityID = Int32.Parse(model.Priority);
                ticket.StatusID = Int32.Parse(model.Status);
                ticket.Created = System.DateTime.UtcNow;
                db.Tickets.Add(ticket);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

                TempData["notice"] = "Ticket successfully created!";
                return RedirectToAction("Index");
            }

            model.Assignees = new SelectList(Db.Users, "Id", "UserName");
            model.Projects = new SelectList(db.Projects, "ID", "Name");
            model.Priorities = new SelectList(db.TicketPriorities, "ID", "Name");
            model.Statuses = new SelectList(db.TicketStatuses, "ID", "Name");
            model.Types = new SelectList(db.TicketTypes, "ID", "Name");

            // If we got this far, something went wrong
            TempData["error"] = "Something went wrong. Ticket could not be created!";
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
