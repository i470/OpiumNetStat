using OpiumNetStat.model;
using Prism.Events;
using System.Collections.Generic;

namespace OpiumNetStat.events
{
    public class ConnectionUpdateEvent: PubSubEvent<List<NetStatResult>>
    {
    }
}
