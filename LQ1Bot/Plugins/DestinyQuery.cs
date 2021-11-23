using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class DestinyQuery : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9984;

        public override string PluginName => "DestinyQuery";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Id;
            if (text == "日报") {
                try {
                    WebClient wc = new WebClient();
                    string res = wc.DownloadString("http://www.tianque.top/d2api/today/");
                    JObject o = JObject.Parse(res);
                    await session.SendFriendMessageAsync(q, new PlainMessage(o["img_url"].ToString()));
                } catch (Exception) {
                    await session.SendFriendMessageAsync(q, new PlainMessage("获取日报出错"));
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;
            if (text == "日报") {
                try {
                    WebClient wc = new WebClient();
                    string res = wc.DownloadString("http://www.tianque.top/d2api/today/");
                    JObject o = JObject.Parse(res);
                    await session.SendGroupMessageAsync(q, new PlainMessage(o["img_url"].ToString()));
                } catch (Exception) {
                    await session.SendGroupMessageAsync(q, new PlainMessage("获取日报出错"));
                }
                return true;
            }
            return false;
        }
    }
}