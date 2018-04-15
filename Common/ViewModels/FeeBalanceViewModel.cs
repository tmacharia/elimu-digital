using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class FeeBalanceViewModel
    {
        public decimal TotalBill { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance
        {
            get
            {
                return TotalBill - Paid;
            }
        }
    }
}
