using System;
using System.Threading.Tasks;

namespace DocumGen.Application.Contracts.MessageBus
{
    public interface IMessageConsumer
    {
        void StartConsuming<T>(string queueName, Func<T, Task> handleMessage);
        void StopConsuming();
    }
}
