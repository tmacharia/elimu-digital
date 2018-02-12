using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Lecturer : Base
    {
        /// <summary>
        /// Creates a new lecturer and links their identity account with the school's
        /// lecturer information.
        /// </summary>
        /// <param name="identityId">Unique identifier used to create an identity
        /// account for this lecturer.
        /// </param>
        public Lecturer(Guid identityId)
        {
            this.AccountId = identityId;
        }

        public Lecturer()
        {

        }

        [Required]
        public Guid AccountId { get; set; }

        public virtual Profile Profile { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
        public virtual ICollection<Content> UploadedContent { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
