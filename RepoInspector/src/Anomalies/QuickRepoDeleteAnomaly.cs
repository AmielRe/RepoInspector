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
        public override string EventName => "repository";

        public override void Act()
        {
            Console.WriteLine("Suspicious repository event detected!");
        }

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
                return false; // Or take appropriate action.
            }
        }

        private bool IsDeleteTimeValid(long deleteTimestamp, string createdAt)
        {
            try
            {
                DateTime eventTime = DateTimeUtils.TimestampToDateTime(deleteTimestamp);
                DateTime createdAtDateTime = DateTime.Parse(createdAt, null, DateTimeStyles.RoundtripKind);

                // Calculate the time difference between the two DateTime objects.
                TimeSpan timeDifference = eventTime - createdAtDateTime;

                TimeSpan maxTimeDifference = TimeSpan.FromMinutes(10);

                return timeDifference <= maxTimeDifference;
            }
            catch(Exception ex)
            {
                // Handle the date parsing error.
                Console.WriteLine($"Date parsing error: {ex.Message}");
                return true; // Or take appropriate action.

            }
        }
    }
}
