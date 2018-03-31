using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class DiscussionBoard : Base
    {
        public string Name { get; set; }
        public int UnitId { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
