using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models
{
    public class ExamSession : Base
    {
        public ExamSession()
        {
            SessionId = Guid.NewGuid();
        }

        public Guid SessionId { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        [DefaultValue(false)]
        public bool IsComplete { get; set; }

        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }
    }
}
