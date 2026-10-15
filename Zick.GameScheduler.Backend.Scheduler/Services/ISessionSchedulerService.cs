namespace Zick.GameScheduler.Backend.Scheduler.Services;

public interface ISessionSchedulerService
{
    Task CreateSessionFor(Guid sessionId);
    Task CreateWatcherForSession(Guid sessionId);
}