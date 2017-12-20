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

        /// 
        /// Global helper functions (exist in every command module until I know how to seperate them)
        /// 
        public async Task DeleteCommandMessage()
        {
            IGuildUser Bot = await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
            if (!Bot.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
            {
                await Context.Channel.SendMessageAsync("`Bot does not have enough permissions to manage messages - Please fix that.`");
                return;
            }
            await Context.Message.DeleteAsync();
        }

      
        //
        // Calendar stuff
        //
        [Command("Newevent")]
        [Alias("newevent", "createevent")]
        [Summary("Creates a new event")]
        public async Task Newevent(string sEventName = "", string sEventDateTime = "", string sPublic = "")
        {
            if (sEventDateTime == "" | sEventName == "")
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please state at least the following: Eventname and event date + time \nExample: \n!newevent " + @"""My Event"" ""2018-12-24 12:34"" ";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else if (Context.Guild == null) 
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else
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
                List<ulong> temp = new List<ulong>();
                temp.Add(Context.User.Id);
                newevent.Attendees = temp;
                newevent.PublicEvent = bPublic;
                newevent.EventGuild = Context.Guild.Id;

                XMLIntIncrementer.XMLIntIncrement.CalendarEvent++;

                CalendarXMLManagement.arrEvents.Add(newevent);
                CalendarXMLManagement.CalendarWriteXML();
                XMLIntIncrementer.IndexWriteXML();

                await ReplyAsync("Added new event with ID " + newevent.EventID);
            }
        }

        [Command("deleteevent")]
        [Alias("dev", "delevent")]
        [Summary("Deletes an event")]
        public async Task Delevent(long iID = 0)
        {
            if (iID == 0)
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please state the ID of the event to delete \nExample: \n!delevent 1337";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else if (Context.Guild == null)
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else
            {
                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.EventID == iID)
                    {
                        // Check if the source of the request comes from the same server as the event was created
                        if (Event.EventGuild == Context.Guild.Id)
                        {
                            CalendarXMLManagement.arrEvents.Remove(Event);
                            CalendarXMLManagement.CalendarWriteXML();
                            await ReplyAsync("Event removed!");
                            return;
                        }
                        else
                        {
                            await ReplyAsync("The event associated to this ID is not assigned to this server. Please verify the event ID.");
                            return;
                        }
                    }
                }

                await ReplyAsync("Event not found!");
            }
        }

        [Command("attend")]
        [Alias("aev", "attendevent", "addme")]
        [Summary("Adds you to the attendee list of an")]
        public async Task Attendevent(long iID = 0)
        {
            if (iID == 0)
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please state the ID of the event to delete \nExample: \n!addme 1337";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else if (Context.Guild == null)
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else
            {
                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.EventID == iID)
                    {
                        if (Event.Attendees.Contains(Context.User.Id))
                        {
                            await ReplyAsync("Your participation has already been added to the event!");
                            return;
                        }
                        else
                        { 
                            // Check if the source of the request comes from the same server as the event was created
                            if (Event.EventGuild == Context.Guild.Id | Event.PublicEvent)
                            {
                                Event.Attendees.Add(Context.User.Id);
                                CalendarXMLManagement.CalendarWriteXML();
                                await ReplyAsync("Your participation has been added to the event!");
                                return;
                            }
                            else
                            {
                                await ReplyAsync("The event associated to this ID is not assigned to this server or not set to public. Please verify the event ID.");
                                return;
                            }
                        }
                    }
                }

                await ReplyAsync("Event not found!");
            }
        }

        [Command("cancel")]
        [Alias("cev", "cancelevent", "removeme")]
        [Summary("Removes you from the attendee list of an event")]
        public async Task Cancelevent(long iID = 0)
        {
            if (iID == 0)
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please state the ID of the event to delete \nExample: \n!removeme 1337";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else if (Context.Guild == null)
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else
            {
                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.EventID == iID)
                    {
                        if (Event.Attendees.Contains(Context.User.Id))
                        {
                            Event.Attendees.Remove(Context.User.Id);
                            CalendarXMLManagement.CalendarWriteXML();
                            await ReplyAsync("Your participation has been removed from the event!");
                            return;
                        }
                        else
                        {
                            await ReplyAsync("You're not registered for this event.");
                            return;                            
                        }
                    }
                }

                await ReplyAsync("Event not found!");
            }
        }

        [Command("Getevents")]
        [Alias("getevents", "listevents")]
        [Summary("Lists all events")]
        public async Task Getevents()
        {
            if (Context.Guild == null)
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else 
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
                            $"Event creator: **{Warthog.Program.client.GetUser(Event.EventCreator)}**\n" +
                            $"Event ID: **{Event.EventID}**";

                        await ReplyAsync("", false, embed.Build());
                    }
                }
            }
        }

        [Command("Calendar")]
        [Alias("cal")]
        [Summary("Lists all events in calendar style")]
        public async Task Calendar()
        {
            if (Context.Guild == null)
            {
                //Text was send by DM, not supported
                string helptext = "Please use this command in a text channel of your server, not via DM.";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
            else
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
                        $"Attendees: **{Event.Attendees.Count}**\n" +
                        $"ID: **{Event.EventID}**\n");

                        MyEmbedBuilder.AddField(MyEmbedField);
                    }
                }

                await ReplyAsync("", false, MyEmbedBuilder);
            }
        }
    }
}
