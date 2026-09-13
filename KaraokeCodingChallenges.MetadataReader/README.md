# Challenge 016 - Metadata Reader Interface

Introduce an abstraction for reading embedded media metadata and remove the direct TagLibSharp dependency from the media metadata scanner.

## Concepts Practised

- Interfaces
- Abstraction
- Dependency inversion
- Dependency injection through method parameters
- Separation of responsibilities
- Records
- Third-party library isolation
- NuGet packages

## Requirements

- Create an `IMetadataReader` interface for reading embedded metadata
- Create an `EmbeddedMetadata` record to represent metadata returned by a reader
- Implement the interface using TagLibSharp
- Keep TagLibSharp-specific code inside the concrete reader implementation
- Update the existing media metadata scanner to depend on `IMetadataReader`
- Preserve the existing metadata output and filename fallback behaviour

## Implementation

The `IMetadataReader` interface defines the contract used by the application:

```csharp
public interface IMetadataReader
{
    EmbeddedMetadata Read(string filePath);
}
```

The interface does not contain any TagLibSharp-specific code.

`TagLibMetadataReader` implements `IMetadataReader` and uses TagLibSharp to read embedded metadata and audio properties.

The reader returns an `EmbeddedMetadata` record containing:

- Title
- Artist
- Album
- Genre
- Year
- Track number
- Duration
- Bitrate
- Sample rate
- Channel count

File-system information such as the file name, extension, and file size is not included in `EmbeddedMetadata`. These values remain the responsibility of the media metadata scanner.

## Embedded Metadata

`EmbeddedMetadata` is used as an intermediate model between the metadata reader and the scanner.

This allows the metadata reader to return only the information obtained from the embedded metadata source without constructing the final application-level `MediaMetadata` record.

## TagLibSharp Implementation

`TagLibMetadataReader` is the concrete implementation of `IMetadataReader`.

It contains all TagLibSharp-specific code, including:

- Opening supported media files
- Reading embedded tags
- Reading duration
- Reading bitrate
- Reading sample rate
- Reading channel count
- Converting missing numeric tag values to nullable values

The rest of the application does not need to know that TagLibSharp is being used.

## Scanner Integration

The existing `MediaMetadataScannerService` was refactored to accept an `IMetadataReader`:

```csharp
MediaMetadataRecord ReadMetadata(string filePath, IMetadataReader metadataReader)
```

The scanner remains responsible for:

- Validating that the file exists
- Reading file name, extension, and file size
- Parsing metadata from filenames
- Applying filename fallback values
- Combining file information and embedded metadata
- Creating the final `MediaMetadata` record

The dependency flow is now:

```text
MediaMetadataScannerService
    -> IMetadataReader
        -> TagLibMetadataReader
            -> TagLibSharp
```

This keeps the scanner independent of the specific third-party metadata library.

## Result

The media metadata scanner produces the same output as before, but the metadata-reading implementation is now isolated behind an application-owned interface.

This makes it easier to replace or extend the metadata-reading implementation in the future without changing the scanner itself.
