using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Comment : Base
    {
        [MinLength(1, ErrorMessage = "Comment messages such small are not allowed.")]
        [DataType(DataType.Html)]
        public string Message { get; set; }

        [Required]
        public virtual Profile By { get; set; }
        public virtual ICollection<Comment> Replies { get; set; }
    }
}
