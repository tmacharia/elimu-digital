using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class SchoolViewModel
    {
        [Required]
        public string Name { get; set; }
        public DateTime DateFounded { get; set; }
        public string ViceChancellor { get; set; }
        public string Website { get; set; }

        public virtual Location Location { get; set; }
    }
}
