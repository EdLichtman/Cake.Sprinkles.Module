using Cake.Sprinkles.Module.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Tests.Models.Validation
{
    public class ThrowsErrorValidationAttribute : TaskArgumentValidationAttribute
    {
        public const string InnerExceptionMessageConstant = $"{nameof(ThrowsErrorValidationAttribute)} Inner Exception thrown";
        public const string DescriptionOfValidationConstant = $"{nameof(ThrowsErrorValidationAttribute)} Description Of Validation";
        public const string ErrorMessageConstant = $"{nameof(ThrowsErrorValidationAttribute)} Error Message";
        
        public override string DescriptionOfValidation => DescriptionOfValidationConstant;

        protected override string ErrorMessage => ErrorMessageConstant;

        protected override bool IsArgumentValid(string? argument)
        {
            throw new Exception(InnerExceptionMessageConstant);
        }
    }
}
