using Microsoft.AspNetCore.Mvc;

namespace Zick.GameScheduler.Backend.API.Utils;

public class ResponseUtilsFactory(ILogger logger)
{
    public static ResponseUtilsFactory Create(ILogger logger)
    {
        return new(logger);
    }

    public FirstMethodException HandleFirstMethodException(Exception e)
    {
        if (e is ArgumentNullException or InvalidOperationException)
        {
            return new FirstMethodException(404, null);
        }

        logger.LogError("an unhandled exception has occured:\nmessage: {errorMessage}\nstack:{stackTrace}",
            e.Message,
            e.StackTrace);
        return new FirstMethodException(500, new ("A database error has occured."));
    }

    public ExceptionDetail HandleAuthExceptions(Exception e)
    {
        if (e is ArgumentNullException or InvalidOperationException)
        {
            return new (400, e.Message);
        }

        if (e is UnauthorizedAccessException)
            return new(401, e.Message);

        return new(500, "An unhandled error has occured, contact the administrators.");
    }
    
}

public class FirstMethodException(int code, string? message)
{
    public int Code { get; } = code;

    public FirstMethodExceptionDetail? Message
    {
        get
        {
            if (message != null)
                return new(message);
            return null;
        }
    }
}

public class ExceptionDetail(int code, string message)
{
    public string Message => message;
    public int Code => code;
}

public class FirstMethodExceptionDetail(string message)
{
    public string Detail { get => message; }
}