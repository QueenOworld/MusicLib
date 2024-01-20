using AngleSharp.Dom;
using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using SpotifyExplode;
using SpotifyExplode.Tracks;
using MessagePack;

namespace Music
{
    [MessagePackObject]
    public struct MusicTrack
    {
        public MusicTrack(string name, List<string> artists, string url)
        {
            Name = name;
            URL = url;
            Artists = artists;
        }

        [Key(0)]
        public string Name { get; }
        [Key(1)]
        public List<string> Artists { get; }
        [Key(2)]
        public string URL { get; }

        public async Task DownloadAsync(string directory)
        {
            Url url = new Url(URL);

            if (url.Host.Contains("spotify"))
            {
                SpotifyClient spotify = new SpotifyClient();

                string? youtubeId = await spotify.Tracks.GetYoutubeIdAsync(URL.ToString());

                url = new Url("https://youtube.com/watch?v=" + youtubeId);
            }

            YoutubeClient client = new YoutubeClient();

            StreamManifest manifest = await client.Videos.Streams.GetManifestAsync(url.ToString());

            IStreamInfo streamInfo = manifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            await client.Videos.Streams.DownloadAsync(streamInfo, $"{Path.TrimEndingDirectorySeparator(directory)}/{string.Join(", ", Artists)} - {Name}.{streamInfo.Container}");
        }

        public static async Task<MusicTrack> FromUrlAsync(string URL)
        {
            Url url = new Url(URL);

            if (url.Host.Contains("spotify"))
            {
                SpotifyClient spotify = new SpotifyClient();

                Track track = await spotify.Tracks.GetAsync(url.ToString());

                string? youtubeId = await spotify.Tracks.GetYoutubeIdAsync(url.ToString());

                return new MusicTrack(track.Title, track.Artists.Select(x => x.Name).ToList(), "https://youtube.com/watch?v=" + youtubeId);
            }
            else
            {
                YoutubeClient client = new YoutubeClient();

                Video video = await client.Videos.GetAsync(url.ToString());

                return new MusicTrack(video.Title, new List<string>() { video.Author.ChannelTitle }, url.ToString());
            }
        }

        public override string ToString() => $"[{string.Join(", ", Artists)} - {Name}]({URL})";
    }
}