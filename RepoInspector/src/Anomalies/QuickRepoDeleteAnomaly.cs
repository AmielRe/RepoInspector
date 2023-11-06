using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RepoInspector.src.Utils;
using Smee.IO.Client.Dto;
using System;
using System.Globalization;

namespace RepoInspector.src.Anomalies
{
    class QuickRepoDeleteAnomaly : BaseAnomaly
    {
        private const string MaxTimeDifferenceInMinutesKey = "MaxTimeDifferenceInMinutes";

        public override string EventName => "repository";

        public override string AnomalyName => "QuickRepoDeleteAnomaly";

        /// <summary>
        /// This method defines the action to take when a quick repository delete anomaly is detected.
        /// </summary>
        public override void Act()
        {
            Console.WriteLine("Suspicious repository event detected!");
        }

        /// <summary>
        /// Determines whether a given SmeeEvent payload is suspicious.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to analyze.</param>
        /// <returns>True if the event is suspicious; otherwise, false.</returns>
        public override bool IsSuspicious(SmeeEvent payload)
        {
            try
            {
                // Deserialize the JSON into a dynamic object or JObject.
                JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

                if ((jsonPayload["action"] is null) || 
                    !string.Equals(jsonPayload["action"].ToString(), "deleted") ||
                    (jsonPayload["repository"] is null) ||
                    (jsonPayload["repository"]["created_at"] is null) ||
                    !IsDeleteTimeValid(payload.Data.Timestamp, jsonPayload["repository"]["created_at"].ToString()))
                {
                    return false;
                }

                return true;
            }
            catch (JsonReaderException ex)
            {
                // Handle the JSON parsing error.
                Console.WriteLine($"JSON parsing error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the time difference between the event and creation of a repository is within the specified limit.
        /// </summary>
        /// <param name="deleteTimestamp">The timestamp of the delete event.</param>
        /// <param name="createdAt">The creation timestamp of the repository.</param>
        /// <returns>True if the time difference is within the limit; otherwise, false.</returns>
        private bool IsDeleteTimeValid(long deleteTimestamp, string createdAt)
        {
            try
            {
                var anomalySection = config.GetSection(AnomalyName);
                int maxTimeDiff = int.Parse(anomalySection[MaxTimeDifferenceInMinutesKey]);

                DateTime eventTime = DateTimeUtils.TimestampToDateTime(deleteTimestamp);
                DateTime createdAtDateTime = DateTime.Parse(createdAt, null, DateTimeStyles.RoundtripKind);

                // Calculate the time difference between the two DateTime objects.
                TimeSpan timeDifference = eventTime - createdAtDateTime;

                return timeDifference <= TimeSpan.FromMinutes(maxTimeDiff);
            }
            catch(Exception ex)
            {
                // Handle the date parsing error.
                Console.WriteLine($"Date parsing error: {ex.Message}");
                return true;

            }
        }
    }
}
