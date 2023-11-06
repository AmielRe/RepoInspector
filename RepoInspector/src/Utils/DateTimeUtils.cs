using System;

namespace RepoInspector.src.Utils
{
    /// <summary>
    /// Provides utility methods for working with DateTime objects.
    /// </summary>
    public static class DateTimeUtils
    {
        /// <summary>
        /// Converts a timestamp value to a DateTime object.
        /// </summary>
        /// <param name="timestamp">A timestamp value in milliseconds since the Unix epoch (1970-01-01).</param>
        /// <returns>A DateTime object representing the timestamp value in the local time zone.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided timestamp is invalid (negative).</exception>
        public static DateTime TimestampToDateTime(long timestamp)
        {
            if (timestamp < 0)
            {
                throw new ArgumentException("Invalid timestamp value.");
            }

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(timestamp).ToLocalTime();
        }
    }
}
