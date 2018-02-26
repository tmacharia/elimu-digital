using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Notification : Base
    {
        [Required]
        public Guid AccountId { get; set; }
        [DefaultValue(false)]
        public bool? Read { get; set; }
        [DataType(DataType.Html)]
        public string Message { get; set; }
    }
}
