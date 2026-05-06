using System.Collections.Concurrent;
using Gearbox.Domain.Entities;

namespace Gearbox.Application.BackgroundJobs
{
    public class EmailQueue
    {
        private readonly ConcurrentQueue<EmailJob> _queue = new();
        private readonly SemaphoreSlim _signal = new(0);

        public void Enqueue(EmailJob job)
        {
            _queue.Enqueue(job);
            _signal.Release();
        }

        public async Task<EmailJob> DequeueAsync(CancellationToken token)
        {
            await _signal.WaitAsync(token);

            _queue.TryDequeue(out var job);
            return job!;
        }
    }
}