using System;

namespace OpiumNetStat.Pipeline
{

    /// yield return new PipelineEvent(
    ///    () => { }, 
    ///    h => DataAccess.LoadCompleted += h, 
    ///    h => DataAccess.LoadCompleted -= h);
    ///    

    public class PipelineEvent:IPipeLine
    {
       
        private readonly Action _begin;

       
        private readonly EventHandler _handler;

     
        private readonly Action<EventHandler> _unregister;

       
        public PipelineEvent(Action begin, Action<EventHandler> register, Action<EventHandler> unregister)
        {
            _begin = begin;
            _unregister = unregister;
            _handler = Completed;
            register(_handler);
        }

        public void Completed(object sender, EventArgs args)
        {
            Result = args;
            _unregister(_handler);
            Invoked();
        }

     
        public EventArgs Result { get; private set; }

       
        public void Invoke()
        {
            _begin();
        }

     
        public Action Invoked { get; set; }
    }


    public class PipelineEvent<T> : IPipeLine where T : EventArgs
    {
        
        private readonly Action _begin;

      
        private readonly EventHandler<T> _handler;

       
        private readonly Action<EventHandler<T>> _unregister;


        public PipelineEvent(Action begin, Action<EventHandler<T>> register, Action<EventHandler<T>> unregister)
        {
            _begin = begin;
            _unregister = unregister;
            _handler = Completed;
            register(_handler);
        }

      
        public void Completed(object sender, T args)
        {
            Result = args;
            _unregister(_handler);
            Invoked();
        }

     
        public T Result { get; private set; }

        
        public void Invoke()
        {
            _begin();
        }

      
        public Action Invoked { get; set; }
    }

}
