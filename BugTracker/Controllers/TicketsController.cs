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
using PagedList;
using System.Linq.Expressions;


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
        public ActionResult Index(int? page, string searchBy , string search)
        {
            var pageNumber = page ?? 1;

            ViewBag.Search = search;
            ViewBag.SearchBy = searchBy ?? "Project";

            // Create a query string for grabbing all tickets from the database
            var tickets = from q in db.Tickets.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType).OrderBy(t => t.ID) select q;
            
            Dictionary<string, Expression<Func<Ticket, bool>>> searchByDictionary = new Dictionary<string, Expression<Func<Ticket,bool>>>()
            {
                {"Project", t => t.Project.Name.Contains(search) },
                {"Status", t => t.TicketStatus.Name.Contains(search) },
                {"Priority", t => t.TicketPriority.Name.Contains(search) },
                {"Type", t => t.TicketType.Name.Contains(search) }
            };

            // Edit query for tickets if user searched
            if (!String.IsNullOrWhiteSpace(search))
            {
                tickets = tickets.Where(searchByDictionary[searchBy]);
            }
            
            // Create a List of ListTicketsViewModels
            List<TicketViewModel> model = new List<TicketViewModel>();
            foreach (var item in tickets)
            {
                model.Add(new TicketViewModel(item));
            }
            IPagedList<TicketViewModel> pagedList = model.ToPagedList(pageNumber, 5);

            var searchByList = new List<SelectListItem>() {
                new SelectListItem() { Value = "Project", Text = "Project", Selected = false },
                new SelectListItem() { Value = "Priority", Text = "Priority", Selected = false },
                new SelectListItem() { Value = "Status", Text = "Status", Selected = false },
                new SelectListItem() { Value = "Type", Text = "Type", Selected = false }
            };

            var modelTuple = new Tuple<IPagedList<TicketViewModel>, List<SelectListItem>>(pagedList, searchByList);

            // Convert List to a PagedList and return to View
            return View(modelTuple);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TicketViewModel model = new TicketViewModel(ticket);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(model);
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

            EditTicketViewModel model = new EditTicketViewModel(ticket);

            return View(model);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditTicketViewModel model)
        {
            if (ModelState.IsValid)
            {
                var ticket = db.Tickets.FirstOrDefault(t => t.ID == model.ID);
                model.UpdateTicket(ticket);

                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // something went wrong!
            return View(model);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TicketViewModel model = new TicketViewModel(ticket);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(model);
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
