using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Validation
{
    public class ValidateIntegerLessThanFiveAttribute : TaskArgumentValidationAttribute
    {
        public const string DescriptionOfValidationConstant = $"{nameof(ValidateIntegerLessThanFiveAttribute)} Description Of Validation";
        public const string ErrorMessageConstant = $"{nameof(ValidateIntegerLessThanFiveAttribute)} Error Message";
        public override string DescriptionOfValidation => DescriptionOfValidationConstant;

        protected override string ErrorMessage => ErrorMessageConstant;

        protected override bool IsArgumentValid(string? argument)
        {
            if (!int.TryParse(argument, out var intValue))
            {
                // ParsingError will occur downstream.
                return true;
            }

            return intValue <= 5;
        }
    }
}
