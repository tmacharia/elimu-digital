using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class StudentCourse : Base
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
