using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

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

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain).ToLower();
            long q = e.Sender.Group.Id;
            #region 启用/禁用控制
            if (text == "!enablebot") {
                if (e.Sender.Permission == GroupPermission.Administrator ||
                    e.Sender.Permission == GroupPermission.Owner ||
                    e.Sender.Id == 2224899528) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>();
                    } else {
                        BlacklistGroups.Remove(q);
                    }
                    await session.SendGroupMessageAsync(q, new PlainMessage("已在此群启用bot"));
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }

            if (BlacklistGroups?.Contains(q) ?? false) {
                return true;
            }

            if (text == "!banbot" || text == "!disablebot") {
                if (e.Sender.Permission == GroupPermission.Administrator ||
                    e.Sender.Permission == GroupPermission.Owner ||
                    e.Sender.Id == 2224899528) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>() { q };
                    } else {
                        BlacklistGroups.Add(q);
                    }
                    await session.SendGroupMessageAsync(q, new PlainMessage("已在此群禁用bot"));
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}