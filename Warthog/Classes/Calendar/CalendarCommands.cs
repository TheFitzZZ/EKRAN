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
        [Command("newevent")]
        [Alias("nev", "createevent")]
        [Summary("Creates a new event")]
        public async Task Newevent(string sEventName = "", string sEventDateTime = "")
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

                //bool bPublic = false;
                //if (sPublic != "") { bPublic = true; }

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
                newevent.PublicEvent = false;
                newevent.EventGuild = Context.Guild.Id;

                XMLIntIncrementer.XMLIntIncrement.CalendarEvent++;

                CalendarXMLManagement.arrEvents.Add(newevent);
                CalendarXMLManagement.CalendarWriteXML();
                XMLIntIncrementer.IndexWriteXML();

                await ReplyAsync("Added new event with ID " + newevent.EventID);

                string helptext = "You can add more details to the event if you like:\n\n" +
                                  "\nAdd a max attendee limit: !editev *eventid* **maxattendees** 15" +
                                  "\nAdd a URL to a briefing: !editev *eventid* **briefingurl** http://someurl.com/briefing" +
                                  "\nAdd a URL to a Discord invite: !editev *eventid* **discordurl** http://disc0rd.gg/yourinviteid \n" +
                                  @"Add a short description to the event: !editev *eventid* **description** ""Description goes here""" +
                                  "\nToggle public flag of the event: !editev *eventid* **public**" +
                                  "\n" + @"Edit the event name: !editev *eventid* **name** ""My Eventname""" +
                                  "\n" + @"Edit the event date: !editev *eventid* **date** ""2018-12-24 13:37""";

                var channel = await Context.User.GetOrCreateDMChannelAsync();
                await channel.SendMessageAsync(helptext);
            }
        }

        [Command("editevent")]
        [Alias("eev", "editev")]
        [Summary("Let's you edit details of an event")]
        public async Task Editevent(int iID = 0, string sCommand = "", string sData = "")
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();

            if ((sData == "" | iID == 0 | sCommand == "") & !(sCommand == "public") )
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please use !help to... well, get help!";
                
                await channel.SendMessageAsync(helptext);
            }
            else
            {
                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.EventID == iID)
                    {
                        var user = Context.User as SocketGuildUser;
                        var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Calendar Admin");
                        if (!user.Roles.Contains(role) | !Event.EventCreator.Equals(Context.User.Id))
                        {
                            // Secrurity check
                            await ReplyAsync("This is not your event, you cannot edit it!");
                            return;
                        }
                        else
                        {
                            // Go through the different options
                            switch (sCommand)
                            {
                                case "public":
                                    string sTemp = "";
                                    if (Event.PublicEvent)
                                    {
                                        Event.PublicEvent = false;
                                        sTemp = "internal";
                                    }
                                    else
                                    {
                                        Event.PublicEvent = true;
                                        sTemp = "public";
                                    }

                                    CalendarXMLManagement.CalendarWriteXML();
                                    await channel.SendMessageAsync($"The event {Event.EventID} is now {sTemp}.");
                                    return;

                                case "briefingurl":
                                    //check url
                                    Uri uriResult;
                                    bool result = Uri.TryCreate(sData, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                                    if(result)
                                    {
                                        Event.BriefingURL = sData;
                                        await channel.SendMessageAsync($"The event {Event.EventID} briefing URL is now {sData}.");
                                        CalendarXMLManagement.CalendarWriteXML();
                                    }
                                    else
                                    {
                                        await channel.SendMessageAsync($"Invalid URL!");
                                    }
                                    return;

                                case "discordurl":
                                    //check url
                                    Uri uriResult2;
                                    bool result2 = Uri.TryCreate(sData, UriKind.Absolute, out uriResult2) && (uriResult2.Scheme == Uri.UriSchemeHttp || uriResult2.Scheme == Uri.UriSchemeHttps);
                                    if (result2)
                                    {
                                        Event.DiscordURL = sData;
                                        await channel.SendMessageAsync($"The event {Event.EventID} Discord URL is now {sData}.");
                                        CalendarXMLManagement.CalendarWriteXML();
                                    }
                                    else
                                    {
                                        await channel.SendMessageAsync($"Invalid URL!");
                                    }
                                    return;

                                case "maxattendees":
                                    //check data
                                    int MAResult = 0;
                                    if(!int.TryParse(sData, out MAResult))
                                    {
                                        await channel.SendMessageAsync($"Invalid number!");
                                    }
                                    else
                                    {
                                        Event.MaxAttendees = MAResult;
                                        await channel.SendMessageAsync($"The event {Event.EventID} has now {MAResult} max attendees set.");
                                        CalendarXMLManagement.CalendarWriteXML();
                                    }
                                    return;

                                case "date":
                                    //check data
                                    DateTime DateResult = new DateTime();
                                    if (!DateTime.TryParse(sData, out DateResult))
                                    {
                                        await channel.SendMessageAsync(@"Invalid date / time! Try it like this (with quote!): ""2018-12-24 14:56""");
                                    }
                                    else
                                    {
                                        Event.EventDate = DateResult;
                                        await channel.SendMessageAsync($"The event {Event.EventID} has now {DateResult} as the date/time set.");
                                        CalendarXMLManagement.CalendarWriteXML();
                                    }
                                    return;

                                case "name":
                                    Event.Eventname = sData;
                                    await channel.SendMessageAsync($"The event {Event.EventID} has now the following name: {sData}");
                                    CalendarXMLManagement.CalendarWriteXML();
                                    return;

                                case "description":
                                    Event.Eventdescription = sData;
                                    await channel.SendMessageAsync($"The event {Event.EventID} has now the following description: {sData}");
                                    CalendarXMLManagement.CalendarWriteXML();
                                    return;

                            }
                        }
                    }
                }
                await ReplyAsync("Event not found!");
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
                            var user = Context.User as SocketGuildUser;
                            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Calendar Admin");
                            if (!user.Roles.Contains(role) & !Event.EventCreator.Equals(Context.User.Id))
                            {
                                // Secrurity check
                                await ReplyAsync("This is not your event, you cannot delete it!");
                                return;
                            }
                            else
                            {
                                CalendarXMLManagement.arrEvents.Remove(Event);
                                CalendarXMLManagement.CalendarWriteXML();
                                await ReplyAsync("Event removed!");
                                return;
                            }
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
        public async Task Attendevent(long iID = 0, string sPlayer = "")
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
                        else if (sPlayer != "" & Event.EventCreator == Context.User.Id)
                        {
                            //foreach (ulong Attendee in Event.Attendees)
                            //{
                            //    IUser user = Warthog.Program.client.GetUser(Attendee);
                            //    string sFullname = user.Username + "#" + user.Discriminator;
                            //    if (sFullname == sPlayer)
                            //    {
                            //        Event.Attendees.Remove(user.Id);
                            //        CalendarXMLManagement.CalendarWriteXML();
                            //        await ReplyAsync($"{user.Username} has been removed from the event!");
                            //        return;
                            //    }
                            //}
                            await ReplyAsync($"Adding other users not implemented yet. Please tell FitzZZ that you'd like to see this feature.");
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

                                //warn if over max attendees
                                if(Event.Attendees.Count > Event.MaxAttendees)
                                {
                                    await ReplyAsync("Please be aware that the event has no more free slots! You're added to the list anyway, but keep in mind that the event creator might not let you participate.");
                                }

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

        [Command("skip")]
        [Alias("sev", "skipevent", "removeme", "removeplayer")]
        [Summary("Removes you from the attendee list of an event")]
        public async Task Skipevent(long iID = 0, string sPlayer = "")
        {
            if (iID == 0)
            {
                //Not enough data given, send user info on how to use this
                string helptext = "Not enough parameters! Please state the ID of the event to delete \nExample: \n!skipevent 1337";

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
                        if (Event.Attendees.Contains(Context.User.Id) & sPlayer == "")
                        {
                            Event.Attendees.Remove(Context.User.Id);
                            CalendarXMLManagement.CalendarWriteXML();
                            await ReplyAsync("Your participation has been removed from the event!");
                            return;
                        }
                        else if (sPlayer != "" & Event.EventCreator == Context.User.Id)
                        {
                            foreach (ulong Attendee in Event.Attendees)
                            {
                                IUser user = Warthog.Program.client.GetUser(Attendee);
                                string sFullname = user.Username + "#" + user.Discriminator;
                                if(sFullname == sPlayer)
                                {
                                    Event.Attendees.Remove(user.Id);
                                    CalendarXMLManagement.CalendarWriteXML();
                                    await ReplyAsync($"{user.Username} has been removed from the event!");
                                    return;
                                }
                            }
                            await ReplyAsync($"{sPlayer} was not found!");
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

        [Command("getevents")]
        [Alias("listevents", "gev", "lev", "events")]
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
                //await ReplyAsync("There are " + CalendarXMLManagement.arrEvents.Count + " events in the file.");
                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.Active & (Context.Guild.Id == Event.EventGuild & (Event.EventDate.Date >= DateTime.UtcNow.Date) | Event.PublicEvent))
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
                        //if (Event.Attendees != null)
                        //{
                        //    foreach (ulong userID in Event.Attendees)
                        //    {
                        //        if (sAttendees != "\n") { sAttendees = sAttendees + ",\n"; }
                        //        sAttendees = sAttendees + Warthog.Program.client.GetUser(userID);
                        //    }
                        //}

                        embed.Title = "Event information";

                        string sEmbedDescription = $"Event Date: **{Event.EventDate} UTC**\n";
                        //sEmbedDescription += $"Attendees: **{sAttendees}**\n";

                        if (Event.MaxAttendees == 0)
                        {
                            sEmbedDescription += $"Attendees: {Event.Attendees.Count}\n";
                        }
                        else
                        {
                            sEmbedDescription += $"Attendees: {Event.Attendees.Count}/{Event.MaxAttendees}\n";
                        }

                        sEmbedDescription +=  
                            $"Event creator: **{Warthog.Program.client.GetUser(Event.EventCreator)}**\n" +
                            $"Event ID: **{Event.EventID}**";

                        embed.Description = sEmbedDescription;
                        await ReplyAsync("", false, embed.Build());
                    }
                }
                await ReplyAsync("``You can get more information on an event by using !event *eventid*``");
            }
        }

        [Command("event")]
        [Alias("ev")]
        [Summary("List details of one event")]
        public async Task Event(long iID)
        {
            //await ReplyAsync("There are " + CalendarXMLManagement.arrEvents.Count + " events in the file.");

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

                            string sEmbedDescription = $"Event Date: **{Event.EventDate} UTC**\n";
                            //sEmbedDescription += $"Attendees: **{sAttendees}**\n";

                            if (Event.MaxAttendees == 0)
                            {
                                sEmbedDescription += $"Attendees ({Event.Attendees.Count}): **{sAttendees}**\n";
                            }
                            else
                            {
                                sEmbedDescription += $"Attendees ({Event.Attendees.Count}/{Event.MaxAttendees}): **{sAttendees}**\n";
                            }

                            sEmbedDescription +=
                                $"Description: **{Event.Eventdescription}**\n" + 
                                $"Briefing: **{Event.BriefingURL}**\n" +
                                $"Discord: **{Event.DiscordURL}**\n" +
                                $"Event creator: **{Warthog.Program.client.GetUser(Event.EventCreator)}**\n" +
                                $"Event ID: **{Event.EventID}**";
                            

                            embed.Description = sEmbedDescription;
                            await ReplyAsync("", false, embed.Build());
                        }
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
                MyFooterBuilder.WithText("You can get more information on an event by using !event *eventid* -- DM the bot with !help to get a command overview.");
                MyFooterBuilder.WithIconUrl("https://vignette2.wikia.nocookie.net/mario/images/f/f6/Question_Block_Art_-_New_Super_Mario_Bros.png/revision/latest?cb=20120603213532");
                MyEmbedBuilder.WithFooter(MyFooterBuilder);

                //Author		
                EmbedAuthorBuilder MyAuthorBuilder = new EmbedAuthorBuilder();
                MyAuthorBuilder.WithName($"{Context.Guild.Name}'s Schedule");
                //MyAuthorBuilder.WithUrl("http://www.google.com");		
                MyEmbedBuilder.WithAuthor(MyAuthorBuilder);

                foreach (CalendarEvent Event in CalendarXMLManagement.arrEvents)
                {
                    if (Event.Active & (Context.Guild.Id == Event.EventGuild & (Event.EventDate.Date >= DateTime.UtcNow.Date) | Event.PublicEvent))
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

                        //if (Event.MaxAttendees == 0)
                        //{
                        //    sEmbedDescription += $"Attendees: **{sAttendees}**\n";
                        //}
                        //else
                        //{
                        //    sEmbedDescription += $"Attendees: **{sAttendees}/{E}**\n";
                        //}

                        MyEmbedBuilder.AddField(MyEmbedField);
                    }
                }

                await ReplyAsync("", false, MyEmbedBuilder);
            }
        }
    }
}
