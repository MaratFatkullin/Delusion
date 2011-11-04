using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AI_.Studmix.Model.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class NotEmptyAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var enumerable = value as IEnumerable<object>;
            if (enumerable == null)
                return true;
            if (enumerable.Count() == 0)
                return false;
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format("Коллекция {0} не может быть пустой.", name);
        }
    }
}