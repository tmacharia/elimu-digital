using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class Post : Base
    {
        public string Message { get; set; }

        public virtual Profile By { get; set; }
        public virtual ICollection<Content> Medias { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

}
