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

Generator parameters:
- min-date
- max-date
- users
- artists
- songs
- count
- output-directory

Run with:

```
 dotnet run --project .\src\Fireblender.DataGen.ListeningHistory\Fireblender.DataGen.ListeningHistory.csproj 2021-01-01 2021-05-13 100 10 50 10000 <output-directory>
```