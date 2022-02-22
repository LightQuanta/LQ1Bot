using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class ApexQuery : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9985;

        public override string PluginName => "ApexQuery";

        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            if (text.StartsWith("牢房排名 ") || text.StartsWith("坐牢排名 ")) {
                string user = text[5..];
                string url = $"https://api.mozambiquehe.re/bridge?version=5&platform=PC&player={HttpUtility.UrlEncode(user)}&auth={Program.Secret.ApexSecret}";
                WebClient wc = new WebClient();
                try {
                    string res = wc.DownloadString(url);
                    var obj = JObject.Parse(res);
                    if (obj.ContainsKey("Error")) {
                        Console.WriteLine(obj["Error"]);
                        await MessageManager.SendFriendMessageAsync(q, $"查询Apex分数出错：{obj["Error"]}");
                    } else {
                        string name = obj["global"]["name"].ToString();
                        string level = obj["global"]["level"].ToString();
                        string percent = obj["global"]["toNextLevelPercent"].ToString();
                        string score = obj["global"]["rank"]["rankScore"].ToString();
                        string rank = obj["global"]["rank"]["rankName"].ToString() switch
                        {
                            "Bronze" => "青铜",
                            "Silver" => "白银",
                            "Gold" => "黄金",
                            "Platinum" => "铂金",
                            "Diamond" => "钻石",
                            "Master" => "大师",
                            "Predator" => "猎杀",
                            _ => obj["global"]["rank"]["rankName"].ToString()
                        };
                        rank += obj["global"]["rank"]["rankDiv"];
                        string kills = obj["total"]["kills"]["value"].ToString();
                        await MessageManager.SendFriendMessageAsync(q, $"名称：{name}\n等级：{level}（{percent}%）\n排位得分：{score}\n段位：{rank}\n总击杀：{kills}");
                    }
                } catch (Exception eee) {
                    Console.WriteLine(eee);
                    await MessageManager.SendFriendMessageAsync(q, "查询Apex分数出错");
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text.StartsWith("牢房排名 ") || text.StartsWith("坐牢排名 ")) {
                string user = text[5..];
                string url = $"https://api.mozambiquehe.re/bridge?version=5&platform=PC&player={HttpUtility.UrlEncode(user)}&auth={Program.Secret.ApexSecret}";
                WebClient wc = new WebClient();
                try {
                    string res = wc.DownloadString(url);
                    var obj = JObject.Parse(res);
                    if (obj.ContainsKey("Error")) {
                        Console.WriteLine(obj["Error"]);
                        await MessageManager.SendGroupMessageAsync(q, $"查询Apex分数出错：{obj["Error"]}");
                    } else {
                        string name = obj["global"]["name"].ToString();
                        string level = obj["global"]["level"].ToString();
                        string percent = obj["global"]["toNextLevelPercent"].ToString();
                        string score = obj["global"]["rank"]["rankScore"].ToString();
                        string rank = obj["global"]["rank"]["rankName"].ToString() switch
                        {
                            "Bronze" => "青铜",
                            "Silver" => "白银",
                            "Gold" => "黄金",
                            "Platinum" => "铂金",
                            "Diamond" => "钻石",
                            "Master" => "大师",
                            "Predator" => "猎杀",
                            _ => obj["global"]["rank"]["rankName"].ToString()
                        };
                        rank += obj["global"]["rank"]["rankDiv"];
                        string kills = obj["total"]["kills"]["value"].ToString();
                        await MessageManager.SendGroupMessageAsync(q, $"名称：{name}\n等级：{level}（{percent}%）\n排位得分：{score}\n段位：{rank}\n总击杀：{kills}");
                    }
                } catch (Exception eee) {
                    Console.WriteLine(eee);
                    await MessageManager.SendGroupMessageAsync(q, "查询Apex分数出错");
                }
                return true;
            }
            return false;
        }
    }
}