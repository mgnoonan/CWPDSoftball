//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Softball.Mvc4
{
    using System;
    using System.Collections.Generic;
    
    public partial class Team
    {
        public Team()
        {
            this.Schedules = new HashSet<Schedule>();
            this.Schedules1 = new HashSet<Schedule>();
        }
    
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string Manager { get; set; }
        public string HomeCell { get; set; }
        public string WorkCell { get; set; }
    
        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Schedule> Schedules1 { get; set; }
    }
}
