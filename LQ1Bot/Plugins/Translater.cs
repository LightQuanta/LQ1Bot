using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class Translater : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9989;
        public override bool CanDisable => true;

        public override string PluginName => "Translater";

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            #region 机翻
            if (Regex.IsMatch(text, @"^翻译([a-z]{1,3})? .+$")) {
                string ToTranslate = text[(text.IndexOf(' ') + 1)..];
                string Lang = text[2] == ' ' ? "zh" : text.Split(' ')[0][2..];
                if (Lang == "cn")
                    Lang = "zh";
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

                    if (o.ContainsKey("error_code") && o["error_code"].ToString() == "58001") {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, "翻译语言选择错误");
                        return true;
                    }

                    string TranslationResult = o["trans_result"][0]["dst"].ToString();
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, $"翻译结果：{TranslationResult}");
                } catch (Exception eee) {
                    Console.WriteLine(eee.Message);
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "获取翻译出错");
                }
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            #region 机翻
            if (Regex.IsMatch(text, @"^翻译([a-z]{1,3})? .+$")) {
                string ToTranslate = text[(text.IndexOf(' ') + 1)..];
                string Lang = text[2] == ' ' ? "zh" : text.Split(' ')[0][2..];
                if (Lang == "cn")
                    Lang = "zh";
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

                    if (o.ContainsKey("error_code") && o["error_code"].ToString() == "58001") {
                        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, "翻译语言选择错误");
                        return true;
                    }

                    string TranslationResult = o["trans_result"][0]["dst"].ToString();
                    await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, $"翻译结果：{TranslationResult}");
                } catch (Exception eee) {
                    Console.WriteLine(eee.Message);
                    await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, "获取翻译出错");
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}