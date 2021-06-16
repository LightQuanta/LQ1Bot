using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;
using System.Text.Json;
using LQ1Bot.Meme;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace LQ1Bot {
    partial class LQ1Bot : ITempMessage, IBotInvitedJoinGroup, IFriendMessage, IGroupMessage, INewFriendApply, IDisconnected {

        private readonly Dictionary<long, (string Msg, int Count)> MsgRepeat = new Dictionary<long, (string, int)>();
        private readonly List<Pick> PickRecord = new List<Pick>();
        private readonly Dictionary<long, string> AutoTranslateList = new Dictionary<long, string>();

        private static MemeManager Meme;
        private static HashSet<long> BlacklistGroups;

        private static readonly List<Type> ExceptionList = new List<Type>();

        private static readonly Dictionary<long, string> LastMeme = new Dictionary<long, string>();

        private static LQ1BotConfig Secret;

        struct Pick {
            public long qq;
            public string Name;
            public int PickCount;
            public int Total;
            public Dictionary<long, string> Player;
        }

        /// <summary>
        /// 获取一个类在其所在的程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类</returns>
        public static List<Type> GetSubClass(Type ParentType) {
            var SubTypeList = new List<Type>();
            var Assembly = ParentType.Assembly;
            var AssemblyAllTypes = Assembly.GetTypes();
            foreach (var ItemType in AssemblyAllTypes) {
                if (ItemType.BaseType?.Name == ParentType.Name) {
                    SubTypeList.Add(ItemType);
                }
            }
            return SubTypeList.ToList();
        }

        public async Task<bool> Disconnected(MiraiHttpSession session, IDisconnectedEventArgs e) {
            MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(Secret.MiraiIp, Secret.MiraiPort, Secret.MiraiSecret);
            while (true) {
                try {
                    await session.ConnectAsync(options, Secret.QQ); // 连到成功为止, QQ号自填, 你也可以另行处理重连的 behaviour
                    return true;
                } catch (Exception) {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<bool> NewFriendApply(MiraiHttpSession session, INewFriendApplyEventArgs e) {
            await session.HandleNewFriendApplyAsync(e, FriendApplyAction.Allow, "感谢加好友哼哼啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊");
            return false;
        }
        public LQ1Bot(LQ1BotConfig config) {
            Secret = config;
            OsuAPI.OsuApiId = config.OsuApiId;
            OsuAPI.OsuApiSecret = config.OsuApiSecret;

            AutoTranslateList.Add(1057879872, "zh");
            var err = new Exception();
            Stack<Type> s = new Stack<Type>();
            s.Push(err.GetType());
            while (s.Count > 0) {
                var v = s.Pop();
                var temp = GetSubClass(v);
                ExceptionList.AddRange(temp);
                temp.ForEach(o => s.Push(o));
            }
            if (File.Exists("meme.json")) {
                try {
                    Meme = MemeManager.ReadFromFile("meme.json");
                } catch (Exception) { }
            }
            if (File.Exists("blacklist.json")) {
                try {
                    BlacklistGroups = JsonSerializer.Deserialize<HashSet<long>>(File.ReadAllText("blacklist.json"));
                } catch (Exception) { }
            }
            if (BlacklistGroups != null) {
                if (BlacklistGroups.Count > 0) {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[黑名单群]");
                    foreach (var v in BlacklistGroups) {
                        Console.WriteLine(v);
                    }
                    Console.ResetColor();
                }
            }
        }
        public static string GetMessageText(IMessageBase[] msg) {
            string text = "";
            foreach (var v in msg) {
                //text += "\n" + v.Type;
                switch (v.Type) {
                    case "Plain":
                        text += ((PlainMessage) v).Message;
                        break;
                    case "At":
                        AtMessage am = (AtMessage) v;
                        if (am.Target == 1727089824 || am.Target == 2224899528)
                            text += "Light_Quanta";
                        text += am.Display;
                        break;
                    //case "Quote":
                    //    QuoteMessage qm = (QuoteMessage) v;
                    //    var oc = qm.OriginChain;
                    //    //text += qm.;
                    //    break;
                    case "Source":
                        //SourceMessage sm = (SourceMessage) v;
                        break;
                }
            }
            return text;
        }
        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"FriendMsg\t{e.Sender.Id}");
            Console.ResetColor();
            string text = GetMessageText(e.Chain);
            Console.WriteLine(text);

            if (IsCreeper(text)) {
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("爬"));
                return true;
            }

            #region 无情的接梗机器
            var reppp = Meme.GetReply(text, 0);
            if (reppp != null) {
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(reppp));
            }
            #endregion
            #region DICE
            if (Regex.IsMatch(text.ToLower().Trim(), @"^dice$|^dice \d+d\d+$")) {
                int count = 1;
                int max;
                if (text.ToLower().Trim() == "dice") {
                    max = 6;
                } else {
                    count = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[0]);
                    max = int.Parse(text.ToLower().Trim().Split(' ')[1].Split('d')[1]);
                    max = max < 1 ? 1 : max;
                    count = count < 1 ? 1 : count;
                    count = count > 10 ? 10 : count;
                }
                Random r = new Random();
                string result = "";
                for (int i = 0; i < count; i++) {
                    if (i == 0) {
                        result += r.Next(1, max + 1).ToString();
                    } else {
                        result += "," + r.Next(1, max + 1).ToString();
                    }
                }
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"你投出了{result}!"));
            }
            #endregion
            #region meme管理
            if (Regex.IsMatch(text.ToLower(), @"^setmeme ((equal|regexmatch|regexreplace|startswith) )?.+#.+")) {
                if (Meme.IsAdmin(e.Sender.Id)) {
                    var match = Regex.Match(text.ToLower(), @"^setmeme (?<mode>(equal|regexmatch|regexreplace|startswith) )?(?<key>.+)#(?<val>.+)$");

                    string key = match.Groups["key"].Value;
                    string val = match.Groups["val"].Value;
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine("{" + match.Groups["mode"].Value + "}");
                    MemeBase.MatchType type = match.Groups["mode"].Value switch
                    {
                        "equal " => MemeBase.MatchType.Equal,
                        "regexmatch " => MemeBase.MatchType.RegexMatch,
                        "regexreplace " => MemeBase.MatchType.RegexReplace,
                        "startswith " => MemeBase.MatchType.StartsWith,
                        _ => MemeBase.MatchType.Equal,
                    };
                    if (Meme.SetMeme(key, rep, type)) {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));
                    } else {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"设置{key}时出错"));
                    }
                    Meme.Save("meme.json");
                    if (File.Exists("/recordings/bot/meme.json")) {
                        File.Delete("/recordings/bot/meme.json");
                    }
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } else {
                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Name}({e.Sender.Id})的的建议\n{text}"));
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("已将建议转发给Light_Quanta"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                if (Meme.IsAdmin(e.Sender.Id)) {
                    string temp = text[8..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();

                    Meme.AddMemeReply(key, rep);
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));

                    Meme.Save("meme.json");
                    if (File.Exists("/recordings/bot/meme.json")) {
                        File.Delete("/recordings/bot/meme.json");
                    }
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } else {
                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Name}({e.Sender.Id})的的建议\n{text}"));
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("已将建议转发给Light_Quanta"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^addalias .+#.+")) {
                if (Meme.IsAdmin(e.Sender.Id)) {
                    string temp = text[9..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine(key);
                    Console.WriteLine(val);

                    if (Meme.AddMemeAlias(key, rep)) {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));
                    } else {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"添加{key}别名失败，请检查是否存在该meme以及该别名是否存在"));
                    }
                    Meme.Save("meme.json");
                    if (File.Exists("/recordings/bot/meme.json")) {
                        File.Delete("/recordings/bot/meme.json");
                    }
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } else {
                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Name}({e.Sender.Id})的的建议\n{text}"));
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("已将建议转发给Light_Quanta"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^remmeme .+#.+")) {
                if (Meme.IsAdmin(e.Sender.Id)) {
                    string temp = text[8..].ToLower();
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1].ToLower();

                    Meme.Save("meme.json");
                    if (File.Exists("/recordings/bot/meme.json")) {
                        File.Delete("/recordings/bot/meme.json");
                    }
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } else {
                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Name}({e.Sender.Id})的的建议\n{text}"));
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("已将建议转发给Light_Quanta"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmeme .+")) {
                string temp = text[8..];
                var memes = Meme.GetMeme(temp);
                if (memes != null) {
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(memes));
                } else {
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("未发现该meme"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^delmeme .+")) {
                if (Meme.IsAdmin(e.Sender.Id)) {
                    string temp = text[8..];
                    if (Meme.RemoveMeme(temp)) {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已移除{temp}"));
                    } else {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"未发现{temp}"));
                    }
                    Meme.Save("meme.json");
                    if (File.Exists("/recordings/bot/meme.json")) {
                        File.Delete("/recordings/bot/meme.json");
                    }
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } else {
                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Name}({e.Sender.Id})的的建议\n{text}"));
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("已将建议转发给Light_Quanta"));
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmemejson .+")) {
                string temp = text[12..];
                string json = Meme.GetMemeJson(temp);
                if (json != null) {
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(json));
                } else {
                    await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("未发现该meme"));
                }
            }
            if (text == "savememe") {
                Meme.Save("meme.json");
                if (File.Exists("/recordings/bot/meme.json")) {
                    File.Delete("/recordings/bot/meme.json");
                }
                File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                File.Copy("meme.json", "/recordings/bot/meme.json");
            }
            if (text == "reloadmeme") {
                try {
                    Meme = MemeManager.ReadFromFile("meme.json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } catch (Exception) { }
            }
            #endregion
            #region 强随机密码生成器
            if (Regex.IsMatch(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$")) {
                var m = Regex.Match(text, @"^[!！]pwd(?<len> \d+)?(?<pat> \S+)?$");
                string len = m.Groups["len"].Value;
                if (!int.TryParse(len, out int Length) || Length < 1 || Length > 64) {
                    Length = 16;
                }
                string pattern = m.Groups["pat"].Value;
                pattern = pattern.Length < 1 ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" : pattern[1..];
                await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(GetStrongRandomString(Length, pattern)));
            }
            #endregion
            if (e.Sender.Id != 2224899528) {
                return true;
            } else {
                if (text.Substring(0, 2) == "gm") {
                    await session.SendGroupMessageAsync(235107078, new PlainMessage(text.Substring(2)));
                }
                return true;
            }
        }
        public static string RandomMsg(params string[] s) {
            Random r = new Random();
            return s[(int) Math.Round(r.NextDouble() * (s.Length - 1))];
        }

        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e) {
            await session.SendTempMessageAsync(e.Sender.Id, e.Sender.Group.Id, new PlainMessage("请先添加好友"));
            return true;
        }

        public async Task<bool> BotInvitedJoinGroup(MiraiHttpSession session, IBotInvitedJoinGroupEventArgs e) {
            await session.HandleGroupApplyAsync(e, GroupApplyActions.Allow);
            return false;
        }

        /// <summary>
        /// 强随机字符串生成
        /// </summary>
        /// <param name="Length">字符串长度</param>
        /// <param name="Pattern">填充字符</param>
        /// <returns></returns>
        public static string GetStrongRandomString(int Length, string Pattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
            if (Length < 1 || Pattern.Length < 2) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Length; i++) {
                sb.Append(Pattern[RandomNumberGenerator.GetInt32(Pattern.Length)]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string GetMD5(string str) {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew) {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
    }
}
