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
    public class BaseCommands : ModuleBase
    {
        private CommandService _service;

        public BaseCommands(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        //
        // Basic command tests
        //

        [Command("Ping")]
        [Alias("ping", "pong")]
        [Summary("Returns a pong")]
        public async Task Say()
        {
            await ReplyAsync("Pong!");
        }

        [Command("Number")]
        [Alias("number")]
        [Summary("Returns you the number you chose")]
        public async Task Say2(int arg = 0)
        {
            await ReplyAsync("Your number is: " + arg);
        }

        [Command("User")]
        [Alias("user")]
        [Summary("Returns you user you gave me")]
        public async Task Say3(IUser user)
        {
            await ReplyAsync("Your user is: " + user.Mention);
        }

        [Command("Array")]
        [Alias("array")]
        [Summary("Returns the array you gave me")]
        public async Task Say4(params String[] array)
        {
            await ReplyAsync("Wow array!");
            foreach (string word in array)
            {
                await ReplyAsync(word);
            }
        }

        [Command("Remainder")]
        [Alias("remainder")]
        [Summary("If you want the parameter to parse untill the end of the command")]
        public async Task Say5([Remainder] String text)
        {
            await ReplyAsync("Wow remainder: " + text);
        }

        /*Like this you send it to a person of your choice*/
        [Command("Tell")]
        [Alias("tell")]
        [Summary("returns said sentence!")]
        public async Task Say6(IUser user)
        {
            var channel = await user.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync("The message here");
        }

        /*But to send it to the person using the command, the following is easier:*/
        [Command("Tellme")]
        [Alias("tellme")]
        [Summary("returns said sentence!")]
        public async Task Say7()
        {
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync("The message here");
        }

        [Command("Tellchan")]
        [Alias("tellchan")]
        [Summary("returns said sentence!")]
        public async Task Say8()
        {
            var channel = await Context.Guild.GetChannelAsync(386545965374373898) as SocketTextChannel;
            await channel.SendMessageAsync("Woops");
        }

        [Command("Sendmevirus")]
        [Alias("sendmevirus")]
        [Summary("returns viruz lulz!")]
        public async Task Say9()
        {
            //await Context.Channel.SendFileAsync(file);
            await ReplyAsync("Fix me");
        }

        [Command("Embed")]
        [Alias("embed")]
        [Summary("returns embed yo")]
        public async Task Say10()
        {
            EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
            MyEmbedBuilder.WithColor(new Color(43, 234, 152));
            MyEmbedBuilder.WithTitle("Your title");

            MyEmbedBuilder.WithUrl("http://www.google.com");
            //MyEmbedBuilder.WithDescription("My description");
            MyEmbedBuilder.WithDescription("[Google](http://www.google.com)");

            MyEmbedBuilder.WithThumbnailUrl("https://forum.codingwithstorm.com/Themes/Modern/images/vertex_image/social_twitter_icon.png");
            MyEmbedBuilder.WithImageUrl("https://forum.codingwithstorm.com/Themes/Modern/images/vertex_image/social_facebook_icon.png");

            //Footer
            EmbedFooterBuilder MyFooterBuilder = new EmbedFooterBuilder();
            MyFooterBuilder.WithText("Your text");
            MyFooterBuilder.WithIconUrl("https://forum.codingwithstorm.com/Smileys/Koloboks/wink3.gif");
            MyEmbedBuilder.WithFooter(MyFooterBuilder);

            //Author
            EmbedAuthorBuilder MyAuthorBuilder = new EmbedAuthorBuilder();
            MyAuthorBuilder.WithName("Your Name");
            MyAuthorBuilder.WithUrl("http://www.google.com");
            MyEmbedBuilder.WithAuthor(MyAuthorBuilder);

            //EmbedField
            EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
            MyEmbedField.WithIsInline(true);
            MyEmbedField.WithName("Your Field Name");
            MyEmbedField.WithValue("Your value");
            MyEmbedField.WithValue("[Youtube](http://www.youtube.com)");

            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);
            MyEmbedBuilder.AddField(MyEmbedField);

            await ReplyAsync("Swaaaag:", false, MyEmbedBuilder);
        }

        [Command("userinfo")]
        [Name("userinfo `<user>`")]
        public async Task UserInfo(IGuildUser user)
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var thumbnailurl = user.GetAvatarUrl();
            var date = $"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}";
            var auth = new EmbedAuthorBuilder()

            {

                Name = user.Username,
                IconUrl = thumbnailurl,

            };

            var embed = new EmbedBuilder()

            {
                Color = new Color(29, 140, 209),
                Author = auth
            };

            var us = user as SocketGuildUser;

            var D = us.Username;

            var A = us.Discriminator;
            var T = us.Id;
            var S = date;
            var C = us.Status;
            var CC = us.JoinedAt;
            var O = us.Game;
            embed.Title = $"**{us.Username}** Information";
            embed.Description = $"Username: **{D}**\nDiscriminator: **{A}**\nUser ID: **{T}**\nCreated at: **{S}**\nCurrent Status: **{C}**\nJoined server at: **{CC}**\nPlaying: **{O}**";

            await ReplyAsync("", false, embed.Build());
        }

        [Command("getuserinfo")]
        [Alias("getuserinfo")]
        [Summary("returns user info")]
        public async Task Say11()
        {
            ulong userID = 119019602574442497;

            IUser user = Warthog.Program.client.GetUser(userID);
            
            await ReplyAsync(user.Username+"#"+user.Discriminator);
        }

        [Command("Deleteme")]
        [Alias("deleteme", "del")]
        [Summary("Deletes the message of the user")]
        public async Task Say12()
        {
            await DeleteCommandMessage();
            await ReplyAsync("Pong and gone!");
        }

        [Command("Clear")]
        public async Task Clear([Remainder] int Delete = 0)
        {
            IGuildUser Bot = await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
            if (!Bot.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
            {
                await Context.Channel.SendMessageAsync("`Bot does not have enough permissions to manage messages`");
                return;
            }
            await Context.Message.DeleteAsync();
            var GuildUser = await Context.Guild.GetUserAsync(Context.User.Id);
            if (!GuildUser.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
            {
                await Context.Channel.SendMessageAsync("`You do not have enough permissions to manage messages`");
                return;
            }
            if (Delete == 0)
            {
                await Context.Channel.SendMessageAsync("`You need to specify the amount | !clear (amount) | Replace (amount) with anything`");
            }
            int Amount = 0;
            foreach (var Item in await Context.Channel.GetMessagesAsync(Delete).Flatten())
            {

                Amount++;
                await Item.DeleteAsync();

            }
            await Context.Channel.SendMessageAsync($"`{Context.User.Username} deleted {Amount} messages`");
        }

        [Command("ServerInfo")]
        [Alias("sinfo", "servinfo")]
        [Remarks("Info about a server")]
        public async Task GuildInfo()
        {
            EmbedBuilder embedBuilder;
            embedBuilder = new EmbedBuilder();
            embedBuilder.WithColor(new Color(0, 71, 171));

            var gld = Context.Guild as SocketGuild;
            var client = Context.Client as DiscordSocketClient;


            if (!string.IsNullOrWhiteSpace(gld.IconUrl))
                embedBuilder.ThumbnailUrl = gld.IconUrl;
            var O = gld.Owner.Username;

            var V = gld.VoiceRegionId;
            var C = gld.CreatedAt;
            var N = gld.DefaultMessageNotifications;
            var VL = gld.VerificationLevel;
            var XD = gld.Roles.Count;
            var X = gld.MemberCount;
            var Z = client.ConnectionState;

            embedBuilder.Title = $"{gld.Name} Server Information";
            embedBuilder.Description = $"Server Owner: **{O}\n**Voice Region: **{V}\n**Created At: **{C}\n**MsgNtfc: **{N}\n**Verification: **{VL}\n**Role Count: **{XD}\n **Members: **{X}\n **Conntection state: **{Z}\n\n**";
            await ReplyAsync("", false, embedBuilder);
        }




        /// 
        /// Global helper functions
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
        // Actual help commands
        //
        [Command("help")]
        [Remarks("Shows a list of all available commands per module.")]
        public async Task HelpAsync()
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync(); /* A channel is created so that the commands will be privately sent to the user, and not flood the chat. */

            string prefix = "!";  /* put your chosen prefix here */
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "These are the commands you can use"
            };

            foreach (var module in _service.Modules) /* we are now going to loop through the modules taken from the service we initiated earlier ! */
            {
                string description = null;
                foreach (var cmd in module.Commands) /* and now we loop through all the commands per module aswell, oh my! */
                {
                    var result = await cmd.CheckPreconditionsAsync(Context); /* gotta check if they pass */
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()}\n"; /* if they DO pass, we ADD that command's first alias (aka it's actual name) to the description tag of this embed */
                }

                if (!string.IsNullOrWhiteSpace(description)) /* if the module wasn't empty, we go and add a field where we drop all the data into! */
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }
            await dmChannel.SendMessageAsync("", false, builder.Build()); /* then we send it to the user. */
        }





        //
        // Detailed help for commands
        //
        // Still broken - needs fix
        //
        [Command("help")]
        [Remarks("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync(string command)
        {
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                                $"Remarks: {cmd.Remarks}";
                    x.IsInline = false;
                });
            }
            await dmChannel.SendMessageAsync("", false, builder.Build());
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
            if(sPublic != "") { bPublic = true; }

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

        ///
        /// Debug commands
        /// 
        [Command("Debug")]
        [Alias("debug")]
        [Summary("just for me")]
        public async Task Debug(string arg)
        {
            if(arg == "readxmlcalendar")
            {
                CalendarXMLManagement.CalendarReadXML();
                await ReplyAsync("There are " + CalendarXMLManagement.arrEvents.Count + " events in the file.");
            }
            else if(arg == "writexml")
            {
                CalendarXMLManagement.CalendarWriteXML();
                //await ReplyAsync("Loaded " + CalendarXMLManagement.arrEvents.Count + " events from file.");
            }
            else if (arg == "readxmlindex")
            {
                XMLIntIncrementer.IndexReadXML();
                //await ReplyAsync("Loaded " + CalendarXMLManagement.arrEvents.Count + " events from file.");
            }
            else if (arg == "writexmlindex")
            {
                XMLIntIncrementer.IndexWriteXML();
                //await ReplyAsync("Loaded " + CalendarXMLManagement.arrEvents.Count + " events from file.");
            }
            else if (arg == "initxmlindex")
            {
                XMLIntIncrementer.InitializeIndexXML();
                //await ReplyAsync("Loaded " + CalendarXMLManagement.arrEvents.Count + " events from file.");
            }
        }

    }
}
