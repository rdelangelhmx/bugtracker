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
    public class TicketCommentsController : Controller
    {
        private BugTrackerEntities1 db = new BugTrackerEntities1();

		// GET: users/{accountId}/projects/{projectId}/tickets/{ticketId}/ticketcomments
        public ActionResult Index(string accountId, int projectId, int ticketId)
        {
			var ticketComments = db.TicketComments.Include(t => t.Ticket);
            return View(ticketComments.ToList());
        }

        // GET: TicketComments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

		// GET: users/{accountId}/projects/{projectId}/tickets/{ticketId}/ticketcomments/new
        public ActionResult New(string accountId, int projectId, int ticketId)
        {
			NewTicketCommentViewModel model = new NewTicketCommentViewModel();
			ViewBag.accountId = accountId;
			ViewBag.projectId = projectId;
			ViewBag.ticketId = ticketId;

            return View(model);
        }

        // POST: users/{accountUsername}/projects/{projectId}/tickets/{ticketId}/ticketcomments	
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string username, int projectId, int ticketId, NewTicketCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
				Ticket ticket = db.Tickets.FirstOrDefault(t => t.ID == ticketId);
				TicketComment ticketComment = new TicketComment();
				ticketComment.Ticket = ticket;
				ticketComment.AspNetUser = db.AspNetUsers.FirstOrDefault(u => u.UserName == User.Identity.Name);
				ticketComment.Created = DateTime.UtcNow;
				ticketComment.Comment = model.Comment;
				ticketComment.UserID = ticketComment.AspNetUser.Id;

                db.TicketComments.Add(ticketComment);
                db.SaveChanges();
                TempData["tab"] = "#comments-3";
				return RedirectToAction("Show", "Tickets", new { accountUsername = username, projectId = projectId, id = ticketId });
            }

			return View("Show", model);
        }

        // GET: TicketComments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "UserID", ticketComment.TicketID);
            return View(ticketComment);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Comment,UserID,Created,TicketID")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketComment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "UserID", ticketComment.TicketID);
            return View(ticketComment);
        }

        // GET: TicketComments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketComment ticketComment = db.TicketComments.Find(id);
            if (ticketComment == null)
            {
                return HttpNotFound();
            }
            return View(ticketComment);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketComment ticketComment = db.TicketComments.Find(id);
            db.TicketComments.Remove(ticketComment);
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
