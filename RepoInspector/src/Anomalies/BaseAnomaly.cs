using Smee.IO.Client.Dto;

namespace RepoInspector.src.Anomalies
{
    abstract class BaseAnomaly : IAnomaly
    {
        public abstract string EventName { get; }

        public abstract void Act();

        public abstract bool IsSuspicious(SmeeEvent payload);

        public void Run(SmeeEvent payload)
        {
            if (!payload.Data.Headers.TryGetValue("x-github-event", out string payloadEvent)) return;

            if (!string.Equals(payloadEvent, EventName)) return;

            if(IsSuspicious(payload))
            {
                Act();
            }
        }
    }
}
