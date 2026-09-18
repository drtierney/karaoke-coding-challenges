# Configuration

Load, validate, and use application settings from a JSON configuration file to control the metadata library scan.

## Challenge 022 - Configuration File

Challenge 022 introduced JSON-based application configuration.

### Concepts Practised

- JSON configuration files
- `System.Text.Json`
- JSON deserialization
- Configuration models
- Static helper classes
- File handling
- Exception handling
- Separation of responsibilities
- Project references
- Integrating configuration into an existing application

### Implementation

Created an `AppConfiguration` model containing the configured library path.

Created a `ConfigurationLoader` class that:

- Reads configuration from `appsettings.json`.
- Deserializes JSON into an `AppConfiguration` object.
- Throws appropriate exceptions when configuration cannot be loaded.

The configuration file is copied to the output directory when the project is built.

The Metadata Library Milestone application was updated to:

- Reference the Configuration project.
- Load its library path from `appsettings.json`.
- Handle missing configuration files.
- Handle invalid JSON.
- Exit cleanly if configuration loading fails.
- Use the configured library path when scanning files.

## Challenge 023 - Configuration Validation

Challenge 023 separated configuration loading from configuration validation.

### Concepts Practised

- Configuration validation
- Separation of responsibilities
- Validation error collections
- `IReadOnlyCollection<T>`
- Distinguishing loading failures from validation failures
- Integrating validation into an existing application

### Implementation

Created a `ConfigurationValidator` class that:

- Accepts a loaded `AppConfiguration` object.
- Checks whether `LibraryPath` is missing or contains only whitespace.
- Returns validation errors as an `IReadOnlyCollection<string>`.

`ConfigurationLoader` is responsible only for reading and deserializing the configuration file.

Configuration failures are handled according to their responsibility:

- Missing files are handled as file-loading failures.
- Invalid JSON is handled as a deserialization failure.
- Invalid configuration values are reported by `ConfigurationValidator`.

The Metadata Library Milestone application was updated to validate configuration after it has been loaded and exit cleanly when validation errors are found.

## Example Configuration

```json
{
  "LibraryPath": "D:\\Music"
}
```

## Example Validation Error

```text
Configuration errors:
- LibraryPath is missing from configuration.
```

## Example Successful Output

```text
Metadata Library Milestone
Configuration loaded.
Files found: 12
Metadata files: 8
```

The exact scan results depend on the contents of the configured library folder.

## Challenge 024 - Multiple Library Sources

Challenge 024 expanded application configuration from a single library path to multiple configurable library sources.

### Concepts Practised

- Collections in configuration models
- Enums
- JSON enum serialization
- `JsonStringEnumConverter`
- `HashSet<T>`
- Case-insensitive duplicate detection
- Enabled and disabled configuration entries
- Handling unavailable or disconnected drives
- Combining results from multiple sources
- Separation of configuration validation from runtime availability

### Implementation

Created a `LibrarySource` model containing:

- `Path` for the configured library folder.
- `Type` to distinguish music and karaoke sources.
- `Enabled` to control whether the source should be scanned.

Created a `LibrarySourceType` enum containing:

- `Music`
- `Karaoke`
- `Mixed`

`AppConfiguration` was updated to store an `IReadOnlyCollection<LibrarySource>` instead of a single library path.

`ConfigurationLoader` was updated to use `JsonStringEnumConverter`, allowing source types to be represented using readable enum names in JSON.

`ConfigurationValidator` was updated to:

- Require at least one configured library source.
- Reject missing or whitespace-only source paths.
- Detect duplicate source paths.
- Compare duplicate paths case-insensitively.

Directory availability is intentionally not treated as a configuration validation error because removable or network drives may be temporarily unavailable.

The Metadata Library Milestone application was updated to:

- Iterate through configured library sources.
- Skip disabled sources.
- Report unavailable sources without stopping the scan.
- Scan all enabled and available sources.
- Combine discovered files into a single collection for metadata scanning, statistics, and karaoke pairing.
- Display which sources are scanned, skipped, or unavailable.

## Example Configuration

```json
{
  "LibrarySources": [
    {
      "Path": "D:\\Music",
      "Type": "Music",
      "Enabled": true
    },
    {
      "Path": "D:\\Karaoke",
      "Type": "Karaoke",
      "Enabled": false
    },
    {
      "Path": "D:\\Backup",
      "Type": "Mixed",
      "Enabled": true
    }
  ]
}
```

## Example Validation Error

```text
Configuration errors:
- Duplicate library source path found: D:\Music
```

## Example Successful Output

```text
Metadata Library Milestone
Configuration loaded.
Scanning Music source: D:\Music
Skipping disabled Karaoke source: D:\Karaoke
Library source unavailable: Mixed - D:\Backup
Files found: 6
Metadata files: 6
```

The exact scan results depend on the contents and availability of the configured library sources.
