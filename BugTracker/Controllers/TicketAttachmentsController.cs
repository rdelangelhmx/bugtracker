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
using System.IO;

namespace BugTracker.Controllers
{
    public class TicketAttachmentsController : Controller
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        // GET: TicketAttachments
        public ActionResult Index(string accountUsername, int projectId, int ticketId)
        {
            var ticketAttachments = db.TicketAttachments.Include(t => t.Ticket);
            return View(ticketAttachments.Where(t => t.ID == ticketId).ToList());
        }

        // GET: TicketAttachments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);

            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // GET: users/{accountUsername}/projects/{projectId}/tickets/{ticketId}/ticketattachments/new
        public ActionResult New(string accountUsername, int projectId, int ticketId)
        {
            ViewBag.UserName = accountUsername;
            ViewBag.ProjectID = projectId;
            ViewBag.TicketID = ticketId;
            NewTicketAttachmentViewModel model = new NewTicketAttachmentViewModel();
            return View(model);
        }

        // POST: users/{accountUsername}/projects/{projectId}/tickets/{ticketId}/ticketattachments    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int projectId, int ticketId, NewTicketAttachmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                TicketAttachment ticketAttachment = new TicketAttachment()
                {
                    TicketID = ticketId,
                    UserID = User.Identity.GetUserId(),
                    Created = System.DateTimeOffset.Now,
                    Description = model.Description
                };

                // Create this new directory if it doesn't already exist.
                string directory = Server.MapPath(String.Format(@"~/Uploads/Project{0}/Ticket{1}/Attachments/", projectId, ticketId));

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string FileName = model.Attachment.FileName;
                string FileContentType = model.Attachment.ContentType;
                string FilePath = directory + FileName;
                model.Attachment.SaveAs(FilePath);
                ticketAttachment.DataFilePath = FilePath;

                db.TicketAttachments.Add(ticketAttachment);
                db.SaveChanges();
                TempData["tab"] = "#attachments-3";
                return RedirectToAction("Show", "Tickets", new {projectId = projectId, id = ticketId });
            }
            
            return View("New", model);
        }

        // GET: TicketAttachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "UserID", ticketAttachment.TicketID);
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TicketID,UserID,DataFilePath,Created,Description")] TicketAttachment ticketAttachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketAttachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TicketID = new SelectList(db.Tickets, "ID", "UserID", ticketAttachment.TicketID);
            return View(ticketAttachment);
        }

        // GET: TicketAttachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            if (ticketAttachment == null)
            {
                return HttpNotFound();
            }
            return View(ticketAttachment);
        }

        // POST: TicketAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketAttachment ticketAttachment = db.TicketAttachments.Find(id);
            db.TicketAttachments.Remove(ticketAttachment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public FileResult Download(int projectId, int ticketId, string fileName)
        {
   
            var fileType = Path.GetExtension(fileName);
 
            var path = Server.MapPath(String.Format(@"~/Uploads/Project{0}/Ticket{1}/Attachments/", projectId, ticketId));
            return File(path + fileName, MimeMapping.GetMimeMapping(fileName));
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
