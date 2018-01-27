using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Unit : Base
    {
        public Unit()
        {
            this.Code = Guid.NewGuid();
        }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public Guid Code { get; set; }

        [Required]
        public virtual Course Course { get; set; }
        public virtual Lecturer Lecturer { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<Content> Contents { get; set; }
    }
}
