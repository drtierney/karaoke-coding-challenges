# Favourites Demo

Manual verification application for Challenge 030 - Favourites.

## Demonstration

The demo:

- Loads previously saved favourites from JSON.
- Creates a `FavouriteManager` from the loaded state.
- Adds favourite tracks.
- Demonstrates duplicate prevention.
- Removes a favourite track.
- Saves the updated favourites back to JSON.
- Displays the current favourites.

Running the demo a second time verifies that favourite state is preserved across application runs.

## Example Outputs
```text
Loaded Favourites 

Added Queen: True 
Added Journey: True 

Removed Queen: True 

Current Favourites 

Journey - Don't Stop Believin'.mp3
```

```text
Loaded Favourites

Journey - Don't Stop Believin'.mp3

Added Queen: True
Added Journey: False

Removed Queen: True

Current Favourites

Journey - Don't Stop Believin'.mp3
```