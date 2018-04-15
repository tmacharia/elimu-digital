using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Fees
{
    public class FeeStructure : Base
    {
        public decimal Library { get; set; }
        public decimal Internet { get; set; }
        public decimal Accomodation { get; set; }
        public decimal Medical { get; set; }
        public decimal Trip { get; set; }
        public decimal Project { get; set; }
        public decimal PerUnit { get; set; }
        public decimal ParallelPerUnit { get; set; }

        [Required]
        public int Semester { get; set; }
        [Required]
        public int Year { get; set; }

        public int AdminId { get; set; }
        [Required(ErrorMessage = "Select course for this fee structure.")]
        public int CourseId { get; set; }
        public DateTime? EditDate { get; set; }

        public virtual Admin PreparedBy { get; set; }
        public virtual Course Course { get; set; }
    }
}
