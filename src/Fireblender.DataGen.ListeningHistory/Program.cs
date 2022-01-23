namespace Fireblender.DataGen.ListeningHistory
{
    using System;
    using Fireblender.DataGen.ListeningHistory.Services;
    using Fireblender.DataGen.Common.Services;
    using Fireblender.DataGen.Common.Enums;
    using Fireblender.DataGen.ListeningHistory.Models;
    using System.Diagnostics;

    class Program
    {
        /// <summary>Listening history dataset generator.</summary>
        /// <param name="outputDirectory">Location of generated dataset</param>
        /// <param name="minDate">Minimum date in generated dataset</param>
        /// <param name="maxDate">Maximum date in generated dataset</param>
        /// <param name="usersCount">Numbers of users in generated dataset</param>
        /// <param name="artistsCount">Numbers of artists in generated dataset</param>
        /// <param name="songsCount">Numbers of songs in generated dataset (each song is mapped to single artist)</param>
        /// <param name="size">Size of generarted data set (total number of song plays)</param>
        /// <param name="sizeOverTime">Data distribution in generated dataset</param>
        /// <param name="bufferingIntervalInSeconds">Buffering interval in seconds for generated data (mimics Kinesis Data Firehose behavior)</param>
        /// <param name="bufferingSizeInMBs">Buffering size in MB for generated data (mimics Kinesis Data Firehose behavior)</param>
        /// <param name="seed">Seed used to initialize random number generator</param>
        /// <param name="verbose">Use verbose logging</param>
        static void Main(
            string outputDirectory,
            DateTime minDate,
            DateTime maxDate,
            int usersCount,
            int artistsCount,
            int songsCount,
            int size,
            SizeOverTime sizeOverTime = SizeOverTime.Uniform,
            int bufferingIntervalInSeconds = 300,
            int bufferingSizeInMBs = 1,
            int seed = 1337,
            bool verbose = false)
        {
            var config = new ListeningHistoryDatasetConfiguration
            {
                MinDate = minDate,
                MaxDate = maxDate,
                UsersCount = usersCount,
                ArtistsCount = artistsCount,
                SongsCount = songsCount,
                Size = size,
                SizeOverTime = sizeOverTime,
                BufferingIntervalInSeconds = bufferingIntervalInSeconds,
                BufferingSizeInMBs = bufferingSizeInMBs,
            };

            var random = new Random(seed);
            var dataGenerator = new ListeningHistoryGenerator(random, config);
            var datasetGenerator = new DatasetGenerator<ListeningHistoryDataPoint>();

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            datasetGenerator.Generate(random, config, dataGenerator, outputDirectory);

            stopwatch.Stop();

            if (verbose)
            {
                Console.WriteLine($"Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
