using System.Collections.Generic;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class DebugTool : PluginBase, IFriendMessage, IGroupMessage {
        public override int Priority => 2000;

        public override string PluginName => "DebugTool";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            //if (text == "picinfo") {
            //    List<string> picresult = new List<string>();
            //    foreach (var msg in e.MessageChain) {
            //        if (msg is ImageMessage tempmsg) {
            //            picresult.Add(tempmsg.ImageId + " " + tempmsg.Url + "\n");
            //        }
            //    }
            //    if (picresult.Count > 0) {
            //        await MessageManager.SendFriendMessageAsync(e.Sender.Id, string.Join("\n", picresult));
            //    }
            //    return true;
            //}
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            //if (text == "picinfo") {
            //    List<string> picresult = new List<string>();
            //    foreach (var msg in e.MessageChain) {
            //        if (msg is ImageMessage tempmsg) {
            //            picresult.Add(tempmsg.ImageId + " " + tempmsg.Url + "\n");
            //        }
            //    }
            //    if (picresult.Count > 0) {
            //        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, string.Join("\n", picresult));
            //    }
            //    return true;
            //}
            return false;
        }
    }
}