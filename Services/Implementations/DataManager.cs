using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DAL.Models;
using System.Reflection;
using Common.ViewModels;

namespace Services
{
    public class DataManager : IDataManager
    {
        private readonly IRepositoryFactory _repos;

        public DataManager(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        public IEnumerable<Lecturer> MyLecturers(int studentId, int count)
        {
            // get all my units
            List<Lecturer> lecturers = _repos.StudentUnits
                                             .ListWith("Unit",
                                                        "Unit.Lecturer",
                                                        "Unit.Lecturer.Profile",
                                                        "Unit.Lecturer.Likes")
                                              .Where(x => x.StudentId == studentId)
                                              .Select(x => x.Unit)
                                              .Select(x => x.Lecturer)
                                              .Distinct()
                                              .ToList();

            // get lecturers by course

            var crs = _repos.Students
                             .GetWith(studentId,
                                       "Course",
                                       "Course.Units",
                                       "Course.Units.Lecturer",
                                       "Course.Units.Lecturer.Profile",
                                       "Course.Units.Lecturer.Likes")
                              .Course;
                              
            if(crs != null)
            {
                var lecs = crs.Units
                              .SkipWhile(x => x.Lecturer == null)
                              .Select(x => x.Lecturer)
                              .Distinct()
                              .ToList();

                lecturers.AddRange(lecs);
            }

            return lecturers.Distinct();
        }

        public IList<Student> MyStudents(int lecturerId, int count)
        {
            IList<Student> students = _repos.Lecturers
                                            .GetWith(lecturerId, "Units",
                                                      "Units.UnitStudents",
                                                      "Units.UnitStudents.Student",
                                                      "Units.UnitStudents.Student.Course",
                                                      "Units.UnitStudents.Student.Profile")
                                            .Units
                                            .SelectMany(x => x.UnitStudents)
                                            .Select(x => x.Student)
                                            .Take(count)
                                            .SkipWhile(x => x == null)
                                            .Distinct()
                                            .ToList();
            return students;
        }

        public IEnumerable<Class> MyClasses<T>(int id, int count) where T : class
        {
            IEnumerable<Class> classes = new List<Class>();

            if(typeof(T) == typeof(Student))
            {
                classes = _repos.Students
                                .GetWith(id,
                                        "StudentUnits",
                                        "StudentUnits.Unit",
                                        "StudentUnits.Unit.Class",
                                        "StudentUnits.Unit.Class.Units",
                                        "StudentUnits.Unit.Class.Likes")
                                .StudentUnits
                                .Select(x => x.Unit)
                                .Select(x => x.Class)
                                .TakeWhile(x => x == null);
            }
            else if(typeof(T) == typeof(Lecturer))
            {
                classes = _repos.Lecturers
                                .GetWith(id,
                                        "Units",
                                        "Units.Class",
                                        "Units.Class.Units",
                                        "Units.Class.Likes")
                                .Units
                                .Select(x => x.Class)
                                .TakeWhile(x => x == null);
            }

            return classes.Take(count);
        }
        public MyCoursesViewModel MyCourses(int studentId)
        {
            MyCoursesViewModel model = new MyCoursesViewModel
            {
                Main = _repos.Students
                               .GetWith(studentId, "Course", "Course.Units")
                               .Course,

                Others = _repos.StudentCourses
                                 .ListWith("Course","Course.Units")
                                 .Where(x => x.StudentId == studentId)
                                 .Select(x => x.Course)
                                 .ToList()
            };

            return model;
        }
        public IEnumerable<Unit> MyUnits<T>(int id, int count) where T : class
        {
            List<Unit> units = new List<Unit>();

            if (typeof(T) == typeof(Student))
            {
                var std = _repos.Students
                              .GetWith(id,
                                       "Course",
                                       "Course.Units",
                                       "Course.Units.Lecturer",
                                       "Course.Units.Lecturer.Profile",
                                       "Course.Units.Class",
                                       "Course.Units.UnitStudents",
                                       "Course.Units.Likes");

                if(std != null)
                {
                    if (std.Course != null)
                    {
                        units.AddRange(std.Course.Units);
                    }
                }

                var unitsInOtherCourses = _repos.StudentCourses
                                                .ListWith("Course",
                                                          "Course.Units",
                                                          "Course.Units.Lecturer",
                                                          "Course.Units.Lecturer.Profile",
                                                          "Course.Units.Class",
                                                          "Course.Units.UnitStudents",
                                                          "Course.Units.Likes")
                                                .Where(x => x.StudentId == id)
                                                .TakeWhile(x => x.Course != null)
                                                .Select(x => x.Course)
                                                .TakeWhile(x => x.Units != null && x.Units.Count > 0)
                                                .SelectMany(x => x.Units);
                                                

                if (unitsInOtherCourses != null)
                {
                    units.AddRange(unitsInOtherCourses);
                }
            }
            else if(typeof(T) == typeof(Lecturer))
            {
                var lec = _repos.Lecturers
                                .GetWith(id,
                                       "Units",
                                       "Units.Course",
                                       "Units.Lecturer",
                                       "Units.Lecturer.Profile",
                                       "Units.UnitStudents",
                                       "Units.Likes");

                if(lec.Units != null)
                {
                    units.AddRange(lec.Units.TakeWhile(x => x != null));
                }
            }

            return units.Take(count);
        }

        public SummaryViewModel GetSummary()
        {
            // create summary view model
            var summary = new SummaryViewModel
            {
                Total_Classes = _repos.Classes.List.Count(),
                Students_Total = _repos.Students.List.Count(),
                Lec_Total = _repos.Lecturers.List.Count(),
                Courses_Total = _repos.Courses.List.Count(),
                Lec_NoProfile = _repos.Lecturers.ListWith("Profile")
                                                .Count(x => x.Profile == null),
                Students_Enrolled = _repos.Students.ListWith("Course")
                                                   .Count(x => x.Course != null),
                Units_NoClass = _repos.Units.ListWith("Class")
                                            .Count(x => x.Class == null)
            };

            return summary;
        }

        public IEnumerable<Student> MyClassMates(int id)
        {
            IEnumerable<Student> students = new List<Student>();

            students = _repos.Students
                             .GetWith(id,
                                    "StudentUnits",
                                    "StudentUnits.Student",
                                    "StudentUnits.Student.Profile",
                                    "StudentUnits.Student.Course")
                             ?.StudentUnits
                             .Select(x => x.Student)
                             .SkipWhile(x => x == null || x.Id == id)
                             .Distinct();

            if(students == null)
            {
                return new List<Student>();
            }
            else
            {
                return students;
            }
        }

        public IEnumerable<Lecturer> MyColleagues(int lecturerId)
        {
            List<Lecturer> lecturers = new List<Lecturer>();

            //var lecturer = _repos.Lecturers
            //                     .ListWith("Units", "Units.Course")
            //                     .FirstOrDefault(x => x.Id == lecturerId);

            //var courses = lecturer.Units
            //                      .Select(x => x.Course)
            //                      .Distinct()
            //                      .ToList();

            lecturers = _repos.Lecturers.ListWith("Units",
                                                  "Units.Course",
                                                  "Units.Course.Units",
                                                  "Units.Course.Units.Lecturer",
                                                  "Units.Course.Units.Lecturer.Profile")
                                        .Where(x => x.Id == lecturerId)
                                        .SelectMany(x => x.Units)
                                        .Select(x => x.Course)
                                        .SelectMany(x => x.Units)
                                        .Select(x => x.Lecturer)
                                        .TakeWhile(x => x != null)
                                        .ToList();

            //foreach (var item in courses)
            //{
            //    var lecs = _repos.Units
            //                     .ListWith("Course",
            //                               "Lecturer",
            //                               "Lecturer.Profile")
            //                     .Where(x => x.Course.Id == item.Id)
            //                     .Select(x => x.Lecturer)
            //                     .ToList();

            //    lecturers.AddRange(lecs);
            //}

            return lecturers.TakeWhile(x => x != null)
                            .TakeWhile(x => x.Id != lecturerId)
                            .Distinct()
                            .ToList();
        }

        public IList<ExamViewModel> MyExams<T>(int id, int count = 5000) where T : class
        {
            List<ExamViewModel> exams = new List<ExamViewModel>();

            if (typeof(T) == typeof(Student))
            {
                var std = _repos.Students
                                .GetWith(id,
                                       "Course",
                                       "Course.Units",
                                       "Course.Units.Exams",
                                       "Course.Units.Exams.Unit");

                if (std != null)
                {
                    if (std.Course != null)
                    {
                        exams.AddRange(std.Course.Units
                                          .SelectMany(x => x.Exams)
                                          .Select(TransformFuncs.ToViewModel()));
                    }
                }

                var _otherExams = _repos.StudentCourses
                                        .ListWith("Course",
                                                  "Course.Units",
                                                  "Course.Units.Exams",
                                                  "Course.Units.Exams.Unit")
                                        .Where(x => x.StudentId == id)
                                        .Select(x => x.Course)
                                        .SelectMany(x => x.Units)
                                        .SelectMany(x => x.Exams)
                                        .Select(TransformFuncs.ToViewModel());


                if (_otherExams != null)
                {
                    exams.AddRange(_otherExams);
                }
            }
            else if (typeof(T) == typeof(Lecturer))
            {
                var lec = _repos.Lecturers
                                .GetWith(id,
                                       "Units",
                                       "Units.Exams",
                                       "Units.Exams.Unit");

                if (lec.Units != null)
                {
                    var _exams = lec.Units.SelectMany(x => x.Exams)
                                          .Select(TransformFuncs.ToViewModel());

                    exams.AddRange(_exams);
                }
            }

            return exams.Take(count)
                        .ToList();
        }
    }
}
