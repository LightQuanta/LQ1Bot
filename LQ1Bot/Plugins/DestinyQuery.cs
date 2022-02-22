using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class DestinyQuery : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9984;

        public override string PluginName => "DestinyQuery";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            if (text == "日报") {
                try {
                    WebClient wc = new WebClient();
                    string res = wc.DownloadString("http://www.tianque.top/d2api/today/");
                    JObject o = JObject.Parse(res);
                    ImageMessage image = new ImageMessage();
                    image.Url = o["img_url"].ToString();
                    await MessageManager.SendFriendMessageAsync(q, image);
                } catch (Exception) {
                    await MessageManager.SendFriendMessageAsync(q, "获取日报出错");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text == "日报") {
                try {
                    WebClient wc = new WebClient();
                    string res = wc.DownloadString("http://www.tianque.top/d2api/today/");
                    JObject o = JObject.Parse(res);
                    ImageMessage image = new ImageMessage();
                    image.Url = o["img_url"].ToString();
                    await MessageManager.SendGroupMessageAsync(q, image);
                } catch (Exception) {
                    await MessageManager.SendGroupMessageAsync(q, "获取日报出错");
                }
                return true;
            }
            return false;
        }
    }
}