using Common.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Comparers
{
    public class ClassUnitEqualityComparer : IEqualityComparer<ClassUnitViewModel>
    {
        public bool Equals(ClassUnitViewModel x, ClassUnitViewModel y)
        {
            return x.UnitId == y.UnitId;
        }

        public int GetHashCode(ClassUnitViewModel obj)
        {
            return obj.UnitId.GetHashCode();
        }
    }
}
