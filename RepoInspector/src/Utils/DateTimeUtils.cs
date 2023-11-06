﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RepoInspector.src.Utils
{
    public static class DateTimeUtils
    {
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
