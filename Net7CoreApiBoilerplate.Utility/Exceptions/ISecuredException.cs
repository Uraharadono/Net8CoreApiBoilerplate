using System.Net;

namespace Net7CoreApiBoilerplate.Utility.Exceptions
{
    /// <summary>
    ///     When an <seealso cref="System.Exception" /> implements this class it can be used to provide details to the client
    ///     about what went wrong. If the exception does not implement this interface a generic error is shown to the client to
    ///     prevent the leaking of details.
    /// </summary>
    public interface ISecuredException
    {
        /// <summary>
        ///     Gets the public message that will be shown to the client.
        /// </summary>
        string PublicMessage { get; }
        string[] PublicErrors { get; }

        /// <summary>
        ///     Gets the HTTP response code that needs to be used to display the message.
        /// </summary>
        HttpStatusCode HttpResponseCode { get; }
    }
}