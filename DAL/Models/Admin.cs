using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models
{
    public class Admin : Base
    {
        /// <summary>
        /// Creates a new system admin and links their identity account with the school's
        /// administrators information.
        /// </summary>
        /// <param name="identityId">Unique identifier used to create an identity
        /// account for this administrator.
        /// </param>
        public Admin(Guid identityId)
        {
            this.AccountId = identityId;
        }

        [Required]
        public Guid AccountId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
