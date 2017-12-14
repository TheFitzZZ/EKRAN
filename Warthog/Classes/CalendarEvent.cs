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
        public string Eventname { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EventDate { get; set; }
        public IList<IUser> Attendees { get; set; }
        public IUser EventCreator { get; set; }
    }
}