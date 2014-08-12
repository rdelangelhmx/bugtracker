using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketAttachmentViewModel
    {
        private BugTrackerEntities1 db = new BugTrackerEntities1();

        public TicketAttachmentViewModel() { }

        public TicketAttachmentViewModel(TicketAttachment ticketAttachment)
        {
            this.UploadedBy = new EditUserViewModel(db.AspNetUsers.FirstOrDefault(u => u.Id == ticketAttachment.UserID));
            this.Description = ticketAttachment.Description;
            this.DataFilePath = ticketAttachment.DataFilePath;
        }

        public int ID { get; set; }
        public EditUserViewModel UploadedBy { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Attachment")]
        public string DataFilePath { get; set; }

        public HttpPostedFileBase AttachmentFile { get; set; }
    }
}