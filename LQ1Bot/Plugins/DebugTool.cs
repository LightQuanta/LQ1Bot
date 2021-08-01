using System.Collections.Generic;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {

    internal class DebugTool : PluginBase, IFriendMessage, IGroupMessage {
        public override int Priority => 2000;

        public override string PluginName => "DebugTool";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain).ToLower();
            if (text == "picinfo") {
                List<string> picresult = new List<string>();
                foreach (var msg in e.Chain) {
                    if (msg is ImageMessage tempmsg) {
                        picresult.Add(tempmsg.ImageId + " " + tempmsg.Url + "\n");
                    }
                }
                if (picresult.Count > 0) {
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(string.Join("\n", picresult)));
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain).ToLower();
            if (text == "picinfo") {
                List<string> picresult = new List<string>();
                foreach (var msg in e.Chain) {
                    if (msg is ImageMessage tempmsg) {
                        picresult.Add(tempmsg.ImageId + " " + tempmsg.Url + "\n");
                    }
                }
                if (picresult.Count > 0) {
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage(string.Join("\n", picresult)));
                }
                return true;
            }
            return false;
        }
    }
}