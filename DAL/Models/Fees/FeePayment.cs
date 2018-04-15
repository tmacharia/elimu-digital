using DAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Fees
{
    public class FeePayment : Base
    {
        [DefaultValue(true)]
        public bool IsGeneral { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public int Level { get; set; }
        public int Semester { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
