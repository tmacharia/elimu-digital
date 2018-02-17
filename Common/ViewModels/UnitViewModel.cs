using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class UnitViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public Guid Code { get; set; }

        public virtual Course Course { get; set; }
        public virtual Lecturer Lecturer { get; set; }
    }
}
