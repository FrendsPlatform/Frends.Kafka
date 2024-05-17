# Changelog
## [2.0.0] - 2024-05-15
### Added
- Support for Confluent Schema Registry based Avro.
- New parameter Options.Debug.
### Changed
- Confluent.Kafka updated from version 1.9.3 to 2.4.
- Input.Partition change: Consume from all topic's partitions if set to -1.
- New parameter: Options.EncodeMessageKey to choose whether this Task will try to encode consumed key to string from byte[]
- Message class change: string Key, string Value replaced by dynamic Key, dynamic Value.
- Result.Messages renamed to Result.Data.
- Removed optional parameters: ApiVersionRequest, ApiVersionFallbackMs, ApiVersionRequestTimeoutMs, AllowAutoCreateTopics, 

## [1.1.0] - 2023-11-27
### Added
- Added a partition as input parameter to the task.

## [1.0.1] - 2023-04-17
### Fixed
- Changed Task to set ssl.SslCaCertificateStores only if it's set as parameter.

## [1.0.0] - 2022-10-19
### Added
- Initial implementation