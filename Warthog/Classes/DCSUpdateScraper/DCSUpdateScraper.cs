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
        public static void GetUpdatePage()
        {
            //get the page
            var web = new HtmlWeb();
            var document = web.Load("http://updates.digitalcombatsimulator.com");
            //var document = web.Load("http://example.com/page.html");
            var page = document.DocumentNode;

            string html = page.InnerHtml.ToString();

            html = html.Substring(html.IndexOf("Latest stable version is" + 25));

            string release = html.Substring(0, html.IndexOf("</h2>")); //Latest stable version is
            string beta = "";
            string alpha = "";

            
    
            Console.WriteLine(release);

            Console.WriteLine(release);

        }
    }
}
