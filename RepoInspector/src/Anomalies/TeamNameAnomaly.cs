using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Smee.IO.Client.Dto;
using System;

namespace RepoInspector.src.Anomalies
{
    class TeamNameAnomaly : BaseAnomaly
    {
        public override void Act()
        {
            Console.WriteLine("Suspicious team creation event detected!");
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

            if (!string.Equals(githubEvent, "team") || !string.Equals(jsonPayload["action"].ToString(), "created") || !jsonPayload["team"]["name"].ToString().ToLower().StartsWith("hacker"))
            {
                return false;
            }

            return true;
        }
    }
}
