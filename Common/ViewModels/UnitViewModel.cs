using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Common.ViewModels
{
    public class UnitViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public Guid Code { get; set; }
        public int Level { get; set; }
        public int Semester { get; set; }

        public Course Course { get; set; }
        public Lecturer Lecturer { get; set; }
    }
}
