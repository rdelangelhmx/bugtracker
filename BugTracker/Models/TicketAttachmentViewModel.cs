using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.IO;
using BugTracker.Validations;

namespace BugTracker.Models
{
    public class TicketAttachmentViewModel
    {
        private BugTrackerEntities db = new BugTrackerEntities();

        public TicketAttachmentViewModel() { }

        public TicketAttachmentViewModel(TicketAttachment ticketAttachment)
        {
            this.UploadedBy = new EditUserViewModel(db.AspNetUsers.FirstOrDefault(u => u.Id == ticketAttachment.UserID));
            this.Description = ticketAttachment.Description;
            this.FileName = Path.GetFileName(ticketAttachment.DataFilePath);
            this.DateCreated = ticketAttachment.Created.ToLocalTime().ToString("g");
        }

        public int ID { get; set; }

        public EditUserViewModel UploadedBy { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Attachment")]
        public string FileName { get; set; }

        public string DateCreated { get; set; }
    }


    public class NewTicketAttachmentViewModel
    {
        public NewTicketAttachmentViewModel() { }

        public string Description { get; set; }

        [ValidateFile(ErrorMessage="File type must be jpeg or pdf and of size less than 1MB!")]
        public HttpPostedFileBase Attachment { get; set; }
    }
}