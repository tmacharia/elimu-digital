using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Fees
{
    public class BaseFeeStructure : Base
    {
        [Display(Name = "Library fees")]
        public decimal Library { get; set; }
        [Display(Name = "IT lab/internet fees")]
        public decimal Internet { get; set; }
        [Display(Name = "Accomodation/hostel charges")]
        public decimal Accomodation { get; set; }
        [Display(Name = "Medical coverage fees")]
        public decimal Medical { get; set; }

        [Display(Name = "Amount for trips")]
        public decimal Trip { get; set; }
        [Display(Name = "Refundable amount for final project.")]
        public decimal Project { get; set; }
        [Display(Name = "Price per unit for govt students.")]
        public decimal PerUnit { get; set; }
        [Display(Name = "Price per unit for self-sponsored students.")]
        public decimal ParallelPerUnit { get; set; }

        [NotMapped]
        [Display(Name = "Total for govt-sponsored")]
        public decimal TotalGovt
        {
            get
            {
                return Library +
                       Internet +
                       Medical +
                       Project +
                       Trip +
                       Accomodation +
                       PerUnit;
            }
        }
        [NotMapped]
        [Display(Name = "Total for self-sponsored")]
        public decimal TotalParallel
        {
            get
            {
                return Library +
                       Internet +
                       Medical +
                       Project +
                       Trip +
                       Accomodation +
                       ParallelPerUnit;
            }
        }

        public DateTime? EditDate { get; set; }
        public int AdminId { get; set; }
        public virtual Admin PreparedBy { get; set; }
    }
}
