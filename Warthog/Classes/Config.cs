using Newtonsoft.Json;
using System;
using System.IO;

namespace Warthog.Config
{
    public class BotConfig
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string Prefix { get; set; }
        public string Token { get; set; }
        public BotConfig()
        {
            Prefix = "!";
            Token = "";
        }

        public void Save(string dir = "configuration\\config.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }

        //TODO: load this once and use it. Don't have the caller call Load all the time
        public static BotConfig Load(string dir = "configuration\\config.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(file));
        }
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}