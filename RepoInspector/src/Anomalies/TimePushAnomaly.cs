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

        public override void Act()
        {
            Console.WriteLine("Suspicious push event detected!");
        }

        public override bool IsSuspicious(SmeeEvent payload)
        {
            if(IsPushTimeValid(payload.Data.Timestamp))
            {
                return false;
            }

            return true;
        }

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
                // Handle the date parsing error.
                Console.WriteLine($"Date parsing error: {ex.Message}");
                return true; // Or take appropriate action.
            }
        }
    }
}
