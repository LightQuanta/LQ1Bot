using System;
using System.Collections.Generic;
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
            if ((text == "!delalarm") && PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                if (AlarmExists(q)) {
                    File.Delete($"alarmcfg/{q}.txt");
                    await MessageManager.SendGroupMessageAsync(q, "已移除所有警钟");
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还未添加警钟！");
                }
            }
            if (text.StartsWith("!addalarm ") && PermissionMgr.IsGroupOrBotAdmin(e.Sender)) {
                string alarm = text[10..];
                if (alarm.Length > 0 && alarm.Split("|").Length == 2) {
                    string time = alarm.Split("|")[0];
                    string content = alarm.Split("|")[1];
                    Match m = Regex.Match(time, @"^(\d{1,4})-(\d{1,2})-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$");
                    if (m.Success) {
                        int year = int.Parse(m.Groups[1].Value);
                        int month = int.Parse(m.Groups[2].Value);
                        int date = int.Parse(m.Groups[3].Value);
                        int hour = int.Parse(m.Groups[4].Value);
                        int minute = int.Parse(m.Groups[5].Value);
                        int second = int.Parse(m.Groups[6].Value);
                        try {
                            DateTime t = new(year, month, date, hour, minute, second);
                            long tick = t.Ticks;
                            List<string> list = new(GetAlarms(q));
                            list.Add(tick + "|" + content);
                            File.WriteAllText($"alarmcfg/{q}.txt", string.Join("\n", list));
                            await MessageManager.SendGroupMessageAsync(q, "成功添加警钟");
                        } catch (ArgumentOutOfRangeException) {
                            await MessageManager.SendGroupMessageAsync(q, "时间输入错误！输入格式应为：\n年-月-日 时:分:秒|回复内容");
                        }
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "时间输入错误！输入格式应为：\n年-月-日 时:分:秒|回复内容");
                    }
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "警钟格式输入错误！输入格式应为：\n年-月-日 时:分:秒|回复内容");
                }
            }

            if (text == "!getalarm") {
                if (AlarmExists(q)) {
                    var alarms = GetAlarms(q);
                    List<string> msg = new();

                    foreach (var v in alarms) {
                        string temp = v;
                        string time = temp.Split("|")[0];
                        string content = temp.Split("|")[1];
                        var InitTime = new DateTime(long.Parse(time));
                        msg.Add($"{InitTime:yyyy-MM-dd HH:mm:ss}|{content}");
                    }
                    await MessageManager.SendGroupMessageAsync(q, string.Join("\n", msg));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还未添加警钟！");
                }
            }

            if (text == "警钟" || text == "警钟长鸣" || text == "警钟敲烂") {
                if (AlarmExists(q)) {
                    string temp = GetAlarms(q)[^1];
                    string time = temp.Split("|")[0];
                    string content = temp.Split("|")[1];
                    var InitTime = new DateTime(long.Parse(time));
                    var Now = DateTime.Now;
                    var Diff = Now - InitTime;
                    string DiffStr = $"{Diff.Days}天{Diff.Hours}小时{Diff.Minutes}分{Diff.Seconds}秒{Diff.Ticks % 10000000 / 10000}毫秒{Diff.Ticks % 10000 / 10}微秒{Diff.Ticks % 10}00纳秒";
                    await MessageManager.SendGroupMessageAsync(q, content.Replace("{time}", DiffStr));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还没有警钟！");
                }
            }
            var match = Regex.Match(text, @"^警钟(\d+)$");
            if (match.Success) {
                if (AlarmExists(q)) {
                    var alarms = GetAlarms(q);
                    int id = int.Parse(match.Groups[1].Value);
                    if (id >= 1 && id <= alarms.Length) {
                        string temp = GetAlarms(q)[id - 1];
                        string time = temp.Split("|")[0];
                        string content = temp.Split("|")[1];
                        var InitTime = new DateTime(long.Parse(time));
                        var Now = DateTime.Now;
                        var Diff = Now - InitTime;
                        string DiffStr = $"{Diff.Days}天{Diff.Hours}小时{Diff.Minutes}分{Diff.Seconds}秒{Diff.Ticks % 10000000 / 10000}毫秒{Diff.Ticks % 10000 / 10}微秒{Diff.Ticks % 10}00纳秒";
                        await MessageManager.SendGroupMessageAsync(q, content.Replace("{time}", DiffStr));
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, "不存在该警钟！");
                    }
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还没有警钟！");
                }
            }

            if (text == "编钟") {
                if (AlarmExists(q)) {
                    var alarms = GetAlarms(q);
                    List<string> msg = new();

                    foreach (var v in alarms) {
                        string temp = v;
                        string time = temp.Split("|")[0];
                        string content = temp.Split("|")[1];
                        var InitTime = new DateTime(long.Parse(time));
                        var Now = DateTime.Now;
                        var Diff = Now - InitTime;
                        string DiffStr = $"{Diff.Days}天{Diff.Hours}小时{Diff.Minutes}分{Diff.Seconds}秒{Diff.Ticks % 10000000 / 10000}毫秒{Diff.Ticks % 10000 / 10}微秒{Diff.Ticks % 10}00纳秒";
                        msg.Add(content.Replace("{time}", DiffStr));
                    }
                    await MessageManager.SendGroupMessageAsync(q, string.Join("\n", msg));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "本群还没有警钟！");
                }
            }
            return false;
        }

        private static string[] GetAlarms(string q) {
            return File.ReadAllLines($"alarmcfg/{q}.txt");
        }
        private static bool AlarmExists(string q) {
            return File.Exists($"alarmcfg/{q}.txt");
        }

    }
}