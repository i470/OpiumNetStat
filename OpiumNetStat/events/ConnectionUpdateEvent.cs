using OpiumNetStat.model;
using Prism.Events;

namespace OpiumNetStat.events
{
    public class ConnectionUpdateEvent: PubSubEvent<NetStatResult>
    {
    }
}
