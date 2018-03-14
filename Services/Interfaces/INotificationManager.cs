using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface INotificationManager
    {
        /// <summary>
        /// When a lecturer uploads new learning material for a
        /// unit, create notifications for all students in the unit, 
        /// post in units discussion board and send students a 
        /// notification email
        /// </summary>
        /// <param name="content">uploaded content</param>
        Task OnNewContent(Content content);
        /// <summary>
        /// When a new exam is set and scheduled by a lecturer,
        /// notify all students of the unit by notification & email. 
        /// Post in the unit's
        /// discussion board and send students emails.
        /// </summary>
        /// <param name="exam">scheduled exam</param>
        Task OnNewExam(Exam exam);
        /// <summary>
        /// When a new unit gets created. Create a default
        /// discussion board for it.
        /// </summary>
        /// <param name="unit">unit created.</param>
        void OnNewUnit(Unit unit);
    }
}
