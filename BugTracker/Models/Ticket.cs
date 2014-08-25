//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BugTracker.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ticket
    {
        public Ticket()
        {
            this.TicketAttachments = new HashSet<TicketAttachment>();
            this.TicketComments = new HashSet<TicketComment>();
            this.TicketHistories = new HashSet<TicketHistory>();
            this.TicketNotifications = new HashSet<TicketNotification>();
            this.AspNetUsers = new HashSet<AspNetUser>();
        }
    
        public int ID { get; set; }
        public string SubmitterID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public System.DateTimeOffset Updated { get; set; }
        public string AssigneeID { get; set; }
        public int ProjectID { get; set; }
        public int TypeID { get; set; }
        public int PriorityID { get; set; }
        public int StatusID { get; set; }
    
        public virtual Project Project { get; set; }
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        public virtual ICollection<TicketComment> TicketComments { get; set; }
        public virtual ICollection<TicketHistory> TicketHistories { get; set; }
        public virtual ICollection<TicketNotification> TicketNotifications { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
