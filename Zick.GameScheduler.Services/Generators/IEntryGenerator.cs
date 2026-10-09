using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Services.Generators;

public interface IEntryGenerator<TUser> where TUser : IdentityUser
{
    Task<string> GenerateEntryListForSessions(Guid sessionId);
    Task<string> CreateEntryListForRestart(Guid sessionId);
}