using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {
    class Hitokoto : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9994;

        public override string PluginName => "Hitokoto";

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            #region 一言
            if (Utils.GetMessageText(e.Chain) == "一言") {
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
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"{msg}\t——{from}"));
                else
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"{msg}"));
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            #region 一言
            if (Utils.GetMessageText(e.Chain) == "一言") {
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
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage($"{msg}\t——{from}"));
                else
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage($"{msg}"));
                return true;
            }
            #endregion
            return false;
        }
    }
}
