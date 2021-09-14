namespace Fireblender.DataGen.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fireblender.DataGen.Common.Enums;
    using Fireblender.DataGen.Common.Interfaces;

    public class TimeSlicer : ITimeSlicer
    {
        public IEnumerable<DateTime> OrderedTimestampsWithinDay(Random random, DateTime baseDate, int count)
        {
            const int secondsInDay = 60 * 60 * 24;

            var midnight = baseDate.Date;

            return Enumerable.Range(0, count)
                .Select(i => random.Next(0, secondsInDay))
                .OrderBy(s => s)
                .Select(s => midnight.AddSeconds(s));
        }

        public IEnumerable<(DateTime date, int count)> SplitOverTime(Random random, DateTime minDate, DateTime maxDate, int count, SizeOverTime sizeOverTime)
        {
            switch (sizeOverTime)
            {
                case SizeOverTime.Uniform:
                    return SplitOverTimeUniform(random, minDate, maxDate, count);
                case SizeOverTime.Unknown:
                default:
                    throw new ArgumentException($"{sizeOverTime} is not supported", nameof(sizeOverTime));
            }
        }

        private IEnumerable<(DateTime date, int count)> SplitOverTimeUniform(Random random, DateTime minDate, DateTime maxDate, int count)
        {
            var days = (int)(maxDate - minDate).TotalDays + 1;

            var counts = new int[days];

            for (int i = 0; i < count; i++)
            {
                var d = random.Next() % days;
                counts[d]++;
            }

            return counts.Select((c, i) => (minDate.AddDays(i), c));
        }
    }
}
