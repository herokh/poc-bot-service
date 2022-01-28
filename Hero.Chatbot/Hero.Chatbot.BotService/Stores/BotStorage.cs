using Microsoft.Bot.Builder;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hero.Chatbot.BotService.Stores
{
    public class BotStorage : IStorage
    {
        private readonly IDictionary<string, object> _store = new Dictionary<string, object>();
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public BotStorage()
        {
        }

        public async Task DeleteAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                foreach (var key in keys)
                {
                    _store.Remove(key);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<IDictionary<string, object>> ReadAsync(string[] keys, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                var r = new Dictionary<string, object>();

                foreach (var key in keys)
                {
                    if (_store.ContainsKey(key))
                    {
                        r.Add(key, _store[key]);
                    }
                }

                return r;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task WriteAsync(IDictionary<string, object> changes, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphoreSlim.WaitAsync();

                foreach (var change in changes)
                {
                    if (_store.ContainsKey(change.Key))
                    {
                        _store[change.Key] = change.Value;

                    }
                    else
                    {
                        _store.Add(change.Key, change.Value);
                    }
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
