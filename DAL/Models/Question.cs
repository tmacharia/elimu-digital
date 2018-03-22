using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Question : Base
    {
        [Required]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public decimal Marks { get; set; }

        public virtual Content Media { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        [Required]
        public virtual Exam Exam { get; set; }
    }
}
