using System.Net;



public class AppException : Exception
{
    
    public HttpStatusCode StatusCode { get; set; }
    //extra data
    public object AdditionalData { get; set; }
    //pass message and status code
    public AppException(string Message, HttpStatusCode statusCode) : base(Message)
    {
        StatusCode = statusCode;
    }
    public AppException(HttpStatusCode statusCode, string Message,  Exception exception, object exteradata)
        : base(Message, exception)
    {
        StatusCode = statusCode;
        AdditionalData = exteradata;
    }
}
