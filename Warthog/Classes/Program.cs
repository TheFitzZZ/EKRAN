﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Warthog.Config;

namespace Warthog
{
    public class Program
    {
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        public static DiscordSocketClient client;
        private CommandHandler handler;

        public async Task Start()
        {
            EnsureBotConfigExists(); // Ensure that the bot configuration json file has been created.

            Warthog.Classes.CalendarXMLManagement.CalendarReadXML(); // Load current events

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose,
            });
            string derp = BotConfig.Load().Token;
            client.Log += Logger;
            await client.LoginAsync(TokenType.Bot, BotConfig.Load().Token);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = new CommandHandler(serviceProvider);
            await handler.ConfigureAsync();
            
            //Block this program untill it is closed
            await Task.Delay(-1);
        }

    

        private static Task Logger(LogMessage lmsg)
        {
            var cc = Console.ForegroundColor;
            switch (lmsg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now} [{lmsg.Severity,8}] {lmsg.Source}: {lmsg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
        
        public static void EnsureBotConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "configuration")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "configuration"));

            string loc = Path.Combine(AppContext.BaseDirectory, "configuration/config.json");

            if (!File.Exists(loc))                              // Check if the configuration file exists.
            {
                var config = new BotConfig();               // Create a new configuration object.
                Console.WriteLine("Please enter the following information to save into your configuration/config.json file");
                //Console.Write("Bot Token: ");
                //config.Token = Console.ReadLine();              // Read the bot token from console.
                config.Token = Secrets.DiscordBotToken;
                Console.Write("Bot Prefix: ");
                //config.Prefix = Console.ReadLine();              // Read the bot prefix from console.
                config.Prefix = "!";
                config.Save();                                  // Save the new configuration object to file.
            }
            Console.WriteLine("Configuration has been loaded");
        }
        
        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                 .AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }
    }
}
