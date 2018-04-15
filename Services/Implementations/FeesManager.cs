using Common.Exceptions;
using Common.ViewModels;
using DAL.Enums;
using DAL.Models;
using DAL.Models.Enums;
using DAL.Models.Fees;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Services.Implementations
{
    public class FeesManager : IFeesManager
    {
        private readonly IRepositoryFactory _repos;
        private static SelectionType _selection;
        private static BaseFeeStructure _baseFee;

        public FeesManager(IRepositoryFactory factory)
        {
            _repos = factory;
        }

        public BaseFeeStructure BaseFee
        {
            get
            {
                if(_baseFee != null)
                {
                    return _baseFee;
                }
                else
                {
                    _baseFee = _repos.BaseFeeStructures.List.First();

                    return _baseFee;
                }
            }
        }

        public FeesViewModel GetFee4Course(int course, int year = 1, int semester = 1)
        {
            var crs = _repos.Courses.GetWith(course, "FeeStructures", "Units");

            if (crs == null)
            {
                return null;
            }
            else
            {
                crs = Trim(crs, year, semester);

                return ToFormat(crs);
            }
        }
        public FeesViewModel GetFull4Course(int course)
        {
            var crs = _repos.Courses.GetWith(course, "FeeStructures","Units");

            if(crs == null)
            {
                return null;
            }
            else
            {
                return ToFormat(crs);
            }
        }

        public FeesViewModel GetFees4Student(int student, int year = 1, int semester = 1)
        {
            List<FeesViewModel> list = new List<FeesViewModel>();

            // main course
            var _student = _repos.Students.GetWith(student,
                                "Course",
                                "Course.Units",
                                "Course.FeesStructures");

            if (_student == null)
            {
                _selection = _student.Selection;

                if (_student.Course != null)
                {
                    _student.Course = Trim(_student.Course, year, semester);
                    list.Add(ToFormat(_student.Course));
                }
            }
            // other courses
            var courses = _repos.StudentCourses
                                .ListWith("Course", "Course.Units", "Course.FeeStructures")
                                .Where(x => x.StudentId == student)
                                .Select(x => x.Course)
                                .ToList();

            for (int i = 0; i < courses.Count; i++)
            {
                var item = Trim(courses[i], year, semester);

                list.Add(ToFormat(item));
            }

            if (list.Count > 0)
            {
                return Aggregate(list);
            }
            else
            {
                return null;
            }
        }
        public FeesViewModel GetFull4Student(int student)
        {
            List<FeesViewModel> list = new List<FeesViewModel>();

            // main course
            var _student = _repos.Students.GetWith(student,
                                "Course",
                                "Course.Units",
                                "Course.FeesStructures");

            if(_student == null)
            {
                _selection = _student.Selection;

                if(_student.Course != null)
                {
                    list.Add(ToFormat(_student.Course));
                }
            }
            // other courses
            var courses = _repos.StudentCourses
                                .ListWith("Course", "Course.Units", "Course.FeeStructures")
                                .Where(x => x.StudentId == student)
                                .Select(x => x.Course)
                                .ToList();

            foreach (var item in courses)
            {
                list.Add(ToFormat(item));
            }

            if(list.Count > 0)
            {
                return Aggregate(list);
            }
            else
            {
                return null;
            }
        }

        public FeePayment MakePayment(int student, PaymentMethod method, decimal amount)
        {
            if(student < 1)
            {
                throw new LepadException("Payment failed! Invalid student id.");
            }

            var _student = _repos.Students.GetWith(student, "Payments");

            if(_student == null)
            {
                throw new LepadException("Payment failed! Student record not found.");
            }

            var payment = new FeePayment()
            {
                Amount = amount,
                IsGeneral = true,
                Method = method,
                StudentId = student
            };

            payment = _repos.FeePayments.Create(payment);
            _repos.Commit();

            if (_student.Payments == null) { _student.Payments = new List<FeePayment>(); };
            _student.Payments.Add(payment);
            _student = _repos.Students.Update(_student);
            _repos.Commit();

            return payment;
        }
        public FeePayment MakePayment(int student, int course, PaymentMethod method, decimal amount)
        {
            if (student < 1)
            {
                throw new LepadException("Payment failed! Invalid student id.");
            }

            var _student = _repos.Students.GetWith(student, "Payments");

            if (_student == null)
            {
                throw new LepadException("Payment failed! Student record not found.");
            }

            var payment = new FeePayment()
            {
                Amount = amount,
                IsGeneral = true,
                Method = method,
                StudentId = student,
                CourseId = course
            };

            payment = _repos.FeePayments.Create(payment);
            _repos.Commit();

            if (_student.Payments == null) { _student.Payments = new List<FeePayment>(); };
            _student.Payments.Add(payment);
            _student = _repos.Students.Update(_student);
            _repos.Commit();

            return payment;
        }

        public FeeBalanceViewModel GetMyBalance(int student)
        {
            // get total bill
            var fees = GetFull4Student(student);
            // get my payments
            decimal paid = _repos.Students.GetWith(student, "Payments").Payments.Sum(x => x.Amount);

            return new FeeBalanceViewModel()
            {
                TotalBill = fees.Total,
                Paid = paid
            };
        }
        public FeeBalanceViewModel GetMyBalance(int student, int course)
        {
            // get total bill
            var fees = GetFull4Course(course);
            // get my payments
            decimal paid = _repos.Students.GetWith(student, "Payments")
                                          .Payments
                                          .Where(x => x.CourseId == course)
                                          .Sum(x => x.Amount);

            return new FeeBalanceViewModel()
            {
                TotalBill = fees.Total,
                Paid = paid
            };
        }
        public FeeBalanceViewModel GetMyBalance(int student, int year = 1, int sem = 1)
        {
            // get bill for specified range
            var fees = GetFees4Student(student, year, sem);

            // get amount paid for specified range
            decimal paid = _repos.Students.GetWith(student, "Payments")
                                 .Payments
                                 .Where(x => x.Level == year && x.Semester == sem)
                                 .Sum(x => x.Amount);

            return new FeeBalanceViewModel()
            {
                TotalBill = fees.Total,
                Paid = paid
            };
        }

        #region Private Section
        private FeesViewModel ToFormatOne(Course course)
        {
            int sems = course.Years * 2;

            var model = new FeesViewModel()
            {
                Accomodation = BaseFee.Accomodation * sems,
                Internet = BaseFee.Internet * sems,
                Library = BaseFee.Library * sems,
                Medical = BaseFee.Medical * sems,
                Project = BaseFee.Project,
                Trip = BaseFee.Trip * 2,
                Course = course.Name
            };

            switch (_selection)
            {
                case SelectionType.Regular:
                    model.Tuition = BaseFee.PerUnit * course.Units.Count;
                    break;
                case SelectionType.Parallel:
                    model.Tuition = BaseFee.ParallelPerUnit * course.Units.Count;
                    break;
                default:
                    break;
            }

            return model;
        }
        private FeesViewModel ToFormat(Course crs)
        {
            if (crs.Units != null || crs.Units.Count > 0)
            {
                if (crs.FeeStructures == null || crs.FeeStructures.Count < 1)
                {
                    if (BaseFee != null)
                    {
                        return ToFormatOne(crs);
                    }
                    else
                    {
                        throw new NullFeeStructure();
                    }
                }
                else
                {
                    return ToFormat(crs);
                }
            }
            else
            {
                throw new LepadException("Generating fees structure from a course with 0 units failed! Contact Administrator.");
            }
        }
        private FeesViewModel Format(Course crs)
        {
            var model = new FeesViewModel()
            {
                Accomodation = crs.FeeStructures.Sum(x => x.Accomodation),
                Internet = crs.FeeStructures.Sum(x => x.Internet),
                Library = crs.FeeStructures.Sum(x => x.Library),
                Medical = crs.FeeStructures.Sum(x => x.Medical),
                Project = crs.FeeStructures.Sum(x => x.Project),
                Trip = crs.FeeStructures.Sum(x => x.Trip),
                Course = crs.Name
            };

            switch (_selection)
            {
                case SelectionType.Regular:
                    model.Tuition = crs.FeeStructures
                                         .Sum(x => x
                                         .PerUnit * crs.Units
                                         .Count(u => u.Level == x.Year && u.Semester == x.Semester));
                    break;
                case SelectionType.Parallel:
                    model.Tuition = crs.FeeStructures
                                         .Sum(x => x
                                         .ParallelPerUnit * crs.Units
                                         .Count(u => u.Level == x.Year && u.Semester == x.Semester));
                    break;
                default:
                    break;
            }

            return model;
        }
        private FeesViewModel Aggregate(List<FeesViewModel> list)
        {
            return new FeesViewModel()
            {
                Accomodation = list.Sum(x => x.Accomodation),
                Internet = list.Sum(x => x.Internet),
                Library = list.Sum(x => x.Library),
                Medical = list.Sum(x => x.Medical),
                Project = list.Sum(x => x.Project),
                Trip = list.Sum(x => x.Trip),
                Tuition = list.Sum(x => x.Tuition),
                Course = list.First().Course
            };
        }
        private Course Trim(Course crs,int year,int semester)
        {
            // trim existing fee structures
            crs.FeeStructures = crs.FeeStructures
                                   .Where(x => x.Year == year && x.Semester == semester)
                                   .ToList();
            // trim units
            crs.Units = crs.Units
                           .Where(x => x.Level == year && x.Semester == semester)
                           .ToList();

            return crs;
        }
        #endregion
    }
}
