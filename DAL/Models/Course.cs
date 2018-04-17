using DAL.Models.Fees;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Course : Base
    {
        public Course()
        {
            this.Code = Guid.NewGuid();
        }

        [Required]
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public string BackdropUrl { get; set; }
        public string DefaultColor { get; set; }
        public Guid Code { get; set; }
        [DefaultValue(0)]
        public ProgramType ProgramType { get; set; }
        [DefaultValue(4)]
        public int Years { get; set; }
        [DefaultValue(2)]
        public CourseType Type { get; set; }

        public virtual School School { get; set; }
        public virtual ICollection<FeeStructure> FeeStructures { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<StudentCourse> CourseStudents { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
