using AngleSharp.Dom;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using SpotifyExplode;
using SpotifyExplode.Tracks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MessagePack;

namespace Music
{
    public struct Playlist : IEnumerable<MusicTrack>
    {
        public Playlist()
        {
            Tracks = new List<MusicTrack>();
        }
        public Playlist(List<MusicTrack> tracks)
        {
            Tracks = tracks;
        }
        private List<MusicTrack> Tracks;

        public MusicTrack this[int index]
        {
            get => Tracks[index];
            set => Tracks[index] = value;
        }

        public IEnumerator<MusicTrack> GetEnumerator()
        {
            return Tracks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static async Task<Playlist> FromUrlAsync(string URL)
        {
            Url url = new Url(URL);

            List<MusicTrack> tracks = new List<MusicTrack>();

            if (url.Host.Contains("spotify"))
            {
                SpotifyClient spotify = new SpotifyClient();

                var SpotifyTracks = await spotify.Playlists.GetAllTracksAsync(url.ToString());


                foreach (var i in SpotifyTracks)
                {
                    tracks.Add(new MusicTrack(i.Title, i.Artists.Select(x => x.Name).ToList(), i.Url));
                }
            }
            else
            {
                YoutubeClient client = new YoutubeClient();

                var YoutubeTracks = await client.Playlists.GetVideosAsync(url.ToString());

                foreach (var i in YoutubeTracks)
                {
                    tracks.Add(new MusicTrack(i.Title, new List<string>() { i.Author.ChannelTitle }, i.Url));
                }
            }

            return new Playlist(tracks);
        }
        public void Import(string filepath)
        {
            Tracks = MessagePackSerializer.Deserialize<List<MusicTrack>>(File.ReadAllBytes(filepath));
        }
        public void Export(string filepath)
        {
            File.WriteAllBytes(filepath, MessagePackSerializer.Serialize(Tracks));
        }
    }
}