# Library Persistence Tests

Tests the JSON library persistence functionality introduced in **Challenge 034 - JSON Library Persistence**.

The test suite contains **20 tests** covering persistence mapping, JSON storage, validation, error handling, versioning, and round-trip behaviour.

## Mapper Tests

`MediaLibraryPersistenceMapperTests` verifies conversion between `MediaLibraryCollection` and the persistence DTOs.

- Maps library tracks to persistence DTOs
- Sets the current persistence format version
- Handles an empty library
- Preserves track ordering when mapping to DTOs
- Reconstructs media tracks from persistence DTOs
- Handles an empty persisted track collection
- Preserves track ordering when reconstructing the library
- Rejects unsupported persistence versions

## JSON Store Tests

`JsonMediaLibraryStoreTests` verifies JSON serialization, file persistence, loading, validation, and error handling.

- Creates a JSON persistence file
- Writes library data to JSON
- Rejects an empty save path
- Rejects a null library
- Loads library data from JSON
- Returns an empty library when the persistence file does not exist
- Rejects JSON containing a null library object
- Rejects invalid JSON
- Rejects an empty load path
- Rejects unsupported persistence versions
- Rejects a null persistence mapper
- Preserves library data and track ordering through a complete save/load round trip

## Running the Tests

Run the persistence tests with:

```text
dotnet test KaraokeCodingChallenges.LibraryPersistence.Tests
```

Or run the complete solution test suite with:

```text
dotnet test
```
