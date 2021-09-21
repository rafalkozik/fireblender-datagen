namespace Fireblender.DataGen.ListeningHistory
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using Newtonsoft.Json;
    using Fireblender.DataGen.ListeningHistory.Services;
    using Fireblender.DataGen.Common.Services;
    using Fireblender.DataGen.Common.Enums;
    using Fireblender.DataGen.Common.Interfaces;
    using Fireblender.DataGen.Common.Models;
    using Fireblender.DataGen.ListeningHistory.Models;

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
            int seed = 1337)
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

            Generate(random, config, dataGenerator, outputDirectory);
        }

        // TODO: move implementation to dedicated service class
        static void Generate<TDataPoint>(
            Random random,
            DatasetConfiguration config,
            IDataGenerator<TDataPoint> dataGenerator,
            string outputDirectory) where TDataPoint : IDataPoint
        {
            var timeSlicer = new TimeSlicer();

            var dataDistribution = timeSlicer.SplitOverTime(
                random, 
                config.MinDate,
                config.MaxDate, 
                config.Size,
                config.SizeOverTime);
            
            foreach ((var date, var count) in dataDistribution)
            {
                if (count == 0) continue;

                var timestampsRandom = new Random(random.Next());
                var dataPointsRandom = new Random(random.Next());

                var orderedTimestamps = timeSlicer.OrderedTimestampsWithinDay(timestampsRandom, date, count);

                var buffer = new List<(IDataPoint dataPoint, string serialized)>();
                var bufferSize = 0;

                void FlushBuffer()
                {
                    if (buffer.Count == 0) return;

                    var firstDataPoint = buffer[0].dataPoint;
                    var timestamp = firstDataPoint.Timestamp;
                    var fileName = $"{timestamp.Year:00}/{timestamp.Month:00}/{timestamp.Day:00}/{timestamp.Hour:00}/{firstDataPoint.Id}.jsonl.gz";
                    var fullFileName = Path.Join(outputDirectory, fileName);

                    Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

                    using var fileStream = File.Create(fullFileName);
                    using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress, leaveOpen: true);
                    using var streamWriter = new StreamWriter(gzipStream);

                    foreach ((var _, var json) in buffer)
                    {
                        streamWriter.WriteLine(json);
                    }

                    buffer.Clear();
                    bufferSize = 0;
                }

                foreach (var dataPoint in dataGenerator.Generate(dataPointsRandom, orderedTimestamps))
                {
                    if (buffer.Count > 0 && (dataPoint.Timestamp - buffer[0].dataPoint.Timestamp).TotalSeconds > config.BufferingIntervalInSeconds)
                    {
                        FlushBuffer();
                    }

                    var json = JsonConvert.SerializeObject(dataPoint);

                    buffer.Add((dataPoint, json));
                    bufferSize += json.Length;

                    if (bufferSize > config.BufferingSizeInBytes)
                    {
                        FlushBuffer();
                    }
                }

                FlushBuffer();
            }
        }
    }
}
