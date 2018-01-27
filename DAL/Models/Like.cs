using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Like : Base
    {
        [DefaultValue(Models.Reaction.Like)]
        public Reaction Reaction { get; set; }
        [Required]
        public virtual Student By { get; set; }
        public virtual Class Class { get; set; }
        public virtual Course Course { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual Lecturer Lecturer { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
