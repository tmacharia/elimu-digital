using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class LecturerViewModel
    {
        public string Bio { get; set; }
        public ICollection<Skill> Skills { get; set; }
    }
}
