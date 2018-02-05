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
        // Random commands
        //
        [Command("cleanfred", RunMode = RunMode.Async)]
        [Summary("Deletes the specified amount of messages.")]
        //[RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task Cleanfred(uint amount = 100)
        {
            var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();
            Console.WriteLine(DateTime.UtcNow + " " + Context.User.Username + "ran the purge command for FredBoat");
            foreach (var Item in messages)
            {
                
                //Console.WriteLine(Item.Author.Id + " " + Item.Author.Username + " " + Item.Content);
                if (Item.Author.Id == 184405311681986560)
                {
                    //Console.WriteLine("Trying to delete message...");
                    await Item.DeleteAsync();
                }


            }
            //await this.Context.Channel.DeleteMessagesAsync(messages);
            const int delay = 5000;
            var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
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

            string helptext = "```These are the commands you can use: \n\n" +
                                "Show event calendar \n" +

                "Command: !calendar (!cal) \n" +
                "Parameters: none \n" +
                "Examples: !calendar \n" +
                "Description: Gives a compact list of upcoming events for your server. \n" +
                " \n" +
                "Show event details \n" +
                "Command: !event (!cev) \n" +
                "Parameters: eventID \n" +
                "Examples: !event 5 \n" +
                "Description: Gives all details about the event given. \n" +
                " \n" +
                "Attend event \n" +
                "Command: !attend  \n" +
                "Parameters: eventID  \n" +
                "Examples: !aev 3 \n" +
                "Description: Put's you on the list of attendees for the event. \n" +
                " \n" +
                "Skip event (aka remove your attendence) \n" +
                "Command: !skip  \n" +
                "Parameters: eventID  \n" +
                "Examples: !sev 3 \n" +
                "Description: Removes you from the given event. \n" +
                " \n" +
                "Show event list \n" +
                "Command: !getevents (or !gev) \n" +
                "Parameters: none  \n" +
                "Examples: !getevents  \n" +
                "Description: Gives a list of all upcoming events for your server. \n" +
                "```";

            var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync(helptext);

            helptext = "```Commands for event creators \n" +
                "New event \n" +
                "Command: !newevent (or !nev)  \n" +
                "Parameters: \"Name\" \"Date Time\"  \n" +
                "Examples: !newevent \"My Event Name\" \"2018 - 12 - 24 13:37\" \n" +
                    "Description: Creates a new event with the given name and date+time. If the name has more than one word, it needs to be put in quotation marks. Date and time is entered in the format \"YYYY - MM - DD HH: MM\" in the 24h format with quotation marks. There is no timezone conversion (yet), so use UTC or the favorite TZ of your server. \n" +
                         " \n" +
                         "After you create an event, the bot will tell you the assigned ID and send you a DM explaining how you can edit & enhance it with more information. \n" +
                         " \n" +
                         "Edit event \n" +
                         "Command: !editevent (or !eev) \n" +
                         "Parameters: several :-) \n" +
                         " \n" +
                         "Add a max attendee limit: !editev eventid maxattendees 15 \n" +
                         "Add a URL to a briefing: !editev eventid briefingurl http://someurl.com/briefing \n" +
                         "Add a URL to a Discord invite: !editev eventid discordurl http://disc0rd.gg/yourinviteid \n" +
                         "Add a short description to the event: !editev eventid description \"Description goes here\" \n" +
                         "Toggle public flag of the event: !editev eventid public \n" +
                "Edit the event name: !editev eventid name \"My Eventname\" \n" +
                "Edit the event date: !editev eventid date \"2018 - 12 - 24 13:37\" \n" +
                    "To state the obvious: replace eventid with the actual id! \n" +
                    " \n" +
                    "Examples: See list above.  \n" +
                    "Description: Allows you to edit several information on the event with the given ID. Only the creator of the event can edit it. \n" +
                    " \n" +
                    "Delete event \n" +
                    "Command: !deleteevent (or !dev)  \n" +
                    "Parameters: eventID  \n" +
                    "Examples: !dev 3  \n" +
                    "Description: Deletes the event with the given ID. You can only delete an event that you created. ```";

            //var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync(helptext);
        }


        [Command("help3")]
        [Remarks("Shows a list of all available commands per module.")]
        public async Task HelpAsync3()
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
        [Command("help2")]
        [Remarks("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync3(string command)
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


    }
}
