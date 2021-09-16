# fireblender-datagen

Tools to generate datasets for fireblender project. Datasets are stored in folder structure that mimics output of [Amazon Kinesis Data Firehose](https://docs.aws.amazon.com/firehose/latest/dev/what-is-this-service.html).

Datasets are saved as gzipped jsonl files. Generated datasets will be used as a benchmark data used in [fireblender](https://github.com/rafalkozik/fireblender) project.

# Generators

## Fireblender.DataGen.ListeningHistory

Random dataset of song listening history, example:

|Id|UserId|ArtistId|SongId|Timestamp|
|--|------|--------|------|---------|
|3fb54320-b186-4314-b999-fa9ad6ce8c6b|1be42aa3-d913-4481-886b-f3d9e21dbd63|a2b6135c-c1ad-44c4-adbc-b2085e12dd6f|b462f548-cdb0-45e4-895b-7d9ee2ddfc6b|2021-08-30T12:36:47Z|
|ed062262-ca44-4ab7-a2e0-9b6da355e9b2|fd31533a-a453-4e7a-9e19-4069dffdcc87|8c799d7b-7d61-42be-9bc2-7c8dbb83bfbc|d9dd2051-8f56-4ee1-98a5-5d0edf794688|2021-08-30T12:38:47Z|

### Parameters
```
$ dotnet run --project .\src\Fireblender.DataGen.ListeningHistory\Fireblender.DataGen.ListeningHistory.csproj -- --help

Fireblender.DataGen.ListeningHistory

Usage:
  Fireblender.DataGen.ListeningHistory [options]

Options:
  --output-directory <output-directory>                            outputDirectory
  --min-date <min-date>                                            minDate
  --max-date <max-date>                                            maxDate
  --users-count <users-count>                                      usersCount
  --artists-count <artists-count>                                  artistsCount
  --songs-count <songs-count>                                      songsCount
  --size <size>                                                    size
  --size-over-time <Uniform|Unknown>                               sizeOverTime [default: Uniform]
  --buffering-interval-in-seconds <buffering-interval-in-seconds>  bufferingIntervalInSeconds [default: 300]
  --buffering-size-in-mbs <buffering-size-in-mbs>                  bufferingSizeInMBs [default: 1]
  --seed <seed>                                                    seed [default: 1337]
```

### Sample usage

```
$ dotnet run --project .\src\Fireblender.DataGen.ListeningHistory\Fireblender.DataGen.ListeningHistory.csproj -- --min-date 2021-05-13 --max-date 2021-08-22 --users-count 100 --artists-count 10 --songs-count 50 --size 10000 --output-directory <output_directory>
```