namespace Zick.GameScheduler.Services.Data;

public interface ISessionService
{
    Task DeleteSession(Guid id);
    Task CancelSession(Guid id);
    Task StartSessionNow(Guid id);
}