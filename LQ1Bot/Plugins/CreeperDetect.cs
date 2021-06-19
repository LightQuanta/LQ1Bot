using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class CreeperDetect : PluginBase, IGroupMessage, IFriendMessage, ITempMessage {
        public override int Priority => 10000;

        public override string PluginName => "CreeperDetector";

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;
            if (e.Sender.Name.Length < 10 && CreeperDetector.IsCreeper(e.Sender.Name)) {
                GroupMemberCardInfo gmc = new GroupMemberCardInfo {
                    Name = "爬"
                };
                try {
                    await session.ChangeGroupMemberInfoAsync(e.Sender.Id, q, gmc);
                } catch (Exception eeeee) {
                    Console.WriteLine(eeeee.Message);
                }
            }
            if (CreeperDetector.IsCreeper(text) && text.IndexOf("http://") == -1 && text.IndexOf("https://") == -1) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("已禁言用户");
                Console.ResetColor();
                await session.SendGroupMessageAsync(q, new PlainMessage("就这？"));

                await session.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(5.0));

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
            return false;
        }

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            if (CreeperDetector.IsCreeper(Utils.GetMessageText(e.Chain))) {
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("爬"));
                return true;
            }
            return false;
        }
        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e) {
            if (CreeperDetector.IsCreeper(Utils.GetMessageText(e.Chain))) {
                await session.SendTempMessageAsync(e.Sender.Id, e.Sender.Group.Id, new PlainMessage("爬"));
                return true;
            }
            return false;
        }
    }
}
