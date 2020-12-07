using System;
using System.Collections.Generic;

namespace OpiumNetStat.Pipeline
{
    public class PipeLineController
    {
        
        private readonly Action<Exception> _exceptionCallback;

       
        private readonly IEnumerator<IPipeLine> _enumerator;

        public PipeLineController(IEnumerable<IPipeLine> workflow, Action<Exception> exceptionCallback)
        {
            _enumerator = workflow.GetEnumerator();
            _exceptionCallback = exceptionCallback;
        }

        
        private void Invoked()
        {
            if (!_enumerator.MoveNext())
                return;

            var next = _enumerator.Current;
            next.Invoked = Invoked;

            // call it
            try
            {
                next.Invoke();
            }
            catch (Exception ex)
            {
                _enumerator.Dispose();
                _exceptionCallback(ex);
            }
        }

        public static void Begin(object pipeLine, Action<Exception> exceptionCallback)
        {
           
            if (pipeLine is IPipeLine)
            {
                pipeLine = new[] { pipeLine as IPipeLine };
            }

         
            if (pipeLine is IEnumerable<IPipeLine>)
            {
                new PipeLineController(pipeLine as IEnumerable<IPipeLine>, exceptionCallback).Invoked();
            }
        }

        public static void Begin(object pipeLine)
        {
            Begin(pipeLine, ex => { });
        }
    }
}
