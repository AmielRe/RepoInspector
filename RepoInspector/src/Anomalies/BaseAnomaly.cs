using Smee.IO.Client.Dto;

namespace RepoInspector.src.Anomalies
{
    abstract class BaseAnomaly : IAnomaly
    {
        private const string GithubEventKey = "x-github-event";
        protected AppConfig config;

        public BaseAnomaly()
        {
            config = new AppConfig();
        }

        public abstract string EventName { get; }

        public abstract string AnomalyName { get; }

        public abstract void Act();

        public abstract bool IsSuspicious(SmeeEvent payload);

        public void Run(SmeeEvent payload)
        {
            if (!payload.Data.Headers.TryGetValue(GithubEventKey, out string payloadEvent)) return;

            if (!string.Equals(payloadEvent, EventName)) return;

            if(IsSuspicious(payload))
            {
                Act();
            }
        }
    }
}
