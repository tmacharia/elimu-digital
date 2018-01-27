using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class StudentUnit : Base
    {
        public int StudentId { get; set; }
        public int UnitId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
