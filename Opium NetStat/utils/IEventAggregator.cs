namespace Opium_NetStat.utils
{

    public interface IEventAggregator
        {
           
            void Publish<TEvent>(TEvent sampleEvent);
            void Subscribe<TEvent>(IEventSink<TEvent> subscriber);
            void SubscribeOnDispatcher<TEvent>(IEventSink<TEvent> subscriber);
             void Unsubscribe<TEvent>(IEventSink<TEvent> unsubscriber);
        }

    public interface IEventSink<in T>
    {
       void HandleEvent(T publishedEvent);
    }
}

