using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Chalkable.AcademicBenchmarkConnector.Connectors;

namespace Chalkable.AcademicBenchmarkImport.Helper
{
    public class LoaderBase<TQueueItem, TResultItem>
    {
        public LoaderBase(IEnumerable<TQueueItem> queue)
        {
            _inputQueue = new ConcurrentQueue<TQueueItem>(queue);
            _outputResult = new ConcurrentBag<TResultItem>();
            _pool = new List<Thread>();
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

            for (var i = 0; i < 10; ++i)
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
    }

    //TODO: Will be tested later if we need this.
    public class StandardRelationsLoader
    {
        public StandardRelationsLoader(IConnectorLocator abConnectorLocator, IEnumerable<Guid> standardIds)
        {
            ConnectorLocator = abConnectorLocator;
            StandardIdsToProcess = new ConcurrentQueue<Guid>(standardIds);
            Result = new ConcurrentBag<AcademicBenchmarkConnector.Models.StandardRelations>();
            _pool = new List<Thread>();
        }

        protected IConnectorLocator ConnectorLocator { get; }
        protected ConcurrentQueue<Guid> StandardIdsToProcess { get; }
        protected ConcurrentBag<AcademicBenchmarkConnector.Models.StandardRelations> Result { get; set; }

        protected void Worker(object o)
        {
            while (!StandardIdsToProcess.IsEmpty)
            {
                Guid standardId;
                while (StandardIdsToProcess.TryDequeue(out standardId))
                {
                    var id = standardId;
                    var standardRel = Task.Run(() => ConnectorLocator.StandardsConnector.GetStandardRelationsById(id)).Result;

                    if (standardRel != null)
                        Result.Add(standardRel);
                }
            }
        }

        public IList<AcademicBenchmarkConnector.Models.StandardRelations> Load()
        {
            for (var i = 0; i < 10; ++i)
            {
                var currentTh = new Thread(Worker);
                currentTh.Start();

                _pool.Add(currentTh);
            }

            WaitAllThreads(_pool);

            return Result.ToList();
        }

        protected static void WaitAllThreads(IList<Thread> threads)
        {
            foreach (var thread in threads)
                thread.Join();
        }

        private readonly IList<Thread> _pool;
    }
}
