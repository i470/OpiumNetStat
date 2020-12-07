using System;

namespace OpiumNetStat.Pipeline
{
    public interface IPipeLine
    {

        void Invoke();

        Action Invoked { get; set; }
    }

}
