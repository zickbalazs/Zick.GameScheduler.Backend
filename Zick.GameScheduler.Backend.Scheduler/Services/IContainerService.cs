namespace Zick.GameScheduler.Backend.Scheduler.Services;

public interface IContainerService
{
    Task<string> StartContainerForSession(Guid sessionId);
    Task StopContainerForSession(Guid sessionId, string containerId);
    Task RecoverContainerForSession(Guid sessionId, string containerId);
}