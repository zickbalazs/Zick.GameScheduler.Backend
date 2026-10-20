using Zick.GameScheduler.Backend.Data.DTOs;

namespace Zick.GameScheduler.Services.API;

public interface IRegistrationService
{
    Task<IList<RegistrationDTO>> GetRegistrations(string userId);
    Task ModifyRegistration(Guid registrationId, Guid newCarId);
    Task CancelRegistration(Guid registrationId, string userId);
}