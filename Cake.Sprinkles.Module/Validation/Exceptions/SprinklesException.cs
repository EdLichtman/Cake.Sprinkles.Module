using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cake.Core;
using Cake.Sprinkles.Module.Annotations;
using NuGet.Protocol.Plugins;

namespace Cake.Sprinkles.Module.Validation.Exceptions
{
    internal class SprinklesException : Exception
    {
        public override string Message { get; }

        public PropertyInfo PropertyInfo { get; init; }

        public string InnerMessage { get; init; }

        public string? NamespaceClassQualifiedPropertyName { get; init; }

        public string? TaskArgumentName { get; init; }

        public string? TaskArgumentDescription { get; init; }

        public string[] AdditionalInformation { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SprinklesException"/> class.
        /// </summary>
        public SprinklesException(PropertyInfo property, string message, params string[] additionalInfo)
        {
            PropertyInfo = property;
            InnerMessage = message;
            NamespaceClassQualifiedPropertyName = SprinklesDecorations.GetNamespaceClassQualifiedPropertyName(property);
            TaskArgumentName = SprinklesDecorations.GetArgumentName(property);
            TaskArgumentDescription = SprinklesDecorations.GetArgumentDescription(property);
            AdditionalInformation = additionalInfo;
            Message = ComposeErrorMessage();
        }


        private string ComposeErrorMessage()
        {
            var formatArguments = new List<string>();
            var formatString = "{0}";
            formatArguments.Add(InnerMessage);

            if (!string.IsNullOrWhiteSpace(NamespaceClassQualifiedPropertyName))
            {
                formatString += $" (Property: {{{formatArguments.Count}}})";
                formatArguments.Add(NamespaceClassQualifiedPropertyName);
            }

            if (!string.IsNullOrWhiteSpace(TaskArgumentName))
            {
                formatString += $" (Parameter: {{{formatArguments.Count}}})";
                formatArguments.Add(TaskArgumentName);
            }

            if (!string.IsNullOrEmpty(TaskArgumentDescription))
            {
                formatString += $" (Description: {{{formatArguments.Count}}})";
                formatArguments.Add(TaskArgumentDescription);
            }

            for (var i = 0; i < AdditionalInformation.Length; i++)
            {
                formatString += $" ({{{formatArguments.Count}}})";
                formatArguments.Add(AdditionalInformation[i]);
            }

            return string.Format(formatString, formatArguments.Cast<object?>().ToArray());
        }
    }
}
