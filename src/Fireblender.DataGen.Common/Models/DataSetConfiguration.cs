namespace Fireblender.DataGen.Common.Models
{
    using System;
    using Fireblender.DataGen.Common.Enums;

    public class DataSetConfiguration
    {
        public DateTime MinDate { get; set; }

        public DateTime MaxDate { get; set; }

        public int Size { get; set; }

        public SizeOverTime SizeOverTime { get; set; }

        public int BufferingSizeInMBs { get; set; }

        public int BufferingIntervalInSeconds { get; set; }
        
        public int BufferingSizeInBytes => BufferingSizeInMBs * 1024 * 1024;
    }
}
