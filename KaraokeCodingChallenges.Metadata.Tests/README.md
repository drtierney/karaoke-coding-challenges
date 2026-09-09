# Challenge 017 - Metadata Resolution Tests

Add automated unit tests for metadata resolution and filename fallback behaviour.

## Concepts Practised

- xUnit
- `[Fact]`
- `[Theory]` and `[InlineData]`
- Arrange, Act, Assert
- Testing exceptions
- Fake implementations
- Temporary test files
- Edge-case testing
- `dotnet test`

## Tests Performed

- Embedded metadata takes precedence when available.
- Filename metadata is used when embedded metadata is missing.
- Whitespace embedded values fall back to filename metadata.
- Embedded track numbers take precedence over filename track numbers.
- Filename track numbers are used when embedded values are missing.
- Artist and title are parsed from supported filename formats.
- Track numbers are parsed from three-part filenames.
- Multiple filename formats are tested with parameterized tests.
- Filenames with no separator use the filename as the title.
- Invalid three-part filenames fall back safely.
- Missing embedded fields can be filled individually from filename metadata.
- Missing files throw `FileNotFoundException`.

## Test Results

```text
Test summary: total: 15, failed: 0, succeeded: 15, skipped: 0
```

## Key Takeaways
- xUnit can be used to verify behaviour automatically.
- Arrange, Act, Assert keeps tests structured and readable.
- `[Theory]` and `[InlineData]` reduce duplication across similar test cases.
- Fake implementations allow dependencies such as `IMetadataReader` to be controlled during testing.
- Temporary files can be used safely when testing file-system-dependent code.
- Tests should verify observable behaviour rather than private implementation details.
