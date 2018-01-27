using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models
{
    public class Student : Base
    {
        /// <summary>
        /// Creates a new student and links their identity account with the school's
        /// student information.
        /// </summary>
        /// <param name="identityId">Unique identifier used to create an identity
        /// account for this student.
        /// </param>
        public Student(Guid identityId)
        {
            this.AccountId = identityId;
        }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string RegNo { get; set; }
        [Required]
        [MaxLength(4)]
        public int YearOfStudy { get; set; }
        [Required]
        [MaxLength(10)]
        public string AcademicYear { get; set; }

        public virtual Profile Profile { get; set; }
        [Required]
        public virtual Course Course { get; set; }
        public virtual ICollection<StudentUnit> StudentUnits { get; set; }
        public virtual ICollection<Score> Scores { get; set; }
    }
}
