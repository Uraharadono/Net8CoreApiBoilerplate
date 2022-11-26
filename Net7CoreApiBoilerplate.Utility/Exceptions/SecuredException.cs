using System;
using System.Net;

namespace Net7CoreApiBoilerplate.Utility.Exceptions
{
    public class SecuredException : Exception, ISecuredException
    {
        public string PublicMessage { get; }
        public string[] PublicErrors { get; }
        public HttpStatusCode HttpResponseCode { get; }

        public SecuredException(HttpStatusCode statusCode, string publicMessage) 
            : base(publicMessage)
        {
            HttpResponseCode = statusCode;
            PublicMessage = publicMessage;
            PublicErrors = null;
        }

        public SecuredException(HttpStatusCode statusCode, string publicMessage, string[] publicErrors)
            : base(publicMessage)
        {
            HttpResponseCode = statusCode;
            PublicMessage = publicMessage;
            PublicErrors = publicErrors;
        }

        public static Exception InvalidRequest()
        {
            return new SecuredException(
                HttpStatusCode.InternalServerError,
                "Request could not be completed because provided data is invalid");
        }

        public static Exception InvalidRequest(string[] errors)
        {
            return new SecuredException(
                HttpStatusCode.InternalServerError,
                "Following validation errors occured",
                errors);
        }

        public static SecuredException ResourceGone(string reason)
        {
            return new SecuredException(HttpStatusCode.Gone, reason);
        }

        public static SecuredException PreconditionFailed(string reason)
        {
            return new SecuredException(HttpStatusCode.PreconditionFailed, reason);
        }

        public static SecuredException Conflict(string reason)
        {
            return new SecuredException(HttpStatusCode.Conflict, reason);
        }

        public static SecuredException BadRequest(string reason)
        {
            return new SecuredException(HttpStatusCode.BadRequest, reason);
        }

        public static SecuredException ResourceNotFound(string reason)
        {
            return new SecuredException(HttpStatusCode.NotFound, reason);
        }
    }
}
