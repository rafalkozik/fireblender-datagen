namespace Fireblender.DataGen.ListeningHistory
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using Newtonsoft.Json;
    using Fireblender.DataGen.ListeningHistory.Services;
    using Fireblender.DataGen.Common.Services;
    using Fireblender.DataGen.Common.Enums;
    using Fireblender.DataGen.Common.Interfaces;
    using Fireblender.DataGen.Common.Models;
    using System.Collections.Generic;

    class Program
    {
        static void Main(string[] args)
        {
            // TODO: better args handling, integrate System.CommandLine
            Generate(
                seed: 1337,
                minDate: DateTime.Parse(args[0]),
                maxDate: DateTime.Parse(args[1]),
                usersCount: int.Parse(args[2]),
                artistsCount: int.Parse(args[3]),
                songsCount: int.Parse(args[4]),
                size: int.Parse(args[5]),
                outputDirectory: args[6]
            );
        }

        static void Generate(
            int seed,
            DateTime minDate,
            DateTime maxDate,
            int usersCount,
            int artistsCount,
            int songsCount,
            int size,
            string outputDirectory)
        {
            var random = new Random(seed);
            var dataGenerator = new ListeningHistoryGenerator(random, usersCount, artistsCount, songsCount);

            var config = new DataSetConfiguration
            {
                MinDate = minDate,
                MaxDate = maxDate,
                Size = size,
                SizeOverTime = SizeOverTime.Uniform,
                BufferingIntervalInSeconds = 300,
                BufferingSizeInMBs = 1,
            };

            Generate(random, config, dataGenerator, outputDirectory);
        }

        // TODO: move implementation to dedicated service class
        static void Generate<TDataPoint>(
            Random random,
            DataSetConfiguration config,
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
