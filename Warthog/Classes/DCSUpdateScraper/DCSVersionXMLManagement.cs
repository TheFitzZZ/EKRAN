using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes
{
    public static class DCSVersionXMLManagement
    {
        //Define the global var for dcs versions
        public static DCSVersion arrDCSVersions = new DCSVersion();

        public static void DCSVersionWriteXML()
        {
            var writer = new System.Xml.Serialization.XmlSerializer(typeof(DCSVersion));
            var wfile = new System.IO.StreamWriter(@"c:\temp\DCSVersion.xml");
            writer.Serialize(wfile, arrDCSVersions);
            wfile.Close();
        }

        public static void DCSVersionReadXML()
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(DCSVersion));
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\temp\DCSVersion.xml");
            arrDCSVersions = (DCSVersion)reader.Deserialize(file);
            file.Close();
        }
    }
}
