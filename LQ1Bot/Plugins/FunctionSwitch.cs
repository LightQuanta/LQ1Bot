using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class FunctionSwitch : PluginBase, IGroupMessage {
        public override int Priority => 15000;
        public override string PluginName => "FunctionSwitch";

        public override bool CanDisable => false;

        public static Dictionary<long, Dictionary<string, bool>> Config = new Dictionary<long, Dictionary<string, bool>>();

        //TODO: 修复新插件设置不会被保存到已存在的配置文件里的问题
        public FunctionSwitch() {
            if (!Directory.Exists("plugincfg")) {
                Directory.CreateDirectory("plugincfg");
            }
            foreach (var v in Directory.GetFiles("plugincfg")) {
                if (long.TryParse(v.Substring(0, v.IndexOf('.'))[10..], out long Group)) {
                    var d = new Dictionary<string, bool>();
                    string json = File.ReadAllText(v);
                    JObject o = JObject.Parse(json);
                    foreach (var oo in o) {
                        d.Add(oo.Key, bool.Parse(oo.Value.ToString()));
                    }
                    //foreach (var Plugin in PluginController.PluginInstance.OrderByDescending(o => o.Priority)) {
                    //    if (Plugin.CanDisable) {
                    //        if (!d.ContainsKey(Plugin.PluginName)) {
                    //            d.Add(Plugin.PluginName, true);
                    //            Console.WriteLine(Plugin.PluginName);
                    //            Console.WriteLine(d.ContainsKey(Plugin.PluginName));
                    //        }
                    //    }
                    //}
                    Config.Add(Group, d);
                }
            }
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            if (text == "!createconfig" && (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner || e.Sender.Id == 2224899528)) {
                JObject o = new JObject();
                Config.Remove(e.Sender.Group.Id);
                var d = new Dictionary<string, bool>();
                foreach (var Plugin in PluginController.PluginInstance) {
                    if (Plugin.CanDisable) {
                        o.Add(Plugin.PluginName, true);
                        d.Add(Plugin.PluginName, true);
                    }
                }
                Config.Add(e.Sender.Group.Id, d);
                File.WriteAllText("plugincfg/" + e.Sender.Group.Id + ".json", o.ToString());
                await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("已创建配置文件"));
                return true;
            }
            if (text.StartsWith("!query ")) {
                var Name = text[7..];
                if (Config.TryGetValue(e.Sender.Group.Id, out Dictionary<string, bool> cfg)) {
                    if (cfg.ContainsKey(Name)) {
                        if (cfg.GetValueOrDefault(Name)) {
                            await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("该功能已启用"));
                        } else {
                            await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("该功能已禁用"));
                        }
                    } else {
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("未发现该功能"));
                    }
                } else {
                    InitGroup(e.Sender.Group.Id);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("该功能已启用"));
                }
                return true;
            }
            if (text.StartsWith("!enable ") && (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner || e.Sender.Id == 2224899528)) {
                var Name = text[8..];
                if (Config.TryGetValue(e.Sender.Group.Id, out Dictionary<string, bool> cfg)) {
                    if (cfg.ContainsKey(Name)) {
                        cfg.Remove(Name);
                        cfg.Add(Name, true);
                        Save(e.Sender.Group.Id);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("已启用" + Name));
                    } else {
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("未发现该功能"));
                    }
                } else {
                    InitGroup(e.Sender.Group.Id);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("已启用" + Name));
                }
                return true;
            }
            if (text.StartsWith("!disable ") && (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner || e.Sender.Id == 2224899528)) {
                var Name = text[9..];
                if (Config.TryGetValue(e.Sender.Group.Id, out Dictionary<string, bool> cfg)) {
                    if (cfg.ContainsKey(Name)) {
                        cfg.Remove(Name);
                        cfg.Add(Name, false);
                        Save(e.Sender.Group.Id);
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("已禁用" + Name));
                    } else {
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("未发现该功能"));
                    }
                } else {
                    InitGroup(e.Sender.Group.Id);
                    cfg = Config.GetValueOrDefault(e.Sender.Group.Id);
                    cfg.Remove(Name);
                    cfg.Add(Name, false);
                    Save(e.Sender.Group.Id);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("已禁用" + Name));
                }
                return true;
            }
            if (text == "!plugins") {
                List<string> Enabled = new List<string>();
                List<string> Disabled = new List<string>();
                if (Config.TryGetValue(e.Sender.Group.Id, out Dictionary<string, bool> cfg)) {
                    foreach (var v in cfg) {
                        if (v.Value) {
                            Enabled.Add(v.Key);
                        } else {
                            Disabled.Add(v.Key);
                        }
                    }
                } else {
                    InitGroup(e.Sender.Group.Id);
                    cfg = Config.GetValueOrDefault(e.Sender.Group.Id);
                    foreach (var v in cfg) {
                        Enabled.Add(v.Key);
                    }
                }
                await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("所有可用插件\n已启用：" + (Enabled.Count == 0 ? "无" : string.Join(" ", Enabled)) + "\n已禁用：" + (Disabled.Count == 0 ? "无" : string.Join(" ", Disabled))));
                return true;
            }
            return false;
        }

        public static bool IsEnabled(long Group, string Name) {
            if (Config.TryGetValue(Group, out Dictionary<string, bool> cfg)) {
                if (cfg.TryGetValue(Name, out bool enabled)) {
                    return enabled;
                } else {
                    return false;
                }
            } else {
                return true;
            }
        }

        private void Save(long Group) {
            if (Config.TryGetValue(Group, out Dictionary<string, bool> cfg)) {
                JObject o = new JObject();
                foreach (var c in cfg) {
                    o.Add(c.Key, c.Value);
                }
                File.WriteAllText("plugincfg/" + Group + ".json", o.ToString());
            } else {
                InitGroup(Group);
                cfg = Config.GetValueOrDefault(Group);
                JObject o = new JObject();
                foreach (var c in cfg) {
                    o.Add(c.Key, c.Value);
                }
                File.WriteAllText("plugincfg/" + Group + ".json", o.ToString());
            }
        }

        private void InitGroup(long Group) {
            Config.Remove(Group);
            var d = new Dictionary<string, bool>();
            foreach (var Plugin in PluginController.PluginInstance) {
                if (Plugin.CanDisable) {
                    d.Add(Plugin.PluginName, true);
                }
            }
            Config.Add(Group, d);
        }
    }
}