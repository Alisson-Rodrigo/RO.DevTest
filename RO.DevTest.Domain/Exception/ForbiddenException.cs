using RO.DevTest.Domain.Exception;
using System.Net;

public class ForbiddenException : ApiException
{
    public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;

    public ForbiddenException(string error) : base(error) { }
}
