using Common.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Interfaces
{
    public interface IProgressTracker
    {
        void TrackInitial(int progressId, decimal current, decimal overall);
        CourseworkProgress TrackProgress(int progressId, decimal current);
        void TrackDownload(int progressId);

        CourseworkProgress GetProgress(int contentId, int studentId);
        CourseworkProgress GetProgress(int progressId);

        IList<StudentProgressViewModel> TrackProgress(int contentId);
        IList<CourseworkProgress> TrackUnitProgress(int unitId);
        IList<CourseworkProgress> TrackUnitsProgress(int lecturerId);
    }
}
