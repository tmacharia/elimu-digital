using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ExamDetailsViewModel
    {
        public Exam Exam { get; set; }
        public Lecturer Instructor { get; set; }
        public ICollection<Unit> CurriculumAreas { get; set; }
    }
}
