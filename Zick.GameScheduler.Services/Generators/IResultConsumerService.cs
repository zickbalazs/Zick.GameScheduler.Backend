namespace Zick.GameScheduler.Services.Generators;

public interface IResultConsumerService
{
    Task ConsumeAndUploadResult(Guid sessionId, string jsonContent);
}