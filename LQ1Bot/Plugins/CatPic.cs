using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class CatPic : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9983;

        public override string PluginName => "CatPic";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Id;
            #region CatPic
            if (text == "来点猫猫" || text == "来点猫图") {
                try {
                    WebClient wc = new WebClient();
                    string json = wc.DownloadString("https://api.thecatapi.com/v1/images/search");
                    string id = JArray.Parse(json)[0]["id"].ToString();
                    string url = "https://cdn2.thecatapi.com/images/" + id + ".jpg";
                    Console.WriteLine("url:" + url);

                    WebRequest imgRequest = WebRequest.Create(url);
                    HttpWebResponse res = (HttpWebResponse) imgRequest.GetResponse();
                    Image downImage = Image.FromStream(res.GetResponseStream());
                    string FileName = id + ".jpg";
                    Console.WriteLine("FileName:" + FileName);

                    downImage.Save("/recordings/botpicture/" + FileName);

                    await session.SendFriendMessageAsync(q, new ImageMessage(null, "http://127.0.0.1:23333/botpicture/" + FileName, null));

                    File.Delete("/recordings/botpicture/" + FileName);
                } catch (Exception ee) {
                    Console.WriteLine(ee.Message);
                    await session.SendFriendMessageAsync(q, new PlainMessage("获取猫图出错"));
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
            long q = e.Sender.Group.Id;
            #region CatPic
            if (text == "来点猫猫" || text == "来点猫图") {
                try {
                    WebClient wc = new WebClient();
                    string json = wc.DownloadString("https://api.thecatapi.com/v1/images/search");
                    string id = JArray.Parse(json)[0]["id"].ToString();
                    string url = "https://cdn2.thecatapi.com/images/" + id + ".jpg";
                    Console.WriteLine("url:" + url);

                    WebRequest imgRequest = WebRequest.Create(url);
                    HttpWebResponse res = (HttpWebResponse) imgRequest.GetResponse();
                    Image downImage = Image.FromStream(res.GetResponseStream());
                    string FileName = id + ".jpg";
                    Console.WriteLine("FileName:" + FileName);

                    downImage.Save("/recordings/botpicture/" + FileName);

                    await session.SendGroupMessageAsync(q, new ImageMessage(null, "http://127.0.0.1:23333/botpicture/" + FileName, null));

                    File.Delete("/recordings/botpicture/" + FileName);
                } catch (Exception ee) {
                    Console.WriteLine(ee.Message);
                    await session.SendGroupMessageAsync(q, new PlainMessage("获取猫图出错"));
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}