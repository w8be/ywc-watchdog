# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

### Added

## 1.3.0 - 2206-11-20

- Added appsettings.json object `"CalibrateTimerSec": 300`
- Added radio temperature status from api response.
- Added radio temperature Calibrate(Tmp) to YWC api response.
- Added radio temperature inhibit on threshold.

### Added

## 1.2.0 - 2206-11-15

- Added /api/cat/radioPower and /api/cat/status/mode objects.
- Added meter Calibrate(Timer/Swr) to YWC api response.
- Added radio power status form api response.
- Added status responses to the log

### Added

- Feature currently in development...

## [1.1.0] - 2026-11-11

### Added

-

### Changed

-

### Fixed

- Resolved issue where watchdog monitoring would hang on timeout.

### Security

-

## [1.0.0] - 2026-10-01

### Added

- Initial public release of `ywc-watchdog`.

[Unreleased]: https://github.com/w8be/ywc-watchdog/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/w8be/ywc-watchdog/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/w8be/ywc-watchdog/releases/tag/v1.0.0
