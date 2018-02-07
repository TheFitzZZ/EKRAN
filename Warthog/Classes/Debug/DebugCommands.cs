using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Discord.Audio;
using System.Diagnostics;

namespace Warthog.Classes
{
    public class DebugCommands : ModuleBase
    {
        private CommandService _service;

        public DebugCommands(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        private Process CreateStream(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            return Process.Start(ffmpeg);
        }

        private async Task SendAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            var ffmpeg = CreateStream(path);
            var output = ffmpeg.StandardOutput.BaseStream;
            var discord = client.CreatePCMStream(AudioApplication.Mixed);
            await output.CopyToAsync(discord);
            await discord.FlushAsync();
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

        ///
        /// Debug commands
        /// 
        [Command("Debug")]
        [Alias("debug")]
        [Summary("just for me")]
        public async Task Debug(string arg)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            if (arg == "readxmlcalendar")
            {
                CalendarXMLManagement.CalendarReadXML();
                await ReplyAsync("There are " + CalendarXMLManagement.arrEvents.Count + " events in the file.");
            }
            else if (arg == "writexml")
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


        //
        // Basic command tests
        //

        [Command("Ping")]
        [Alias("ping", "pong")]
        [Summary("Returns a pong")]
        public async Task Say()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            await ReplyAsync("Pong!");
        }

        [Command("Number")]
        [Alias("number")]
        [Summary("Returns you the number you chose")]
        public async Task Say2(int arg = 0)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            await ReplyAsync("Your number is: " + arg);
        }

        [Command("User")]
        [Alias("user")]
        [Summary("Returns you user you gave me")]
        public async Task Say3(IUser user)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            await ReplyAsync("Your user is: " + user.Mention);
        }

        [Command("Array")]
        [Alias("array")]
        [Summary("Returns the array you gave me")]
        public async Task Say4(params String[] array)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
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
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            await ReplyAsync("Wow remainder: " + text);
        }

        /*Like this you send it to a person of your choice*/
        [Command("Tell")]
        [Alias("tell")]
        [Summary("returns said sentence!")]
        public async Task Say6(IUser user)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            var channel = await user.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync("The message here");
        }

        /*But to send it to the person using the command, the following is easier:*/
        [Command("Tellme")]
        [Alias("tellme")]
        [Summary("returns said sentence!")]
        public async Task Say7()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            var channel = await Context.User.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync("The message here");
        }

        [Command("Tellchan")]
        [Alias("tellchan")]
        [Summary("returns said sentence!")]
        public async Task Say8()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            var channel = await Context.Guild.GetChannelAsync(386545965374373898) as SocketTextChannel;
            await channel.SendMessageAsync("Woops");
        }

        const ulong serverId = 386545514067263498; // the id of your server
        const ulong channelId = 386545965374373898; // the id of the channel

        [Command("Tellchanserv")]
        [Summary("returns said sentence!")]
        public async Task Say86()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            //var server = awai

            //await ((ISocketMessageChannel)client.GetChannel(channelID)).SendMessageAsync("");
            var channel = await Context.Guild.GetChannelAsync(386545965374373898) as SocketTextChannel;
            await channel.SendMessageAsync("Woops");
        }

        [Command("Sendmevirus")]
        [Alias("sendmevirus")]
        [Summary("returns viruz lulz!")]
        public async Task Say9()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            //await Context.Channel.SendFileAsync(file);
            await ReplyAsync("Fix me");
        }

        [Command("Embed")]
        [Alias("embed")]
        [Summary("returns embed yo")]
        public async Task Say10()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
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
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
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
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            ulong userID = 119019602574442497;

            IUser user = Warthog.Program.client.GetUser(userID);

            await ReplyAsync(user.Username + "#" + user.Discriminator);
        }

        [Command("testrole")]
        [Summary("tests if you have the role needed to exec that command")]
        public async Task Say21()
        {
            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Calendar Admin");
            if (user.Roles.Contains(role))
            {
                await ReplyAsync("You can do that!");
            }
            else
            {
                await ReplyAsync("You can't do that!");
            }
        }

        [Command("testdmrole")]
        [Summary("tests if you have the role needed to exec a command and are on the respective server")]
        public async Task Say221()
        {
            //vars to be filled later
            var guild = 386541111067263498;

            var user = Context.User as SocketGuildUser;
            var role = (user as IGuildUser).Guild.Roles.FirstOrDefault(x => x.Name == "Calendar Admin");
            if (user.Roles.Contains(role))
            {
                await ReplyAsync("You can do that!");
            }
            else
            {
                await ReplyAsync("You can't do that!");
            }
        }

        [Command("Deleteme")]
        [Alias("deleteme", "del")]
        [Summary("Deletes the message of the user")]
        public async Task Say12()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
            await DeleteCommandMessage();
            await ReplyAsync("Pong and gone!");
        }

        [Command("Clear")]
        public async Task Clear([Remainder] int Delete = 0)
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
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
                Console.WriteLine(Item.Author.Username);
                if (Item.Author.Username == "Fred-Lagz")
                {
                    await Item.DeleteAsync();
                }


            }
            //await Context.Channel.SendMessageAsync($"`{Context.User.Username} deleted {Amount} messages`");
        }

        [Command("purge", RunMode = RunMode.Async)]
        [Summary("Deletes the specified amount of messages.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task PurgeChat(uint amount)
        {
            var messages = await this.Context.Channel.GetMessagesAsync((int)amount + 1).Flatten();

            await this.Context.Channel.DeleteMessagesAsync(messages);
            const int delay = 5000;
            var m = await this.ReplyAsync($"Purge completed. _This message will be deleted in {delay / 1000} seconds._");
            await Task.Delay(delay);
            await m.DeleteAsync();
        }

        [Command("ServerInfo")]
        [Alias("sinfo", "servinfo")]
        [Remarks("Info about a server")]
        public async Task GuildInfo()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }
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


        [Command("update")]
        [Summary("Returns a update")]
        public async Task Htmltest()
        {
            if (119019602574442497 != Context.User.Id)
            {
                // Secrurity check
                await ReplyAsync("This is a debug command, you cannot use it!");
                return;
            }

            Warthog.Classes.DCSUpdateScraper.CheckDCSUpdate();

            await ReplyAsync("Pong!");
        }
    }
}
