using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class Skill : Base
    {
        public string Name { get; set; }
        public decimal PercentageMastery { get; set; }
    }
}
