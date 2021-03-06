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
    
    public partial class Project
    {
        public Project()
        {
            this.Tickets = new HashSet<Ticket>();
            this.UserProjectRoles = new HashSet<UserProjectRole>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Manager { get; set; }
        public string Creator { get; set; }
    
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<UserProjectRole> UserProjectRoles { get; set; }
    }
}
