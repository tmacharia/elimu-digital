using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ViewModels
{
    /// <summary>
    /// Results from update reflector showing the number of fields with differences
    /// in values between models and the new updated entity with updated data
    /// from the new view model.
    /// 
    /// </summary>
    /// <typeparam name="T">Type of object</typeparam>
    public class UpdateResult<T> where T : class
    {
        public int TotalUpdates { get; set; }
        public T Value { get; set; }
    }
}
