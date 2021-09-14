namespace Fireblender.DataGen.Common.Interfaces
{
    using System;

    public interface IDataPoint
    {
        public Guid Id { get; set; }

        public DateTime Timestamp { get; }
    }
}
