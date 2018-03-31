using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class ExamViewModel
    {
        public int Id { get; set; }
        public Guid Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public DateTime Moment { get; set; }
    }
}
