# MusicLib
Simple C# library with a MusicTrack class and a Playlist class
## Features
Playlist inherits IEnumerable and can be indexed
```c#
public struct Playlist : IEnumerable<MusicTrack> { ... }
```
---
Tracks and playlists can be fetched directly from URLs from either Spotify or Youtube
```c#
MusicTrack track = await MusicTrack.FromUrlAsync("https://www.youtube.com/watch?v=OR6olLu61u8");
```
```c#
Playlist playlist = await Playlist.FromUrlAsync("https://open.spotify.com/playlist/5vTqAr4UQ6i2JSa5P3Mjk0");
```
---
Playlists can be imported and exported as a file
```c#
Playlist playlist = await Playlist.FromUrlAsync("https://www.youtube.com/watch?v=L5q4uYj-gyg&list=PLGPnvYCC8I1Wu3zIL2rsM6PLwLCtU6Gsp");
playlist.Export("./playlist");

Playlist newPlaylist = new Playlist();
newPlaylist.Import("./playlist");
```
---
Download tracks
```c#
MusicTrack track = await MusicTrack.FromUrlAsync("https://open.spotify.com/track/0k6cQBcTYYxRp5gXsjDtAY?si=e07ef9d0c4054cc2");
await track.DownloadAsync("./music/intense/");
```
