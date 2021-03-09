using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Dishes
{
    public class QueueHandler
    {
        private const string LogfileTxt = @"C:\Code\Dishes\Dishes\bin\Debug\net5.0\logfile.txt";
        private readonly Action _filterGui;
        private readonly ConcurrentQueue<string> _queue;
        public List<Task> Tasks { get; }
        private static readonly SemaphoreLocker Locker = new();

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