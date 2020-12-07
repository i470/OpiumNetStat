using System;

namespace OpiumNetStat.Pipeline
{
    public class PipelineAction: IPipline
    {
      
        private readonly bool _immediate;

        
        public Action Execute { get; set; }

      
        public PipelineAction()
        {
        }

        public PipelineAction(bool immediate)
        {
            _immediate = immediate;
        }

       
        public PipelineAction(Action action)
        {
            _immediate = false;
            Execute = action;
        }

        
        public PipelineAction(Action action, bool immediate)
        {
            _immediate = immediate;
            Execute = action;
        }

        
        public void Invoke()
        {
            Execute();
            if (_immediate)
            {
                Invoked();
            }
        }

        public Action Invoked { get; set; }
    }
}

