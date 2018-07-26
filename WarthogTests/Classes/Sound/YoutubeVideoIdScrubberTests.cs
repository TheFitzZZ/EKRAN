using Microsoft.VisualStudio.TestTools.UnitTesting;
using Warthog.Classes.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes.Sound.Tests
{
    [TestClass()]
    public class YoutubeVideoLinkScrubberTests
    {
        private const string expectedVideoId = "fIuO3RpMvHg";
        private const string expectedCleanVideoUrl = "https://www.youtube.com/watch?v=fIuO3RpMvHg";

        private List<(string input, string expected)> cases = new List<(string, string)>()
            {
                (@"https://www.youtube.com/watch?v=fIuO3RpMvHg",expectedVideoId),
                (@"https://www.youtube.com/watch?v=fIuO3RpMvHg&list=PLy37-5gQy0-55yMYVK0evD-jBCpeIN5cl&index=2&t=0s",expectedVideoId),
                (@"https://www.youtube.com/watch?list=PLy37-5gQy0-55yMYVK0evD-jBCpeIN5cl&v=fIuO3RpMvHg&index=2&t=0s",expectedVideoId),
            };

        [TestMethod()]
        public void GetYoutubeVideoIdFromUrlTest()
        {
            int caseCounter = 0;
            foreach (var test in cases)
            {
                caseCounter++;
                var res = YoutubeVideoLinkScrubber.GetYoutubeVideoIdFromUrl(test.input);
                Assert.AreEqual(test.expected, res, $"Testcase #{caseCounter}: The video id must be extraced as expected");
            }

            Assert.AreEqual(cases.Count, caseCounter, "All test cases must be run.");
        }

        [TestMethod()]
        public void GetCleanWatchUrlFromFromUrlTest()
        {
            int caseCounter = 0;
            foreach (var test in cases)
            {
                var res = YoutubeVideoLinkScrubber.GetCleanWatchUrlFromFromUrl(test.input);
                Assert.AreEqual(expectedCleanVideoUrl, res, $"Testcase #{caseCounter}: A clean video url with the correct id must be returned");
                caseCounter++;
            }

            Assert.AreEqual(cases.Count, caseCounter, "All test cases must be run.");
        }
    }
}