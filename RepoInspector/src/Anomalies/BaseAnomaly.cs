using Smee.IO.Client.Dto;

namespace RepoInspector.src.Anomalies
{
    /// <summary>
    /// Base class for implementing anomaly detection strategies.
    /// </summary>
    abstract class BaseAnomaly : IAnomaly
    {
        private const string GithubEventKey = "x-github-event";
        protected AppConfig config;

        /// <summary>
        /// Initializes a new instance of the BaseAnomaly class.
        /// </summary>
        public BaseAnomaly()
        {
            config = new AppConfig();
        }

        /// <summary>
        /// Gets the name of the GitHub event type associated with this anomaly.
        /// </summary>
        public abstract string EventName { get; }

        /// <summary>
        /// Gets the name of the anomaly for identification and configuration access.
        /// </summary>
        public abstract string AnomalyName { get; }

        /// <summary>
        /// Defines the action to take when a suspicious event is detected.
        /// </summary>
        public abstract void Act();

        /// <summary>
        /// Determines whether a given SmeeEvent payload is suspicious and should trigger the action.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        /// <returns>True if the event is suspicious; otherwise, false.</returns>
        public abstract bool IsSuspicious(SmeeEvent payload);

        /// <summary>
        /// Executes the anomaly detection logic on a SmeeEvent payload and triggers the action if needed.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
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
