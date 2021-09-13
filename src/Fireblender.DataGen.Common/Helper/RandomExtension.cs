namespace Fireblender.DataGen.Common.Helper
{
    using System;

    public static class RandomExtension
    {
        public static Guid NextGuid(this Random random)
        {
            var bytes = new byte[16];
            random.NextBytes(bytes);
            return new Guid(bytes);
        }

        public static T Pick<T>(this Random random, T[] items)
        {
            return items[random.Next(items.Length)];
        }

        public static DateTime NextDateTimeWithinDay(this Random random, DateTime baseDate)
        {
            const int secondsInDay = 60 * 60 * 24;

            return baseDate.Date.AddSeconds(random.Next(secondsInDay));
        }
    }
}
