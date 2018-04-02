using DAL.Models;
using Services.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Common.ViewModels;

namespace Services.Implementations
{
    public class ProgressTracker : IProgressTracker
    {
        private readonly IRepositoryFactory _repos;

        public ProgressTracker(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        public CourseworkProgress GetProgress(int contentId, int studentId)
        {
            var progress =  _repos.CourseworkProgress
                                  .Get(x => x.ContentId == contentId && x.StudentId == studentId);

            if(progress == null)
            {
                progress = new CourseworkProgress()
                {
                    ContentId = contentId,
                    StudentId = studentId
                };

                progress = _repos.CourseworkProgress.Create(progress);
                _repos.Commit();
            }

            return progress;
        }

        public CourseworkProgress GetProgress(int progressId)
        {
            return _repos.CourseworkProgress
                         .Get(progressId);
        }

        public void TrackDownload(int progressId)
        {
            var prev = _repos.CourseworkProgress.Get(progressId);

            prev.Downloaded = true;

            prev = _repos.CourseworkProgress.Update(prev);
            _repos.Commit();
        }

        public void TrackInitial(int progressId, decimal current, decimal overall)
        {
            var progress = _repos.CourseworkProgress.Get(progressId);

            progress.Current = current;
            progress.Overall = overall;

            progress = _repos.CourseworkProgress.Update(progress);
            _repos.Commit();
        }

        public CourseworkProgress TrackProgress(int progressId, decimal current)
        {
            var progress = _repos.CourseworkProgress.Get(progressId);

            if(current > progress.Current)
            {
                progress.Current = current;

                progress = _repos.CourseworkProgress.Update(progress);
                _repos.Commit();
            }

            return progress;
        }

        public IList<StudentProgressViewModel> TrackProgress(int contentId)
        {
            List<StudentProgressViewModel> list = new List<StudentProgressViewModel>();

            var students = _repos.Contents.GetWith(contentId,
                                  "Unit",
                                  "Unit.UnitStudents",
                                  "Unit.UnitStudents.Student",
                                  "Unit.UnitStudents.Student.Profile")
                                  .Unit
                                  .UnitStudents
                                  .Select(x => x.Student)
                                  .ToList();

            foreach (var item in students)
            {
                var model = new StudentProgressViewModel()
                {
                    FullNames = item.Profile.FullNames
                };

                var progress = _repos.CourseworkProgress
                                     .Get(x => x.StudentId == item.Id && x.ContentId == contentId);

                if (progress == null)
                    model.Progress = 0;
                else
                    model.Progress = progress.PercentageComplete;

                list.Add(model);
            }

            return list.OrderByDescending(x => x.Progress)
                       .ToList();
        }

        public IList<CourseworkProgress> TrackUnitProgress(int unitId)
        {
            List<CourseworkProgress> list = new List<CourseworkProgress>();

            var contents = _repos.Units.GetWith(unitId, "Contents")
                                        .Contents
                                        .ToList();

            for (int i = 0; i < contents.Count; i++)
            {
                var contentProgress = TrackProgress(contents[i].Id);

                //list.AddRange(contentProgress);
            }

            return list;
        }

        public IList<CourseworkProgress> TrackUnitsProgress(int lecturerId)
        {
            List<CourseworkProgress> list = new List<CourseworkProgress>();

            var units = _repos.Lecturers.GetWith(lecturerId, "Units")
                                        .Units
                                        .ToList();

            for (int i = 0; i < units.Count; i++)
            {
                var unitProgress = TrackUnitProgress(units[i].Id);

                list.AddRange(unitProgress);
            }

            return list;
        }
        public IList<CourseWrkPrgsVM> GetProgressByUnit(int unitId, int studentId, int count = 5000)
        {
            List<CourseWrkPrgsVM> list = new List<CourseWrkPrgsVM>();
            IEnumerable<Content> unitContents = new List<Content>();
            IList<CourseworkProgress> _myprogress = _repos.CourseworkProgress
                                                         .List
                                                         .Where(x => x.StudentId == studentId)
                                                         .ToList();

            var unit = _repos.Units.GetWith(unitId, "Contents");

            if (unit != null)
            {
                unitContents = unit.Contents;
            }

            foreach (var item in unitContents)
            {
                var prg = _myprogress.FirstOrDefault(x => x.ContentId == item.Id);

                if (prg != null)
                {
                    list.Add(ContentToViewModel(item, prg));
                }
                else
                {
                    list.Add(ContentToViewModel(item));
                }
            }

            return list.Take(count)
                       .ToList();
        }
        private CourseWrkPrgsVM ContentToViewModel(Content item, CourseworkProgress prg = null)
        {
            var vm = new CourseWrkPrgsVM()
            {
                Id = (prg != null) ? prg.Id : 0,
                Content = item.Title,
                Current = (prg != null) ? prg.Current : 0,
                IsComplete = (prg != null) ? prg.IsComplete : false,
                Overall = (prg != null) ? prg.Overall : 0,
                PercentageComplete = (prg != null) ? prg.PercentageComplete : 0
            };

            return vm;
        }
    }
}
