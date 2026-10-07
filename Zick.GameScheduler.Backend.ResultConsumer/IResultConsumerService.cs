namespace Zick.GameScheduler.Backend.ResultConsumer;

public interface IResultConsumerService
{
    Task ConsumeAndUploadResult(Guid sessionId, string jsonContent);
}