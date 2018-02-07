using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;
using Discord.WebSocket;
using Discord.Commands;
using Discord;

namespace Warthog.Classes
{
    public class DCSUpdateScraper : ModuleBase
    {
        private CommandService _service;

        public DCSUpdateScraper(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        public static void InitializeDCSVersionXML()
        {
            try { DCSVersionXMLManagement.DCSVersionReadXML(); }
            catch
            {
                //If file not found or inaccessible, generate a new one
                DCSVersionXMLManagement.arrDCSVersions.Stable = "0.0.0.0";
                DCSVersionXMLManagement.arrDCSVersions.Beta = "0.0.0.0";
                DCSVersionXMLManagement.arrDCSVersions.Alpha = "0.0.0.0";
                DCSVersionXMLManagement.arrDCSVersions.StableDate = Convert.ToDateTime("2018-01-01");
                DCSVersionXMLManagement.arrDCSVersions.BetaDate = Convert.ToDateTime("2018-01-01");
                DCSVersionXMLManagement.arrDCSVersions.AlphaDate = Convert.ToDateTime("2018-01-01");
                DCSVersionXMLManagement.DCSVersionWriteXML();
            }
        }

        public static void CheckDCSUpdate()
        {
            Console.WriteLine($"{DateTime.Now} [Scheduler] DCS Update Check: Running DCS Update checker.");
            
            //get the page
            var web = new HtmlWeb();
            var document = web.Load("http://updates.digitalcombatsimulator.com");
            var page = document.DocumentNode;

            string html = page.InnerHtml.ToString().ToLower();

            var a = html.IndexOf("stable version is");
            html = html.Substring(a + 18);
            string stable = html.Substring(0, html.IndexOf("</h2>")); //Latest stable version is

            a = html.IndexOf("current openbeta is");
            html = html.Substring(a + 20);
            string beta = html.Substring(0, html.IndexOf("</h2>")); //<h2>Current openbeta is 2.5.0.13818.311</h2>

            a = html.IndexOf("current openalpha is");
            html = html.Substring(a + 21);
            string alpha = html.Substring(0, html.IndexOf("</h2>"));  // <h2>Current openalpha is 2.2.0.12843.297</h2>
            
            //Console.WriteLine(stable);
            //Console.WriteLine(beta);
            //Console.WriteLine(alpha);

            var stableverold = new Version(stable.Substring(2));
            var stablevernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Stable.Substring(2));
            var betaverold = new Version(beta.Substring(2));
            var betavernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Beta.Substring(2));
            var alphaverold = new Version(alpha.Substring(2));
            var alphavernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Alpha.Substring(2));

            var channel = Program.client.GetChannel(386545965374373898) as SocketTextChannel;

            double iDays = 0;

            if (channel != null ) //& (1 == 2)
            {
                if (stablevernew.CompareTo(stableverold) == -1)
                {
                    //Calc days since update
                    iDays = Math.Floor((DateTime.UtcNow - DCSVersionXMLManagement.arrDCSVersions.StableDate).TotalDays);

                    //Write data to xml
                    //DCSVersionXMLManagement.arrDCSVersions.Stable = stable;
                    DCSVersionXMLManagement.arrDCSVersions.StableDate = DateTime.UtcNow;

                    //Build the embed
                    EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
                    MyEmbedBuilder.WithColor(new Color(43, 234, 152));
                    MyEmbedBuilder.WithTitle("New DCS Update released!");
                    MyEmbedBuilder.WithDescription("[DCS Update Page](http://updates.digitalcombatsimulator.com)");

                    MyEmbedBuilder.WithThumbnailUrl("http://is1.mzstatic.com/image/thumb/Purple49/v4/71/b8/bc/71b8bca9-dfc5-e040-4e0f-54488d6a913b/source/175x175bb.jpg");
                    //MyEmbedBuilder.WithImageUrl("https://avatars2.githubusercontent.com/u/204635?s=64&v=4");

                    EmbedFooterBuilder MyFooterBuilder = new EmbedFooterBuilder();
                    MyFooterBuilder.WithText("Days since last update: " + iDays);
                    MyFooterBuilder.WithIconUrl("http://is1.mzstatic.com/image/thumb/Purple49/v4/71/b8/bc/71b8bca9-dfc5-e040-4e0f-54488d6a913b/source/175x175bb.jpg");
                    MyEmbedBuilder.WithFooter(MyFooterBuilder);

                    EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
                    MyEmbedField.WithIsInline(true);
                    MyEmbedField.WithName("New stable version!");
                    MyEmbedField.WithValue("New version: " + stable);

                    //Announce in channel
                    MyEmbedBuilder.AddField(MyEmbedField);

                    //await ReplyAsync("Swaaaag:", false, MyEmbedBuilder);
                    channel.SendMessageAsync("", false, MyEmbedBuilder);

                    //try { channel.SendMessageAsync("DCS stable update released: " + stable + " - Days since last update: " + iDays); }
                    //catch { }

                    //Log in console
                    Console.WriteLine(DateTime.UtcNow + " Update stable found, announced!");
                }

                if (betavernew.CompareTo(betaverold) == -1)
                {
                    //Calc days since update
                    iDays = Math.Floor((DateTime.UtcNow - DCSVersionXMLManagement.arrDCSVersions.BetaDate).TotalDays);
                    //Write data to xml
                    DCSVersionXMLManagement.arrDCSVersions.Beta = beta;
                    DCSVersionXMLManagement.arrDCSVersions.BetaDate = DateTime.UtcNow;
                    //Announce in channel
                    try { channel.SendMessageAsync("DCS beta update released: " + stable + " - Days since last update: " + iDays); }
                    catch { }
                    //Log in console
                    Console.WriteLine(DateTime.UtcNow + " Update beta found, announced!");
                }

                if (alphavernew.CompareTo(alphaverold) == -1)
                {
                    //Calc days since update
                    iDays = Math.Floor((DateTime.UtcNow - DCSVersionXMLManagement.arrDCSVersions.AlphaDate).TotalDays);
                    //Write data to xml
                    DCSVersionXMLManagement.arrDCSVersions.Alpha = alpha;
                    DCSVersionXMLManagement.arrDCSVersions.AlphaDate = DateTime.UtcNow;
                    //Announce in channel
                    try { channel.SendMessageAsync("DCS alpha update released: " + stable + " - Days since last update: " + iDays); }
                    catch { }
                    //Log in console
                    Console.WriteLine(DateTime.UtcNow + " Update alpha found, announced!");
                }
            }
        }

    }
}
