using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes
{
    public static class XMLIntIncrementer
    {
        //Define the global var for all incremental counter
        public static XMLIntIncrementerClass XMLIntIncrement = new XMLIntIncrementerClass();

        public static void InitializeIndexXML()
        {
            try { IndexReadXML(); }
            catch
            {
                //If file not found or inaccessible, generate a new one
                XMLIntIncrement.CalendarEvent = 1;
            }
        }

        public static void IndexWriteXML()
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(XMLIntIncrementerClass));
            var wfile = new System.IO.StreamWriter(@"c:\temp\Indexes.xml");
            writer.Serialize(wfile, XMLIntIncrement);
            wfile.Close();
        }

        public static void IndexReadXML()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(XMLIntIncrementerClass));
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\temp\Indexes.xml");
            XMLIntIncrement = (XMLIntIncrementerClass)reader.Deserialize(file);
            file.Close();
        }
    }
}
