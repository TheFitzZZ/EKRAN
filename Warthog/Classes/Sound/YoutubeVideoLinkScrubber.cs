using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Warthog.Classes.Sound
{
    public static class YoutubeVideoLinkScrubber
    {
        private static readonly Regex regex = new Regex(@"(?:youtube(?:-nocookie)?\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");

        public static string GetYoutubeVideoIdFromUrl(string url)
        {
            var matches = regex.Match(url);
            var res = matches.Groups.Count > 1 ? matches.Groups[1].Value : null;
            return res;
        }

        private static string ConstructCleanWatchUrlFromVideoId(string videoId)
        {
            return $"https://www.youtube.com/watch?v={videoId}";
        }

        public static string GetCleanWatchUrlFromFromUrl(string url)
        {
            var id = GetYoutubeVideoIdFromUrl(url);
            return id != null ? ConstructCleanWatchUrlFromVideoId(id) : null;
        }
    }
}
