using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class School : Base
    {
        [Required]
        public string Name { get; set; }
        public DateTime DateFounded { get; set; }
        public string ViceChancellor { get; set; }
        public string Website { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
    }
}
