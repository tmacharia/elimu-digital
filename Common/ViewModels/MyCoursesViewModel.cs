using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class MyCoursesViewModel
    {
        public Course Main { get; set; }
        public ICollection<Course> Others { get; set; }
    }
}
