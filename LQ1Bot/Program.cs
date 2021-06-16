using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LQ1Bot.Secret;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Newtonsoft.Json.Linq;

namespace LQ1Bot {
    class Program {

        static async Task Main(string[] args) {
            LQ1BotConfig Secret = new LQ1BotConfig();
            Secret.Init();

            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(Secret.MiraiIp, Secret.MiraiPort, Secret.MiraiSecret);
            
            await using MiraiHttpSession session = new MiraiHttpSession();
            
            //命令行参数发送群消息
            if (args.Length > 0) {
                switch (args[0]) {
                    case "-sg":
                        if (args.Length > 2) {
                            await session.ConnectAsync(options, Secret.QQ);
                            long group = long.Parse(args[1]);
                            string msg = args[2];
                            Console.WriteLine($"群号：{group}\t消息内容：{msg}");
                            await session.SendGroupMessageAsync(group, new PlainMessage(msg));
                            Console.WriteLine("发送成功");
                            Environment.Exit(0);
                        }
                        break;
                }
            }
            LQ1Bot plugin = new LQ1Bot(Secret);
            session.AddPlugin(plugin);

            //特殊定制功能就不公开了
            SpecialFunction sf = new SpecialFunction(Secret);
            session.AddPlugin(sf);

            await session.ConnectAsync(options, Secret.QQ); 
            Console.WriteLine("已成功连接至Mirai");

            //控制台指令
            while (true) {
                string temp = await Console.In.ReadLineAsync();
                if (temp == "exit") {
                    return;
                }
                if (Regex.IsMatch(temp, @"^send \d+ .+$")) {
                    long q = long.Parse(temp.Split(' ')[1]);
                    string msg = temp.Split(' ')[2];
                    await session.SendGroupMessageAsync(q, new PlainMessage(msg));
                }
                if (Regex.IsMatch(temp, @"^spam \d+ \d+ .+$")) {
                    long q = long.Parse(temp.Split(' ')[1]);
                    string msg = temp.Split(' ')[3];
                    int count = int.Parse(temp.Split(' ')[2]);
                    for (int i = 0; i < count; i++) {
                        await session.SendGroupMessageAsync(q, new PlainMessage(msg));
                    }
                }
            }
        }
    }
}
