using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class WhatIsThis : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9987;

        public override string PluginName => "WhatIsThis";

        public override bool CanDisable => true;

        private DateTime Cooldown = DateTime.Now;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);

            string q = e.Sender.Id;

            if (Regex.IsMatch(text, @"^.{1,20}是啥。。。$")) {
                if (DateTime.Now < Cooldown) {
                    await MessageManager.SendFriendMessageAsync(q, "功能正在冷却，请稍后再试");
                    return true;
                }

                text = text[..^5];
                try {
                    JObject o = new JObject {
                        { "phrase", text },
                        { "page", 1 }
                    };

                    string json = JsonConvert.SerializeObject(o, Formatting.None);
                    Console.WriteLine(json);

                    WebRequest req = WebRequest.Create("https://api.jikipedia.com/go/search_entities");
                    req.Method = "POST";
                    req.Headers.Add("Client", "web");
                    req.Headers.Add("Client-Version", "2.6.3l");
                    req.ContentType = "application/json";


                    byte[] data = Encoding.Default.GetBytes(json);
                    req.ContentLength = data.Length;
                    using Stream reqStream = req.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();

                    using HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                    using StreamReader sr = new StreamReader(res.GetResponseStream());
                    string resp = sr.ReadToEnd();

                    JObject responceJson = JObject.Parse(resp);

                    JArray dataArray = (JArray) responceJson["data"];

                    foreach (var dat in dataArray) {
                        if (dat["category"].ToString() == "definition") {
                            string name = dat["definitions"][0]["term"]["title"].ToString();
                            string def = dat["definitions"][0]["content"].ToString();

                            //中括号去除
                            def = Regex.Replace(def, @"\[.+(,.+)*:(?<str>.+)\]", "${str}");

                            await MessageManager.SendFriendMessageAsync(q, $"可能的全名：{name}\n释义：{def}\n详细信息：https://jikipedia.com/search?phrase={HttpUtility.UrlEncode(text)}");

                            //添加全局冷却
                            Cooldown = DateTime.Now.AddMinutes(0.5);

                            return true;
                        }
                    }
                    await MessageManager.SendGroupMessageAsync(q, "未找到解释！");
                } catch (WebException eee) {
                    if (((HttpWebResponse) eee.Response).StatusCode == HttpStatusCode.Locked) {
                        Cooldown = DateTime.Now.AddMinutes(5.0);
                        await MessageManager.SendFriendMessageAsync(q, "请求次数过多，请稍后再试");
                        return true;
                    }

                    Console.WriteLine(eee.Message);
                    using StreamReader sr = new StreamReader(eee.Response.GetResponseStream());
                    Console.WriteLine(sr.ReadToEnd());
                    await MessageManager.SendFriendMessageAsync(q, "获取解释出错");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;

            if (Regex.IsMatch(text, @"^.{1,20}是啥。。。$")) {
                if (DateTime.Now < Cooldown) {
                    await MessageManager.SendGroupMessageAsync(q, "功能正在冷却，请稍后再试");
                    return true;
                }

                text = text[..^5];
                try {
                    JObject o = new JObject {
                        { "phrase", text },
                        { "page", 1 }
                    };

                    string json = JsonConvert.SerializeObject(o, Formatting.None);
                    Console.WriteLine(json);

                    WebRequest req = WebRequest.Create("https://api.jikipedia.com/go/search_entities");
                    req.Method = "POST";
                    req.Headers.Add("Client", "web");
                    req.Headers.Add("Client-Version", "2.6.3l");
                    req.ContentType = "application/json";

                    byte[] data = Encoding.Default.GetBytes(json);
                    req.ContentLength = data.Length;
                    using Stream reqStream = req.GetRequestStream();
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();

                    using HttpWebResponse res = (HttpWebResponse) req.GetResponse();
                    using StreamReader sr = new StreamReader(res.GetResponseStream());
                    string resp = sr.ReadToEnd();

                    JObject responceJson = JObject.Parse(resp);

                    JArray dataArray = (JArray) responceJson["data"];

                    foreach (var dat in dataArray) {
                        if (dat["category"].ToString() == "definition") {
                            string name = dat["definitions"][0]["term"]["title"].ToString();
                            string def = dat["definitions"][0]["content"].ToString();

                            //中括号去除
                            def = Regex.Replace(def, @"\[.+(,.+)*:(?<str>.+)\]", "${str}");

                            await MessageManager.SendGroupMessageAsync(q, $"可能的全名：{name}\n释义：{def}\n详细信息：https://jikipedia.com/search?phrase={HttpUtility.UrlEncode(text)}");

                            //添加全局冷却
                            Cooldown = DateTime.Now.AddMinutes(0.5);

                            return true;
                        }
                    }
                    await MessageManager.SendGroupMessageAsync(q, "未找到解释！");
                } catch (WebException eee) {
                    if (((HttpWebResponse) eee.Response).StatusCode == HttpStatusCode.Locked) {
                        Cooldown = DateTime.Now.AddMinutes(5.0);
                        await MessageManager.SendGroupMessageAsync(q, "请求次数过多，请稍后再试");
                        return true;
                    }

                    Console.WriteLine(eee.Message);
                    using StreamReader sr = new StreamReader(eee.Response.GetResponseStream());
                    Console.WriteLine(sr.ReadToEnd());
                    await MessageManager.SendGroupMessageAsync(q, "获取解释出错");
                }
                return true;
            }
            return false;
        }
    }
}