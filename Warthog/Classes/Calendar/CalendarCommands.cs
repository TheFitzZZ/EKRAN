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
    class CalendarCommands : ModuleBase
    {
        private CommandService _service;

        public CalendarCommands(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        [Command("woop")]
        [Alias("woop", "woop")]
        [Summary("Creates a new event")]
        public async Task Say()
        {
            await ReplyAsync("Pong!");
        }
    }
}
