using System;

namespace Components.Helpers
{
    public class UnixTime
    {
        public static int Now()
        {
            var unixTimeNow = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            return unixTimeNow;
        }
    }
}