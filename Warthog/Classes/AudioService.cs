//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Threading.Tasks;
//using Discord;
//using Discord.Audio;

//namespace Warthog.Classes
//{
    
//    public class AudioService
//    {
//        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
//        public static IAudioClient client;
//        public static List<string> songstore = new List<string>();

//        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
//        {
//            IAudioClient client;
//            if (ConnectedChannels.TryGetValue(guild.Id, out client))
//            {
//                return;
//            }
//            if (target.Guild.Id != guild.Id)
//            {
//                return;
//            }

//            var audioClient = await target.ConnectAsync();

//            if (ConnectedChannels.TryAdd(guild.Id, audioClient))
//            {
//                // If you add a method to log happenings from this service,
//                // you can uncomment these commented lines to make use of that.
//                //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
//                Console.WriteLine("Connected to voice channel.");
//            }
//        }

//        public async Task LeaveAudio(IGuild guild)
//        {
//            IAudioClient client;
//            if (ConnectedChannels.TryRemove(guild.Id, out client))
//            {
//                await client.StopAsync();
//                //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
//            }
//        }

//        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
//        {
//            // Your task: Get a full path to the file if the value of 'path' is only a filename.
//            if (!File.Exists(path))
//            {
//                await channel.SendMessageAsync("File does not exist.");
//                return;
//            }
//            IAudioClient client;
//            if (ConnectedChannels.TryGetValue(guild.Id, out client))
//            {
//                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
//                using (var output = CreateStream(path).StandardOutput.BaseStream)
//                using (var stream = client.CreatePCMStream(AudioApplication.Music))
//                {
//                    try { await output.CopyToAsync(stream); }
//                    finally { await stream.FlushAsync(); }
//                }

//                //var output = CreateStream(path).StandardOutput.BaseStream;
//                //var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024);
//                //await output.CopyToAsync(stream);
//                //await stream.FlushAsync().ConfigureAwait(false);
//            }
//        }

//        public async Task SendLinkAsync(IGuild guild, IMessageChannel channel, string path)
//        {
//            if (ConnectedChannels.TryGetValue(guild.Id, out client))
//            {
//                var output = CreateLinkStream(path).StandardOutput.BaseStream;
//                var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024); //, 128 * 1024
//                await output.CopyToAsync(stream);
//                await stream.FlushAsync().ConfigureAwait(false);
//            }
//        }


//        //public async Task SendYTAsync(IGuild guild, IMessageChannel channel, VideoInfo info)
//        //{
//        //    if (ConnectedChannels.TryGetValue(guild.Id, out client))
//        //    {
//        //        var AudioStream = GetAudioStream(info);

//        //        var Output = CreateYTStream(AudioStream.Url).StandardOutput.BaseStream;
//        //        var Stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024);
//        //        await Output.CopyToAsync(Stream);
//        //        await Stream.FlushAsync();
//        //    }
//        //}

//        public async Task StopAudio(IGuild guild)
//        {
//            await client.StopAsync();
//            return;
//        }


//        private Process CreateStream(string path)
//        {
//            return Process.Start(new ProcessStartInfo
//            {
//                FileName = "ffmpeg.exe",
//                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
//                UseShellExecute = false,
//                RedirectStandardOutput = true
//            });
//        }


//        public Process CreateLinkStream(string url)
//        {
//            Process currentsong = new Process();

//            currentsong.StartInfo = new ProcessStartInfo
//            {
//                FileName = "cmd.exe",
//                Arguments = $"/C youtube-dl.exe -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
//                UseShellExecute = false,
//                RedirectStandardOutput = true,
//                CreateNoWindow = true
//            };

//            currentsong.Start();
//            return currentsong;
//        }

//        //private AudioStreamInfo GetAudioStream(VideoInfo VideoInfo)
//        //{
//        //    foreach (var AudioStream in VideoInfo.AudioStreams)
//        //    {
//        //        if (AudioStream.AudioEncoding == AudioEncoding.Opus)
//        //        {
//        //            return AudioStream;
//        //        }
//        //    }

//        //    return null;
//        //}
//    }
//}
