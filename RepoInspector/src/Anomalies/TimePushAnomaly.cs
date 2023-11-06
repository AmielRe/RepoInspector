using RepoInspector.src.Utils;
using Smee.IO.Client.Dto;
using System;

namespace RepoInspector.src.Anomalies
{
    class TimePushAnomaly : BaseAnomaly
    {
        public override string EventName => "push";

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
                TimeSpan start = new TimeSpan(14, 0, 0); // 14 o'clock
                TimeSpan end = new TimeSpan(16, 0, 0); // 16 o'clock

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
