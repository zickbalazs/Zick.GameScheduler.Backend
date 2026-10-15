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
        return new FirstMethodException(500, "A database error has occured, contact the administrators.");
    }
    
}

public class FirstMethodException(int code, string? message)
{
    public int Code { get; } = code;
    public string? Message { get; } = message;
}