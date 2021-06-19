using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class Repeater : PluginBase, IGroupMessage {
        public override int Priority => 1000;
        public override bool CanDisable => true;

        public override string PluginName => "Repeater";

        private readonly Dictionary<long, (string Msg, int Count)> MsgRepeat = new Dictionary<long, (string, int)>();
        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;
            #region 复读机
            if (MsgRepeat.TryGetValue(q, out var valll)) {
                MsgRepeat.Remove(q);
                if (valll.Msg == text) {
                    MsgRepeat.Add(q, (text, valll.Count + 1));
                    if (valll.Count >= 2 && (new Random()).Next(1, 11) > (valll.Count + 3)) {
                        await session.SendGroupMessageAsync(q, new PlainMessage(text));
                        return true;
                    }
                }
            } else {
                MsgRepeat.Add(q, (text, 1));
            }
            #endregion
            return false;
        }
    }
}
