using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class Dice : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9997;

        public override string PluginName => "Dice";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            #region DICE
            if (Regex.IsMatch(text.ToLower().Trim(), @"^dice$|^dice \d+d\d+$")) {
                int count = 1;
                int max;
                if (text.ToLower().Trim() == "dice") {
                    max = 6;
                } else {
                    count = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[0]);
                    max = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[1]);
                    max = max < 1 ? 1 : max;
                    count = count < 1 ? 1 : count;
                    count = count > 10 ? 10 : count;
                }
                Random r = new Random();
                string res = "";
                for (int i = 0; i < count; i++) {
                    if (i == 0) {
                        res += r.Next(1, max + 1).ToString();
                    } else {
                        res += "," + r.Next(1, max + 1).ToString();
                    }
                }
                await MessageManager.SendFriendMessageAsync(q, $"{e.Sender.NickName}投出了{res}！");
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            #region DICE
            if (Regex.IsMatch(text.ToLower().Trim(), @"^dice$|^dice \d+d\d+$")) {
                int count = 1;
                int max;
                if (text.ToLower().Trim() == "dice") {
                    max = 6;
                } else {
                    count = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[0]);
                    max = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[1]);
                    max = max < 1 ? 1 : max;
                    count = count < 1 ? 1 : count;
                    count = count > 10 ? 10 : count;
                }
                Random r = new Random();
                string res = "";
                for (int i = 0; i < count; i++) {
                    if (i == 0) {
                        res += r.Next(1, max + 1).ToString();
                    } else {
                        res += "," + r.Next(1, max + 1).ToString();
                    }
                }
                await MessageManager.SendGroupMessageAsync(q, $"{e.Sender.Name}投出了{res}！");
                return true;
            }
            #endregion
            return false;
        }
    }
}