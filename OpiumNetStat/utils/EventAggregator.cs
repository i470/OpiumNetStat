using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;


namespace OpiumNetStat.utils
{

    [Export(typeof(IEventAggregator))]
    public class EventAggregatorService : IEventAggregator
    {
        
        private static readonly object _mutex = new object();

        
        private static readonly Dictionary<Type, List<Tuple<bool, WeakReference>>>
            _subscribers = new Dictionary<Type, List<Tuple<bool, WeakReference>>>();

        
        private static void _RegisterEvent<TEvent>()
        {
            if (_subscribers.ContainsKey(typeof(TEvent)))
            {
                return;
            }

            Monitor.Enter(_mutex);
            if (!_subscribers.ContainsKey(typeof(TEvent)))
            {
                _subscribers.Add(typeof(TEvent), new List<Tuple<bool, WeakReference>>());
            }
            Monitor.Exit(_mutex);
        }

        
        public void Publish<TEvent>(TEvent sampleEvent)
        {
          

            if (!_subscribers.ContainsKey(typeof(TEvent)))
            {
                // no one is listening
                return;
            }

            // snapshot the list
            Monitor.Enter(_mutex);
            var subscribers = (from sub in _subscribers[typeof(TEvent)] select sub).ToArray();
            Monitor.Exit(_mutex);

            // now filter through and mark any dead subscriptions
            var dead = new List<Tuple<bool, WeakReference>>();
            foreach (var sub in subscribers)
            {
                var sink = sub.Item2.Target as IEventSink<TEvent>;
                if (sink == null || !sub.Item2.IsAlive)
                {
                    dead.Add(sub);
                }
                else
                {
                    if (sub.Item1)
                    {
                        Helper.ExecuteOnUI(() => sink.HandleEvent(sampleEvent));
                    }
                    else
                    {
                        sink.HandleEvent(sampleEvent);
                    }
                }
            }

            // scrub the dead subscriptions
            Monitor.Enter(_mutex);
            foreach (var deadSub in dead.Where(deadSub => _subscribers[typeof(TEvent)].Contains(deadSub)))
            {
                _subscribers[typeof(TEvent)].Remove(deadSub);
            }
            Monitor.Exit(_mutex);
        }

      
        public void Subscribe<TEvent>(IEventSink<TEvent> subscriber)
        {
            _RegisterEvent<TEvent>();
            _subscribers[typeof(TEvent)].Add(Tuple.Create(false, new WeakReference(subscriber)));
        }

        
        public void SubscribeOnDispatcher<TEvent>(IEventSink<TEvent> subscriber)
        {
            _RegisterEvent<TEvent>();
            _subscribers[typeof(TEvent)].Add(Tuple.Create(true, new WeakReference(subscriber)));
        }
        
        public void Unsubscribe<TEvent>(IEventSink<TEvent> unsubscriber)
        {
            if (!_subscribers.ContainsKey(typeof(TEvent)))
            {
                return;
            }
            var unsub = (from s in _subscribers[typeof(TEvent)]
                         where ReferenceEquals(s.Item2.Target, unsubscriber)
                         select s).ToList();
            Monitor.Enter(_mutex);
            {
                foreach (var subCast in
                    unsub.Select(sub => sub)
                        .Where(subCast => _subscribers[typeof(TEvent)].Contains(subCast)))
                {
                    _subscribers[typeof(TEvent)].Remove(subCast);
                }
            }
            Monitor.Exit(_mutex);
        }
    }
}