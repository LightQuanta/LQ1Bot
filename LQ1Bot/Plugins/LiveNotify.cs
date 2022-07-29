using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class LiveNotify : PluginBase, IGroupMessage {
        public override int Priority => 9979;

        public override string PluginName => "LiveNotify";

        public override bool CanDisable => true;

        private readonly Dictionary<long, HashSet<long>> UidBind = new();
        private readonly Dictionary<long, long> LastLiveTime = new();

        private const int cooldown = 30;   //冷却时间(s)

        public LiveNotify() {
            try {
                string uid = File.ReadAllText("livecfg/uidbind.json");
                string livetime = File.ReadAllText("livecfg/lastlivetime.json");

                var temp = JsonSerializer.Deserialize<Dictionary<string, HashSet<long>>>(uid);
                foreach (var v in temp) {
                    UidBind.Add(long.Parse(v.Key), v.Value);
                }
                LastLiveTime = JsonSerializer.Deserialize<Dictionary<long, long>>(livetime);
            } catch (Exception ex) {
                File.WriteAllText("livecfg/uidbind.json", "{}");
                File.WriteAllText("livecfg/lastlivetime.json", "{}");
                Console.WriteLine(ex);
            }
            (new Thread(new ThreadStart(async () => {
                while (true) {
                    Thread.Sleep(1000 * cooldown);
                    await NotifyLive();
                }
            }))).Start();
        }
        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text.StartsWith("!subscribe ") && PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                if (long.TryParse(text[11..], out long mid)) {
                    if (UidBind.TryGetValue(mid, out var groups)) {
                        if (!groups.Contains(long.Parse(q))) {
                            groups.Add(long.Parse(q));
                            UidBind.Remove(mid);
                            UidBind.Add(mid, groups);

                            File.WriteAllText("livecfg/uidbind.json", JsonSerializer.Serialize(UidBind));
                            await MessageManager.SendGroupMessageAsync(q, "成功添加关注！");
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "已关注该主播！");
                        }
                    } else {
                        UidBind.Add(mid, new HashSet<long>() { long.Parse(q) });
                        File.WriteAllText("livecfg/uidbind.json", JsonSerializer.Serialize(UidBind));
                        await MessageManager.SendGroupMessageAsync(q, "成功添加关注！");
                    }
                }
                return true;
            }
            if (text.StartsWith("!unsubscribe ") && PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                if (long.TryParse(text[13..], out long mid)) {
                    if (UidBind.TryGetValue(mid, out var groups)) {
                        if (groups.Contains(long.Parse(q))) {
                            groups.Remove(long.Parse(q));
                            UidBind.Remove(mid);
                            if (groups.Count != 0) {
                                UidBind.Add(mid, groups);
                            }
                            File.WriteAllText("livecfg/uidbind.json", JsonSerializer.Serialize(UidBind));
                            await MessageManager.SendGroupMessageAsync(q, "成功取消关注！");
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "未关注该主播！");
                        }
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "未关注该主播！");
                    }
                }
                return true;
            }
            if (text == "!showsubscribe") {
                List<long> result = new();
                foreach (var v in UidBind) {
                    if (v.Value.Contains(long.Parse(q))) {
                        result.Add(v.Key);
                    }
                }
                if (result.Count > 0) {
                    await MessageManager.SendGroupMessageAsync(q, "本群关注的主播uid：" + string.Join(", ", result));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还未关注任何主播！");
                }
            }
            return false;
        }

        private async Task NotifyLive() {
            try {
                string json = await $"https://api.live.bilibili.com/room/v1/Room/get_status_info_by_uids?rnd={new Random().Next()}"
                    .PostJsonAsync(new { uids = UidBind.Keys })
                    .ReceiveString();
                JObject data = JObject.Parse(json);
                if (data["code"].ToObject<int>() == 0) {
                    JObject livedata = (JObject) data["data"];
                    foreach (var v in livedata.Properties()) {
                        long mid = int.Parse(v.Name);
                        long lastlive = v.Value["live_time"].ToObject<long>();      //上次直播时间
                        int livestatus = v.Value["live_status"].ToObject<int>();    //是否开播

                        string username = v.Value["uname"].ToString();
                        string roomid = v.Value["room_id"].ToString();
                        string title = v.Value["title"].ToString();
                        string cover = v.Value["cover_from_user"].ToString();

                        if (livestatus == 1 &&
                            lastlive > LastLiveTime.GetValueOrDefault(mid, 1) &&
                            UidBind.TryGetValue(mid, out var val)) {

                            LastLiveTime.Remove(mid);
                            LastLiveTime.Add(mid, lastlive);
                            File.WriteAllText("livecfg/lastlivetime.json", JsonSerializer.Serialize(LastLiveTime));


                            foreach (var group in val) {
                                try {
                                    await MessageManager.SendGroupMessageAsync(group.ToString(), $"{username}开播了！\n{title}\nhttps://live.bilibili.com/{roomid}".Append(new ImageMessage() { Url = cover }));
                                    Thread.Sleep(1000);
                                } catch { }
                            }
                        } else if (livestatus != 1 && LastLiveTime.GetValueOrDefault(mid, 0) > 1) {
                            var groups = UidBind.GetValueOrDefault(mid);
                            LastLiveTime.Remove(mid);
                            File.WriteAllText("livecfg/lastlivetime.json", JsonSerializer.Serialize(LastLiveTime));
                            foreach (var group in groups) {
                                try {
                                    await MessageManager.SendGroupMessageAsync(group.ToString(), $"{username}下播了！");
                                    Thread.Sleep(1000);
                                } catch { }
                            }
                        }
                    }
                } else {
                    Console.Error.WriteLine(JsonSerializer.Serialize(data));
                    Thread.Sleep(5 * 60 * 1000);
                }
            } catch (Exception ee) {
                Console.Error.WriteLine(ee.Message);
            }
        }
    }
}