using Common.ViewModels;
using DAL.Enums;
using DAL.Models.Fees;

namespace Services.Interfaces
{
    public interface IFeesManager
    {
        // Get for course
        FeesViewModel GetFee4Course(int course, int year=1,int semester=1);
        FeesViewModel GetFull4Course(int course);

        // Get for student
        FeesViewModel GetFees4Student(int student,int year=1,int semester=1);
        FeesViewModel GetFull4Student(int student);

        // Get for a student of a certain course
        FeeBalanceViewModel GetMyBalance(int student);
        FeeBalanceViewModel GetMyBalance(int student, int course);
        FeeBalanceViewModel GetMyBalance(int student, int year = 1, int sem = 1);

        // Payments
        FeePayment MakePayment(int student, PaymentMethod paymentMethod, decimal amount);
        FeePayment MakePayment(int student, int course, PaymentMethod method, decimal amount);
    }
}
