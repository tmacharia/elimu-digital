using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class CourseViewModel
    {
        [Required]
        public string Name { get; set; }
        public CourseType Type { get; set; }
        public Guid Code { get; set; }

        public virtual ICollection<Unit> Units { get; set; }
    }
}
