using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Dishes.Services;

namespace Dishes.Tools
{
    public class QueueHandler
    {
        private static readonly string LogfileTxt = AppSettingsService.Instance.QueueHandlerLogFile;
        private readonly Action _filterGui;
        private readonly ConcurrentQueue<string> _queue;
        // ReSharper disable once CollectionNeverQueried.Global
        public List<Task> Tasks { get; }
        private static readonly SemaphoreLocker Locker = new SemaphoreLocker();

        public QueueHandler(Action filterGui)
        {
            File.Delete(LogfileTxt);
            _filterGui = filterGui;
            _queue = new ConcurrentQueue<string>();
            Tasks = new List<Task>();
        }

        public void Trigger(string s)
        {
            Locker.Lock(() =>
            {
                AppendText($"Add {s} to queue");
                _queue.Enqueue(s);
                Tasks.Add(Task.Run(Do));
            });
        }

        private void AppendText(string s)
        {
            using (StreamWriter sw = File.AppendText(LogfileTxt))
            {
                sw.WriteLine($"{DateTime.Now.ToLongTimeString()} - {s}");
            }
        }

        private async Task Do()
        {
            await Task.Delay(500);
            await Locker.LockAsync(async () =>
            {
                try
                {
                    if (_queue.TryDequeue(out var s))
                    {
                        if (_queue.Any())
                        {
                            AppendText($"Skipping {s} since queue not empty");
                        }
                        else
                        {
                            await Dispatcher.UIThread.InvokeAsync(_filterGui);
                            AppendText($"{s} processed");
                        }
                    }
                    else
                    {
                        AppendText($"Could not dequeue");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}