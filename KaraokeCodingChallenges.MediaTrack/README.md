# Challenge 001 - MediaTrack Model

## Objective

Create a reusable C# model for representing tracks in a music and karaoke library.

## Requirements

* Create a `MediaTrackModel` class.
* Add properties for:

  * `Title`
  * `Artist`
  * `FilePath`
  * `DurationSeconds`
  * `IsKaraoke`
* Create a constructor for the main track information.

  * Allow duration to default to `0` when it is not yet known.
  * Allow karaoke status to default to `false`.
* Create at least five sample tracks.
* Store the tracks in a `List<MediaTrackModel>`.
* Display each track using a `foreach` loop.
* Add a method that formats the track duration from seconds into minutes and seconds.

  * For example, `189` seconds should display as `3:09`.
* Create a read-only `DisplayName` property that returns the artist and title in this format:

  * `Artist - Title`
* Override the `ToString()` method to provide a readable multi-line representation of a `MediaTrackModel`.
* Use the `ToString()` override to simplify displaying tracks with `Console.WriteLine(track)`.

## Concepts Practised

* Classes and objects
* Properties
* Read-only calculated properties
* Constructors
* Optional constructor parameters
* Positional and named arguments
* Instance methods
* `List<T>`
* `foreach`
* Integer division
* Modulo (`%`)
* String interpolation
* Numeric formatting with `D2`
* Method overriding
* Overriding `ToString()`

## What I Learned

* How constructors initialise an object.
* Why some constructor parameters can have default values.
* The difference between storing a value and calculating a value.
* How integer division and modulo can be used to convert seconds into minutes and seconds.
* How a read-only property can calculate its value from other properties.
* What happens when `ToString()` is overridden.

## Notes

* This is my first project in this coding challenge using Visual Studio, so I had to figure out how to create a new project and add a class to it.
* I tested with a console application, but I could have also used a unit test project to test the `MediaTrackModel` class.
* I found that certain Unicode characters did not display correctly in the console, such as `Axwell Λ Ingrosso`. This appears to be related to the console output or rendering rather than the string itself and could be investigated further in a future exercise.

## Improvements

- Add validation for required properties and invalid durations.
- Consider making some properties less freely mutable.
- Derive karaoke status from matching media files rather than setting it manually.
- Review whether ToString() should remain a detailed multi-line representation.
- Add unit tests for duration formatting and object construction.

## Sample Output

```text
Title: The Only Exception
Artist: Paramore
FilePath: F:\Music\Paramore - The Only Exception.mp3
DurationFormatted: 2:47
IsKaraoke: False

Title: Sign of the Times
Artist: Harry Styles
FilePath: F:\Music\Harry Styles - Sign of the Times.mp3
DurationFormatted: 5:40
IsKaraoke: False

Title: Something New
Artist: Axwell ? Ingrosso
FilePath: F:\Music\Axwell ? Ingrosso - Something New.mp3
DurationFormatted: 4:55
IsKaraoke: False

Title: Young Hearts run free
Artist: Candi Stanton
FilePath: F:\Karaoke\CANDI STANTON - YOUNG HEARTS RUN FREE.mp3
DurationFormatted: 4:01
IsKaraoke: True

Title: Red red wine
Artist: UB40
FilePath: F:\Karaoke\UB40 - Red red wine.mp3
DurationFormatted: 3:09
IsKaraoke: True
```
