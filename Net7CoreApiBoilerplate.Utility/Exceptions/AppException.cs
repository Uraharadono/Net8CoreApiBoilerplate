using System;
using System.Runtime.Serialization;

namespace Net7CoreApiBoilerplate.Utility.Exceptions
{
    public class AppException : ApplicationException
    {
        public AppException(string message)
            : base(message)
        {
        }

        public AppException(string format, params object[] args)
            : this(string.Format(format, args))
        {
        }

        public AppException(Exception exception, string format, params object[] args)
            : base(string.Format(format, args), exception)
        {
        }

        public AppException(string message, Exception exception)
            : base(message, exception)
        {
        }

        internal AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Only used so that derived classes can restore the internal exception context.
        }
    }
}
