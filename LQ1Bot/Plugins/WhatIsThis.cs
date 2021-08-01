using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class WhatIsThis : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9987;

        public override string PluginName => "WhatIsThis";

        public override bool CanDisable => true;

        private DateTime Cooldown = DateTime.Now;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);

            return await Task.Run(() => false);
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;

            if (Regex.IsMatch(text, @"^.{1,20}是啥。。。$")) {
                if (DateTime.Now < Cooldown) {
                    await session.SendGroupMessageAsync(q, new PlainMessage("功能正在冷却，请稍后再试"));
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

                    var v = req.Headers.GetEnumerator();
                    Console.WriteLine();
                    foreach (var vv in req.Headers.AllKeys) {
                        Console.WriteLine(vv + ":" + req.Headers.Get(vv));
                    }
                    Console.WriteLine();

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

                            def = Regex.Replace(def, @"\[.+(,.+)*:(?<str>.+)\]", "${str}");

                            await session.SendGroupMessageAsync(q, new PlainMessage($"可能的全名：{name}\n释义：{def}\nhttps://jikipedia.com/search?phrase={HttpUtility.UrlEncode(text)}"));

                            Cooldown = DateTime.Now.AddMinutes(0.5);

                            break;
                        }
                    }
                } catch (WebException eee) {
                    if (((HttpWebResponse) eee.Response).StatusCode == HttpStatusCode.Locked) {
                        Cooldown = DateTime.Now.AddMinutes(5.0);
                        await session.SendGroupMessageAsync(q, new PlainMessage("请求次数过多，请稍后再试"));
                        return true;
                    }

                    Console.WriteLine(eee.Message);
                    using StreamReader sr = new StreamReader(eee.Response.GetResponseStream());
                    Console.WriteLine(sr.ReadToEnd());
                    await session.SendGroupMessageAsync(q, new PlainMessage("获取解释出错"));
                }
                return true;
            }
            return false;
        }
    }
}