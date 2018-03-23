using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Answer : Base
    {
        [Required(ErrorMessage = "Answer value cannot be null.")]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public virtual Content Media { get; set; }

        public virtual Question Question { get; set; }
    }
}
