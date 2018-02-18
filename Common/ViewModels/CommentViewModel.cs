using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class CommentViewModel
    {
        [MinLength(1, ErrorMessage = "Comment messages such small are not allowed.")]
        [DataType(DataType.Html)]
        public string Message { get; set; }
    }
}
