using System;
using System.Collections.Generic;
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

    internal class AutoTranslate : PluginBase, IGroupMessage {
        public override int Priority => 9988;

        public override string PluginName => "AutoTranslate";

        public override bool CanDisable => true;

        private readonly Dictionary<string, string> AutoTranslateList = new Dictionary<string, string>();

        public AutoTranslate() {
            AutoTranslateList.Add("1057879872", "zh");
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            #region 自动翻译
            if (Regex.IsMatch(text, @"^autotranslate \d+ \w+")) {
                if (long.TryParse(text.Split(' ')[1], out long target)) {
                    string TargetLang = text.Split(' ')[2];
                    if (TargetLang == "reset") {
                        AutoTranslateList.Remove(target.ToString());
                        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id,$"已为QQ号{target}关闭自动翻译");
                        return true;
                    } else {
                        AutoTranslateList.Remove(target.ToString());
                        AutoTranslateList.Add(target.ToString(), text.Split(' ')[2]);
                        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, $"已为QQ号{target}开启自动翻译");
                    }
                }
                return true;
            }
            if (AutoTranslateList.ContainsKey(e.Sender.Id)) {
                string ToTranslate = text;
                string Lang = AutoTranslateList.GetValueOrDefault(e.Sender.Id);
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
                    if (o["from"].ToString() != Lang) {
                        string TranslationResult = o["trans_result"][0]["dst"].ToString();
                        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id,$"【{TranslationResult}】");
                    }
                } catch (Exception) { }
                return false;
            }
            #endregion
            return false;
        }
    }
}