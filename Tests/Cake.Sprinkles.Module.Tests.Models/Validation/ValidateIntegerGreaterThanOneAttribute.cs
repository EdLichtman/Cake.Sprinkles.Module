using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Validation
{
    public class ValidateIntegerGreaterThanOneAttribute : TaskArgumentValidationAttribute
    {
        public const string DescriptionOfValidationConstant = $"{nameof(ValidateIntegerGreaterThanOneAttribute)} Description Of Validation";
        public const string ErrorMessageConstant = $"{nameof(ValidateIntegerGreaterThanOneAttribute)} Error Message";
        public override string DescriptionOfValidation => DescriptionOfValidationConstant;
        protected override string ErrorMessage => ErrorMessageConstant;

        protected override bool IsArgumentValid(string? argument)
        {
            if (!int.TryParse(argument, out var intValue))
            {
                // ParsingError will occur downstream.
                return true;
            }

            return intValue > 1;
        }
    }
}
