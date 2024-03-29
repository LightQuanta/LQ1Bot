﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl.Http;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Mirai.Net.Utils.Scaffolds;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class DDTool : PluginBase, IFriendMessage, IGroupMessage {
        public override int Priority => 9995;

        public override string PluginName => "DDTool";

        public override bool CanDisable => true;

        private Dictionary<string, DateTime> Cooldown = new Dictionary<string, DateTime>();
        private Dictionary<string, string> Override = new Dictionary<string, string>();

        private string Api = "https://cfapi.vtbs.moe";
        public DDTool() {
            try {
                Override = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("ddtoolcfg/config.json"));
            } catch (Exception ex) {
                Console.WriteLine(ex);
                if (File.Exists("ddtoolcfg/config.json")) {
                    File.Move("ddtoolcfg/config.json", "ddtoolcfg/config.json.bak");
                }
                File.WriteAllText("ddtoolcfg/config.json", JsonSerializer.Serialize(Override));
            }
        }
        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            string q = e.Sender.Id;
            switch (text) {
                #region 今天看谁
                case "今天看谁":
                case "今天d谁":
                    try {
                        string res = await $"{Api}/v1/vtbs".GetStringAsync();
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        JObject RndVtbId = (JObject) Vtbs[index];

                        string userId = RndVtbId["mid"].ToString();

                        var RndVtb = JObject.Parse(await ($"{Api}/v1/detail/" + userId).GetStringAsync());

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        bool online = RndVtb["online"].ToObject<bool>();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();
                        if (online == false) {
                            await MessageManager.SendFriendMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}
直播间地址：https://live.bilibili.com/{roomId}").Build());
                        } else {
                            await MessageManager.SendFriendMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}

当前正在直播！
直播间标题：{title}
直播间地址：https://live.bilibili.com/{roomId}").Build());
                        }
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await MessageManager.SendFriendMessageAsync(q, "获取vtb信息出错");
                    }
                    return true;
                #endregion
                #region 现在看谁
                case "现在看谁":
                case "现在d谁":
                    try {
                        string res = await $"{Api}/v1/living".GetStringAsync();
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        long RndVtbId = Vtbs[index].ToObject<long>();

                        var RoomInfo = JObject.Parse(await ($"{Api}/v1/room/" + RndVtbId).GetStringAsync());
                        string uid = RoomInfo["uid"].ToString();
                        string popularity = RoomInfo["popularity"].ToString();

                        var RndVtb = JObject.Parse(await ($"{Api}/v1/detail/" + uid).GetStringAsync());

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        string online = RndVtb["online"].ToString();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();

                        await MessageManager.SendFriendMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"{title}
https://live.bilibili.com/{roomId}
人气：{popularity}

vtb信息
名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{uid}").Build());
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await MessageManager.SendFriendMessageAsync(q, "获取vtb信息出错");
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain).ToLower();
            string q = e.Sender.Group.Id;
            switch (text) {
                #region 今天看谁
                case "今天看谁":
                case "今天d谁":
                    if (Cooldown.ContainsKey(e.Sender.Id)) {
                        DateTime d = Cooldown.GetValueOrDefault(e.Sender.Id);
                        if ((DateTime.Now - d).TotalSeconds < 300) {
                            return true;
                        }
                    }

                    try {
                        string userId;
                        if (Override.TryGetValue(q, out string mid)) {
                            userId = mid;
                            var result = JObject.Parse(await ("http://api.bilibili.com/x/web-interface/card?mid=" + mid).GetStringAsync());
                            if (result["code"].ToString() == "0") {
                                string name = result["data"]["card"]["name"].ToString();
                                string sign = result["data"]["card"]["sign"].ToString();
                                string followers = result["data"]["card"]["fans"].ToString();

                                await MessageManager.SendGroupMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(result["data"]["card"]["face"].ToString() + "@150h")
                                .Plain($@"名称：{name}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}").Build());
                            } else {
                                await MessageManager.SendGroupMessageAsync(q, "获取vtb信息出错");
                            }
                        } else {
                            string res = await ($"{Api}/v1/vtbs").GetStringAsync();
                            JArray Vtbs = JArray.Parse(res);

                            int index = (new Random()).Next(Vtbs.Count);
                            JObject RndVtbId = (JObject) Vtbs[index];
                            userId = RndVtbId["mid"].ToString();
                            var RndVtb = JObject.Parse(await ($"{Api}/v1/detail/" + userId).GetStringAsync());

                            string userName = RndVtb["uname"].ToString();
                            string roomId = RndVtb["roomid"].ToString();
                            string faceUrl = RndVtb["face"].ToString() + "@150h";
                            string followers = RndVtb["follower"].ToString();
                            bool online = RndVtb["online"].ToObject<bool>();
                            string sign = RndVtb["sign"].ToString();
                            string title = RndVtb["title"].ToString();
                            string id = "";

                            if (online == false) {
                                id = await MessageManager.SendGroupMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}
直播间地址：https://live.bilibili.com/{roomId}").Build());
                            } else {
                                id = await MessageManager.SendGroupMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}

当前正在直播！
直播间标题：{title}
直播间地址：https://live.bilibili.com/{roomId}").Build());
                            }
                            Cooldown.Remove(e.Sender.Id);
                            Cooldown.Add(e.Sender.Id, DateTime.Now);
                        }
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await MessageManager.SendGroupMessageAsync(q, "获取vtb信息出错");
                    }
                    return true;
                #endregion
                #region 现在看谁
                case "现在看谁":
                case "现在d谁":
                    if (Cooldown.ContainsKey(e.Sender.Id)) {
                        DateTime d = Cooldown.GetValueOrDefault(e.Sender.Id);
                        if ((DateTime.Now - d).TotalSeconds < 300) {
                            return true;
                        }
                    }
                    try {
                        if (Override.TryGetValue(q, out string mid)) {
                            var result = JObject.Parse(await ("http://api.bilibili.com/x/web-interface/card?mid=" + mid).GetStringAsync());
                            if (result["code"].ToString() == "0") {
                                string name = result["data"]["card"]["name"].ToString();
                                string sign = result["data"]["card"]["sign"].ToString();
                                string followers = result["data"]["card"]["fans"].ToString();

                                await MessageManager.SendGroupMessageAsync(q, new MessageChainBuilder()
                                    .ImageFromUrl(result["data"]["card"]["face"].ToString() + "@150h")
                                    .Plain($@"名称：{name}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{mid}").Build());
                            } else {
                                await MessageManager.SendGroupMessageAsync(q, "获取vtb信息出错");
                            }
                        } else {
                            string res = await ($"{Api}/v1/living").GetStringAsync();
                            JArray Vtbs = JArray.Parse(res);

                            int index = (new Random()).Next(Vtbs.Count);
                            string RndVtbId = Vtbs[index].ToObject<string>();

                            var RoomInfo = JObject.Parse(await ($"{Api}/v1/room/" + RndVtbId).GetStringAsync());
                            string uid = RoomInfo["uid"].ToString();
                            string popularity = RoomInfo["popularity"].ToString();

                            var RndVtb = JObject.Parse(await ($"{Api}/v1/detail/" + uid).GetStringAsync());

                            string userName = RndVtb["uname"].ToString();
                            string roomId = RndVtb["roomid"].ToString();
                            string faceUrl = RndVtb["face"].ToString() + "@150h";
                            string followers = RndVtb["follower"].ToString();
                            string online = RndVtb["online"].ToString();
                            string sign = RndVtb["sign"].ToString();
                            string title = RndVtb["title"].ToString();
                            string id = await MessageManager.SendGroupMessageAsync(q, new MessageChainBuilder()
                                .ImageFromUrl(faceUrl)
                                .Plain($@"{title}
https://live.bilibili.com/{roomId}
人气：{popularity}

vtb信息
名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{uid}").Build());
                            Cooldown.Remove(e.Sender.Id);
                            Cooldown.Add(e.Sender.Id, DateTime.Now);
                        }
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await MessageManager.SendGroupMessageAsync(q, "获取vtb信息出错");
                    }
                    return true;
                #endregion
                default:
                    if (text.StartsWith("!setddtool ") && PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                        string uid = text[11..];
                        if (long.TryParse(uid, out long target)) {
                            if (Override.ContainsKey(q)) {
                                Override.Remove(q);
                            }
                            Override.Add(q, target.ToString());
                            File.WriteAllText("ddtoolcfg/config.json", JsonSerializer.Serialize(Override));
                            await MessageManager.SendGroupMessageAsync(q, "已设置DDTool指定抽取用户uid:" + target);
                        }
                    }
                    return false;
            }
        }
    }
}