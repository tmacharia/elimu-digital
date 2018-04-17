using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ClassUnitViewModel
    {
        public int ClassId { get; set; }
        public int UnitId { get; set; }
        public string Unit { get; set; }
        public string Room { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration
        {
            get
            {
                return this.EndTime.Subtract(this.StartTime);
            }
        }

        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
