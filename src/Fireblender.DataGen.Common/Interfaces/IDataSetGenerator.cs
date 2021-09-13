namespace Fireblender.DataGen.Common.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IDataSetGenerator<TDataPoint> where TDataPoint : IDataPoint
    {
        IEnumerable<TDataPoint> Generate(Random random, DateTime date, long count);
    }
}
