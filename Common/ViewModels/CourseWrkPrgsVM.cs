using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class RootCourseWorkPrgs
    {
        public bool IsDone
        {
            get
            {
                if(this.Data.Count < 1)
                {
                    return true;
                }
                else
                {
                    if(this.Data.Any(x => x.IsComplete == false))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        public ICollection<CourseWrkPrgsVM> Data { get; set; }
    }
    public class CourseWrkPrgsVM
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public decimal Current { get; set; }
        public decimal Overall { get; set; }
        public bool IsComplete { get; set; }
        public decimal PercentageComplete { get; set; }
    }
}
