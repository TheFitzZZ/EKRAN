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
            foreach(string word in array)
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

            await ReplyAsync("Swaaaag:",false,MyEmbedBuilder);
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

            string prefix = "!";
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
        public async Task Newevent(string sEventName, string sEventDateTime)
        {
            // try to adapt what the time is supposed to be
            DateTime dtEventDateTime = DateTime.Parse(sEventDateTime);


            await ReplyAsync("Active: " + true + "\n" +
                             "Eventname: " + sEventName + "\n" +
                             "Created Date: " + DateTime.Now + "\n" +
                             "Eventd Date: " + dtEventDateTime + "\n" +
                             "Attendees: " + Context.User.Username + "\n" +
                             "Event creator: " + Context.User.Username );

            CalendarEvent woop = new CalendarEvent();
            woop.Active = true;
            woop.Eventname = sEventName;
            woop.EventDate = dtEventDateTime;
            woop.EventCreator = Context.User;
            woop.CreatedDate = DateTime.Now;
            string[] temp = { "derp", "wupr" };
            woop.Attendees = temp;

            CalendarEventList.arrEvents.Add(woop);

        }
    }
}
