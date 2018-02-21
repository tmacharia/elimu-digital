using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Notification : Base
    {
        [Required]
        public Guid AccountId { get; set; }
    }
}
