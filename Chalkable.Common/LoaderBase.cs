﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Chalkable.Common
{
    public class LoaderBase<TQueueItem, TResultItem>
    {
        public LoaderBase(IEnumerable<TQueueItem> queue, int maxThreadCount = 10)
        {
            _inputQueue = new ConcurrentQueue<TQueueItem>(queue);
            _outputResult = new ConcurrentBag<TResultItem>();
            _pool = new List<Thread>();
            _maxThreadCount = maxThreadCount;
        }

        private void Worker(object o)
        {
            while (!_inputQueue.IsEmpty)
            {
                TQueueItem inputValue;
                while (_inputQueue.TryDequeue(out inputValue))
                {
                    var result = _getFunc(inputValue);

                    if (result != null)
                        _outputResult.Add(result);
                }
            }
        }

        public IList<TResultItem> Load(Func<TQueueItem, TResultItem> getFunc)
        {
            _getFunc = getFunc;

            for (var i = 0; i < _maxThreadCount; ++i)
            {
                var currentTh = new Thread(Worker);
                currentTh.Start();

                _pool.Add(currentTh);
            }

            WaitAllThreads(_pool);

            return _outputResult.ToList();
        }

        private static void WaitAllThreads(IList<Thread> threads)
        {
            foreach (var thread in threads)
                thread.Join();
        }

        private readonly ConcurrentQueue<TQueueItem> _inputQueue;
        private readonly ConcurrentBag<TResultItem> _outputResult;
        private readonly IList<Thread> _pool;
        private Func<TQueueItem, TResultItem> _getFunc;
        private int _maxThreadCount;
    }
}
