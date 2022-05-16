using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class LivePrompt : PluginBase, IGroupMessage {
        public override int Priority => 9981;

        public override string PluginName => "LivePrompt";

        public override bool CanDisable => true;


        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;

            if (text == "催播") {
                Init(q);
                try {
                    Init(q);
                    string temp = File.ReadAllText($"livepromptcfg/{q}.txt");

                    string uid = temp.Split("|")[0];
                    string content = temp.Split("|")[1];

                    var Now = DateTime.Now;

                    WebClient client = new WebClient();
                    var obj = JObject.Parse(client.DownloadString("https://api.vtbs.moe/v1/detail/" + uid));

                    if (obj["liveStatus"].ToObject<bool>() == true) {
                        await MessageManager.SendGroupMessageAsync(q, $"{obj["uname"]}正在直播！\nhttps://live.bilibili.com/{obj["roomid"]}");
                    } else {
                        var LastLiveTime = new DateTime(1970, 1, 1).AddTicks(obj["lastLive"]["time"].ToObject<long>() * 10000);

                        var Diff = Now - LastLiveTime;
                        string DiffStr = $"{Diff.Days}天{Diff.Hours}小时{Diff.Minutes}分{Diff.Seconds}秒{Diff.Ticks % 10000000 / 10000}毫秒{Diff.Ticks % 10000 / 10}微秒{Diff.Ticks % 10}00纳秒";
                        await MessageManager.SendGroupMessageAsync(q, content.Replace("{time}", DiffStr).Replace("{name}", obj["uname"].ToString()));
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    await MessageManager.SendGroupMessageAsync(q, "查询开播状态出错");
                }
            }
            return false;
        }

        private void Init(string q) {
            if (!File.Exists($"livepromptcfg/{q}.txt")) {
                if (!Directory.Exists("livepromptcfg")) {
                    Directory.CreateDirectory("livepromptcfg");
                }
                File.Create($"livepromptcfg/{q}.txt");
            }
        }
    }
}