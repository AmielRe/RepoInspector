using Smee.IO.Client.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoInspector.src.Anomalies
{
    abstract class BaseAnomaly : IAnomaly
    {
        public abstract void Act();

        public abstract bool IsSuspicious(SmeeEvent payload);

        public void Run(SmeeEvent payload)
        {
            if(IsSuspicious(payload))
            {
                Act();
            }
        }
    }
}
