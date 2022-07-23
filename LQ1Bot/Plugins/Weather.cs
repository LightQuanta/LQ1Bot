using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;
using Flurl.Http;
using System.Collections.Generic;

namespace LQ1Bot.Plugins {

    internal class Weather : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9980;

        public override string PluginName => "Weather";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            if (text.Length < 10 && text.StartsWith("天气查询 ")) {
                try {
                    string city = text[5..];
                    string result = await $"https://query.asilu.com/weather/gaode/?city={city}"
                        .GetStringAsync();
                    JObject o = JObject.Parse(result);
                    if (((JArray) o["forecasts"]).Count > 0) {
                        JArray casts = (JArray) o["forecasts"][0]["casts"];
                        string name = o["forecasts"][0]["city"].ToString();
                        string time = o["forecasts"][0]["reporttime"].ToString();
                        string dayweather = $"{casts[0]["dayweather"]} {casts[0]["daytemp"]}℃ {casts[0]["daywind"]}风{casts[0]["daypower"]}级";
                        string nightweather = $"{casts[0]["nightweather"]} {casts[0]["nighttemp"]}℃ {casts[0]["nightwind"]}风{casts[0]["nightpower"]}级";
                        string msg = $"{name}天气 更新时间：{time}\n日间天气：{dayweather}\n夜间天气：{nightweather}";

                        List<string> fc = new();
                        for (int i = 1; i < casts.Count; i++) {
                            dayweather = $"{casts[i]["dayweather"]} {casts[i]["daytemp"]}℃ {casts[i]["daywind"]}风{casts[i]["daypower"]}级";
                            nightweather = $"{casts[i]["nightweather"]} {casts[i]["nighttemp"]}℃ {casts[i]["nightwind"]}风{casts[i]["nightpower"]}级";
                            fc.Add(casts[i]["date"] + ": " + dayweather + " => " + nightweather);
                        }

                        msg += "\n" + string.Join("\n", fc);
                        await MessageManager.SendFriendMessageAsync(q, msg);
                    } else {
                        await MessageManager.SendFriendMessageAsync(q, "城市名称错误！");
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    await MessageManager.SendFriendMessageAsync(q, "获取天气出错");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text.Length < 10 && text.StartsWith("天气查询 ")) {
                try {
                    string city = text[5..];
                    string result = await $"https://query.asilu.com/weather/gaode/?city={city}"
                        .GetStringAsync();
                    JObject o = JObject.Parse(result);
                    if (((JArray) o["forecasts"]).Count > 0) {
                        JArray casts = (JArray) o["forecasts"][0]["casts"];
                        string name = o["forecasts"][0]["city"].ToString();
                        string time = o["forecasts"][0]["reporttime"].ToString();
                        string dayweather = $"{casts[0]["dayweather"]} {casts[0]["daytemp"]}℃ {casts[0]["daywind"]}风{casts[0]["daypower"]}级";
                        string nightweather = $"{casts[0]["nightweather"]} {casts[0]["nighttemp"]}℃ {casts[0]["nightwind"]}风{casts[0]["nightpower"]}级";
                        string msg = $"{name}天气 更新时间：{time}\n日间天气：{dayweather}\n夜间天气：{nightweather}";

                        List<string> fc = new();
                        for (int i = 1; i < casts.Count; i++) {
                            dayweather = $"{casts[i]["dayweather"]} {casts[i]["daytemp"]}℃ {casts[i]["daywind"]}风{casts[i]["daypower"]}级";
                            nightweather = $"{casts[i]["nightweather"]} {casts[i]["nighttemp"]}℃ {casts[i]["nightwind"]}风{casts[i]["nightpower"]}级";
                            fc.Add(casts[i]["date"] + ": " + dayweather + " => " + nightweather);
                        }

                        msg += "\n" + string.Join("\n", fc);
                        await MessageManager.SendGroupMessageAsync(q, msg);
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "城市名称错误！");
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex);
                    await MessageManager.SendGroupMessageAsync(q, "获取天气出错");
                }
                return true;
            }
            return false;
        }
    }
}