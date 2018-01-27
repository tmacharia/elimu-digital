using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public enum Grade
    {
        /// <summary>
        /// 70% and above
        /// </summary>
        A,
        /// <summary>
        /// 60% - 70%
        /// </summary>
        B,
        /// <summary>
        /// 50% - 60%
        /// </summary>
        C,
        /// <summary>
        /// 40% - 50%
        /// </summary>
        D,
        /// <summary>
        /// below 40%
        /// </summary>
        E,
        /// <summary>
        /// Grade can't be calculated because either the student didn't take the exam
        /// or their score in the exam is faulty.
        /// </summary>
        Faulty
    }
}
