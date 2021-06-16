using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Aliyun.OSS;
using LQ1Bot.Meme;
using Microsoft.Data.Sqlite;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Newtonsoft.Json.Linq;

namespace LQ1Bot {

    partial class LQ1Bot {
        #region HV
        private readonly string[] HiiroVoiceList = { "Astronomia.mp3", "Bad Apple.mp3", "Bad Apple2.mp3", "Butter-fly.mp3", "DD.mp3", "DD发言.mp3", "DokiDoki.mp3", "D都能D.mp3", "GMnya.mp3", "Hiiro的动听歌声.mp3", "How could you do that.mp3", "I can do it.mp3", "LSP.mp3", "Last Night, Good Night.mp3", "Last Night, Good Night？.mp3", "NTR.mp3", "No way, nonononoon.mp3", "PONPONPON.mp3", "Single Dog.mp3", "Weight Of The World.mp3", "are you bulibuli.mp3", "awsl.mp3", "awsl2.mp3", "awsl3.mp3", "baba.mp3", "darling.mp3", "debudebu.mp3", "debu猫.mp3", "gg.mp3", "goodbye~.mp3", "hiiro suki.mp3", "hiiro可爱吗.mp3", "hiiro开心.mp3", "hiiro要去睡觉了.mp3", "kimo.mp3", "kimo2.mp3", "kimo3.mp3", "kimo不喜欢起飞.mp3", "kimo连击.mp3", "king.mp3", "kksk.mp3", "lemon.mp3", "mua.mp3", "mua2.mp3", "m~u~a~.mp3", "nie × 9.mp3", "nieeeeeeeee.mp3", "nie↗↘.mp3", "no × 425.mp3", "no!.mp3", "no.mp3", "nononono.mp3", "nonononono.mp3", "nonononononononono.mp3", "nononopls.mp3", "nya.mp3", "nyanyanyanya.mp3", "nya~nyanya~.mp3", "nya~nya~nya~.mp3", "nya×9.mp3", "o ni i sa ma.mp3", "o ni i sa ma2.mp3", "ohnonononononono.mp3", "oh~my~.mp3", "okokok.mp3", "popcat.mp3", "prprpr.mp3", "prprprpr.mp3", "soft猫猫.mp3", "sososososososo.mp3", "summertime.mp3", "summertime2.mp3", "unravel.mp3", "wtf.mp3", "❤.mp3", "かくしん的☆めたまるふぉ~ぜっ!.mp3", "かくしん的☆めたまるふぉ~ぜっ!2.mp3", "ばかみたい.mp3", "ふたりのきもちのほんとのひみつ.mp3", "ふたりのきもちのほんとのひみつ2.mp3", "チカっとチカ千花っ(full).mp3", "チカっとチカ千花っ.mp3", "チカっとチカ千花っ2.mp3", "ワールドイズマイン.mp3", "不可思議のカルテ.mp3", "不可思議のカルテ2.mp3", "不对呢不对呢不对呢.mp3", "不是啊我不是.mp3", "不要用剪刀.mp3", "为什么.mp3", "为什么为什么.mp3", "为什么你不会有女朋友.mp3", "为什么你可以知道我是不是debu.mp3", "为什么喵.mp3", "什么时候debu可以站起来.mp3", "什么样的V什么样的D.mp3", "你不对啊.mp3", "你不对啊这个人.mp3", "你不是好人.mp3", "你不是我的老公.mp3", "你们不是好孩子.mp3", "你可以做我的狗吗.mp3", "你可以做我的狗吗2.mp3", "你可以做我的狗吗双语.mp3", "你可以做我的舔狗吗.mp3", "你好kimo啊.mp3", "你想跟我去睡觉吗.mp3", "你是单身狗没关系的.mp3", "你是狗.mp3", "你是谁.mp3", "你没有女朋友.mp3", "你的XP好奇怪.mp3", "你的朋友是你吧.mp3", "你这个debu.mp3", "先等一下.mp3", "全部老M.mp3", "八嘎.mp3", "八嘎2.mp3", "八嘎3.mp3", "八嘎八嘎.mp3", "八嘎八嘎八嘎.mp3", "关门了我关门了.mp3", "初见可爱单推.mp3", "别的老婆也好看.mp3", "别说了别说了别说了.mp3", "别骂了别骂了.mp3", "剪刀.mp3", "努力学习天天向上.mp3", "勾指起誓.mp3", "卡哇伊.mp3", "卡哇伊2.mp3", "去睡觉.mp3", "双语大哥哥.mp3", "口胡.mp3", "可以可以可以.mp3", "可以来抱抱我吗.mp3", "可恶.mp3", "可爱くなりたい(nya).mp3", "可爱くなりたい.mp3", "可爱くなりたい2.mp3", "可爱的声音.mp3", "可爱的声音10.mp3", "可爱的声音11.mp3", "可爱的声音12.mp3", "可爱的声音13.mp3", "可爱的声音14.mp3", "可爱的声音15.mp3", "可爱的声音2.mp3", "可爱的声音3.mp3", "可爱的声音4.mp3", "可爱的声音5.mp3", "可爱的声音6.mp3", "可爱的声音7.mp3", "可爱的声音8.mp3", "可爱的声音9.mp3", "吃桃.mp3", "吃桃2.mp3", "名言连击.mp3", "听不懂.mp3", "吶.mp3", "吸氧.mp3", "呀咩嗲.mp3", "呀咩嗲~呀~咩~嗲~.mp3", "呐呐呐呐呐.mp3", "呵↑呵↑呵↑呵↑呵↑.mp3", "呵↑呵↑呵↑呵↑呵↑2.mp3", "呼气.mp3", "哈x21.mp3", "哈↑哈↑哈↑哈↑.mp3", "哈↑哈↑哈↑哈↑哈↑哈↑.mp3", "哈哈哈哈哈.mp3", "哈？.mp3", "哦~.mp3", "哦尼酱.mp3", "哦尼酱~.mp3", "哭.mp3", "哭2.mp3", "哭3.mp3", "哼×5.mp3", "哼哼哼哼哼.mp3", "啊.mp3", "啊2.mp3", "啊~.mp3", "啊~啊~.mp3", "啊~啊~啊~.mp3", "啊~啊~啊~啊~.mp3", "啊呜.mp3", "啊呜2.mp3", "啊啊啊哈哈哈哈哈.mp3", "啊啊啊啊.mp3", "啊啊啊啊啊.mp3", "啊啊啊啊啊啊.mp3", "啊啊啊啊啊啊啊啊啊啊啊啊.mp3", "啊啊老公老公.mp3", "啊老公.mp3", "啊这.mp3", "啊这×3.mp3", "啊这个.mp3", "啊！这！个！.mp3", "喂喂喂喂喂.mp3", "喜欢.mp3", "喝水.mp3", "喵.mp3", "喵2.mp3", "喵~.mp3", "喵~喵~喵~喵~.mp3", "喵呜~.mp3", "喵喵喵.mp3", "喵喵喵喵喵喵喵喵.mp3", "嗷呜.mp3", "嘻嘻嘻嘻嘻嘻.mp3", "噢呜~.mp3", "多喝热水.mp3", "多喝热水2.mp3", "大哥哥.mp3", "大姐姐我没有女朋友啊.mp3", "大姐姐的声音.mp3", "大姐姐的声音2.mp3", "大姐姐谁不喜欢呢.mp3", "大家好我是hiiro.mp3", "天ノ弱.mp3", "天ノ弱2.mp3", "天天DD.mp3", "天天LSP.mp3", "天天爬.mp3", "太色了.mp3", "奇怪的声音.mp3", "奇怪的声音2.mp3", "奇怪的声音3.mp3", "女仆猫猫.mp3", "她是我的老婆.mp3", "好孩子.mp3", "好耶.mp3", "好耶喵.mp3", "好难呀.mp3", "妈妈我要吃苹果.mp3", "季姬击鸡记.mp3", "学猫叫.mp3", "害怕.mp3", "对不起.mp3", "对不起我不要.mp3", "对不起我刚刚起床.mp3", "对不起我错了.mp3", "开心吗.mp3", "开水.mp3", "开水2.mp3", "开玩笑开玩笑开玩笑开玩笑.mp3", "开玩笑的哦.mp3", "很好我好喜欢.mp3", "快乐星猫.mp3", "愛Dee.mp3", "愛言葉III.mp3", "我 不 是 debu.mp3", "我Lemon了.mp3", "我不同意.mp3", "我不喜欢你了.mp3", "我不是LSP.mp3", "我不是debu.mp3", "我不是啊.mp3", "我不清楚.mp3", "我不知道啊.mp3", "我不要骂你.mp3", "我不要（哭）.mp3", "我也喜欢大姐姐.mp3", "我们一起吃桃.mp3", "我们不是人.mp3", "我们可以吃桃.mp3", "我刚刚起床.mp3", "我可以给你一个好人卡.mp3", "我呢我呢我呢.mp3", "我呢我呢我呢2.mp3", "我喜欢你.mp3", "我好了.mp3", "我好了~.mp3", "我好可爱.mp3", "我好喜欢你啊.mp3", "我就是王牛奶.mp3", "我开始咯.mp3", "我想睡觉了.mp3", "我愿意.mp3", "我懂了我懂了.mp3", "我打你.mp3", "我打你们的屁股.mp3", "我打你啊.mp3", "我打你啊2.mp3", "我是LSP.mp3", "我是debu你是八嘎.mp3", "我是一个卡哇伊漂亮的哦捏桑.mp3", "我是一个可爱猫猫.mp3", "我是一个小孩子.mp3", "我是一个很可爱很甜菜的小猫猫.mp3", "我是你的爸爸.mp3", "我是你的老婆.mp3", "我是只好猫.mp3", "我是大姐姐了.mp3", "我是好人.mp3", "我是甜菜.mp3", "我是甜菜哒哟.mp3", "我有一个朋友(剧情版).mp3", "我有一个朋友.mp3", "我永远单推debiiro.mp3", "我爱你.mp3", "我生气了.mp3", "我的中文不好.mp3", "我的天呐.mp3", "我的天啊.mp3", "我的天啊2.mp3", "我的心好痛.mp3", "我的狗在哪.mp3", "我的王牛奶.mp3", "我的王牛奶结婚我了.mp3", "我笑死了.mp3", "我绿了.mp3", "我要去睡觉了.mp3", "我要哭啊.mp3", "我要哭啦.mp3", "我鲨了你.mp3", "所以我要在你怀里.mp3", "才八点.mp3", "打你啊.mp3", "打哈欠.mp3", "拜拜.mp3", "捶桌.mp3", "摩多摩多摩多.mp3", "施氏食狮史.mp3", "无路赛.mp3", "无路赛八嘎.mp3", "无路赛八嘎连击.mp3", "无路赛喵.mp3", "无路赛无路赛无路赛.mp3", "无路赛连击.mp3", "旺旺（bushi）.mp3", "明白了.mp3", "是开玩笑的哦.mp3", "晚上好.mp3", "晚安(日语).mp3", "晚安喵.mp3", "晚安晚安.mp3", "晚安晚安晚安晚安.mp3", "来了来了.mp3", "来击剑.mp3", "桥豆麻袋.mp3", "水开了.mp3", "永远的神.mp3", "没关系.mp3", "没问题.mp3", "没问题吧.mp3", "潮汐.mp3", "烧开水.mp3", "烧开水2.mp3", "烧开水3.mp3", "爸爸.mp3", "爸爸2.mp3", "爸爸要哭了.mp3", "狗狗.mp3", "猫SP.mp3", "猫之呼吸.mp3", "猫之呼吸2.mp3", "猫式呼吸.mp3", "猫猫.mp3", "猫猫伸懒腰.mp3", "王先生.mp3", "王小姐.mp3", "王小姐是我的老婆.mp3", "甜菜猫.mp3", "白金ディスコ.mp3", "稍等一下喵.mp3", "突然秃头了.mp3", "笑.mp3", "笑2.mp3", "笑3.mp3", "笑4.mp3", "笑5.mp3", "笑6.mp3", "笑7.mp3", "笑？.mp3", "米娜LSP.mp3", "米娜爱我吗.mp3", "糟糕的声音.mp3", "糟糕的声音2.mp3", "糟糕的声音3.mp3", "纳尼纳尼纳尼.mp3", "给你个好人卡.mp3", "给你好人卡.mp3", "给我红包.mp3", "老公~.mp3", "老公抱抱.mp3", "老婆~.mp3", "老婆~老婆~.mp3", "老板大气.mp3", "耳朵没有了.mp3", "耶~.mp3", "胖次.mp3", "自我介绍.mp3", "舔狗.mp3", "舔狗是真的牛逼.mp3", "花に亡霊.mp3", "莫多莫多.mp3", "菜B.mp3", "诶诶诶.mp3", "谁啊.mp3", "谁是LSP.mp3", "谢谢你啊.mp3", "起床hiiro.mp3", "起床啊你这个debu.mp3", "起飞.mp3", "辛苦了(幼).mp3", "辛苦了.mp3", "这个不对啊.mp3", "这个人不对.mp3", "这个人没问题吧.mp3", "这个重要吗.mp3", "这什么鬼东西.mp3", "这就是DD呢.mp3", "这是啥啊.mp3", "那个那个那个那个那个.mp3", "错了错了.mp3", "阿巴巴阿巴阿巴.mp3", "阿巴阿巴阿巴.mp3", "阿巴阿巴阿巴阿巴.mp3", "阿里噶多.mp3", "阿里噶多2.mp3", "阿里噶多3.mp3", "鳥の詩.mp3", "麻烦你原地去世.mp3" };
        #endregion

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            long q = e.Sender.Group.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"GroupMessage\t{q}");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{string.Join(null, (IEnumerable<IMessageBase>) e.Chain)}");
            string text = GetMessageText(e.Chain).Trim();
            Console.WriteLine(text);

            #region 启用/禁用控制
            if (text == "!enablebot") {
                if (e.Sender.Permission == GroupPermission.Administrator ||
                    e.Sender.Permission == GroupPermission.Owner ||
                    e.Sender.Id == 2224899528) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>();
                    } else {
                        BlacklistGroups.Remove(q);
                    }
                    await session.SendGroupMessageAsync(q, new PlainMessage("已在此群启用bot"));
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }

            if (BlacklistGroups?.Contains(q) ?? false) {
                return true;
            }

            if (text == "!banbot") {
                if (e.Sender.Permission == GroupPermission.Administrator ||
                    e.Sender.Permission == GroupPermission.Owner ||
                    e.Sender.Id == 2224899528) {
                    if (BlacklistGroups == null) {
                        BlacklistGroups = new HashSet<long>() { q };
                    } else {
                        BlacklistGroups.Add(q);
                    }
                    await session.SendGroupMessageAsync(q, new PlainMessage("已在此群禁用bot"));
                    File.WriteAllText("blacklist.json", JsonSerializer.Serialize(BlacklistGroups));
                    return true;
                }
            }
            #endregion
            #region meme回复管理
            if (Meme.IsAdmin(e.Sender.Id) && text == "y") {
                QuoteMessage qm = null;
                foreach (var v in e.Chain) {
                    if (v.Type == "Quote") {
                        qm = (QuoteMessage) v;
                    }
                }
                if (qm != null) {
                    string RepText = GetMessageText(qm.OriginChain);
                    if (Regex.IsMatch(RepText, @"^(set|add|rem)meme .+#.+|^delmeme .+|^addalias .+#.+")) {
                        text = RepText;
                    }
                }
            }
            #endregion
            #region 用户名检测
            if (e.Sender.Name.Length < 10 && IsCreeper(e.Sender.Name)) {
                GroupMemberCardInfo gmc = new GroupMemberCardInfo {
                    Name = "爬"
                };
                try {
                    await session.ChangeGroupMemberInfoAsync(e.Sender.Id, q, gmc);
                } catch (Exception eeeee) {
                    Console.WriteLine(eeeee.Message);
                }
            }
            #endregion
            switch (text.ToLower().Trim()) {
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("今天还没人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("昨天没人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("目前无人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("今天还没人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("昨天没人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
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
                            await session.SendGroupMessageAsync(q, new PlainMessage(rep + list));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("目前无人发草！"));
                        }
                    } catch (Exception ee) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("读取生草榜出现错误\n" + ee.Message);
                        Console.ResetColor();
                    }
                    break;
                #endregion
                #region 禁言抽奖
                case "禁言抽奖":
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
                    break;
                #endregion
                #region 一言
                case "一言":
                    (new Thread(new ThreadStart(() => {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("一言");
                        Console.ResetColor();
                        string result = "";
                        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(@"https://v1.hitokoto.cn/");
                        req.Timeout = 3000;
                        try {
                            HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                            Stream stream = resp.GetResponseStream();
                            using (StreamReader reader = new StreamReader(stream)) {
                                result = reader.ReadToEnd();
                            }
                            stream.Close();
                        } catch (Exception) { }
                        JObject jj = JObject.Parse(result);
                        string msg = jj["hitokoto"]?.ToString();
                        string from = jj["from_who"]?.ToString();
                        if (from != null && from != "" && from != "null")
                            session.SendGroupMessageAsync(q, new PlainMessage($"{msg}\t——{from}"));
                        else
                            session.SendGroupMessageAsync(q, new PlainMessage($"{msg}"));
                    }))).Start();
                    break;
                #endregion
                #region 手动报错
                case "来点bug":
                case "来点异常":
                case "来点错误":
                case "来点exception":
                    (new Thread(new ThreadStart(async () => {
                        var ExceptionType = ExceptionList[(new Random()).Next(ExceptionList.Count)];
                        try {
                            throw ((Exception) Activator.CreateInstance(ExceptionType));
                        } catch (Exception eee) {
                            await session.SendGroupMessageAsync(q, new PlainMessage(eee.ToString()));
                        }
                    }))).Start();
                    break;
                #endregion
                #region PicInfo
                case "picinfo":
                    List<string> picresult = new List<string>();
                    foreach (var msg in e.Chain) {
                        if (msg is ImageMessage tempmsg) {
                            picresult.Add(tempmsg.ImageId + " " + tempmsg.Url + "\n");
                        }
                    }
                    if (picresult.Count > 0) {
                        await session.SendGroupMessageAsync(q, new PlainMessage(string.Join("\n", picresult)));
                    }
                    break;
                #endregion
                #region 今天看谁
                case "今天看谁":
                case "今天d谁":
                    (new Thread(new ThreadStart(async () => {
                        try {
                            WebClient wc = new WebClient();
                            string res = wc.DownloadString("https://api.vtbs.moe/v1/vtbs");
                            JArray Vtbs = JArray.Parse(res);

                            int index = (new Random()).Next(Vtbs.Count);
                            JObject RndVtbId = (JObject) Vtbs[index];

                            string userId = RndVtbId["mid"].ToString();

                            var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + userId));

                            string userName = RndVtb["uname"].ToString();
                            string roomId = RndVtb["roomid"].ToString();
                            string faceUrl = RndVtb["face"].ToString() + "@150h";
                            string followers = RndVtb["follower"].ToString();
                            string online = RndVtb["online"].ToString();
                            string sign = RndVtb["sign"].ToString();
                            string title = RndVtb["title"].ToString();
                            MessageBuilder b = new MessageBuilder();
                            b.Add(new ImageMessage(null, faceUrl, null));

                            if (online == "0") {
                                b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}
直播间地址：https://live.bilibili.com/{roomId}"));
                            } else {
                                b.Add(new PlainMessage($@"名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{userId}

当前正在直播！
直播间标题：{title}
直播间地址：https://live.bilibili.com/{roomId}"));
                            }
                            await session.SendGroupMessageAsync(q, b);
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                            await session.SendGroupMessageAsync(q, new PlainMessage("获取vtb信息出错"));
                        }
                    }))).Start();
                    break;
                #endregion
                #region 现在看谁
                case "现在看谁":
                case "现在d谁":
                    (new Thread(new ThreadStart(async () => {
                        try {
                            WebClient wc = new WebClient();
                            string res = wc.DownloadString("https://api.vtbs.moe/v1/living");
                            JArray Vtbs = JArray.Parse(res);

                            int index = (new Random()).Next(Vtbs.Count);
                            long RndVtbId = Vtbs[index].ToObject<long>();

                            var RoomInfo = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/room/" + RndVtbId));
                            string uid = RoomInfo["uid"].ToString();
                            string popularity = RoomInfo["popularity"].ToString();

                            var RndVtb = JObject.Parse(wc.DownloadString("https://api.vtbs.moe/v1/detail/" + uid));

                            string userName = RndVtb["uname"].ToString();
                            string roomId = RndVtb["roomid"].ToString();
                            string faceUrl = RndVtb["face"].ToString() + "@150h";
                            string followers = RndVtb["follower"].ToString();
                            string online = RndVtb["online"].ToString();
                            string sign = RndVtb["sign"].ToString();
                            string title = RndVtb["title"].ToString();
                            MessageBuilder b = new MessageBuilder();
                            b.Add(new ImageMessage(null, faceUrl, null));

                            b.Add(new PlainMessage($@"{title}
https://live.bilibili.com/{roomId}
人气：{popularity}

vtb信息
名称：{userName}
签名：{sign}
粉丝数：{followers}
主页地址：https://space.bilibili.com/{uid}"));
                            await session.SendGroupMessageAsync(q, b);
                        } catch (Exception e) {
                            Console.WriteLine(e.Message);
                            await session.SendGroupMessageAsync(q, new PlainMessage("获取vtb信息出错"));
                        }
                    }))).Start();
                    break;
                #endregion
                default:
                    #region 爬行者判断
                    if (IsCreeper(text) && text.IndexOf("http://") == -1 && text.IndexOf("https://") == -1) {
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
                    #endregion
                    #region 无情的接梗机器
                    if (text == "再来点") {
                        if (LastMeme.TryGetValue(q, out string last)) {
                            text = last;
                        }
                    }
                    if (text == "来点hiiro" || text == "來點hiiro") {
                        LastMeme.Remove(q);
                        LastMeme.Add(q, text);
                        string url = @"https://cdn.jsdelivr.net/gh/blacktunes/hiiro-button@master/public/voices/" + System.Web.HttpUtility.UrlEncode(RandomMsg(HiiroVoiceList), Encoding.Default);
                        await session.SendGroupMessageAsync(q, new VoiceMessage(null, url, null));
                        return true;
                    }
                    if (Meme.HasReply(text)) {
                        var rep = Meme.GetReply(text, q);
                        if (rep != null) {
                            LastMeme.Remove(q);
                            LastMeme.Add(q, text);
                            if (rep.StartsWith("[picture]") && rep.Length > 9) {
                                await session.SendImageToGroupAsync(q, new string[] { $"http://127.0.0.1:23333/botpicture/{rep[9..]}" });
                            } else {
                                int n = await session.SendGroupMessageAsync(q, new PlainMessage(rep));
                                if (rep.Length > 200) {
                                    (new Thread(new ThreadStart(async () => {
                                        Thread.Sleep(60000);
                                        try {
                                            await session.RevokeMessageAsync(n);
                                        } catch (Exception) { }
                                    }))).Start();
                                }
                                MsgRepeat.Remove(q);
                            }
                        }
                    }
                    if (text.ToLower() == "guid") {
                        await session.SendGroupMessageAsync(q, new PlainMessage(Guid.NewGuid().ToString()));
                    }
                    if (text.ToLower() == "来点原神kfc") {
                        LastMeme.Remove(q);
                        LastMeme.Add(q, text);
                        await session.SendGroupMessageAsync(q, new VoiceMessage(null, @"https://img.nga.178.com/attachments/mon_202103/08/i2Qj0k-i7puZg.mp3?duration=40%A1%E5%E2%80%8B", null));
                    }
                    #endregion
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
                                            session.SendGroupMessageAsync(q, new PlainMessage("未找到该玩家！"));
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
                                                + hit_accuracy + "\r游玩次数：\t"
                                                + play_count + " (" + (play_count - playcount1 >= 0 ? "+" : "") + (play_count - playcount1) + ")\r总分：\t\t"
                                                + total_score + "\r总命中次数：\t"
                                                + total_hits + "\r最大连击数：\t"
                                                + maximum_combo;
                                            session.SendGroupMessageAsync(q, new PlainMessage(info));
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
                                                    + "0.0%" + "\r游玩次数：\t"
                                                    + p.playcount + "\r总分：\t\t"
                                                    + "0" + "\r总命中次数：\t"
                                                    + "0" + "\r最大连击数：\t"
                                                    + "0";
                                        session.SendGroupMessageAsync(q, new PlainMessage(info));
                                        Console.WriteLine("Query Ended");
                                    } catch (Exception e) {
                                        Console.WriteLine(e.Message);
                                        session.SendGroupMessageAsync(q, new PlainMessage(e.Message));
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
                    #region DICE
                    if (Regex.IsMatch(text.ToLower().Trim(), @"^dice$|^dice \d+d\d+$")) {
                        MsgRepeat.Remove(q);
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
                        string res = "";
                        for (int i = 0; i < count; i++) {
                            if (i == 0) {
                                res += r.Next(1, max + 1).ToString();
                            } else {
                                res += "," + r.Next(1, max + 1).ToString();
                            }
                        }
                        await session.SendGroupMessageAsync(q, new PlainMessage($"{e.Sender.Name}投出了{res}！"));
                    }
                    #endregion
                    #region 草记录
                    if (Regex.IsMatch(text.ToLower(), @"[草艸艹🌿]|w{3,}|kusa|cao|grass]")) {
                        Console.WriteLine("草");
                        try {
                            SqliteConnection conn = new SqliteConnection("Data Source=chat.db");
                            conn.Open();
                            SqliteCommand cmd = new SqliteCommand("", conn) {
                                CommandText = $"insert into main values (strftime('%Y-%m-%d %H:%M:%f','now','localtime'),@nickname,{e.Sender.Id},{q},'cao')"
                            };
                            cmd.Parameters.AddWithValue("@nickname", e.Sender.Name);

                            SqliteDataReader dr = cmd.ExecuteReader();
                            dr.Close();
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine("成功写入草，用户名：\t" + e.Sender.Name);

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
                                await session.MuteAsync(e.Sender.Id, q, TimeSpan.FromMinutes(3.0));
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                Console.WriteLine("已禁言用户");
                                Console.ResetColor();
                                await session.SendGroupMessageAsync(q, new PlainMessage("禁止刷屏"));
                            }
                        } catch (Exception ee) {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("写入数据库出现错误\n" + ee.Message);
                            Console.ResetColor();
                        }
                    }
                    #endregion
                    #region 自定义禁言
                    if (Regex.IsMatch(text.ToLower().Trim(), @"^自助禁言 \d+$")) {
                        if (e.Sender.Permission == GroupPermission.Administrator || e.Sender.Permission == GroupPermission.Owner) {
                            await session.SendGroupMessageAsync(q, new PlainMessage("在？有种把管理卸了"));
                            return false;
                        }

                        int stime = int.Parse(text.Trim()[5..]);

                        if (stime <= 0) {
                            await session.SendGroupMessageAsync(q, new PlainMessage("时间都打不对还想领禁言套餐？"));
                        } else {
                            stime = stime > 43200 ? 43200 : stime;

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
                    }
                    #endregion
                    #region 复读机
                    if (MsgRepeat.TryGetValue(q, out var valll)) {
                        MsgRepeat.Remove(q);
                        if (valll.Msg == text) {
                            MsgRepeat.Add(q, (text, valll.Count + 1));
                            if (valll.Count >= 2 && (new Random()).Next(1, 11) > (valll.Count + 3)) {
                                await session.SendGroupMessageAsync(q, new PlainMessage(text));
                            }
                        }
                    } else {
                        MsgRepeat.Add(q, (text, 1));
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
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));
                            } else {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"设置{key}时出错"));
                            }
                            Meme.Save("meme.json");
                            File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                        } else {
                            await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"));
                            await session.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                        }
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                        if (Meme.IsAdmin(e.Sender.Id)) {
                            string temp = text[8..];
                            string key = temp.Split('#')[0];
                            string val = temp.Split('#')[1];
                            HashSet<string> rep = val.Split("|").ToHashSet();

                            Meme.AddMemeReply(key, rep);
                            await session.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));

                            Meme.Save("meme.json");
                            File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                        } else { 
                            await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"));
                            await session.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
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
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));
                            } else {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"添加{key}别名失败，请检查是否存在该meme以及该别名是否存在"));
                            }
                            Meme.Save("meme.json");
                            File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                        } else {
                            await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"));
                            await session.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                        }
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^remmeme .+#.+")) {
                        if (Meme.IsAdmin(e.Sender.Id)) {
                            string temp = text[8..].ToLower();
                            string key = temp.Split('#')[0].ToLower();
                            string val = temp.Split('#')[1].ToLower();

                            Meme.Save("meme.json");
                            File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                        } else {
                            await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"));
                            await session.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                        }
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^getmeme .+")) {
                        string temp = text[8..];
                        var memes = Meme.GetMeme(temp);
                        if (memes != null) {
                            await session.SendGroupMessageAsync(q, new PlainMessage(memes));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("未发现该meme"));
                        }
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^delmeme .+")) {
                        if (Meme.IsAdmin(e.Sender.Id)) {
                            string temp = text[8..];
                            if (Meme.RemoveMeme(temp)) {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已移除{temp}"));
                            } else {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"未发现{temp}"));
                            }
                            Meme.Save("meme.json");
                            File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                        } else {
                            await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"));
                            await session.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                        }
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^getmemejson .+")) {
                        string temp = text[12..];
                        string json = Meme.GetMemeJson(temp);
                        if (json != null) {
                            await session.SendGroupMessageAsync(q, new PlainMessage(json));
                        } else {
                            await session.SendGroupMessageAsync(q, new PlainMessage("未发现该meme"));
                        }
                    }
                    if (text == "savememe") {
                        Meme.Save("meme.json");
                        File.Copy("meme.json", Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                    }
                    if (text == "reloadmeme") {
                        try {
                            Meme = MemeManager.ReadFromFile("meme.json");
                            File.Copy("meme.json", "/recordings/bot/meme.json");
                        } catch (Exception) { }
                    }
                    if (text == "uploadpic") {
                        if (e.Chain.Length == 3 && (e.Chain[2] is ImageMessage im)) {
                            string FileName = im.ImageId;
                            if (!File.Exists("/recordings/botpicture/" + FileName + ".png")) {
                                if (Meme.IsAdmin(e.Sender.Id)) {
                                    (new Thread(new ThreadStart(async () => {
                                        try {
                                            WebRequest imgRequest = WebRequest.Create(im.Url);
                                            HttpWebResponse res = (HttpWebResponse) imgRequest.GetResponse();
                                            Image downImage = Image.FromStream(res.GetResponseStream());
                                            downImage.Save("/recordings/botpicture/" + FileName);
                                            await session.SendGroupMessageAsync(q, new PlainMessage($"上传图片{FileName}成功！"));
                                            Console.WriteLine(im.ImageId);
                                        } catch (Exception e) {
                                            Console.WriteLine(e.Message);
                                            await session.SendGroupMessageAsync(q, new PlainMessage("上传图片出错"));
                                        }
                                    }))).Start();

                                } else {
                                    await session.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的图片上传请求\n图片名称：{FileName}"), new ImageMessage(im.ImageId, im.Url, null));
                                    await session.SendGroupMessageAsync(q, new PlainMessage($"已将建议转发给Light_Quanta\n图片名称：{FileName}"));
                                }
                            } else {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"该图片{FileName}已存在！"));
                            }
                        }
                    }
                    #endregion
                    #region 抽奖
                    if (Regex.IsMatch(text.ToLower(), @"^[!！]pick start \d{1,3}/\d{1,10} \S+$")) {
                        int PickCount = int.Parse(text.ToLower().Split(' ')[2].Split('/')[0]);
                        int Total = int.Parse(text.ToLower().Split(' ')[2].Split('/')[1]);
                        string name = text.ToLower().Split(' ')[3];

                        if (PickCount < 1) {
                            await session.SendGroupMessageAsync(q, new PlainMessage($"能教我一下怎么抽{PickCount}个人吗？"));
                            return true;
                        }
                        if (PickCount > Total) {
                            await session.SendGroupMessageAsync(q, new PlainMessage($"能教我一下怎么从{Total}个人中抽出{PickCount}个人吗？"));
                            return true;
                        }

                        foreach (var v in PickRecord) {
                            if (v.Name == name) {
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已存在名称为{name}的抽奖！"));
                                return true;
                            }
                        }

                        PickRecord.Add(new Pick() {
                            Name = name,
                            qq = e.Sender.Id,
                            PickCount = PickCount,
                            Total = Total,
                            Player = new Dictionary<long, string>()
                        });

                        await session.SendGroupMessageAsync(q, new PlainMessage($"已添加名称为{name}的抽奖！\n将从最多{Total}人中抽取{PickCount}人\n输入!pick join {name}即可参与抽奖"));
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^[!！]pick join \S+$")) {
                        MsgRepeat.Remove(q);
                        string name = text.ToLower().Split(' ')[2];
                        foreach (var v in PickRecord) {
                            if (v.Name == name) {
                                if (v.Player.Count < v.Total) {
                                    if (v.Player.ContainsKey(e.Sender.Id)) {
                                        await session.SendGroupMessageAsync(q, new PlainMessage("您已参与该抽奖！"));
                                    } else {
                                        v.Player.Add(e.Sender.Id, e.Sender.Name);
                                        await session.SendGroupMessageAsync(q, new PlainMessage($"成功参与抽奖！当前参与人数：{v.Player.Count}/{v.Total}"));
                                    }
                                } else {
                                    await session.SendGroupMessageAsync(q, new PlainMessage($"抽奖人数已达{v.Total}人上限！"));
                                }
                                return true;
                            }
                        }
                        await session.SendGroupMessageAsync(q, new PlainMessage("未找到该抽奖！"));
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^[!！]pick end \S+$")) {
                        string name = text.ToLower().Split(' ')[2];
                        foreach (var v in PickRecord) {
                            if (v.Name == name) {
                                if (v.qq != e.Sender.Id && e.Sender.Permission != GroupPermission.Administrator && e.Sender.Permission != GroupPermission.Owner) {
                                    await session.SendGroupMessageAsync(q, new PlainMessage("您不是该抽奖的发起者！"));
                                } else {
                                    if (v.Player.Count < v.PickCount) {
                                        await session.SendGroupMessageAsync(q, new PlainMessage($"参与抽奖的人数未达到抽奖所需的最小人数！\n当前参与抽奖的人数为:{v.Player.Count}/{v.Total}，至少要有{v.PickCount}人参与才能抽奖！"));
                                    } else {
                                        var l = new List<long>();
                                        foreach (var vv in v.Player) {
                                            l.Add(vv.Key);
                                        }
                                        var ll = new List<long>();
                                        for (int i = 0; i < v.PickCount; i++) {
                                            int rnd = (new Random()).Next(0, l.Count);
                                            ll.Add(l[rnd]);
                                            l.RemoveAt(rnd);
                                        }
                                        var lllll = new List<string>();
                                        foreach (var vv in ll) {
                                            v.Player.TryGetValue(vv, out string temp);
                                            lllll.Add(temp);
                                        }
                                        string resultt = string.Join(",", lllll.ToArray());
                                        await session.SendGroupMessageAsync(q, new PlainMessage($"恭喜{resultt}中奖！"));
                                        PickRecord.Remove(v);
                                    }
                                }
                                return true;
                            }
                        }
                        await session.SendGroupMessageAsync(q, new PlainMessage("未找到该抽奖！"));
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^[!！]pick remove \S+$")) {
                        string name = text.ToLower().Split(' ')[2];
                        foreach (var v in PickRecord) {
                            if (v.Name == name) {
                                if (e.Sender.Id != v.qq && e.Sender.Permission != GroupPermission.Administrator && e.Sender.Permission != GroupPermission.Owner) {
                                    await session.SendGroupMessageAsync(q, new PlainMessage("你不是该抽奖的发起者！"));
                                    return true;
                                } else {
                                    PickRecord.Remove(v);
                                    await session.SendGroupMessageAsync(q, new PlainMessage($"已移除{name}"));
                                    return true;
                                }
                            }
                        }
                        await session.SendGroupMessageAsync(q, new PlainMessage("未找到该抽奖！"));
                    }
                    if (Regex.IsMatch(text.ToLower(), @"^[!！]pick info$")) {
                        if (PickRecord.Count == 0) {
                            await session.SendGroupMessageAsync(q, new PlainMessage("当前还没有抽奖！"));
                        } else {
                            string resultt = "当前所有抽奖";
                            foreach (var v in PickRecord) {
                                resultt += $"\n{v.Name}，从最多{v.Total}人中抽取{v.PickCount}人，目前已有{v.Player.Count}人参与";
                            }
                            await session.SendGroupMessageAsync(q, new PlainMessage(resultt));
                        }
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
                        await session.SendGroupMessageAsync(q, new PlainMessage(GetStrongRandomString(Length, pattern)));
                    }
                    #endregion
                    #region 机翻
                    if (Regex.IsMatch(text, @"^翻译(\w+)? .+$")) {
                        string ToTranslate = text[(text.IndexOf(' ') + 1)..];
                        string Lang = text[2] == ' ' ? "zh" : text.Split(' ')[0][2..];
                        string AppId = Secret.BaiduTranslateAppId;
                        string salt = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();
                        Console.WriteLine(salt);
                        string sign = GetMD5(AppId + ToTranslate + salt + Secret.BaiduTranslateSecret);
                        Console.WriteLine(sign);
                        string url = @"http://" + @$"api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(ToTranslate)}&from=auto&to={Lang}&appid={AppId}&salt={salt}&sign={sign}";
                        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                        req.Timeout = 5000;
                        string result = "";
                        (new Thread(new ThreadStart(async () => {
                            try {
                                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                                Stream stream = resp.GetResponseStream();
                                using (StreamReader reader = new StreamReader(stream)) {
                                    result = reader.ReadToEnd();
                                }
                                stream.Close();
                                Console.WriteLine(result);
                                JObject o = JObject.Parse(result);
                                string TranslationResult = o["trans_result"][0]["dst"].ToString();
                                await session.SendGroupMessageAsync(q, new PlainMessage($"翻译结果：{TranslationResult}"));
                            } catch (Exception eee) {
                                Console.WriteLine(eee.Message);
                                await session.SendGroupMessageAsync(q, new PlainMessage("获取翻译出错"));
                            }
                        }))).Start();
                    }
                    #endregion
                    #region 自动翻译
                    if (AutoTranslateList.ContainsKey(e.Sender.Id)) {
                        string ToTranslate = text;
                        string Lang = AutoTranslateList.GetValueOrDefault(e.Sender.Id);
                        string AppId = Secret.BaiduTranslateAppId;
                        string salt = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();
                        Console.WriteLine(salt);
                        string sign = GetMD5(AppId + ToTranslate + salt + Secret.BaiduTranslateSecret);
                        Console.WriteLine(sign);
                        string url = @"http://" + @$"api.fanyi.baidu.com/api/trans/vip/translate?q={HttpUtility.UrlEncode(ToTranslate)}&from=auto&to={Lang}&appid={AppId}&salt={salt}&sign={sign}";
                        HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
                        req.Timeout = 5000;
                        string result = "";
                        (new Thread(new ThreadStart(async () => {
                            try {
                                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                                Stream stream = resp.GetResponseStream();
                                using (StreamReader reader = new StreamReader(stream)) {
                                    result = reader.ReadToEnd();
                                }
                                stream.Close();
                                Console.WriteLine(result);
                                JObject o = JObject.Parse(result);
                                if (o["from"].ToString() != Lang) {
                                    string TranslationResult = o["trans_result"][0]["dst"].ToString();
                                    await session.SendGroupMessageAsync(q, new PlainMessage($"【{TranslationResult}】"));
                                }
                            } catch (Exception) { }
                        }))).Start();
                    }
                    if (Regex.IsMatch(text, @"^autotranslate \d+ \w+")) {
                        if (long.TryParse(text.Split(' ')[1], out long target)) {
                            string TargetLang = text.Split(' ')[2];
                            //IGroupMemberCardInfo info;
                            //try {
                            //    info = await session.GetGroupMemberInfoAsync(target, q);
                            //} catch (Exception) {
                            //    await session.SendGroupMessageAsync(q, new PlainMessage($"未发现该群成员！"));
                            //    return false;
                            //}
                            if (TargetLang == "reset") {
                                AutoTranslateList.Remove(target);
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已为QQ号{target}关闭自动翻译"));
                                return true;
                            } else {
                                AutoTranslateList.Remove(target);
                                AutoTranslateList.Add(target, text.Split(' ')[2]);
                                await session.SendGroupMessageAsync(q, new PlainMessage($"已为QQ号{target}开启自动翻译"));
                            }
                        }
                    }
                    #endregion                
                    break;
            }
            return false;
        }
    }
}