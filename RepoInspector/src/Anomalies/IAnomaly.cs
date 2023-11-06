using Smee.IO.Client.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoInspector.src.Anomalies
{
    interface IAnomaly
    {
        public bool IsSuspicious(SmeeEvent payload);

        public void Act();

        public void Run(SmeeEvent payload);
    }
}
