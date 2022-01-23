namespace Fireblender.DataGen.ListeningHistory.Models
{
    using System;
    using Fireblender.DataGen.Common.Interfaces;

    public class ListeningHistoryDataPoint : IDataPoint
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        
        public Guid ArtistId { get; set; }
        
        public Guid SongId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
