using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpiumNetStat.ViewModels
{
    public class TrafficViewModel: BindableBase
    {
        IEventAggregator _ea;
        IConnectionsService _cs;
    }
}
