using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Sprinkles.Module.Annotations;
using NuGet.Protocol.Plugins;

namespace Cake.Sprinkles.Module
{
    internal class SprinklesException : Exception
    {
        public override string Message { get; }

        public string InnerMessage { get; init; }

        public string TaskArgumentName { get; init; }

        public string TaskArgumentDescription { get; init; }

        public string[] AdditionalInformation { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SprinklesException"/> class.
        /// </summary>
        public SprinklesException(PropertyInfo property, string message, params string[] additionalInfo)
        {
            InnerMessage = message;
            TaskArgumentName = SprinklesDecorations.GetArgumentName(property);
            TaskArgumentDescription = SprinklesDecorations.GetArgumentDescription(property);
            AdditionalInformation = additionalInfo;
            Message = ComposeErrorMessage();
        }

        private string ComposeErrorMessage()
        {
            var formatArguments = new List<string>();
            var formatString = "{0} (Parameter: '{1}')";

            formatArguments.Add(InnerMessage);
            formatArguments.Add(TaskArgumentName);

            if (!String.IsNullOrEmpty(TaskArgumentDescription))
            {
                formatString += " (Description: {2})";
                formatArguments.Add(TaskArgumentDescription);
            }
            else
            {
                formatArguments.Add(String.Empty);
            }

            for (var i = 0; i < AdditionalInformation.Length; i++)
            {
                formatString += " ({" + (i + 3) + "})";
                formatArguments.Add(AdditionalInformation[i]);
            }

            return String.Format(formatString, formatArguments.Cast<object?>().ToArray());
        }
    }
}
