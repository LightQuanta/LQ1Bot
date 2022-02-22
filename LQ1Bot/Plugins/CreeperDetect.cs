using System;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Microsoft.Data.Sqlite;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class CreeperDetect : PluginBase, IGroupMessage, IFriendMessage, ITempMessage {
        public override int Priority => 10000;

        public override string PluginName => "CreeperDetector";

        public override bool CanDisable => true;

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (e.Sender.Name.Length < 10 && CreeperDetector.IsCreeper(e.Sender.Name)) {
                try {
                    await GroupManager.SetMemberInfoAsync(e.Sender.Id, q, "爬");
                } catch (Exception eeeee) {
                    Console.WriteLine(eeeee.Message);
                }
            }
            if (CreeperDetector.IsCreeper(text) && text.IndexOf("http://") == -1 && text.IndexOf("https://") == -1) {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("已禁言用户");
                Console.ResetColor();
                await MessageManager.SendGroupMessageAsync(q, "就这？");

                await GroupManager.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(5.0));

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

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            if (CreeperDetector.IsCreeper(Utils.GetMessageText(e.MessageChain))) {
                await MessageManager.SendFriendMessageAsync(e.Sender.Id, "爬");
                return true;
            }
            return false;
        }

        public async Task<bool> TempMessage(TempMessageReceiver e) {
            if (CreeperDetector.IsCreeper(Utils.GetMessageText(e.MessageChain))) {
                await MessageManager.SendTempMessageAsync(e.Sender.Id, e.Sender.Group.Id, "爬");
                return true;
            }
            return false;
        }
    }
}