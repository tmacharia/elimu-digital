using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Like : Base
    {
        [DefaultValue(Reaction.Like)]
        public Reaction Reaction { get; set; }

        [Required]
        public virtual Profile By { get; set; }
    }
}
