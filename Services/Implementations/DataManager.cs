using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DAL.Models;
using System.Reflection;
using Common.ViewModels;
using System.Collections;

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
                                            .TakeWhile(x => x.Student != null)
                                            .Select(x => x.Student)
                                            .Take(count)
                                            .Distinct()
                                            .ToList();
            return students;
        }

        public IEnumerable<Class> MyClasses<T>(int id, int count) where T : class
        {
            List<Class> classes = new List<Class>();

            if(typeof(T) == typeof(Student))
            {
                var _cl = _repos.Students
                                .ListWith(
                                        "StudentUnits",
                                        "StudentUnits.Unit",
                                        "StudentUnits.Unit.Class",
                                        "StudentUnits.Unit.Class.Units",
                                        "StudentUnits.Unit.Class.Likes")
                                .First(x => x.Id == id)
                                .StudentUnits
                                .Select(x => x.Unit)
                                .Select(x => x.Class)
                                .TakeWhile(x => x == null);

                if(_cl != null)
                {
                    classes.AddRange(_cl);
                }

                var _cls = _repos.Students
                                 .ListWith(
                                  "Course",
                                  "Course.Units",
                                  "Course.Units.Class",
                                  "Course.Units.Class.Units",
                                  "Course.Units.Class.Likes")
                                  .First(x => x.Id == id)
                                  ?.Course
                                  ?.Units
                                  .TakeWhile(x => x.Class != null)
                                  .Select(x => x.Class);

                if(_cls != null)
                {
                    classes.AddRange(_cls);
                }
            }
            else if(typeof(T) == typeof(Lecturer))
            {
                var _cl = _repos.Lecturers
                                .GetWith(id,
                                        "Units",
                                        "Units.Class",
                                        "Units.Class.Units",
                                        "Units.Class.Likes")
                                .Units
                                .TakeWhile(x => x == null)
                                .Select(x => x.Class);

                if(_cl != null)
                {
                    classes.AddRange(_cl);
                }
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

        public ExamDetailsViewModel GetExam(int id)
        {
            Exam exam = _repos.Exams.GetWith(id, "Unit",
                                     "Unit.Course",
                                     "Unit.Lecturer",
                                     "Unit.Lecturer.Profile",
                                     "Questions",
                                     "Questions.Answers",
                                     "Likes",
                                     "Comments");


            if (exam == null)
            {
                throw new Exception("Exam with that id does not exist in records.");
            }

            var model = new ExamDetailsViewModel()
            {
                Code = exam.Code,
                Comments = exam.Comments,
                Date = exam.Date,
                End = exam.End,
                Id = exam.Id,
                Instructor = exam.Unit.Lecturer.Profile,
                Unit = new ExamUnit()
                {
                    Id = exam.Unit.Id,
                    Name = exam.Unit.Name
                },
                Course = new ExamCourse()
                {
                    Id = exam.Unit.Course.Id,
                    Code = exam.Unit.Course.Code,
                    Name = exam.Unit.Course.Name,
                    Type = exam.Unit.Course.Type
                },
                Name = exam.Name,
                Likes = exam.Likes,
                Start = exam.Start,
                Moment = exam.Moment,
                Questions = exam.Questions.Select(q => new ExamQuestion()
                {
                    Id = q.Id,
                    Marks = q.Marks,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new QuestionAnswer()
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };

            return model;
        }
    }
}
