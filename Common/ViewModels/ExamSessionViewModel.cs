using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ExamSessionViewModel
    {
        public int Id { get; set; }
        public Guid SessionId { get; set; }
        public int TotalQuestions { get; set; }
        public decimal TotalMarks { get; set; }

        public ExamDetailsViewModel Exam { get; set; }
    }
}
