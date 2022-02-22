using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using LQ1Bot.Meme;
using Mirai.Net.Data.Messages.Concretes;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class Meme : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9998;

        public override string PluginName => "Meme";
        public override bool CanDisable => true;

        private MemeManager MemeMgr;

        private readonly Dictionary<string, string> LastMeme = new Dictionary<string, string>();

        private readonly Dictionary<string, (string, string)> Rep = new Dictionary<string, (string, string)>();

        #region HV
        private readonly string[] HiiroVoiceList = { "Astronomia.mp3", "Bad Apple.mp3", "Bad Apple2.mp3", "Butter-fly.mp3", "DD.mp3", "DD发言.mp3", "DokiDoki.mp3", "D都能D.mp3", "GMnya.mp3", "Hiiro的动听歌声.mp3", "How could you do that.mp3", "I can do it.mp3", "LSP.mp3", "Last Night, Good Night.mp3", "Last Night, Good Night？.mp3", "NTR.mp3", "No way, nonononoon.mp3", "PONPONPON.mp3", "Single Dog.mp3", "Weight Of The World.mp3", "are you bulibuli.mp3", "awsl.mp3", "awsl2.mp3", "awsl3.mp3", "baba.mp3", "darling.mp3", "debudebu.mp3", "debu猫.mp3", "gg.mp3", "goodbye~.mp3", "hiiro suki.mp3", "hiiro可爱吗.mp3", "hiiro开心.mp3", "hiiro要去睡觉了.mp3", "kimo.mp3", "kimo2.mp3", "kimo3.mp3", "kimo不喜欢起飞.mp3", "kimo连击.mp3", "king.mp3", "kksk.mp3", "lemon.mp3", "mua.mp3", "mua2.mp3", "m~u~a~.mp3", "nie × 9.mp3", "nieeeeeeeee.mp3", "nie↗↘.mp3", "no × 425.mp3", "no!.mp3", "no.mp3", "nononono.mp3", "nonononono.mp3", "nonononononononono.mp3", "nononopls.mp3", "nya.mp3", "nyanyanyanya.mp3", "nya~nyanya~.mp3", "nya~nya~nya~.mp3", "nya×9.mp3", "o ni i sa ma.mp3", "o ni i sa ma2.mp3", "ohnonononononono.mp3", "oh~my~.mp3", "okokok.mp3", "popcat.mp3", "prprpr.mp3", "prprprpr.mp3", "soft猫猫.mp3", "sososososososo.mp3", "summertime.mp3", "summertime2.mp3", "unravel.mp3", "wtf.mp3", "❤.mp3", "かくしん的☆めたまるふぉ~ぜっ!.mp3", "かくしん的☆めたまるふぉ~ぜっ!2.mp3", "ばかみたい.mp3", "ふたりのきもちのほんとのひみつ.mp3", "ふたりのきもちのほんとのひみつ2.mp3", "チカっとチカ千花っ(full).mp3", "チカっとチカ千花っ.mp3", "チカっとチカ千花っ2.mp3", "ワールドイズマイン.mp3", "不可思議のカルテ.mp3", "不可思議のカルテ2.mp3", "不对呢不对呢不对呢.mp3", "不是啊我不是.mp3", "不要用剪刀.mp3", "为什么.mp3", "为什么为什么.mp3", "为什么你不会有女朋友.mp3", "为什么你可以知道我是不是debu.mp3", "为什么喵.mp3", "什么时候debu可以站起来.mp3", "什么样的V什么样的D.mp3", "你不对啊.mp3", "你不对啊这个人.mp3", "你不是好人.mp3", "你不是我的老公.mp3", "你们不是好孩子.mp3", "你可以做我的狗吗.mp3", "你可以做我的狗吗2.mp3", "你可以做我的狗吗双语.mp3", "你可以做我的舔狗吗.mp3", "你好kimo啊.mp3", "你想跟我去睡觉吗.mp3", "你是单身狗没关系的.mp3", "你是狗.mp3", "你是谁.mp3", "你没有女朋友.mp3", "你的XP好奇怪.mp3", "你的朋友是你吧.mp3", "你这个debu.mp3", "先等一下.mp3", "全部老M.mp3", "八嘎.mp3", "八嘎2.mp3", "八嘎3.mp3", "八嘎八嘎.mp3", "八嘎八嘎八嘎.mp3", "关门了我关门了.mp3", "初见可爱单推.mp3", "别的老婆也好看.mp3", "别说了别说了别说了.mp3", "别骂了别骂了.mp3", "剪刀.mp3", "努力学习天天向上.mp3", "勾指起誓.mp3", "卡哇伊.mp3", "卡哇伊2.mp3", "去睡觉.mp3", "双语大哥哥.mp3", "口胡.mp3", "可以可以可以.mp3", "可以来抱抱我吗.mp3", "可恶.mp3", "可爱くなりたい(nya).mp3", "可爱くなりたい.mp3", "可爱くなりたい2.mp3", "可爱的声音.mp3", "可爱的声音10.mp3", "可爱的声音11.mp3", "可爱的声音12.mp3", "可爱的声音13.mp3", "可爱的声音14.mp3", "可爱的声音15.mp3", "可爱的声音2.mp3", "可爱的声音3.mp3", "可爱的声音4.mp3", "可爱的声音5.mp3", "可爱的声音6.mp3", "可爱的声音7.mp3", "可爱的声音8.mp3", "可爱的声音9.mp3", "吃桃.mp3", "吃桃2.mp3", "名言连击.mp3", "听不懂.mp3", "吶.mp3", "吸氧.mp3", "呀咩嗲.mp3", "呀咩嗲~呀~咩~嗲~.mp3", "呐呐呐呐呐.mp3", "呵↑呵↑呵↑呵↑呵↑.mp3", "呵↑呵↑呵↑呵↑呵↑2.mp3", "呼气.mp3", "哈x21.mp3", "哈↑哈↑哈↑哈↑.mp3", "哈↑哈↑哈↑哈↑哈↑哈↑.mp3", "哈哈哈哈哈.mp3", "哈？.mp3", "哦~.mp3", "哦尼酱.mp3", "哦尼酱~.mp3", "哭.mp3", "哭2.mp3", "哭3.mp3", "哼×5.mp3", "哼哼哼哼哼.mp3", "啊.mp3", "啊2.mp3", "啊~.mp3", "啊~啊~.mp3", "啊~啊~啊~.mp3", "啊~啊~啊~啊~.mp3", "啊呜.mp3", "啊呜2.mp3", "啊啊啊哈哈哈哈哈.mp3", "啊啊啊啊.mp3", "啊啊啊啊啊.mp3", "啊啊啊啊啊啊.mp3", "啊啊啊啊啊啊啊啊啊啊啊啊.mp3", "啊啊老公老公.mp3", "啊老公.mp3", "啊这.mp3", "啊这×3.mp3", "啊这个.mp3", "啊！这！个！.mp3", "喂喂喂喂喂.mp3", "喜欢.mp3", "喝水.mp3", "喵.mp3", "喵2.mp3", "喵~.mp3", "喵~喵~喵~喵~.mp3", "喵呜~.mp3", "喵喵喵.mp3", "喵喵喵喵喵喵喵喵.mp3", "嗷呜.mp3", "嘻嘻嘻嘻嘻嘻.mp3", "噢呜~.mp3", "多喝热水.mp3", "多喝热水2.mp3", "大哥哥.mp3", "大姐姐我没有女朋友啊.mp3", "大姐姐的声音.mp3", "大姐姐的声音2.mp3", "大姐姐谁不喜欢呢.mp3", "大家好我是hiiro.mp3", "天ノ弱.mp3", "天ノ弱2.mp3", "天天DD.mp3", "天天LSP.mp3", "天天爬.mp3", "太色了.mp3", "奇怪的声音.mp3", "奇怪的声音2.mp3", "奇怪的声音3.mp3", "女仆猫猫.mp3", "她是我的老婆.mp3", "好孩子.mp3", "好耶.mp3", "好耶喵.mp3", "好难呀.mp3", "妈妈我要吃苹果.mp3", "季姬击鸡记.mp3", "学猫叫.mp3", "害怕.mp3", "对不起.mp3", "对不起我不要.mp3", "对不起我刚刚起床.mp3", "对不起我错了.mp3", "开心吗.mp3", "开水.mp3", "开水2.mp3", "开玩笑开玩笑开玩笑开玩笑.mp3", "开玩笑的哦.mp3", "很好我好喜欢.mp3", "快乐星猫.mp3", "愛Dee.mp3", "愛言葉III.mp3", "我 不 是 debu.mp3", "我Lemon了.mp3", "我不同意.mp3", "我不喜欢你了.mp3", "我不是LSP.mp3", "我不是debu.mp3", "我不是啊.mp3", "我不清楚.mp3", "我不知道啊.mp3", "我不要骂你.mp3", "我不要（哭）.mp3", "我也喜欢大姐姐.mp3", "我们一起吃桃.mp3", "我们不是人.mp3", "我们可以吃桃.mp3", "我刚刚起床.mp3", "我可以给你一个好人卡.mp3", "我呢我呢我呢.mp3", "我呢我呢我呢2.mp3", "我喜欢你.mp3", "我好了.mp3", "我好了~.mp3", "我好可爱.mp3", "我好喜欢你啊.mp3", "我就是王牛奶.mp3", "我开始咯.mp3", "我想睡觉了.mp3", "我愿意.mp3", "我懂了我懂了.mp3", "我打你.mp3", "我打你们的屁股.mp3", "我打你啊.mp3", "我打你啊2.mp3", "我是LSP.mp3", "我是debu你是八嘎.mp3", "我是一个卡哇伊漂亮的哦捏桑.mp3", "我是一个可爱猫猫.mp3", "我是一个小孩子.mp3", "我是一个很可爱很甜菜的小猫猫.mp3", "我是你的爸爸.mp3", "我是你的老婆.mp3", "我是只好猫.mp3", "我是大姐姐了.mp3", "我是好人.mp3", "我是甜菜.mp3", "我是甜菜哒哟.mp3", "我有一个朋友(剧情版).mp3", "我有一个朋友.mp3", "我永远单推debiiro.mp3", "我爱你.mp3", "我生气了.mp3", "我的中文不好.mp3", "我的天呐.mp3", "我的天啊.mp3", "我的天啊2.mp3", "我的心好痛.mp3", "我的狗在哪.mp3", "我的王牛奶.mp3", "我的王牛奶结婚我了.mp3", "我笑死了.mp3", "我绿了.mp3", "我要去睡觉了.mp3", "我要哭啊.mp3", "我要哭啦.mp3", "我鲨了你.mp3", "所以我要在你怀里.mp3", "才八点.mp3", "打你啊.mp3", "打哈欠.mp3", "拜拜.mp3", "捶桌.mp3", "摩多摩多摩多.mp3", "施氏食狮史.mp3", "无路赛.mp3", "无路赛八嘎.mp3", "无路赛八嘎连击.mp3", "无路赛喵.mp3", "无路赛无路赛无路赛.mp3", "无路赛连击.mp3", "旺旺（bushi）.mp3", "明白了.mp3", "是开玩笑的哦.mp3", "晚上好.mp3", "晚安(日语).mp3", "晚安喵.mp3", "晚安晚安.mp3", "晚安晚安晚安晚安.mp3", "来了来了.mp3", "来击剑.mp3", "桥豆麻袋.mp3", "水开了.mp3", "永远的神.mp3", "没关系.mp3", "没问题.mp3", "没问题吧.mp3", "潮汐.mp3", "烧开水.mp3", "烧开水2.mp3", "烧开水3.mp3", "爸爸.mp3", "爸爸2.mp3", "爸爸要哭了.mp3", "狗狗.mp3", "猫SP.mp3", "猫之呼吸.mp3", "猫之呼吸2.mp3", "猫式呼吸.mp3", "猫猫.mp3", "猫猫伸懒腰.mp3", "王先生.mp3", "王小姐.mp3", "王小姐是我的老婆.mp3", "甜菜猫.mp3", "白金ディスコ.mp3", "稍等一下喵.mp3", "突然秃头了.mp3", "笑.mp3", "笑2.mp3", "笑3.mp3", "笑4.mp3", "笑5.mp3", "笑6.mp3", "笑7.mp3", "笑？.mp3", "米娜LSP.mp3", "米娜爱我吗.mp3", "糟糕的声音.mp3", "糟糕的声音2.mp3", "糟糕的声音3.mp3", "纳尼纳尼纳尼.mp3", "给你个好人卡.mp3", "给你好人卡.mp3", "给我红包.mp3", "老公~.mp3", "老公抱抱.mp3", "老婆~.mp3", "老婆~老婆~.mp3", "老板大气.mp3", "耳朵没有了.mp3", "耶~.mp3", "胖次.mp3", "自我介绍.mp3", "舔狗.mp3", "舔狗是真的牛逼.mp3", "花に亡霊.mp3", "莫多莫多.mp3", "菜B.mp3", "诶诶诶.mp3", "谁啊.mp3", "谁是LSP.mp3", "谢谢你啊.mp3", "起床hiiro.mp3", "起床啊你这个debu.mp3", "起飞.mp3", "辛苦了(幼).mp3", "辛苦了.mp3", "这个不对啊.mp3", "这个人不对.mp3", "这个人没问题吧.mp3", "这个重要吗.mp3", "这什么鬼东西.mp3", "这就是DD呢.mp3", "这是啥啊.mp3", "那个那个那个那个那个.mp3", "错了错了.mp3", "阿巴巴阿巴阿巴.mp3", "阿巴阿巴阿巴.mp3", "阿巴阿巴阿巴阿巴.mp3", "阿里噶多.mp3", "阿里噶多2.mp3", "阿里噶多3.mp3", "鳥の詩.mp3", "麻烦你原地去世.mp3" };
        #endregion

        public Meme() {
            if (File.Exists("meme.json")) {
                try {
                    MemeMgr = MemeManager.ReadFromFile("meme.json");
                } catch (Exception) { }
            }
            if (!Directory.Exists(Program.Secret.MemeBackupDirectory)) {
                Directory.CreateDirectory(Program.Secret.MemeBackupDirectory);
            }
        }

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Id;
            if (text == "")
                return false;
            #region 指定回复
            if (Regex.IsMatch(text, @"^setrep .+#.+$")) {
                text = text[7..];
                string meme = text.Split("#")[0];
                string rep = text.Split("#")[1];
                string AllRep = MemeMgr.GetMeme(meme);
                if (AllRep != null) {
                    foreach (var r in AllRep.Split("|")) {
                        if (r.Contains(rep)) {
                            Rep.Remove(q);
                            Rep.Add(q, (meme, r));
                            await MessageManager.SendFriendMessageAsync(q, $"已指定回复内容\n{meme} -> {r}");
                            return true;
                        }
                    }
                    await MessageManager.SendFriendMessageAsync(q, "未发现该回复内容！");
                } else {
                    await MessageManager.SendFriendMessageAsync(q, "未发现该回复！");
                }
                return true;
            }
            #endregion

            #region meme管理
            if (Regex.IsMatch(text.ToLower(), @"^setmeme ((equal|regexmatch|regexreplace|startswith) )?.+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
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
                    if (MemeMgr.SetMeme(key, rep, type)) {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));
                    } else {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"设置{key}时出错"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}");
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();

                    MemeMgr.AddMemeReply(key, rep);
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}");
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^addalias .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[9..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine(key);
                    Console.WriteLine(val);

                    if (MemeMgr.AddMemeAlias(key, rep)) {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已更新{key}"));
                    } else {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"添加{key}别名失败，请检查是否存在该meme以及该别名是否存在"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}");
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^remmeme .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..].ToLower();
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1].ToLower();

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}");
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmeme .+")) {
                string temp = text[8..];
                var memes = MemeMgr.GetMeme(temp);
                if (memes != null) {
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(memes));
                } else {
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("未发现该回复"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^delmeme .+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..];
                    if (MemeMgr.RemoveMeme(temp)) {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"已移除{temp}"));
                    } else {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage($"未发现{temp}"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}");
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmemejson .+")) {
                string temp = text[12..];
                string json = MemeMgr.GetMemeJson(temp);
                if (json != null) {
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(json));
                } else {
                    await MessageManager.SendFriendMessageAsync(e.Sender.Id, new PlainMessage("未发现该回复"));
                }
                return true;
            }
            if (text == "savememe") {
                MemeMgr.Save("meme.json");
                File.Copy("meme.json", "memebkup/meme-" + DateTime.Now.Ticks + ".json");
                return true;
            }
            if (text == "reloadmeme") {
                try {
                    MemeMgr = MemeManager.ReadFromFile("meme.json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } catch (Exception) { }
                return true;
            }
            #endregion
            #region meme回复
            if (Rep.TryGetValue(q, out (string, string) valll)) {
                if (text == valll.Item1) {
                    if (valll.Item2.StartsWith("[picture]") && valll.Item2.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Url = $"/recordings/botpicture/{valll.Item2[9..]}";
                        await MessageManager.SendGroupMessageAsync(q, image);
                        //await session.SendImageToFriendAsync(q, new string[] {  });
                    } else {
                        string n = await MessageManager.SendFriendMessageAsync(q, valll.Item2);
                    }
                    Rep.Remove(q);
                    return true;
                }
            }
            //if (text == "来点hiiro" || text == "來點hiiro") {
            //    LastMeme.Remove(q);
            //    LastMeme.Add(q, text);
            //    string url = @"https://cdn.jsdelivr.net/gh/blacktunes/hiiro-button@master/public/voices/" + System.Web.HttpUtility.UrlEncode(Utils.RandomMsg(HiiroVoiceList), Encoding.Default);
            //    await MessageManager.SendFriendMessageAsync(q, new VoiceMessage(null, url, null));
            //    return true;
            //}
            if (MemeMgr.HasReply(text)) {
                var rep = MemeMgr.GetReply(text, long.Parse(q));
                if (rep != null) {
                    LastMeme.Remove(q);
                    LastMeme.Add(q, text);
                    if (rep.StartsWith("[picture]") && rep.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Path = $"/recordings/botpicture/{ rep[9..]}";
                        await MessageManager.SendGroupMessageAsync(q, image);
                        //await session.SendImageToFriendAsync(q, new string[] {  });
                    } else {
                        string n = await MessageManager.SendFriendMessageAsync(q, new PlainMessage(rep));
                    }
                    return true;
                }
            }
            if (text.ToLower() == "guid") {
                await MessageManager.SendFriendMessageAsync(q, new PlainMessage(Guid.NewGuid().ToString()));
                return true;
            }
            //if (text.ToLower() == "来点原神kfc") {
            //    LastMeme.Remove(q);
            //    LastMeme.Add(q, text);
            //    await MessageManager.SendFriendMessageAsync(q, new VoiceMessage(null, @"https://img.nga.178.com/attachments/mon_202103/08/i2Qj0k-i7puZg.mp3?duration=40%A1%E5%E2%80%8B", null));
            //    return true;
            //}
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text == "")
                return false;
            #region meme添加管理
            //if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id)) && text == "y") {
            //    QuoteMessage qm = null;
            //    foreach (var v in e.MessageChain) {
            //        if (v.Type == "Quote") {
            //            qm = (QuoteMessage) v;
            //        }
            //    }
            //    if (qm != null) {
            //        string RepText = Utils.GetMessageText(qm.OriginChain);
            //        if (Regex.IsMatch(RepText, @"^(set|add|rem)meme .+#.+|^delmeme .+|^addalias .+#.+")) {
            //            text = RepText;
            //        }
            //    }
            //}
            #endregion
            #region meme回复
            if (Rep.TryGetValue(e.Sender.Id, out (string, string) valll)) {
                if (text == valll.Item1) {
                    if (valll.Item2.StartsWith("[picture]") && valll.Item2.Length > 9) {
                        var image = new ImageMessage();
                        image.Path = $"/recordings/botpicture/{valll.Item2[9..]}";
                        await MessageManager.SendGroupMessageAsync(q, image);
                        //await session.SendImageToGroupAsync(q, new string[] { $"http://127.0.0.1:23333/botpicture/{valll.Item2[9..]}" });
                    } else {
                        string n = await MessageManager.SendGroupMessageAsync(q, valll.Item2);
                        if (valll.Item2.Length > 200) {
                            (new Thread(new ThreadStart(async () => {
                                Thread.Sleep(60000);
                                try {
                                    await MessageManager.RecallAsync(n);
                                } catch (Exception) { }
                            }))).Start();
                        }
                    }
                    Rep.Remove(e.Sender.Id);
                    return true;
                }
            }
            if (text == "再来点") {
                if (LastMeme.TryGetValue(q, out string last)) {
                    text = last;
                }
            }
            if (text == "来点hiiro" || text == "來點hiiro") {
                LastMeme.Remove(q);
                LastMeme.Add(q, text);
                string url = @"https://cdn.jsdelivr.net/gh/blacktunes/hiiro-button@master/public/voices/" + System.Web.HttpUtility.UrlEncode(Utils.RandomMsg(HiiroVoiceList), Encoding.Default);

                //VoiceMessage vm = new VoiceMessage();
                //vm.
                //await MessageManager.SendGroupMessageAsync(q, new VoiceMessage(null, url, null));
                return true;
            }
            if (MemeMgr.HasReply(text)) {
                var rep = MemeMgr.GetReply(text, long.Parse(q));
                if (rep != null) {
                    LastMeme.Remove(q);
                    LastMeme.Add(q, text);
                    if (rep.StartsWith("[picture]") && rep.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Path = $"/recordings/botpicture/{rep[9..]}";
                        await MessageManager.SendGroupMessageAsync(q, image);
                        //await session.SendImageToGroupAsync(q, new string[] {  });
                    } else {
                        string n = await MessageManager.SendGroupMessageAsync(q, rep);
                        if (rep.Length > 200) {
                            (new Thread(new ThreadStart(async () => {
                                Thread.Sleep(60000);
                                try {
                                    await MessageManager.RecallAsync(n);
                                } catch (Exception) { }
                            }))).Start();
                        }
                    }
                }
                return true;
            }
            if (text.ToLower() == "guid") {
                await MessageManager.SendGroupMessageAsync(q, new PlainMessage(Guid.NewGuid().ToString()));
                return true;
            }
            if (text.ToLower() == "来点原神kfc") {
                LastMeme.Remove(q);
                LastMeme.Add(q, text);
                //await MessageManager.SendGroupMessageAsync(q, new VoiceMessage(null, @"https://img.nga.178.com/attachments/mon_202103/08/i2Qj0k-i7puZg.mp3?duration=40%A1%E5%E2%80%8B", null));
                return true;
            }
            #endregion
            #region meme管理
            if (Regex.IsMatch(text.ToLower(), @"^setmeme ((equal|regexmatch|regexreplace|startswith) )?.+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
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
                    if (MemeMgr.SetMeme(key, rep, type)) {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"设置{key}时出错"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();

                    MemeMgr.AddMemeReply(key, rep);
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^addalias .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[9..];
                    string key = temp.Split('#')[0];
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine(key);
                    Console.WriteLine(val);

                    if (MemeMgr.AddMemeAlias(key, rep)) {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"已更新{key}"));
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"添加{key}别名失败，请检查是否存在该meme以及该别名是否存在"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^remmeme .+#.+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..].ToLower();
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1].ToLower();

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmeme .+")) {
                string temp = text[8..];
                var memes = MemeMgr.GetMeme(temp);
                if (memes != null) {
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage(memes));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("未发现该meme"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^delmeme .+")) {
                if (MemeMgr.IsAdmin(long.Parse(e.Sender.Id))) {
                    string temp = text[8..];
                    if (MemeMgr.RemoveMeme(temp)) {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"已移除{temp}"));
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"未发现{temp}"));
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("已将建议转发给Light_Quanta"));
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmemejson .+")) {
                string temp = text[12..];
                string json = MemeMgr.GetMemeJson(temp);
                if (json != null) {
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage(json));
                } else {
                    await MessageManager.SendGroupMessageAsync(q, new PlainMessage("未发现该回复"));
                }
                return true;
            }
            if (text == "savememe") {
                MemeMgr.Save("meme.json");
                File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                return true;
            }
            if (text == "reloadmeme") {
                try {
                    MemeMgr = MemeManager.ReadFromFile("meme.json");
                    File.Copy("meme.json", "/recordings/bot/meme.json");
                } catch (Exception) { }
                return true;
            }
            //if (text == "uploadpic") {
            //    if (e.MessageChain.Length == 3 && (e.MessageChain[2] is ImageMessage im)) {
            //        string FileName = im.ImageId;
            //        if (!File.Exists("/recordings/botpicture/" + FileName + ".png")) {
            //            if (MemeMgr.IsAdmin(e.Sender.Id)) {
            //                (new Thread(new ThreadStart(async () => {
            //                    try {
            //                        WebRequest imgRequest = WebRequest.Create(im.Url);
            //                        HttpWebResponse res = (HttpWebResponse) imgRequest.GetResponse();
            //                        Image downImage = Image.FromStream(res.GetResponseStream());
            //                        downImage.Save("/recordings/botpicture/" + FileName);
            //                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"上传图片{FileName}成功！"));
            //                        Console.WriteLine(im.ImageId);
            //                    } catch (Exception e) {
            //                        Console.WriteLine(e.Message);
            //                        await MessageManager.SendGroupMessageAsync(q, new PlainMessage("上传图片出错"));
            //                    }
            //                }))).Start();
            //            } else {
            //                await MessageManager.SendFriendMessageAsync(2224899528, new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.NickName}({e.Sender.Id})的图片上传请求\n图片名称：{FileName}"), new ImageMessage(im.ImageId, im.Url, null));
            //                await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"已将建议转发给Light_Quanta\n图片名称：{FileName}"));
            //            }
            //        } else {
            //            await MessageManager.SendGroupMessageAsync(q, new PlainMessage($"该图片{FileName}已存在！"));
            //        }
            //    }
            //    return true;
            //}
            #endregion
            return false;
        }
    }
}