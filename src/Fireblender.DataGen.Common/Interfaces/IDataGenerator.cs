namespace Fireblender.DataGen.Common.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IDataGenerator<TDataPoint> where TDataPoint : IDataPoint
    {
        IEnumerable<TDataPoint> Generate(Random random, IEnumerable<DateTime> orderedTimestamps);
    }
}
