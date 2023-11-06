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
        public override void Act()
        {
            Console.WriteLine("Suspicious repository event detected!");
        }

        public override bool IsSuspicious(SmeeEvent payload)
        {
            string githubEvent;

            if (!payload.Data.Headers.TryGetValue("x-github-event", out githubEvent))
            {
                return false;
            }

            // Deserialize the JSON into a dynamic object or JObject.
            JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

            if (!string.Equals(githubEvent, "repository") || !string.Equals(jsonPayload["action"].ToString(), "deleted") || !IsDeleteTimeValid(payload.Data.Timestamp, jsonPayload["repository"]["created_at"].ToString()))
            {
                return false;
            }

            return true;
        }

        private bool IsDeleteTimeValid(long deleteTimestamp, string createdAt)
        {
            DateTime eventTime = DateTimeUtils.TimestampToDateTime(deleteTimestamp);
            DateTime createdAtDateTime = DateTime.Parse(createdAt, null, DateTimeStyles.RoundtripKind);

            // Calculate the time difference between the two DateTime objects.
            TimeSpan timeDifference = eventTime - createdAtDateTime;

            TimeSpan maxTimeDifference = TimeSpan.FromMinutes(10);

            return timeDifference <= maxTimeDifference;
        }
    }
}
