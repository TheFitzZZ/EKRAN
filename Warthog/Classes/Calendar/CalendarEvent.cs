using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warthog.Classes
{
    public class CalendarEvent
    {
        public long EventID { get; set; }
        public string Eventname { get; set; }
        public string Eventdescription { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public int MaxAttendees { get; set; }
        public List<ulong> Attendees { get; set; }
        public string BriefingURL { get; set; }
        public string DiscordURL { get; set; }
        public ulong EventCreator { get; set; }
        public ulong EventGuild { get; set; }
        public bool PublicEvent { get; set; }
    }
}