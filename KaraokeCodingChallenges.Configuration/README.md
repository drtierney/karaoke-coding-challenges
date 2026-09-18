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
