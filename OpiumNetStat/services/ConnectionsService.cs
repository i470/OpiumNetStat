using OpiumNetStat.events;
using Prism.Events;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace OpiumNetStat.services
{
    public class ConnectionsService : IConnectionsService
    {
        IEventAggregator _ea;
        CancellationTokenSource wtoken;
        ActionBlock<DateTimeOffset> task;
      

        public ConnectionsService(IEventAggregator ea)
        {
            _ea = ea;
        }


        public void StartWork()
        {

            wtoken = new CancellationTokenSource();
            task = (ActionBlock<DateTimeOffset>)CreateNeverEndingTask( now =>  DoWork(), wtoken.Token);
            task.Post(DateTimeOffset.Now);
        }

        public void DoWork()
        {
           var ports =  NetStatService.GetNetStatPorts();

            _ea.GetEvent<NetStatReadEvent>().Publish(ports);

        }


        void StopWork()
        {

            using (wtoken)
            {

                wtoken.Cancel();
            }

            wtoken = null;
            task = null;
        }


        ITargetBlock<DateTimeOffset> CreateNeverEndingTask(Action<DateTimeOffset> action, CancellationToken cancellationToken)
        {
            // Validate parameters.
            if (action == null) throw new ArgumentNullException("action");


            ActionBlock<DateTimeOffset> block = null;

            block = new ActionBlock<DateTimeOffset>(async now =>
            {

                action(now);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
                block.Post(DateTimeOffset.Now);
            },
            new ExecutionDataflowBlockOptions
            {
                CancellationToken = cancellationToken
            });

            return block;
        }

    }
}
