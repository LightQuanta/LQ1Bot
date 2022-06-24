using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class PermissionMgr : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 50000;

        public override string PluginName => "PermissionMgr";

        public override bool CanDisable => false;

        private static readonly Dictionary<string, HashSet<string>> Permissions = new Dictionary<string, HashSet<string>>();
        public PermissionMgr() {
            if (File.Exists("permissions.json")) {
                try {
                    string json = File.ReadAllText("permissions.json");
                    JObject o = JObject.Parse(json);
                    foreach (var v in o) {
                        Permissions.Add(v.Key, v.Value.ToObject<HashSet<string>>());
                    }
                    Console.WriteLine(JObject.FromObject(Permissions).ToString());
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text == "whoami") {
                await MessageManager.SendGroupMessageAsync(q, "permissions: " + string.Join(",", Permissions.GetValueOrDefault(e.Sender.Id)));
            }
            if (Permissions.GetValueOrDefault(e.Sender.Id)?.Contains("admin") ?? false) {
                if (text.StartsWith("!ban ")) {
                    string id = text[5..];
                    if (long.TryParse(id, out long id2ban)) {
                        if (Permissions.TryGetValue(id2ban.ToString(), out HashSet<String> permissions)) {
                            permissions.Add("banned");
                        } else {
                            Permissions.Add(id2ban.ToString(), new HashSet<string>() { "banned" });
                        }
                        Save();
                        await MessageManager.SendGroupMessageAsync(q, "已屏蔽" + id2ban);
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "输入格式错误");
                    }
                }
                if (text.StartsWith("!unban ")) {
                    string id = text[7..];
                    if (long.TryParse(id, out long id2ban)) {
                        Permissions.Remove(id2ban.ToString());
                        Save();
                        await MessageManager.SendGroupMessageAsync(q, "已解封" + id2ban);
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "输入格式错误");
                    }
                }
                return false;
            }
            return Permissions.GetValueOrDefault(e.Sender.Id)?.Contains("banned") ?? false;
        }

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            if (text == "whoami") {
                await MessageManager.SendFriendMessageAsync(q, "permissions: " + string.Join(",", Permissions.GetValueOrDefault(e.Sender.Id)));
            }
            if (Permissions.GetValueOrDefault(e.Sender.Id)?.Contains("admin") ?? false) {
                if (text.StartsWith("!ban ")) {
                    string id = text[5..];
                    if (long.TryParse(id, out long id2ban)) {
                        if (Permissions.TryGetValue(id2ban.ToString(), out HashSet<String> permissions)) {
                            permissions.Add("banned");
                        } else {
                            Permissions.Add(id2ban.ToString(), new HashSet<string>() { "banned" });
                        }
                        Save();
                        await MessageManager.SendFriendMessageAsync(q, "已屏蔽" + id2ban);
                    } else {
                        await MessageManager.SendFriendMessageAsync(q, "输入格式错误");
                    }
                }
                if (text.StartsWith("!unban ")) {
                    string id = text[7..];
                    if (long.TryParse(id, out long id2ban)) {
                        Permissions.Remove(id2ban.ToString());
                        Save();
                        await MessageManager.SendFriendMessageAsync(q, "已解封" + id2ban);
                    } else {
                        await MessageManager.SendFriendMessageAsync(q, "输入格式错误");
                    }
                }
                return false;
            }
            return false;
            //return Permissions.GetValueOrDefault(e.Sender.Id)?.Contains("banned") ?? false;
        }


        private void Save() {
            File.WriteAllText("permissions.json", JObject.FromObject(Permissions).ToString());
        }
        public static bool HasPermissionOrAdmin(string id, string permission) {
            return HasPermission(id, permission) || IsBotAdmin(id);
        }
        public static bool HasPermission(string id, string permission) {
            if (Permissions.TryGetValue(id, out HashSet<string> permissions)) {
                return permissions.Contains(permission);
            }
            return false;
        }
        public static HashSet<string> GetPermissions(string id) {
            if (Permissions.TryGetValue(id, out HashSet<string> permissions)) {
                return permissions;
            } else {
                var permission = new HashSet<string>();
                Permissions.Add(id, permission);
                return permission;
            }
        }
        public static bool IsGroupOrBotAdmin(Mirai.Net.Data.Shared.Member q) {
            return q.Permission != Mirai.Net.Data.Shared.Permissions.Member || (Permissions.GetValueOrDefault(q.Id)?.Contains("admin") ?? false);
        }
        public static bool IsBotAdmin(string q) {
            return Permissions.GetValueOrDefault(q)?.Contains("admin") ?? false;
        }
    }
}