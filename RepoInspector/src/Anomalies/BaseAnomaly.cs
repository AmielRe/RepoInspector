using NLog;
using Smee.IO.Client.Dto;

namespace RepoInspector.src.Anomalies
{
    /// <summary>
    /// Base class for implementing anomaly detection strategies.
    /// </summary>
    abstract class BaseAnomaly : IAnomaly
    {
        protected Logger Log { get; private set; }
        private const string GithubEventKey = "x-github-event";
        protected AppConfig config;

        /// <summary>
        /// Initializes a new instance of the BaseAnomaly class.
        /// </summary>
        public BaseAnomaly()
        {
            config = new AppConfig();
            Log = LogManager.GetLogger(GetType().ToString());
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
            Log.Debug($"Running {AnomalyName} anomaly...");
            if (!payload.Data.Headers.TryGetValue(GithubEventKey, out string payloadEvent)) return;

            if (!string.Equals(payloadEvent, EventName)) return;

            if (!IsJsonPayloadValid(payload)) return;

            if(IsSuspicious(payload))
            {
                Log.Debug($"{AnomalyName} found suspicious event, handling it now");
                Act();
                Log.Debug($"Finish handling {AnomalyName} suspicious event");
            }
        }

        public abstract bool IsJsonPayloadValid(SmeeEvent payload);
    }
}
