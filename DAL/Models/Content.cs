using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Content : Base
    {
        [Required]
        [DefaultValue(Models.ContentType.Document)]
        public ContentType Type { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        public virtual Lecturer UploadedBy { get; set; }
        [Required]
        public virtual Unit Unit { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
