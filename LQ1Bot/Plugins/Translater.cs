using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {
    class Translater : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9989;
        public override bool CanDisable => true;

        public override string PluginName => "Translater";

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            #region 机翻
            if (Regex.IsMatch(text, @"^翻译(\w+)? .+$")) {
                string ToTranslate = text[(text.IndexOf(' ') + 1)..];
                string Lang = text[2] == ' ' ? "zh" : text.Split(' ')[0][2..];
                string AppId = Program.Secret.BaiduTranslateAppId;
                string salt = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();
                Console.WriteLine(salt);
                string sign = Utils.GetMD5(AppId + ToTranslate + salt + Program.Secret.BaiduTranslateSecret);
                Console.WriteLine(sign);
                string url = @"http://" + @$"api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(ToTranslate)}&from=auto&to={Lang}&appid={AppId}&salt={salt}&sign={sign}";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                req.Timeout = 5000;
                string result = "";
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                    Console.WriteLine(result);
                    JObject o = JObject.Parse(result);
                    string TranslationResult = o["trans_result"][0]["dst"].ToString();
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"翻译结果：{TranslationResult}"));
                } catch (Exception eee) {
                    Console.WriteLine(eee.Message);
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("获取翻译出错"));
                }
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            #region 机翻
            if (Regex.IsMatch(text, @"^翻译(\w+)? .+$")) {
                string ToTranslate = text[(text.IndexOf(' ') + 1)..];
                string Lang = text[2] == ' ' ? "zh" : text.Split(' ')[0][2..];
                string AppId = Program.Secret.BaiduTranslateAppId;
                string salt = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();
                Console.WriteLine(salt);
                string sign = Utils.GetMD5(AppId + ToTranslate + salt + Program.Secret.BaiduTranslateSecret);
                Console.WriteLine(sign);
                string url = @"http://" + @$"api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(ToTranslate)}&from=auto&to={Lang}&appid={AppId}&salt={salt}&sign={sign}";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                req.Timeout = 5000;
                string result = "";
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                    Console.WriteLine(result);
                    JObject o = JObject.Parse(result);
                    string TranslationResult = o["trans_result"][0]["dst"].ToString();
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage($"翻译结果：{TranslationResult}"));
                } catch (Exception eee) {
                    Console.WriteLine(eee.Message);
                    await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage("获取翻译出错"));
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}
