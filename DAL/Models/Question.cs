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

        public decimal Marks { get; set; }

        public virtual Content Media { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        
        public virtual Exam Exam { get; set; }
    }
}
