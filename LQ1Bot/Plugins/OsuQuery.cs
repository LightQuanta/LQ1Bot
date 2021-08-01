using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class OsuQuery : PluginBase, IFriendMessage, IGroupMessage {
        public override int Priority => 9996;

        public override string PluginName => "OsuQuery";
        public override bool CanDisable => true;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Id;
            #region OSU
            if (Regex.IsMatch(text.ToLower(), @"^where .+ (osu|std|mania|ctb|taiko)$|^where .+ ?$")) {
                OsuAPI.mode = OsuAPI.OSU;
                string s = text.ToLower();
                if (s.IndexOf(' ') != s.LastIndexOf(' ')) {
                    switch (s[(s.LastIndexOf(' ') + 1)..]) {
                        case "osu":
                        case "std":
                            OsuAPI.mode = OsuAPI.OSU;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "ctb":
                            OsuAPI.mode = OsuAPI.CTB;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "mania":
                            OsuAPI.mode = OsuAPI.MANIA;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "taiko":
                            OsuAPI.mode = OsuAPI.TAIKO;
                            s = s[..s.LastIndexOf(' ')];
                            break;
                    }
                }
                Thread t = new Thread(new ThreadStart(() => {
                    try {
                        string name = s[(s.IndexOf(' ') + 1)..];
                        if (name.IndexOf("'") == -1) {
                            OsuAPI.PInfo p = OsuAPI.Query(name);
                            string responce = OsuAPI.GetUserInfo(name, OsuAPI.ReadToken());
                            if (responce == "auth") {
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine("Auth needed");
                                Console.ResetColor();
                                OsuAPI.UpdateToken();
                                Thread.Sleep(500);
                                responce = OsuAPI.GetUserInfo(name, OsuAPI.ReadToken());
                            }
                            switch (responce) {
                                case "404":
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("404 not found");
                                    Console.ResetColor();
                                    session.SendFriendMessageAsync(q, new PlainMessage("未找到该玩家！"));
                                    break;

                                default:
                                    var jo = JObject.Parse(responce);
                                    JObject stat = (JObject) jo.GetValue("statistics");
                                    float.TryParse(stat.GetValue("pp")?.ToString(), out float pp2);
                                    int pp = (int) Math.Round(pp2);
                                    int.TryParse(stat.GetValue("global_rank")?.ToString(), out int rank);
                                    string hit_accuracy = stat.GetValue("hit_accuracy")?.ToString() + "%";
                                    int.TryParse(stat.GetValue("play_count")?.ToString(), out int play_count);
                                    string total_score = stat.GetValue("total_score")?.ToString();
                                    string total_hits = stat.GetValue("total_hits")?.ToString();
                                    string maximum_combo = stat.GetValue("maximum_combo")?.ToString();
                                    JObject rankk = (JObject) stat.GetValue("rank");
                                    int.TryParse(rankk.GetValue("country")?.ToString(), out int crank);

                                    OsuAPI.Update(name, pp, rank, crank, play_count);

                                    int pp1 = p.pp;
                                    int rank1 = p.rank;
                                    int crank1 = p.crank;
                                    int playcount1 = p.playcount;
                                    string info = "玩家" + name + "的osu!个人信息\r\rPP：\t\t" + pp + " (" + (pp - pp1 >= 0 ? "+" : "") + (pp - p.pp) + ")\r全球排名：\t"
                                        + rank + " (" + (rank1 - rank >= 0 ? "+" : "") + (rank1 - rank) + ")\r全国排名：\t"
                                        + crank + " (" + (crank1 - crank >= 0 ? "+" : "") + (crank1 - crank) + ")\r准度：\t\t"
                                        + hit_accuracy + "\r游戏次数：\t"
                                        + play_count + " (" + (play_count - playcount1 >= 0 ? "+" : "") + (play_count - playcount1) + ")\r总分：\t\t"
                                        + total_score + "\r总命中次数：\t"
                                        + total_hits + "\r最大连击数：\t"
                                        + maximum_combo;
                                    session.SendFriendMessageAsync(q, new PlainMessage(info));
                                    Console.WriteLine("Query Ended");
                                    break;
                            }
                        } else {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Sql Inject Detected");
                            Console.ResetColor();
                            try {
                                OsuAPI.PInfo p = OsuAPI.QueryInjectable(ref name);
                                string info = "玩家" + name + "的osu!个人信息\r\rPP：\t\t" + p.pp + "\r全球排名：\t"
                                            + p.rank + "\r全国排名：\t"
                                            + p.crank + "\r准度：\t\t"
                                            + "0.0%" + "\r游戏次数：\t"
                                            + p.playcount + "\r总分：\t\t"
                                            + "0" + "\r总命中次数：\t"
                                            + "0" + "\r最大连击数：\t"
                                            + "0";
                                session.SendFriendMessageAsync(q, new PlainMessage(info));
                                Console.WriteLine("Query Ended");
                            } catch (Exception e) {
                                Console.WriteLine(e.Message);
                                session.SendFriendMessageAsync(q, new PlainMessage(e.Message));
                            }
                        }
                    } catch (Exception e) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Osu Query failed");
                        Console.WriteLine(e);
                        Console.ResetColor();
                    }
                }));
                t.Start();
                return true;
            }
            #endregion
            return await Task.Run(() => false);
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;
            #region OSU
            if (Regex.IsMatch(text.ToLower(), @"^where .+ (osu|std|mania|ctb|taiko)$|^where .+ ?$")) {
                OsuAPI.mode = OsuAPI.OSU;
                string s = text.ToLower();
                if (s.IndexOf(' ') != s.LastIndexOf(' ')) {
                    switch (s[(s.LastIndexOf(' ') + 1)..]) {
                        case "osu":
                        case "std":
                            OsuAPI.mode = OsuAPI.OSU;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "ctb":
                            OsuAPI.mode = OsuAPI.CTB;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "mania":
                            OsuAPI.mode = OsuAPI.MANIA;
                            s = s[..s.LastIndexOf(' ')];
                            break;

                        case "taiko":
                            OsuAPI.mode = OsuAPI.TAIKO;
                            s = s[..s.LastIndexOf(' ')];
                            break;
                    }
                }
                try {
                    string name = s[(s.IndexOf(' ') + 1)..];
                    if (name.IndexOf("'") == -1) {
                        OsuAPI.PInfo p = OsuAPI.Query(name);
                        string responce = OsuAPI.GetUserInfo(name, OsuAPI.ReadToken());
                        if (responce == "auth") {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("Auth needed");
                            Console.ResetColor();
                            OsuAPI.UpdateToken();
                            Thread.Sleep(500);
                            responce = OsuAPI.GetUserInfo(name, OsuAPI.ReadToken());
                        }
                        switch (responce) {
                            case "404":
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("404 not found");
                                Console.ResetColor();
                                await session.SendGroupMessageAsync(q, new PlainMessage("未找到该玩家！"));
                                break;

                            default:
                                var jo = JObject.Parse(responce);
                                JObject stat = (JObject) jo.GetValue("statistics");
                                float.TryParse(stat.GetValue("pp")?.ToString(), out float pp2);
                                int pp = (int) Math.Round(pp2);
                                int.TryParse(stat.GetValue("global_rank")?.ToString(), out int rank);
                                string hit_accuracy = stat.GetValue("hit_accuracy")?.ToString() + "%";
                                int.TryParse(stat.GetValue("play_count")?.ToString(), out int play_count);
                                string total_score = stat.GetValue("total_score")?.ToString();
                                string total_hits = stat.GetValue("total_hits")?.ToString();
                                string maximum_combo = stat.GetValue("maximum_combo")?.ToString();
                                JObject rankk = (JObject) stat.GetValue("rank");
                                int.TryParse(rankk.GetValue("country")?.ToString(), out int crank);

                                OsuAPI.Update(name, pp, rank, crank, play_count);

                                int pp1 = p.pp;
                                int rank1 = p.rank;
                                int crank1 = p.crank;
                                int playcount1 = p.playcount;
                                string info = "玩家" + name + "的osu!个人信息\r\rPP：\t\t" + pp + " (" + (pp - pp1 >= 0 ? "+" : "") + (pp - p.pp) + ")\r全球排名：\t"
                                    + rank + " (" + (rank1 - rank >= 0 ? "+" : "") + (rank1 - rank) + ")\r全国排名：\t"
                                    + crank + " (" + (crank1 - crank >= 0 ? "+" : "") + (crank1 - crank) + ")\r准度：\t\t"
                                    + hit_accuracy + "\r游戏次数：\t"
                                    + play_count + " (" + (play_count - playcount1 >= 0 ? "+" : "") + (play_count - playcount1) + ")\r总分：\t\t"
                                    + total_score + "\r总命中次数：\t"
                                    + total_hits + "\r最大连击数：\t"
                                    + maximum_combo;
                                await session.SendGroupMessageAsync(q, new PlainMessage(info));
                                Console.WriteLine("Query Ended");
                                break;
                        }
                    } else {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Sql Inject Detected");
                        Console.ResetColor();
                        try {
                            OsuAPI.PInfo p = OsuAPI.QueryInjectable(ref name);
                            string info = "玩家" + name + "的osu!个人信息\r\rPP：\t\t" + p.pp + "\r全球排名：\t"
                                        + p.rank + "\r全国排名：\t"
                                        + p.crank + "\r准度：\t\t"
                                        + "0.0%" + "\r游戏次数：\t"
                                        + p.playcount + "\r总分：\t\t"
                                        + "0" + "\r总命中次数：\t"
                                        + "0" + "\r最大连击数：\t"
                                        + "0";
                            await session.SendGroupMessageAsync(q, new PlainMessage(info));
                            Console.WriteLine("Query Ended");
                        } catch (Exception ee) {
                            Console.WriteLine(ee.Message);
                            await session.SendGroupMessageAsync(q, new PlainMessage(ee.Message));
                        }
                    }
                } catch (Exception ee) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Osu Query failed");
                    Console.WriteLine(ee);
                    Console.ResetColor();
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}