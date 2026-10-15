using Zick.GameScheduler.Backend.Data.Models;

namespace Zick.GameScheduler.Backend.Scheduler.Services;

public interface ISessionStatusUpdateService
{
    Task UpdateSessionWithStatus(Guid sessionId, SessionStatus toStatus);
}