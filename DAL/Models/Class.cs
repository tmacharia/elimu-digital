using DAL.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Class : Base
    {
        [Required]
        public string Room { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        [TimeCompare(Comparison.GreaterThan, "StartTime", DurationSpec.Hours, 2)]
        public DateTime EndTime { get; set; }
        [NotMapped]
        public TimeSpan Duration
        {
            get
            {
                return this.EndTime.Subtract(this.StartTime);
            }
        }

        [Required]
        public virtual Unit Unit { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
