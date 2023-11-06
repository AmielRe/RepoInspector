using RepoInspector.src.Utils;
using Smee.IO.Client.Dto;
using System;

namespace RepoInspector.src.Anomalies
{
    class TimePushAnomaly : BaseAnomaly
    {
        public override void Act()
        {
            Console.WriteLine("Suspicious push event detected!");
        }

        public override bool IsSuspicious(SmeeEvent payload)
        {
            string githubEvent;

            if (!payload.Data.Headers.TryGetValue("x-github-event", out githubEvent))
            {
                return false;
            }

            if(!string.Equals(githubEvent, "push") || IsPushTimeValid(payload.Data.Timestamp))
            {
                return false;
            }

            return true;
        }

        private bool IsPushTimeValid(long timestamp)
        {
            DateTime eventTime = DateTimeUtils.TimestampToDateTime(timestamp);
            TimeSpan start = new TimeSpan(14, 0, 0); // 14 o'clock
            TimeSpan end = new TimeSpan(16, 0, 0); // 16 o'clock

            if ((eventTime.TimeOfDay > start) && (eventTime.TimeOfDay < end))
            {
                return false;
            }

            return true;
        }
    }
}
