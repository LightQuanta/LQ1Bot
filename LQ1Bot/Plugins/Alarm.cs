using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LQ1Bot.Plugins {

    internal class Alarm : PluginBase, IGroupMessage {
        public override int Priority => 9982;

        public override string PluginName => "Alarm";

        public override bool CanDisable => true;


        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;

            if (text == "警钟") {
                Init(q);
                string temp = File.ReadAllText($"alarmcfg/{q}.txt");
                string time = temp.Split("|")[0];
                string content = temp.Split("|")[1];
                var InitTime = new DateTime(long.Parse(time));
                var Now = DateTime.Now;
                var Diff = Now - InitTime;
                string DiffStr = $"{Diff.Days}天{Diff.Hours}小时{Diff.Minutes}分{Diff.Seconds}秒{Diff.Ticks % 10000000 / 10000}毫秒{Diff.Ticks % 10000 / 10}微秒{Diff.Ticks % 10}00纳秒";
                await MessageManager.SendGroupMessageAsync(q, content.Replace("{time}", DiffStr));
            }
            return false;
        }

        private void Init(string q) {
            if (!File.Exists($"alarmcfg/{q}.txt")) {
                if (!Directory.Exists("alarmcfg")) {
                    Directory.CreateDirectory("alarmcfg");
                }
                File.Create($"alarmcfg/{q}.txt");
            }
        }
    }
}