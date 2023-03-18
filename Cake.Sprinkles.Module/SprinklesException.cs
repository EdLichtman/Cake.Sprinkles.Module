using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Sprinkles.Module.Annotations;

namespace Cake.Sprinkles.Module
{
    internal class SprinklesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SprinklesException"/> class.
        /// </summary>
        public SprinklesException(PropertyInfo property, string message, params string[] additionalInfo) : base(ComposeErrorMessage(property, message, additionalInfo)) { }

        private static String ComposeErrorMessage(PropertyInfo property, string message, string[] additionalInfo)
        {
            var formatArguments = new List<string>();
            var formatString = "{0} (Parameter: '{1}')";

            formatArguments.Add(message);
            formatArguments.Add(SprinklesDecorations.GetArgumentName(property));

            var description = SprinklesDecorations.GetArgumentDescription(property);
            if (!String.IsNullOrEmpty(description))
            {
                formatString += " (Description: {2})";
                formatArguments.Add(description);
            }
            else
            {
                formatArguments.Add(String.Empty);
            }
            
            for (var i = 0; i < additionalInfo.Length; i++)
            {
                formatString += " ({" + (i + 3) + "})";
                formatArguments.Add(additionalInfo[i]);
            }

            return String.Format(formatString, formatArguments.Cast<object?>().ToArray());
        }
    }
}
