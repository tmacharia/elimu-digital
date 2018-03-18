using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class CourseworkProgress : Base
    {
        public decimal Current { get; set; }
        public decimal Overall { get; set; }
        [DefaultValue(false)]
        public bool Downloaded { get; set; }

        [NotMapped]
        public bool IsComplete
        {
            get
            {
                var _current = Math.Round(Current, 0);
                var _total = Math.Round(Overall, 0);

                if(Overall == 0)
                {
                    return false;
                }
                else if(_current < _total)
                {
                    return false;
                }
                else if(_current == _total)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }
        [NotMapped]
        public decimal PercentageComplete
        {
            get
            {
                try
                {
                    decimal val = (Current / Overall) * 100;

                    return Math.Round(val, 0);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int StudentId { get; set; }
        public int ContentId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Content Content { get; set; }
    }
}
