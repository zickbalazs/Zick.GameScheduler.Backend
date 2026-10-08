using Microsoft.AspNetCore.Identity;

namespace Zick.GameScheduler.Backend.EntryListGenerator;

public interface IEntryGenerator<TUser> where TUser : IdentityUser
{
    Task<string> GenerateEntryListForSessions(Guid sessionId);
    Task<string> CreateEntryListForRestart(Guid sessionId);
}