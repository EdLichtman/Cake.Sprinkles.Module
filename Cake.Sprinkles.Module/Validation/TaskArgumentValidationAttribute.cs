using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cake.Sprinkles.Module.Validation.Exceptions;

namespace Cake.Sprinkles.Module.Validation
{
    /// <summary>
    /// Allows you to create your own validation for incoming Task Arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public abstract class TaskArgumentValidationAttribute : Attribute
    {
        internal const string Message_IsArgumentValidShouldNotThrow = $"The IsArgumentValid implementation for a {nameof(TaskArgumentValidationAttribute)} should not throw an exception. It should only return true, if the value is valid, or false if the value is not valid.";
        /// <summary>
        /// Gets a short Description to indicate to users of the build tool how an argument is validated.
        /// </summary>
        public abstract string DescriptionOfValidation { get; }

        /// <summary>
        /// Gets the error message to indicate to users of the build tool that a failure occurred.
        /// </summary>
        protected abstract string ErrorMessage { get; }

        /// <summary>
        /// Validates the incoming string argument.
        /// </summary>
        /// <param name="argument">The argument</param>
        /// <returns>A value indicating whether <c>True</c> is valid, or <c>False</c> if not valid.</returns>
        protected abstract bool IsArgumentValid(string? argument);

        internal void Validate(PropertyInfo propertyInfo, string? argument)
        {
            bool isArgumentValid;
            try
            {
                isArgumentValid = IsArgumentValid(argument);
            }
            catch(Exception ex)
            {
                throw new SprinklesCaptureException(
                    ex,
                    GetType(),
                    Message_IsArgumentValidShouldNotThrow);
            }
            
            if (!isArgumentValid)
            {
                throw new SprinklesException(propertyInfo, 
                    ErrorMessage,
                    $"Invalid Argument: {argument}");
            }
        }

        internal void Validate(PropertyInfo propertyInfo, IList<string> arguments)
        {
            var invalidArguments = new List<string>();
            foreach(var argument in arguments) {
                try
                {
                    Validate(propertyInfo, argument);
                } 
                catch(Exception ex)
                {
                    var invalidArgument = (ex as SprinklesException)?.AdditionalInformation?.FirstOrDefault();
                    if (invalidArgument == null || !invalidArgument.StartsWith("Invalid Argument: ")) {
                        throw new SprinklesCaptureException(
                            ex,
                            GetType(),
                            Message_IsArgumentValidShouldNotThrow);
                    }

                    invalidArguments.Add(invalidArgument);
                }
            }

            if (invalidArguments.Any())
            {
                throw new SprinklesException(
                    propertyInfo,
                    ErrorMessage,
                    invalidArguments.ToArray());
            }
        }

    }
}
