using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;

namespace Warthog.Classes
{
    public class CalendarCommands : ModuleBase
    {
        private CommandService _service;

        public CalendarCommands(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        //
        // Calendar stuff
        //
        [Command("Newevent")]
        [Alias("newevent", "createevent")]
        [Summary("Creates a new event")]
        public async Task Newevent(string sEventName, string sEventDateTime, string sPublic = "")
        {
            // try to adapt what the time is supposed to be
            DateTime dtEventDateTime = DateTime.Parse(sEventDateTime);

            bool bPublic = false;
            if (sPublic != "") { bPublic = true; }

            CalendarEvent newevent = new CalendarEvent();
            newevent.EventID = XMLIntIncrementer.XMLIntIncrement.CalendarEvent;
            newevent.Active = true;
            newevent.Eventname = sEventName;
            newevent.EventDate = dtEventDateTime;
            newevent.EventCreator = Context.User.Id;
            newevent.CreatedDate = DateTime.Now;
            ulong[] temp = { 0, 0 };
            newevent.Attendees = temp;
            newevent.PublicEvent = bPublic;
            newevent.EventGuild = Context.Guild.Id;

            XMLIntIncrementer.XMLIntIncrement.CalendarEvent++;

            CalendarXMLManagement.arrEvents.Add(newevent);
            CalendarXMLManagement.CalendarWriteXML();
            XMLIntIncrementer.IndexWriteXML();

            await ReplyAsync("Added new event...");
        }

        [Command("Getevents")]
        [Alias("getevents", "listevents")]
        [Summary("Lists all events")]
        public async Task Getevents()
        {
            await ReplyAsync("There are " + CalendarXMLManagement.arrEvents.Count + " events in the file.");
            foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
            {
                if (Event.Active & (Context.Guild.Id == Event.EventGuild | Event.PublicEvent))
                {
                    string sType = null;
                    if (Event.PublicEvent) { sType = "Public"; }
                    else { sType = "Internal"; }

                    var thumbnailurl = "https://cdn4.iconfinder.com/data/icons/small-n-flat/24/calendar-512.png";
                    var auth = new EmbedAuthorBuilder()
                    {
                        Name = Event.Eventname + $" ({sType})",
                        IconUrl = thumbnailurl,
                    };
                    var embed = new EmbedBuilder()
                    {
                        Color = new Color(29, 140, 209),
                        Author = auth
                    };

                    string sAttendees = "\n";
                    if (Event.Attendees != null)
                    {
                        foreach (ulong userID in Event.Attendees)
                        {
                            if (sAttendees != "\n") { sAttendees = sAttendees + ",\n"; }
                            sAttendees = sAttendees + Warthog.Program.client.GetUser(userID);
                        }
                    }

                    embed.Title = "Event information";
                    embed.Description = $"Event Date: **{Event.EventDate.ToUniversalTime()} UTC**\n" +
                        $"Attendees: **{sAttendees}**\n" +
                        $"Event creator: **{Warthog.Program.client.GetUser(Event.EventCreator)}**";

                    await ReplyAsync("", false, embed.Build());
                }
            }
        }

        [Command("Calendar")]
        [Alias("cal")]
        [Summary("Lists all events in calendar style")]
        public async Task Calendar()
        {
            EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
            MyEmbedBuilder.WithColor(new Color(43, 234, 152));
            MyEmbedBuilder.WithTitle("Planned Events");

            MyEmbedBuilder.WithDescription("These are our upcoming events:");

            MyEmbedBuilder.WithThumbnailUrl("https://cdn4.iconfinder.com/data/icons/small-n-flat/24/calendar-512.png");
            //MyEmbedBuilder.WithImageUrl("https://cdn4.iconfinder.com/data/icons/small-n-flat/24/calendar-512.png");

            //Footer
            EmbedFooterBuilder MyFooterBuilder = new EmbedFooterBuilder();
            MyFooterBuilder.WithText("DM the bot with !help to get a command overview.");
            MyFooterBuilder.WithIconUrl("https://vignette2.wikia.nocookie.net/mario/images/f/f6/Question_Block_Art_-_New_Super_Mario_Bros.png/revision/latest?cb=20120603213532");
            MyEmbedBuilder.WithFooter(MyFooterBuilder);

            //Author		
            EmbedAuthorBuilder MyAuthorBuilder = new EmbedAuthorBuilder();
            MyAuthorBuilder.WithName($"{Context.Guild.Name}'s Schedule");
            //MyAuthorBuilder.WithUrl("http://www.google.com");		
            MyEmbedBuilder.WithAuthor(MyAuthorBuilder);
            
            foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
            {
                if (Event.Active & (Context.Guild.Id == Event.EventGuild | Event.PublicEvent))
                {
                    string sType = null;
                    if (Event.PublicEvent) { sType = "Public"; }
                    else { sType = "Internal"; }
                  
                    //EmbedField		
                    EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
                    MyEmbedField.WithIsInline(true);
                    MyEmbedField.WithName(Event.Eventname + $" ({sType})");
                    MyEmbedField.WithValue(
                    $"Date: **{Event.EventDate.ToShortDateString()} **\n" +
                    $"Time: **{Event.EventDate.ToShortTimeString()} UTC**\n" +
                    $"Attendees: **{Event.Attendees.Length}**\n");

                    MyEmbedBuilder.AddField(MyEmbedField);
                }
            }

            await ReplyAsync("", false, MyEmbedBuilder);
        }
    }
}
