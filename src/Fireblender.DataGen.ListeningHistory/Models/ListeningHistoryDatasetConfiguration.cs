namespace Fireblender.DataGen.ListeningHistory.Models
{
    using Fireblender.DataGen.Common.Models;

    public class ListeningHistoryDatasetConfiguration : DatasetConfiguration
    {
        public int UsersCount { get; set; }

        public int ArtistsCount { get; set; }

        public int SongsCount { get; set; }
    }
}
