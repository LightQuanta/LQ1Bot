using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class DDTool : PluginBase, IFriendMessage, IGroupMessage {
        public override int Priority => 9995;

        public override string PluginName => "DDTool";

        public override bool CanDisable => true;

        private Dictionary<long, DateTime> Cooldown = new Dictionary<long, DateTime>();

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain).ToLower();
            long q = e.Sender.Id;
            switch (text) {
                #region 今天看谁
                case "今天看谁":
                case "今天d谁":
                    try {
                        WebClient wc = new WebClient();
                        string res = wc.DownloadString("https://api.vtbs.moe/v1/vtbs");
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        JObject RndVtbId = (JObject) Vtbs[index];

                        string userId = RndVtbId["mid"].ToString();

                        var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + userId));

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        string online = RndVtb["online"].ToString();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();
                        MessageBuilder b = new MessageBuilder();
                        b.Add(new ImageMessage(null, faceUrl, null));

                        if (online == "0") {
                            b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}
直播间地址：https://live.bilibili.com/{roomId}"));
                        } else {
                            b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}

当前正在直播！
直播间标题：{title}
直播间地址：https://live.bilibili.com/{roomId}"));
                        }
                        await session.SendFriendMessageAsync(q, b);
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await session.SendFriendMessageAsync(q, new PlainMessage("获取vtb信息出错"));
                    }
                    return true;
                #endregion
                #region 现在看谁
                case "现在看谁":
                case "现在d谁":
                    try {
                        WebClient wc = new WebClient();
                        string res = wc.DownloadString("https://api.vtbs.moe/v1/living");
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        long RndVtbId = Vtbs[index].ToObject<long>();

                        var RoomInfo = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/room/" + RndVtbId));
                        string uid = RoomInfo["uid"].ToString();
                        string popularity = RoomInfo["popularity"].ToString();

                        var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + uid));

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        string online = RndVtb["online"].ToString();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();
                        MessageBuilder b = new MessageBuilder();
                        b.Add(new ImageMessage(null, faceUrl, null));

                        b.Add(new PlainMessage($@"{title}
https://live.bilibili.com/{roomId}
人气：{popularity}

vtb信息
名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{uid}"));
                        await session.SendFriendMessageAsync(q, b);
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await session.SendFriendMessageAsync(q, new PlainMessage("获取vtb信息出错"));
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain).ToLower();
            long q = e.Sender.Group.Id;
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
                        WebClient wc = new WebClient();
                        string res = wc.DownloadString("https://api.vtbs.moe/v1/vtbs");
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        JObject RndVtbId = (JObject) Vtbs[index];

                        string userId = RndVtbId["mid"].ToString();

                        var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + userId));

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        string online = RndVtb["online"].ToString();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();
                        MessageBuilder b = new MessageBuilder();
                        //b.Add(new ImageMessage(null, faceUrl, null));

                        if (online == "0") {
                            b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}
直播间地址：https://live.bilibili.com/{roomId}"));
                        } else {
                            b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}

当前正在直播！
直播间标题：{title}
直播间地址：https://live.bilibili.com/{roomId}"));
                        }
                        int id = await session.SendGroupMessageAsync(q, b);
                        Cooldown.Remove(e.Sender.Id);
                        Cooldown.Add(e.Sender.Id, DateTime.Now);
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await session.SendGroupMessageAsync(q, new PlainMessage("获取vtb信息出错"));
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
                        WebClient wc = new WebClient();
                        string res = wc.DownloadString("https://api.vtbs.moe/v1/living");
                        JArray Vtbs = JArray.Parse(res);

                        int index = (new Random()).Next(Vtbs.Count);
                        long RndVtbId = Vtbs[index].ToObject<long>();

                        var RoomInfo = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/room/" + RndVtbId));
                        string uid = RoomInfo["uid"].ToString();
                        string popularity = RoomInfo["popularity"].ToString();

                        var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + uid));

                        string userName = RndVtb["uname"].ToString();
                        string roomId = RndVtb["roomid"].ToString();
                        string faceUrl = RndVtb["face"].ToString() + "@150h";
                        string followers = RndVtb["follower"].ToString();
                        string online = RndVtb["online"].ToString();
                        string sign = RndVtb["sign"].ToString();
                        string title = RndVtb["title"].ToString();
                        MessageBuilder b = new MessageBuilder();
                        //b.Add(new ImageMessage(null, faceUrl, null));

                        b.Add(new PlainMessage($@"{title}
https://live.bilibili.com/{roomId}
人气：{popularity}

vtb信息
名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{uid}"));
                        int id = await session.SendGroupMessageAsync(q, b);
                        Cooldown.Remove(e.Sender.Id);
                        Cooldown.Add(e.Sender.Id, DateTime.Now);
                    } catch (Exception ee) {
                        Console.WriteLine(ee.Message);
                        await session.SendGroupMessageAsync(q, new PlainMessage("获取vtb信息出错"));
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }
    }
}