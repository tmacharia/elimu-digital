using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DAL.Models
{
    public class Score : Base
    {
        [Required]
        [MinLength(0)]
        [MaxLength(100)]
        public decimal Attained { get; set; }

        [NotMapped]
        public decimal Percentage
        {
            get
            {
                decimal total = this.Exam.Questions
                                         .Sum(x => x.Marks);

                return (this.Attained / total) * 100;
            }
        }
        [NotMapped]
        [DefaultValue(Models.Grade.Faulty)]
        public Grade Grade
        {
            get
            {
                if(this.Percentage >= 70 && this.Percentage <= 100)
                {
                    return Grade.A;
                }
                else if(this.Percentage >= 60)
                {
                    return Grade.B;
                }
                else if(this.Percentage >= 50)
                {
                    return Grade.C;
                }
                else if(this.Percentage >= 40)
                {
                    return Grade.D;
                }
                else if(this.Percentage > 0 && this.Percentage < 40)
                {
                    return Grade.E;
                }
                else
                {
                    return Grade.Faulty;
                }
            }
        }

        [Required]
        public virtual Student Student { get; set; }
        [Required]
        public virtual Exam Exam { get; set; }
    }
}
