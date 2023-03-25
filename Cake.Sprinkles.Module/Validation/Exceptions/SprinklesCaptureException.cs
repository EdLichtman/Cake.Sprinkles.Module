using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cake.Sprinkles.Module.Validation.Exceptions
{
    /// <summary>
    /// An exception that is thrown when an exception that is not of type <see cref="SprinklesException"/> is captured.
    /// </summary>
    internal class SprinklesCaptureException : Exception
    {
        public override string Message { get; }
        public string InnerMessage { get; }
        public new Exception InnerException { get; }
        public Type Initiator { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SprinklesException"/> class.
        /// </summary>
        public SprinklesCaptureException(Exception innerException, Type initiator, string message)
        {
            InnerMessage = message;
            Initiator = initiator;
            InnerException = innerException;
            Message = ComposeErrorMessage();
        }

        private string ComposeErrorMessage()
        {
            return $"{Message} (Initiating Type: {Initiator}) (InnerException: {InnerException.Message})";
        }
    }
}
