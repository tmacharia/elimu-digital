using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class ContentViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
