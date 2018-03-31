using Common.ViewModels;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class TransformFuncs
    {
        public static Func<Exam, ExamViewModel> ToViewModel()
        {
            ExamViewModel func(Exam exam)
            {
                if(exam != null)
                {
                    var model = new ExamViewModel()
                    {
                        Id = exam.Id,
                        Code = exam.Code,
                        Moment = exam.Moment,
                        Name = exam.Name,
                        Unit = exam.Unit.Name
                    };

                    return model;
                }
                else
                {
                    return null;
                }
            }

            return func;
        }
    }
}
