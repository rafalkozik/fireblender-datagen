namespace Fireblender.DataGen.ListeningHistory.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fireblender.DataGen.Common.Helper;
    using Fireblender.DataGen.Common.Interfaces;
    using Fireblender.DataGen.ListeningHistory.Models;

    class ListeningHistoryGenerator : IDataSetGenerator<ListeningHistoryItem>
    {
        private readonly Guid[] users;
        private readonly Guid[] artists;
        private readonly Guid[] songs;

        public ListeningHistoryGenerator(
            Random random,
            int nUsers,
            int nArtists,
            int nSongs)
        {
            this.users = Enumerable.Range(0, nUsers).Select(u => random.NextGuid()).ToArray();
            this.artists = Enumerable.Range(0, nArtists).Select(a => random.NextGuid()).ToArray();
            this.songs = Enumerable.Range(0, nSongs).Select(s => random.NextGuid()).ToArray();
        }

        public IEnumerable<ListeningHistoryItem> Generate(Random random, DateTime date, long count)
        {
            for (var i = 0; i < count; i++)
            {
                var item = new ListeningHistoryItem
                {
                    Id = random.NextGuid(),
                    UserId = random.Pick(users),
                    ArtistId = random.Pick(artists),
                    SongId = random.Pick(songs),
                    Timestamp = random.NextDateTimeWithinDay(date),
                };

                yield return item;
            }
        }
    }
}
    
    