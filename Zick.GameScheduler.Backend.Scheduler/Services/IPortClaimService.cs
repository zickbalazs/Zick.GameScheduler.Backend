namespace Zick.GameScheduler.Backend.Scheduler.Services;

public interface IPortClaimService
{
    Task<string> ClaimPortForSession(Guid id);
    Task EndClaimForSession(Guid id);
}