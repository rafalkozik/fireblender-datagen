namespace Fireblender.DataGen.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text.Json;
    using Fireblender.DataGen.Common.Interfaces;
    using Fireblender.DataGen.Common.Models;

    public class DatasetGenerator<TDataPoint> where TDataPoint : IDataPoint
    {
        public void Generate(
            Random random,
            DatasetConfiguration config,
            IDataGenerator<TDataPoint> dataGenerator,
            string outputDirectory)
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

                    var json = JsonSerializer.Serialize(dataPoint);

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
