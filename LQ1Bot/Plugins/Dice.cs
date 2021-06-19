using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class Dice : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9997;

        public override string PluginName => "Dice";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Id;
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
                await session.SendFriendMessageAsync(q, new PlainMessage($"{e.Sender.Name}投出了{res}！"));
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;
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
                await session.SendGroupMessageAsync(q, new PlainMessage($"{e.Sender.Name}投出了{res}！"));
                return true;
            }
            #endregion
            return false;
        }
    }
}
