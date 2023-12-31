﻿using Newtonsoft.Json;
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

        /// <summary>
        /// This method defines the action to take when a suspicious team name anomaly is detected.
        /// </summary>
        public override void Act()
        {
            Console.WriteLine("Suspicious team creation event detected!");
        }

        /// <summary>
        /// Checks whether the JSON payload in a SmeeEvent is valid and contains the expected fields.
        /// </summary>
        /// <param name="payload">The SmeeEvent payload to be validated.</param>
        /// <returns>
        ///   <c>true</c> if the JSON payload contains the expected fields; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        ///   This method deserializes the JSON payload from a SmeeEvent into a dynamic object or JObject
        ///   and checks whether it contains the necessary fields: "action," "team," and "team.name."
        ///   If any of these fields are missing or null, the payload is considered invalid.
        /// </remarks>
        public override bool IsJsonPayloadValid(SmeeEvent payload)
        {
            // Deserialize the JSON into a dynamic object or JObject.
            JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

            return !(jsonPayload["action"] is null) ||
                    !(jsonPayload["team"] is null) ||
                    !(jsonPayload["team"]["name"] is null);
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
                var anomalySection = config.GetSection(AnomalyName);
                string forbiddenPrefix = anomalySection[PrefixConfigKey];

                // Deserialize the JSON into a dynamic object or JObject
                JObject jsonPayload = JObject.Parse(JsonConvert.SerializeObject(payload.Data.Body));

                if (!string.Equals(jsonPayload["action"].ToString(), "created") ||
                    !jsonPayload["team"]["name"].ToString().ToLower().StartsWith(forbiddenPrefix))
                {
                    return false;
                }

                return true;
            }
            catch (JsonReaderException ex)
            {
                // Log JSON parsing error
                Log.Error(ex);
                return false;
            }
        }
    }
}
