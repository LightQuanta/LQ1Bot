using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class SwitchControl : PluginBase, IGroupMessage {
        public override int Priority => 20000;

        public override string PluginName => "SwitchControl";
        public override bool CanDisable => false;

        private static HashSet<long> BlacklistGroups;

        public SwitchControl() {
            if (File.Exists("blacklist.json")) {
                try {
                    BlacklistGroups = JsonSerializer.Deserialize<HashSet<long>>(File.ReadAllText("blacklist.json"));
                } catch (Exception) { }
            }
            if (BlacklistGroups != null) {
                if (BlacklistGroups.Count > 0) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[黑名单群]");
                    foreach (var v in BlacklistGroups) {
                        Console.WriteLine(v);
                    }
                    Console.ResetColor();
                }
            }
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            string q = e.Sender.Group.Id;
            #region 启用/禁用控制
            if (text == "!enablebot") {
                if (PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>();
                    } else {
                        BlacklistGroups.Remove(long.Parse(q));
                    }
                    await MessageManager.SendGroupMessageAsync(q, "已在此群启用bot");
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }

            if (BlacklistGroups?.Contains(long.Parse(q)) ?? false) {
                return true;
            }

            if (text == "!banbot" || text == "!disablebot") {
                if (PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>() { long.Parse(q) };
                    } else {
                        BlacklistGroups.Add(long.Parse(q));
                    }
                    await MessageManager.SendGroupMessageAsync(q, "已在此群禁用bot");
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}