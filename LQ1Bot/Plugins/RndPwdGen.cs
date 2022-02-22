using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class RndPwdGen : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9990;

        public override bool CanDisable => true;
        public override string PluginName => "RandomPasswordGenerator";

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            #region 强随机密码生成器
            if (Regex.IsMatch(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$")) {
                var m = Regex.Match(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$");
                string len = m.Groups["len"].Value;
                if (!int.TryParse(len, out int Length) || Length < 1 || Length > 64) {
                    Length = 16;
                }
                string pattern = m.Groups["pat"].Value;
                pattern = pattern.Length < 1 ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : pattern[1..];
                await MessageManager.SendFriendMessageAsync(e.Sender.Id, Utils.GetStrongRandomString(Length, pattern));
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            #region 强随机密码生成器
            if (Regex.IsMatch(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$")) {
                var m = Regex.Match(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$");
                string len = m.Groups["len"].Value;
                if (!int.TryParse(len, out int Length) || Length < 1 || Length > 64) {
                    Length = 16;
                }
                string pattern = m.Groups["pat"].Value;
                pattern = pattern.Length < 1 ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : pattern[1..];
                await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, Utils.GetStrongRandomString(Length, pattern));
                return true;
            }
            #endregion
            return false;
        }
    }
}