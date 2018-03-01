using DAL.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAL.Attributes
{
    public class TimeCompare : ValidationAttribute
    {
        private readonly Comparison _comp;
        private readonly string _otherProp;
        private readonly int _duration;
        private readonly DurationSpec _durationSpec;

        /// <summary>
        /// Validation attribute that compares two timestamps using a comparison specification
        /// to ensure the correct timestamps are used.
        /// </summary>
        /// <param name="comparison">Specification to use in comparing with the other timestamp.
        /// </param>
        /// <param name="otherProperty">The Name of the other property to compare with. Use
        /// the exact PropertyName or DisplayName.
        /// </param>
        /// <param name="duration">What is the maximum allowable difference between the two
        /// timestamps;
        /// </param>
        /// <param name="durationSpec">Specification for the <paramref name="duration"/> value
        /// specified. It can either be: Days, Hours, Minutes or Seconds.
        /// </param>
        public TimeCompare(Comparison comparison=Comparison.GreaterThan,
                           string otherProperty=null,
                           DurationSpec durationSpec = DurationSpec.Hours,
                           int duration=1)
        {
            _comp = comparison;
            _otherProp = otherProperty;
            _duration = duration;
            _durationSpec = durationSpec;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null)
            {
                return new ValidationResult("First datetime value required.");
            }

            var type = validationContext.ObjectType;

            object propValue = new object();

            if (!string.IsNullOrWhiteSpace(_otherProp))
            {
                propValue = type.GetProperty(_otherProp).GetValue(validationContext.ObjectInstance);
            }

            DateTime startTime = Convert.ToDateTime(propValue);
            DateTime endTime = Convert.ToDateTime(value);

            TimeSpan diff = endTime.Subtract(startTime);

            var res = CheckDurationSpec(diff);
            if(res != null)
            {
                return res;
            }

            var startValidity = ValidateStart(startTime);
            if (!startValidity.Item1)
            {
                return new ValidationResult(startValidity.Item2.First());
            }

            
            var endValidity = ValidateEnd(endTime);
            if (!endValidity.Item1)
            {
                return new ValidationResult(endValidity.Item2.First());
            }

            return null;
        }
        private static Tuple<bool, List<string>> ValidateStart(DateTime start)
        {
            TimeSpan span = start.TimeOfDay;
            List<string> errors = new List<string>();

            // not below 7:00 A.M
            if (start.Meridiem() == Meridiem.AM)
            {
                if (span.Hours < 7)
                {
                    errors.Add("University policy requires all classes to begin at or after 7:00 A.M");
                }
            }

            // not greater than 7:00 P.M
            if (start.Meridiem() == Meridiem.PM)
            {
                if (span.TotalHours > (7+12))
                {
                    errors.Add("A class can only take a minimum of 2 hours and cannot go beyond 9:00 P.M." +
                        $"Starting at {span.Hours}:{span.Minutes} {start.Meridiem().ToString()} does not follow University class policy rules.");
                }
            }

            return new Tuple<bool, List<string>>((errors.Count > 0) ? false : true, errors);
        }
        private static Tuple<bool, List<string>> ValidateEnd(DateTime stop)
        {
            TimeSpan span = stop.TimeOfDay;
            List<string> errors = new List<string>();

            // not after 9:00 P.M
            if (stop.Meridiem() == Meridiem.PM)
            {
                if (span.Hours > (8+12))
                {
                    errors.Add("University policy requires all classes to end before 9:00 P.M");
                }
            }

            // not before 9:00 A.M
            if (stop.Meridiem() == Meridiem.AM)
            {
                if (span.TotalHours < 9)
                {
                    errors.Add("A class can only take a minimum of 2 hours and cannot start before 7:00 A.M." +
                        $"Ending at {span.Hours}:{span.Minutes} {stop.Meridiem().ToString()} does not follow University class policy rules.");
                }
            }

            return new Tuple<bool, List<string>>((errors.Count > 0) ? false : true, errors);
        }
        private ValidationResult CheckDurationSpec(TimeSpan timeSpan)
        {
            switch (_durationSpec)
            {
                case DurationSpec.Days:
                    if(_comp == Comparison.GreaterThan)
                    {
                        if (timeSpan.Days >= _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be greater than StartTime by at-least {_duration} day(s).");
                    }
                    else if(_comp == Comparison.LessThan)
                    {
                        if (timeSpan.Days < _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be less than StartTime by at-least {_duration} day(s).");
                    }
                    else
                        return null;
                case DurationSpec.Hours:
                    if (_comp == Comparison.GreaterThan)
                    {
                        if (timeSpan.Hours >= _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be greater than StartTime by at-least {_duration} hour(s).");
                    }
                    else if (_comp == Comparison.LessThan)
                    {
                        if (timeSpan.Hours < _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be less than StartTime by at-least {_duration} hour(s).");
                    }
                    else
                        return null;
                case DurationSpec.Minutes:
                    if (_comp == Comparison.GreaterThan)
                    {
                        if (timeSpan.Minutes >= _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be greater than StartTime by at-least {_duration} minute(s).");
                    }
                    else if (_comp == Comparison.LessThan)
                    {
                        if (timeSpan.Minutes < _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be less than StartTime by at-least {_duration} minute(s).");
                    }
                    else
                        return null;
                case DurationSpec.Seconds:
                    if (_comp == Comparison.GreaterThan)
                    {
                        if (timeSpan.Seconds >= _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be greater than StartTime by at-least {_duration} second(s).");
                    }
                    else if (_comp == Comparison.LessThan)
                    {
                        if (timeSpan.Seconds < _duration)
                            return ValidationResult.Success;
                        else
                            return new ValidationResult($"EndTime should be less than StartTime by at-least {_duration} second(s).");
                    }
                    else
                        return null;
                default:
                    return null;
            }
        }
    }
}
