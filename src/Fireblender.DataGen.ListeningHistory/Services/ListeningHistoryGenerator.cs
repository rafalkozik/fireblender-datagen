namespace Fireblender.DataGen.ListeningHistory.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fireblender.DataGen.Common.Helper;
    using Fireblender.DataGen.Common.Interfaces;
    using Fireblender.DataGen.ListeningHistory.Models;

    class ListeningHistoryGenerator : IDataGenerator<ListeningHistoryDataPoint>
    {
        private readonly Guid[] users;
        private readonly (Guid songId, Guid artistId)[] songs;

        public ListeningHistoryGenerator(
            Random random,
            ListeningHistoryDatasetConfiguration config)
        {
            this.users = Enumerable.Range(0, config.UsersCount).Select(u => random.NextGuid()).ToArray();

            var artists = Enumerable.Range(0, config.ArtistsCount).Select(a => random.NextGuid()).ToArray();

            this.songs = Enumerable.Range(0, config.SongsCount)
                .Select(s => (random.NextGuid(), random.Pick(artists)))
                .ToArray();
        }

        public IEnumerable<ListeningHistoryDataPoint> Generate(Random random, IEnumerable<DateTime> orderedTimestamps)
        {
            return orderedTimestamps.Select(timestamp =>
            {
                var (songId, artistId) = random.Pick(this.songs);

                return new ListeningHistoryDataPoint
                {
                    Id = random.NextGuid(),
                    UserId = random.Pick(users),
                    ArtistId = artistId,
                    SongId = songId,
                    Timestamp = timestamp,
                };
            });
        }
    }
}
    
    