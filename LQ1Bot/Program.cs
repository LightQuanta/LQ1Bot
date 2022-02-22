using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using LQ1Bot.Plugins;
using LQ1Bot.Secret;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot {

    internal class Program {
        public static LQ1BotConfig Secret;

        private static async Task Main(string[] args) {
            Secret = new LQ1BotConfig();
            Secret.Init();

            using var bot = new MiraiBot {
                Address = Secret.MiraiIp + ":" + Secret.MiraiPort,
                QQ = Secret.QQ.ToString(),
                VerifyKey = Secret.MiraiSecret
            };

            await bot.LaunchAsync();
            Console.WriteLine("已成功连接至Mirai");

            //命令行参数发送群消息
            if (args.Length > 0) {
                switch (args[0]) {
                    case "-sg":
                        if (args.Length > 2) {
                            long group = long.Parse(args[1]);
                            string msg = args[2];
                            Console.WriteLine($"群号：{group}\t消息内容：{msg}");
                            await MessageManager.SendGroupMessageAsync(group.ToString(), msg);
                            Console.WriteLine("发送成功");
                            Environment.Exit(0);
                        }
                        break;
                    case "-sf":
                        if (args.Length > 2) {
                            long qq = long.Parse(args[1]);
                            string msg = args[2];
                            Console.WriteLine($"好友QQ号：{qq}\t消息内容：{msg}");
                            await MessageManager.SendFriendMessageAsync(qq.ToString(), msg);
                            Console.WriteLine("发送成功");
                            Environment.Exit(0);
                        }
                        break;
                }
            }


            //特殊定制功能就不公开了
            SpecialFunction sf = new SpecialFunction(Secret, bot);

            PluginController Controller = new PluginController();
            Controller.LoadPlugins();

            LQ1Bot plugin = new LQ1Bot(Secret, bot);

            bot.MessageReceived
                .OfType<GroupMessageReceiver>()
                .Subscribe(async receiver => {
                    if (!await sf.GroupMessage(receiver)) {
                        bool result = false;
                        foreach (var v in PluginController.PluginInstance.OrderByDescending(o => o.Priority)) {
                            if (v is IGroupMessage) {
                                if (!v.CanDisable || FunctionSwitch.IsEnabled(long.Parse(receiver.Sender.Group.Id), v.PluginName)) {
                                    try {
                                        result = await ((IGroupMessage) v).GroupMessage(receiver);
                                        if (result) {
                                            break;
                                        }
                                    } catch (Exception e) {
                                        Console.Error.WriteLine(e);
                                    }
                                }
                            }
                        }
                    }
                });

            bot.MessageReceived
                .OfType<FriendMessageReceiver>()
                .Subscribe(async receiver => {
                    if (!await sf.FriendMessage(receiver)) {
                        bool result = false;
                        foreach (var v in PluginController.PluginInstance.OrderByDescending(o => o.Priority)) {
                            if (v is IFriendMessage) {
                                try {
                                    result = await ((IFriendMessage) v).FriendMessage(receiver);
                                    if (result) {
                                        break;
                                    }
                                } catch (Exception e) {
                                    Console.Error.WriteLine(e);
                                }
                            }
                        }
                    }
                });


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
                    await MessageManager.SendGroupMessageAsync(q.ToString(), msg);
                }
                if (Regex.IsMatch(temp, @"^spam \d+ \d+ .+$")) {
                    long q = long.Parse(temp.Split(' ')[1]);
                    string msg = temp.Split(' ')[3];
                    int count = int.Parse(temp.Split(' ')[2]);
                    for (int i = 0; i < count; i++) {
                        await MessageManager.SendGroupMessageAsync(q.ToString(), msg);
                    }
                }
            }
        }
    }
}