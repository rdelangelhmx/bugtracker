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
    
    public partial class TicketHistory
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public System.DateTimeOffset Changed { get; set; }
        public string UserID { get; set; }
    
        public virtual Ticket Ticket { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
