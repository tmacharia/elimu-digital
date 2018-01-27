using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Profile : Base
    {
        [Required]
        [MinLength(8)]
        [MaxLength(8)]
        public long NationalID { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullNames { get; set; }
        [MinLength(10)]
        [DataType(DataType.ImageUrl)]
        public string PhotoUrl { get; set; }

        // Social Media
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
    }
}
