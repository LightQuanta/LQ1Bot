using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class Hitokoto : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9994;

        public override string PluginName => "Hitokoto";
        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            #region 一言
            if (Utils.GetMessageText(e.MessageChain) == "一言") {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("一言");
                Console.ResetColor();
                string result = "";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(@"https://v1.hitokoto.cn/");
                req.Timeout = 3000;
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                } catch (Exception) { }
                JObject jj = JObject.Parse(result);
                string msg = jj["hitokoto"]?.ToString();
                string from = jj["from_who"]?.ToString();
                if (from != null && from != "" && from != "null")
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id,$"{msg}\t——{from}");
                else
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, $"{msg}");
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            #region 一言
            if (Utils.GetMessageText(e.MessageChain) == "一言") {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("一言");
                Console.ResetColor();
                string result = "";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(@"https://v1.hitokoto.cn/");
                req.Timeout = 3000;
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                } catch (Exception) { }
                JObject jj = JObject.Parse(result);
                string msg = jj["hitokoto"]?.ToString();
                string from = jj["from_who"]?.ToString();
                if (from != null && from != "" && from != "null")
                    await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, $"{msg}\t——{from}");
                else
                    await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, $"{msg}");
                return true;
            }
            #endregion
            return false;
        }
    }
}