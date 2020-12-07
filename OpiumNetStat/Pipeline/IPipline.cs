using System;

namespace OpiumNetStat.Pipeline
{
    public interface IPipline
    {

        void Invoke();

        Action Invoked { get; set; }
    }

}
