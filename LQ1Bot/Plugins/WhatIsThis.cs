using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Flurl.Http;
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
                    var o = new {
                        phrase = text,
                        page = 1
                    };

                    string json = JsonConvert.SerializeObject(o, Formatting.None);
                    Console.WriteLine(json);

                    string resp = await "https://api.jikipedia.com/go/search_entities"
                        .WithHeader("Content-Type", "application/json;charset=utf-8")
                        .WithHeader("Client", "web")
                        .WithHeader("Client-Version", "2.7.2k")
                        .WithHeader("XID", GenerateXID())
                        .PostJsonAsync(o)
                        .ReceiveString();


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
                    var o = new {
                        phrase = text,
                        page = 1
                    };

                    string json = JsonConvert.SerializeObject(o, Formatting.None);
                    Console.WriteLine(json);

                    string resp = await "https://api.jikipedia.com/go/search_entities"
                        .WithHeader("Content-Type", "application/json;charset=utf-8")
                        .WithHeader("Client", "web")
                        .WithHeader("Client-Version", "2.7.2k")
                        .WithHeader("XID", GenerateXID())
                        .PostJsonAsync(o)
                        .ReceiveString();

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

        private static string GenerateXID() {
            //random guid
            string content = "jikipedia_xid_5ea1338b-2d8d-435d-b04d-0a5d9e41d147";
            //fixed version info
            string pwd = "web_2.7.2k_12uh00]35#@(poj[";
            byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(pwd));

            //random iv
            byte[] iv = RandomNumberGenerator.GetBytes(16);
            var encrypted = EncryptStringToBytes_Aes(content, key, iv);

            byte[] resultbin = iv.Concat(encrypted).ToArray();
            return Convert.ToBase64String(resultbin);
        }
        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV) {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create()) {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream()) {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}