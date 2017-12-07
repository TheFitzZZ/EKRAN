using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Warthog
{
    public class CommandHandler
    {
        private CommandService commands;
        private DiscordSocketClient bot;
        private IServiceProvider map;

        public CommandHandler(IServiceProvider provider)
        {
            map = provider;
            bot = map.GetService<DiscordSocketClient>();
            bot.UserJoined += AnnounceUserJoined;
            bot.UserLeft += AnnounceLeftUser;
            //Send user message to get handled
            bot.MessageReceived += HandleCommand;
            bot.Ready += ReadyEvent;
            commands = map.GetService<CommandService>();
        }

        //Set the "playing..." message
        public async Task ReadyEvent()
        {
            await bot.SetGameAsync("Watch EKRAN!");
        }

        
        public async Task AnnounceLeftUser(SocketGuildUser user)
        {
            var thumbnailurl = user.GetAvatarUrl();
            var embed = new EmbedBuilder();
            embed.WithColor(new Color(0, 71, 171));

            {
                var channel = bot.GetChannel(000000000000) as SocketTextChannel;
                {
                    embed.ThumbnailUrl = user.GetAvatarUrl();
                    embed.Title = $"**{user.Username} Left The Server:**";
                    embed.Description = $"**User:** {user.Mention}\n **Time**: {DateTime.UtcNow}: \n **Server:** {user.Guild.Name}";
                    await channel.SendMessageAsync("", false, embed);
                }
            }

        }

        public async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var channel = bot.GetChannel(000000000000) as SocketTextChannel;
            var embed = new EmbedBuilder();
            embed.ThumbnailUrl = user.GetAvatarUrl();
            embed.WithColor(new Color(0x13ef42));
            embed.Title = $"**{user.Username} Joined The Server:**";
            embed.Description = ($" **User:** {user.Mention} \n **Time**: {DateTime.UtcNow}: \n **Server:** {user.Guild.Name}");
            await channel.SendMessageAsync("", false, embed: embed);

        }
        public async Task ConfigureAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage pMsg)
        {
            //Don't handle the command if it is a system message
            var message = pMsg as SocketUserMessage;
            if (message == null)
                return;
            var context = new SocketCommandContext(bot, message);

            //Mark where the prefix ends and the command begins
            int argPos = 0;
            //Determine if the message has a valid prefix, adjust argPos
            if (message.HasStringPrefix("!", ref argPos))
            {
                if (message.Author.IsBot)
                    return;
                //Execute the command, store the result
                var result = await commands.ExecuteAsync(context, argPos, map);

                //If the command failed, notify the user
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.")

                    await message.Channel.SendMessageAsync($"**Error:** {result.ErrorReason}");
            }




        }
    }
}
