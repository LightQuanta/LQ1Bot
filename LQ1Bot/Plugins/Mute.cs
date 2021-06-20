using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class Mute : PluginBase, IGroupMessage {
        public override int Priority => 9993;

        public override string PluginName => "Mute";

        public override bool CanDisable => true;

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain).ToLower().Trim();
            long q = e.Sender.Group.Id;
            #region 禁言抽奖
            if (text == "禁言抽奖" || text == "加班抽奖") {
                if (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner) {
                    await session.SendGroupMessageAsync(q, new PlainMessage($"恭喜{e.Sender.Name}抽中了1145141919810分钟禁言套餐！"));
                    return true;
                }
                int[] times = { 1, 1, 2, 2, 3, 3, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 23, 25, 30, 40, 45, 50, 60, 70, 80, 100, 114, 120, 180 };
                int time = times[(new Random()).Next(0, times.Length)];
                if (e.Sender.Id == 1916160394) {
                    if (time <= 10) {
                        time = (time + 10) * 10;
                    } else {
                        time *= 10;
                    }
                }
                if (e.Sender.Id == 1916160394)
                    await session.SendGroupMessageAsync(q, new PlainMessage($"恭喜{e.Sender.Name}抽中了『 超 级 加 倍 』{time}分钟禁言套餐！"));
                else
                    await session.SendGroupMessageAsync(q, new PlainMessage($"恭喜{e.Sender.Name}抽中了{time}分钟禁言套餐！"));
                await session.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(time));
                try {
                    SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                    conn.Open();
                    SqliteCommand cmd = new SqliteCommand("", conn) {
                        CommandText = $"insert into main values (strftime('%Y-%m-%d %H:%M:%f','now','localtime'),@nickname,{e.Sender.Id},{q},'creep')"
                    };
                    cmd.Parameters.AddWithValue("@nickname", e.Sender.Name);

                    SqliteDataReader dr = cmd.ExecuteReader();
                    dr.Close();
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    conn.Close();

                    Console.ResetColor();
                } catch (Exception ee) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("写入数据库出现错误\n" + ee.Message);
                    Console.ResetColor();
                }
                return true;
            }
            #endregion
            #region 自助禁言
            if (text.StartsWith("自助禁言 ") || text.StartsWith("自助加班 ")) {
                if (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner) {
                    await session.SendGroupMessageAsync(q, new PlainMessage("在？有种把管理卸了"));
                    return false;
                }
                if (int.TryParse(text[5..],out int stime)) {
                    if (stime <= 0) {
                        await session.SendGroupMessageAsync(q, new PlainMessage($"在？教我怎么设置{stime}分钟禁言"));
                    } else {
                        stime = stime > 720 ? 720 : stime;

                        await session.SendGroupMessageAsync(q, new PlainMessage($"恭喜{e.Sender.Name}成功领取了{stime}分钟的禁言套餐！"));
                        await session.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(stime));

                        try {
                            SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                            conn.Open();
                            SqliteCommand cmd = new SqliteCommand("", conn) {
                                CommandText = $"insert into main values (strftime('%Y-%m-%d %H:%M:%f','now','localtime'),@nickname,{e.Sender.Id},{q},'creep')"
                            };
                            cmd.Parameters.AddWithValue("@nickname", e.Sender.Name);

                            SqliteDataReader dr = cmd.ExecuteReader();
                            dr.Close();
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            conn.Close();

                            Console.ResetColor();
                        } catch (Exception ee) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("写入数据库出现错误\n" + ee.Message);
                            Console.ResetColor();
                        }
                    }
                } else {
                    await session.SendGroupMessageAsync(q, new PlainMessage("在？你管这叫int32整数？"));
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}
