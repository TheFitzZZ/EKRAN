using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace Warthog.Classes
{
    public static class DCSUpdateScraper
    {
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
            //get the page
            var web = new HtmlWeb();
            var document = web.Load("http://updates.digitalcombatsimulator.com");
            var page = document.DocumentNode;

            Console.WriteLine(DateTime.UtcNow + " Checking for DCS Updates...");

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



            Console.WriteLine(stable);
            Console.WriteLine(beta);
            Console.WriteLine(alpha);

            var stableverold = new Version(stable.Substring(2));
            var stablevernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Stable.Substring(2));
            var betaverold = new Version(beta.Substring(2));
            var betavernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Beta.Substring(2));
            var alphaverold = new Version(alpha.Substring(2));
            var alphavernew = new Version(DCSVersionXMLManagement.arrDCSVersions.Alpha.Substring(2));


            if (stablevernew.CompareTo(stableverold) == -1)
            {
                //Write data to xml
                DCSVersionXMLManagement.arrDCSVersions.Stable = stable;
                DCSVersionXMLManagement.arrDCSVersions.StableDate = DateTime.UtcNow;
                //Announce in channel


                //Log in console
                Console.WriteLine(DateTime.UtcNow + " Update stable found, announced!");
            }

            if (betavernew.CompareTo(betaverold) == -1)
            {
                //Write data to xml
                DCSVersionXMLManagement.arrDCSVersions.Beta = beta;
                DCSVersionXMLManagement.arrDCSVersions.BetaDate = DateTime.UtcNow;
                //Announce in channel

                //Log in console
                Console.WriteLine(DateTime.UtcNow + " Update beta found, announced!");
            }

            if (alphavernew.CompareTo(alphaverold) == -1)
            {
                //Write data to xml
                DCSVersionXMLManagement.arrDCSVersions.Alpha = alpha;
                DCSVersionXMLManagement.arrDCSVersions.AlphaDate = DateTime.UtcNow;
                //Announce in channel

                //Log in console
                Console.WriteLine(DateTime.UtcNow + " Update alpha found, announced!");
            }

            //Just write that stuff to the XML
            DCSVersionXMLManagement.DCSVersionWriteXML();

            //Warthog.Classes.BaseCommands.
        }

    }
}
