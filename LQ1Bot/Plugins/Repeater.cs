using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class Repeater : PluginBase, IGroupMessage {
        public override int Priority => 1000;
        public override bool CanDisable => true;

        public override string PluginName => "Repeater";

        private readonly Dictionary<long, (string Msg, int Count)> MsgRepeat = new Dictionary<long, (string, int)>();

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            //if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
            //    return false;
            //}
            string text = Utils.GetMessageText(e.MessageChain);
            long q = long.Parse(e.Sender.Id);
            #region 复读机 
            if (MsgRepeat.TryGetValue(q, out var valll)) {
                MsgRepeat.Remove(q);
                if (valll.Msg == text) {
                    MsgRepeat.Add(q, (text, valll.Count + 1));
                    if (valll.Count >= 3 && (new Random()).Next(1, 11) > (valll.Count + 3)) {
                        MsgRepeat.Remove(q);
                        MsgRepeat.Add(q, (text, valll.Count + 114));
                        await MessageManager.SendGroupMessageAsync(q.ToString(), text);
                        return true;
                    }
                }
            } else {
                MsgRepeat.Add(q, (text, 1));
                return false;
            }
            #endregion
            return false;
        }
    }
}