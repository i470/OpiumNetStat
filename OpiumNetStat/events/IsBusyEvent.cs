using Prism.Events;
using System;

namespace OpiumNetStat.events
{
    public class IsBusyEvent : PubSubEvent<bool>
    {
        internal void Subcribe()
        {
            throw new NotImplementedException();
        }
    }
}
