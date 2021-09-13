namespace Fireblender.DataGen.ListeningHistory
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using Newtonsoft.Json;
    using Fireblender.DataGen.ListeningHistory.Services;

    class Program
    {
        static void Main(string[] args)
        {
            // TODO: better args handling, integrate System.CommandLine
            Generate(
                seed: 1337,
                dateStart: DateTime.Parse(args[0]),
                dateEnd: DateTime.Parse(args[1]),
                nUsers: int.Parse(args[2]),
                nArtists: int.Parse(args[3]),
                nSongs: int.Parse(args[4]),
                nItems: int.Parse(args[5]),
                outputDirectory: args[6]
            );
        }

        static void Generate(
            int seed,
            DateTime dateStart,
            DateTime dateEnd,
            int nUsers,
            int nArtists,
            int nSongs,
            long nItems,
            string outputDirectory)
        {
            var random = new Random(seed);
            var generator = new ListeningHistoryGenerator(random, nUsers, nArtists, nSongs);

            // TODO: move code for orchestrating generation into separate service
            var days = (int)(dateEnd - dateStart).TotalDays + 1;
            var dailyConfigs = new (int seed, DateTime date, long nItems)[days];

            for (int d = 0; d < days; d++)
            {
                dailyConfigs[d] = (random.Next(), dateStart.AddDays(d), 0);
            }

            // TODO: different generation patterns (i.e. growing data set, shrinking dataset, use Fireblender.DataGen.Common.Enums.SizeOverTime)
            for (int i = 0; i < nItems; i++)
            {
                var d = random.Next() % days;
                dailyConfigs[d].nItems++;
            }

            for (int d = 0; d < days; d++)
            {
                var dailyConfig = dailyConfigs[d];
                var dailyRandom = new Random(dailyConfig.seed);

                if (dailyConfig.nItems == 0) continue;

                foreach (var item in generator.Generate(dailyRandom, dailyConfig.date, dailyConfig.nItems))
                {
                    // TODO: implement batching (size / time)
                    var timestamp = item.Timestamp;
                    var fileName = $"{timestamp.Year:00}/{timestamp.Month:00}/{timestamp.Day:00}/{timestamp.Hour:00}/{item.Id}.jsonl.gz";
                    var fullFileName = Path.Join(outputDirectory, fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

                    var json = JsonConvert.SerializeObject(item);

                    using var fileStream = File.Create(fullFileName);
                    using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress, leaveOpen: true);
                    using var streamWriter = new StreamWriter(gzipStream);
                    
                    streamWriter.WriteLine(json);
                }
            }
        }
    }
}
