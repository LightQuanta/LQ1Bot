using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Microsoft.Data.Sqlite;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class KusaRecorder : PluginBase, IGroupMessage {
        public override int Priority => 9999;

        public override string PluginName => "KusaRecorder";
        public override bool CanDisable => true;

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            #region 生草榜查询
            switch (text) {
                #region 今日生草榜读取
                case "今日除草剂":
                case "今日生草榜":
                case "今日生草机":
                case "今日百草枯":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where time>=date('now','localtime') and type='cao' and fromgroup=" + q + " group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "今日生草榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "今天还没人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                #endregion
                #region 昨日生草榜读取
                case "昨日除草剂":
                case "昨日生草榜":
                case "昨日生草机":
                case "昨日百草枯":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where time<date('now','localtime') and time>=date('now','-1 day','localtime') and type='cao' and fromgroup=" + q + " group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "昨日生草榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "昨天没人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                #endregion
                #region 生草榜读取
                case "除草剂":
                case "生草榜":
                case "生草机":
                case "百草枯":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where type='cao' and fromgroup=" + q + " group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "生草榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "目前无人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                #endregion
                #region 今日生草榜总榜读取
                case "今日除草剂总榜":
                case "今日生草榜总榜":
                case "今日生草机总榜":
                case "今日百草枯总榜":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where time>=date('now','localtime') and type='cao' group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "今日生草榜全群总榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "今天还没人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                #endregion
                #region 昨日生草榜总榜读取
                case "昨日除草剂总榜":
                case "昨日生草榜总榜":
                case "昨日生草机总榜":
                case "昨日百草枯总榜":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where time<date('now','localtime') and time>=date('now','-1 day','localtime') and type='cao' group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "昨日生草榜全群总榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "昨天没人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                #endregion
                #region 生草榜总榜读取
                case "除草剂总榜":
                case "生草榜总榜":
                case "生草机总榜":
                case "百草枯总榜":
                    try {
                        using SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                        conn.Open();
                        SqliteCommand cmd = new SqliteCommand("", conn) {
                            CommandText = "select name,count(*) from main where type='cao' group by qq order by count(*) desc limit 10"
                        };
                        SqliteDataReader dr = cmd.ExecuteReader();

                        string rep = "生草榜全群总榜前10名\r";
                        string list = "";
                        while (dr.Read()) {
                            list += dr["name"] + "  生草数：" + dr["count(*)"] + "\r";
                        }
                        dr.Close();

                        if (list != "") {
                            await MessageManager.SendGroupMessageAsync(q, rep + list);
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "目前无人发草！");
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    return true;
                    #endregion
            }
            #endregion
            #region 生草记录
            if (Regex.IsMatch(text.ToLower(), @"[草艸艹🌿]|w{3,}|kusa|cao|grass]")) {
                Console.WriteLine("草");
                try {
                    SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                    conn.Open();
                    SqliteCommand cmd = new SqliteCommand("", conn);
                    SqliteDataReader dr;
                    cmd.CommandText = "select count(*) from main where qq=" + e.Sender.Id + " and time>=datetime('now','localtime','-1 minutes') and type='cao' and fromgroup=" + q;
                    dr = cmd.ExecuteReader();

                    int count = 0;
                    while (dr.Read()) {
                        count = int.Parse(dr["count(*)"].ToString());
                    }
                    dr.Close();
                    conn.Close();

                    Console.WriteLine("一分钟内生草次数：\t" + count.ToString());
                    Console.ResetColor();

                    if (count >= 7) {
                        await GroupManager.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(3.0));
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("已禁言用户");
                        Console.ResetColor();
                        await MessageManager.SendGroupMessageAsync(q, "禁止刷屏");
                    } else {
                        cmd = new SqliteCommand("", conn) {
                            CommandText = $"insert into main values (strftime('%Y-%m-%d %H:%M:%f','now','localtime'),@nickname,{e.Sender.Id},{q},'cao')"
                        };
                        cmd.Parameters.AddWithValue("@nickname", e.Sender.Name);

                        dr = cmd.ExecuteReader();
                        dr.Close();
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("成功写入草，用户名：\t" + e.Sender.Name);
                    }
                } catch (Exception ee) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("写入数据库出现错误\n" + ee.Message);
                    Console.ResetColor();
                }
                return false;
            }
            #endregion
            return false;
        }
    }
}