using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    public class FeesViewModel
    {
        public decimal Tuition { get; set; }
        public decimal Library { get; set; }
        public decimal Internet { get; set; }
        public decimal Accomodation { get; set; }
        public decimal Medical { get; set; }
        public decimal Trip { get; set; }
        public decimal Project { get; set; }

        public decimal Total
        {
            get
            {
                return Tuition +
                       Library +
                       Internet +
                       Accomodation +
                       Medical +
                       Trip +
                       Project;
            }
        }

        public string Course { get; set; }
    }
}
