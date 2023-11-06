using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smee.IO.Client.Dto;
using System;

namespace RepoInspector.src.Anomalies
{
    class TeamNameAnomaly : BaseAnomaly
    {
        private const string PrefixConfigKey = "Prefix";

        public override string EventName => "team";

        public override string AnomalyName => "TeamNameAnomaly";

        public override void Act()
        {
            Console.WriteLine("Suspicious team creation event detected!");
        }

        public override bool IsSuspicious(SmeeEvent payload)
        {
            try
            {
                var anomalySection = config.GetSection(AnomalyName);
                string forbiddenPrefix = anomalySection[PrefixConfigKey];

                // Deserialize the JSON into a dynamic object or JObject.
                JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

                if ((jsonPayload["action"] is null) ||
                    !string.Equals(jsonPayload["action"].ToString(), "created") ||
                    (jsonPayload["team"] is null) ||
                    (jsonPayload["team"]["name"] is null) ||
                    !jsonPayload["team"]["name"].ToString().ToLower().StartsWith("hacker"))
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
    }
}
