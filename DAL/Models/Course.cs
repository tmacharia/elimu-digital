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
        public Guid Code { get; set; }

        [DefaultValue(2)]
        public CourseType Type { get; set; }

        public virtual School School { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
