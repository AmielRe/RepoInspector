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

                if (!string.Equals(jsonPayload["action"].ToString(), "deleted") ||
                    !IsDeleteTimeValid(payload.Data.Timestamp, jsonPayload["repository"]["created_at"].ToString()))
                {
                    return false;
                }

                return true;
            }
            catch (JsonReaderException ex)
            {
                // Log the JSON parsing error
                Log.Error(ex);
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
                int maxTimeDiff = GetMaxTimeDifference();

                DateTime eventTime = DateTimeUtils.TimestampToDateTime(deleteTimestamp);
                DateTime createdAtDateTime = DateTime.Parse(createdAt, null, DateTimeStyles.RoundtripKind);

                // Convert created at time to local time (to match event time)
                DateTime createdAtLocalDateTime = DateTimeUtils.ConvertUtcToLocalTimeZone(createdAtDateTime);

                // Calculate the time difference between the two DateTime objects
                TimeSpan timeDifference = eventTime - createdAtLocalDateTime;

                return timeDifference <= TimeSpan.FromMinutes(maxTimeDiff);
            }
            catch(Exception ex)
            {
                // Log date parsing error and act like valid
                Log.Error(ex);
                return true;

            }
        }

        private int GetMaxTimeDifference()
        {
            var anomalySection = config.GetSection(AnomalyName);
            return int.Parse(anomalySection[MaxTimeDifferenceInMinutesKey]);
        }

        public override bool IsJsonPayloadValid(SmeeEvent payload)
        {
            // Deserialize the JSON into a dynamic object or JObject.
            JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

            return !(jsonPayload["action"] is null) ||
                    !(jsonPayload["repository"] is null) ||
                    !(jsonPayload["repository"]["created_at"] is null);
        }
    }
}
