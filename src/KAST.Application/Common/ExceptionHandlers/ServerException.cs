using System.Net;

namespace KAST.Application.Common.ExceptionHandlers;

public class ServerException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : Exception(message)
{
    public IEnumerable<string> ErrorMessages { get; } = [message];

    public HttpStatusCode StatusCode { get; } = statusCode;
}