using OpiumNetStat.Model;
using Prism.Events;
using System.Collections.Generic;

namespace OpiumNetStat.events
{
    public class  NetStatReadEvent: PubSubEvent<IList<PortInfo>>
    {
    }
}
