using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class SummaryViewModel
    {
        public int Lec_Total { get; set; }
        public int Lec_NoProfile { get; set; }
        public int Courses_Total { get; set; }
        public int Students_Total { get; set; }
        public int Students_Enrolled { get; set; }
        public int Total_Classes { get; set; }
        public int Units_NoClass { get; set; }
    }
}
