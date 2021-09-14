namespace Fireblender.DataGen.Common.Interfaces
{
    using System;
    using System.Collections.Generic;
    using Fireblender.DataGen.Common.Enums;

    interface ITimeSlicer
    {
        IEnumerable<DateTime> OrderedTimestampsWithinDay(Random random, DateTime baseDate, int count);

        IEnumerable<(DateTime date, int count)> SplitOverTime(Random random, DateTime minDate, DateTime maxDate, int count, SizeOverTime sizeOverTime);
    }
}
