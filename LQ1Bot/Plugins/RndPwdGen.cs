using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class RndPwdGen : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9990;

        public override bool CanDisable => true;
        public override string PluginName => "RandomPasswordGenerator";

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain).ToLower();
            #region 强随机密码生成器
            if (Regex.IsMatch(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$")) {
                var m = Regex.Match(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$");
                string len = m.Groups["len"].Value;
                if (!int.TryParse(len, out int Length) || Length < 1 || Length > 64) {
                    Length = 16;
                }
                string pattern = m.Groups["pat"].Value;
                pattern = pattern.Length < 1 ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : pattern[1..];
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(Utils.GetStrongRandomString(Length, pattern)));
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain).ToLower();
            #region 强随机密码生成器
            if (Regex.IsMatch(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$")) {
                var m = Regex.Match(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$");
                string len = m.Groups["len"].Value;
                if (!int.TryParse(len, out int Length) || Length < 1 || Length > 64) {
                    Length = 16;
                }
                string pattern = m.Groups["pat"].Value;
                pattern = pattern.Length < 1 ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : pattern[1..];
                await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage(Utils.GetStrongRandomString(Length, pattern)));
                return true;
            }
            #endregion
            return false;
        }
    }
}
