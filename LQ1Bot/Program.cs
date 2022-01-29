using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LQ1Bot.Plugins;
using LQ1Bot.Secret;
using Mirai_CSharp;
using Mirai_CSharp.Models;

namespace LQ1Bot {

    internal class Program {
        public static LQ1BotConfig Secret;

        private static async Task Main(string[] args) {
            Secret = new LQ1BotConfig();
            Secret.Init();

            MiraiHttpSessionOptions Options = new MiraiHttpSessionOptions(Secret.MiraiIp, Secret.MiraiPort, Secret.MiraiSecret);

            await using MiraiHttpSession Session = new MiraiHttpSession();

            //命令行参数发送群消息
            if (args.Length > 0) {
                switch (args[0]) {
                    case "-sg":
                        if (args.Length > 2) {
                            await Session.ConnectAsync(Options, Secret.QQ);
                            long group = long.Parse(args[1]);
                            string msg = args[2];
                            Console.WriteLine($"群号：{group}\t消息内容：{msg}");
                            await Session.SendGroupMessageAsync(group, new PlainMessage(msg));
                            Console.WriteLine("发送成功");
                            Environment.Exit(0);
                        }
                        break;
                    case "-sf":
                        if (args.Length > 2) {
                            await Session.ConnectAsync(Options, Secret.QQ);
                            long qq = long.Parse(args[1]);
                            string msg = args[2];
                            Console.WriteLine($"好友QQ号：{qq}\t消息内容：{msg}");
                            await Session.SendFriendMessageAsync(qq, new PlainMessage(msg));
                            Console.WriteLine("发送成功");
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            
            //特殊定制功能就不公开了
            SpecialFunction sf = new SpecialFunction(Secret);
            Session.AddPlugin(sf);

            PluginController Controller = new PluginController(Session);
            Controller.LoadPlugins();

            LQ1Bot plugin = new LQ1Bot(Secret);
            Session.AddPlugin(plugin);

            await Session.ConnectAsync(Options, Secret.QQ);
            Console.WriteLine("已成功连接至Mirai");

            //控制台指令
            while (true) {
                string temp = await Console.In.ReadLineAsync();
                if (temp == "exit") {
                    return;
                }
                if (temp == "plugins") {
                    Controller.ShowPlugins();
                }
                if (Regex.IsMatch(temp, @"^send \d+ .+$")) {
                    long q = long.Parse(temp.Split(' ')[1]);
                    string msg = temp.Split(' ')[2];
                    await Session.SendGroupMessageAsync(q, new PlainMessage(msg));
                }
                if (Regex.IsMatch(temp, @"^spam \d+ \d+ .+$")) {
                    long q = long.Parse(temp.Split(' ')[1]);
                    string msg = temp.Split(' ')[3];
                    int count = int.Parse(temp.Split(' ')[2]);
                    for (int i = 0; i < count; i++) {
                        await Session.SendGroupMessageAsync(q, new PlainMessage(msg));
                    }
                }
            }
        }
    }
}