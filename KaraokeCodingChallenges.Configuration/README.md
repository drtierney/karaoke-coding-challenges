# Challenge 022 - Configuration File

Load application settings from a JSON configuration file and use them to control the metadata library scan.

## Concepts Practised

- JSON configuration files
- `System.Text.Json`
- JSON deserialization
- Configuration models
- Static helper classes
- File handling
- Validation
- Exception handling
- Separation of responsibilities
- Project references
- Integrating configuration into an existing application

## Implementation

Created an `AppConfiguration` model containing the configured library path.

Created a `ConfigurationLoader` class that:

- Reads configuration from `appsettings.json`.
- Deserializes JSON into an `AppConfiguration` object.
- Validates that `LibraryPath` has been provided.
- Throws appropriate exceptions when configuration cannot be loaded.

The configuration file is copied to the output directory when the project is built.

The Metadata Library Milestone application was updated to:

- Reference the Configuration project.
- Load its library path from `appsettings.json`.
- Handle missing configuration files.
- Handle invalid JSON.
- Handle invalid configuration values.
- Exit cleanly if configuration loading fails.
- Use the configured library path when scanning files.

## Example Configuration

```json
{
  "LibraryPath": "D:\\Music"
}
```

## Example Output

```text
Metadata Library Milestone
Configuration loaded.
Files found: 12
Metadata files: 8
```

The exact scan results depend on the contents of the configured library folder.
