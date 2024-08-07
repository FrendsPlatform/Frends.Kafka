# Changelog
## [2.0.0] - 2024-05-14
### Added
- Support for Confluent Schema Registry based Avro.
- New parameter Options.Debug.
### Changed 
- Result.Status and Result.Timestamp have been replaced by Result.Data.
- Changed how partitions are handled when producing message. See Input.Partition description.
- All but Kerberos SASL settings can be used on Windows platform.
- Confluent.Kafka updated from version 1.9.3 to 2.4.
- Parameter removed: Options.ApiVersionRequest.

## [1.2.0] - 2023-10-11
### Changed 
- Input.Message will no longer be serialized into JSON text before sending.

## [1.1.0] - 2023-09-01
### Added 
- Input.Partition, set the partition.
- Input.Key, set message key.

## [1.0.1] - 2023-04-04
### Fixed 
- Changed Task to set ssl.SslCaCertificateStores only if it's set as parameter.

## [1.0.0] - 2022-10-19
### Added
- Initial implementation