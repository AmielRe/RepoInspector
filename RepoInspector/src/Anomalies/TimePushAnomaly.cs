using RepoInspector.src.Utils;
using Smee.IO.Client.Dto;
using System;

namespace RepoInspector.src.Anomalies
{
    class TimePushAnomaly : BaseAnomaly
    {
        private const string StartTimeConfigKey = "StartTime";
        private const string EndTimeConfigKey = "EndTime";

        public override string EventName => "push";

        public override string AnomalyName => "TimePushAnomaly";

        /// <summary>
        /// This method defines the action to take when a suspicious time push anomaly is detected.
        /// </summary>
        public override void Act()
        {
            Console.WriteLine("Suspicious push event detected!");
        }

        /// <summary>
        /// Determines whether a given SmeeEvent payload is suspicious.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        /// <returns>True if the event is suspicious; otherwise, false.</returns>
        public override bool IsSuspicious(SmeeEvent payload)
        {
            if(IsPushTimeValid(payload.Data.Timestamp))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the timestamp of the push event falls within a specified time range.
        /// </summary>
        /// <param name="timestamp">The timestamp of the push event.</param>
        /// <returns>True if the event timestamp is not within the specified range; otherwise, false.</returns>
        private bool IsPushTimeValid(long timestamp)
        {
            try
            {
                DateTime eventTime = DateTimeUtils.TimestampToDateTime(timestamp);

                var anomalySection = config.GetSection(AnomalyName);
                TimeSpan start = TimeSpan.Parse(anomalySection[StartTimeConfigKey]);
                TimeSpan end = TimeSpan.Parse(anomalySection[EndTimeConfigKey]);

                if ((eventTime.TimeOfDay > start) && (eventTime.TimeOfDay < end))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                // Log date parsing error and act as valid.
                Log.Error(ex);
                return true;
            }
        }
    }
}
