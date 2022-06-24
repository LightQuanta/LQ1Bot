using System;
using System.Threading.Tasks;
using Flurl.Http;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class CatPic : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9983;

        public override string PluginName => "CatPic";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            #region CatPic
            if (text == "来点猫猫" || text == "来点猫图") {
                try {
                    string json = await "https://api.thecatapi.com/v1/images/search".GetStringAsync();
                    string id = JArray.Parse(json)[0]["url"].ToString();
                    ImageMessage image = new();
                    image.Url = id;
                    await MessageManager.SendFriendMessageAsync(q, image);
                } catch (Exception ee) {
                    Console.WriteLine(ee.Message);
                    await MessageManager.SendFriendMessageAsync(q, "获取猫图出错");
                }
                return true;
            }
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            #region CatPic
            if (text == "来点猫猫" || text == "来点猫图") {
                try {
                    string json = await "https://api.thecatapi.com/v1/images/search".GetStringAsync();
                    string id = JArray.Parse(json)[0]["url"].ToString();
                    ImageMessage image = new();
                    image.Url = id;
                    await MessageManager.SendGroupMessageAsync(q, image);
                } catch (Exception ee) {
                    Console.WriteLine(ee.Message);
                    await MessageManager.SendGroupMessageAsync(q, "获取猫图出错");
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}