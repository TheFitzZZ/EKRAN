using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes
{
    public static class CalendarXMLManagement
    {
        //Define the global var for all calendar events
        public static List<CalendarEvent> arrEvents = new List<CalendarEvent>();

        public static void CalendarWriteXML()
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(List<CalendarEvent>));
            var wfile = new System.IO.StreamWriter(@"c:\temp\SerializationOverview.xml");
            writer.Serialize(wfile, arrEvents);
            wfile.Close();
        }

        public static void CalendarReadXML()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(List<CalendarEvent>));
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\temp\SerializationOverview.xml");
            arrEvents = (List<CalendarEvent>)reader.Deserialize(file);
            file.Close();
        }
        
    }
}
