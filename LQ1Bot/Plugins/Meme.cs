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

        private readonly Dictionary<string, (string, string)> Rep = new Dictionary<string, (string, string)>();

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
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    var match = Regex.Match(text.ToLower(), @"^setmeme (?<mode>(equal|regexmatch|regexreplace|startswith) )?(?<key>.+)#(?<val>.+)$");

                    string key = match.Groups["key"].Value;
                    string val = match.Groups["val"].Value;
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine("{" + match.Groups["mode"].Value + "}");
                    MemeBase.MatchType type = match.Groups["mode"].Value switch {
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
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#")) {
                if (e.MessageChain.Count() > 2 && e.MessageChain.ToArray()[2] is ImageMessage image) {
                    if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                        //下载图片
                        string fileName = image.ImageId.Replace("{", "").Replace("}", "").Replace("-", "");
                        WebClient wc = new WebClient();
                        wc.DownloadFile(image.Url, "/recordings/botpicture/" + fileName);

                        //addmeme
                        string temp = text[8..];
                        string key = temp.Split('#')[0].ToLower();
                        string value = "[picture]" + fileName;
                        MemeMgr.AddMemeReply(key, value);

                        MemeMgr.Save("meme.json");

                        await MessageManager.SendFriendMessageAsync(q, $"已更新{key}");
                    } else {
                        await MessageManager.SendFriendMessageAsync("2224899528", new PlainMessage($"来自{e.Sender.NickName}({e.Sender.Id})的的建议\n{text}"), image);
                        await MessageManager.SendFriendMessageAsync(q, "已将建议转发给Light_Quanta");
                    }
                    return true;
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    string temp = text[8..];
                    string key = temp.Split('#')[0].ToLower();
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
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
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
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
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
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
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
            if (text.StartsWith("findmeme ")) {
                string temp = text[9..].ToLower();
                if (temp.Length > 0) {
                    if (temp.Contains("#")) {
                        string key = temp.Split('#')[0];
                        string val = temp.Split('#')[1];
                        if (MemeMgr.HasReply(key)) {
                            string meme = MemeMgr.GetMeme(key);
                            List<string> rep = new List<string>();
                            foreach (var v in meme.Split("|")) {
                                if (v.ToLower().Contains(val) && rep.Count() < 8) {
                                    rep.Add(v);
                                }
                            }
                            if (rep.Count() > 0) {
                                await MessageManager.SendFriendMessageAsync(q, "查找到的回复：\n" + string.Join("\n", rep));
                            } else {
                                await MessageManager.SendFriendMessageAsync(q, "未查找到指定的回复内容！");
                            }
                        } else {
                            await MessageManager.SendFriendMessageAsync(q, "未查找到该关键词！");
                        }
                    } else {
                        List<MemeBase> memes = new List<MemeBase>();
                        foreach (var v in MemeMgr.Memes) {
                            if (v.Name.ToLower().Contains(temp) && memes.Count() < 8) {
                                memes.Add(v);
                            } else {
                                if (v.Alias != null) {
                                    foreach (var vv in v.Alias) {
                                        if (vv.ToLower().Contains(temp) && memes.Count() < 8 && !memes.Contains(v)) {
                                            memes.Add(v);
                                        }
                                    }
                                }
                            }
                        }
                        List<string> tmp = new List<string>();
                        foreach (var vvv in memes) {
                            string t = vvv.Name;
                            if (vvv.Alias != null && vvv.Alias.Count() > 0) {
                                t += "(别名：" + string.Join(",", vvv.Alias) + ")";
                            }
                            tmp.Add(t);
                        }

                        if (memes.Count() > 0) {
                            await MessageManager.SendFriendMessageAsync(q, "查找到的关键词：\n" + string.Join("\n", tmp));
                        } else {
                            await MessageManager.SendFriendMessageAsync(q, "未查找到指定的关键词！");
                        }
                    }
                }
                return true;
            }
            #endregion
            #region meme回复
            if (Rep.TryGetValue(q, out (string, string) valll)) {
                if (text == valll.Item1) {
                    if (valll.Item2.StartsWith("[picture]") && valll.Item2.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Url = $"/recordings/botpicture/{valll.Item2[9..]}";
                        await MessageManager.SendFriendMessageAsync(q, image);
                    } else {
                        string n = await MessageManager.SendFriendMessageAsync(q, valll.Item2);
                    }
                    Rep.Remove(q);
                    return true;
                }
            }
            if (MemeMgr.HasReply(text)) {
                var rep = MemeMgr.GetReply(text, long.Parse(q));
                if (rep != null) {
                    if (rep.StartsWith("[picture]") && rep.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Path = $"/recordings/botpicture/{ rep[9..]}";
                        await MessageManager.SendFriendMessageAsync(q, image);
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
            #endregion
            return false;
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string text = Utils.GetMessageText(e.MessageChain);
            string q = e.Sender.Group.Id;
            if (text == "")
                return false;
            #region meme回复
            if (Rep.TryGetValue(e.Sender.Id, out (string, string) valll)) {
                if (text == valll.Item1) {
                    if (valll.Item2.StartsWith("[picture]") && valll.Item2.Length > 9) {
                        var image = new ImageMessage {
                            Path = $"/recordings/botpicture/{valll.Item2[9..]}"
                        };
                        await MessageManager.SendGroupMessageAsync(q, image);
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
            if (MemeMgr.HasReply(text)) {
                var rep = MemeMgr.GetReply(text, long.Parse(q));
                if (rep != null) {
                    if (rep.StartsWith("[picture]") && rep.Length > 9) {
                        ImageMessage image = new ImageMessage();
                        image.Path = $"/recordings/botpicture/{rep[9..]}";
                        await MessageManager.SendGroupMessageAsync(q, image);
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
            #endregion
            #region meme管理
            if (Regex.IsMatch(text.ToLower(), @"^setmeme ((equal|regexmatch|regexreplace|startswith) )?.+#.+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    var match = Regex.Match(text.ToLower(), @"^setmeme (?<mode>(equal|regexmatch|regexreplace|startswith) )?(?<key>.+)#(?<val>.+)$");

                    string key = match.Groups["key"].Value;
                    string val = match.Groups["val"].Value;

                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine("{" + match.Groups["mode"].Value + "}");
                    MemeBase.MatchType type = match.Groups["mode"].Value switch {
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
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#")) {
                if (e.MessageChain.Count() > 2 && e.MessageChain.ToArray()[2] is ImageMessage image) {
                    if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                        //下载图片
                        string fileName = image.ImageId.Replace("{", "").Replace("}", "").Replace("-", "");
                        WebClient wc = new WebClient();
                        wc.DownloadFile(image.Url, "/recordings/botpicture/" + fileName);

                        //addmeme
                        string temp = text[8..];
                        string key = temp.Split('#')[0].ToLower();
                        string value = "[picture]" + fileName;
                        MemeMgr.AddMemeReply(key, value);

                        MemeMgr.Save("meme.json");

                        await MessageManager.SendGroupMessageAsync(q, $"已更新{key}");
                    } else {
                        await MessageManager.SendFriendMessageAsync("2224899528", new PlainMessage($"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}"), image);
                        await MessageManager.SendGroupMessageAsync(q, "已将建议转发给Light_Quanta");
                    }
                    return true;
                }
            }
            if (Regex.IsMatch(text.ToLower(), @"^addmeme .+#.+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    string temp = text[8..];
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();

                    MemeMgr.AddMemeReply(key, rep);
                    await MessageManager.SendGroupMessageAsync(q, $"已更新{key}");

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^addalias .+#.+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    string temp = text[9..];
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1];
                    HashSet<string> rep = val.Split("|").ToHashSet();
                    Console.WriteLine(key);
                    Console.WriteLine(val);

                    if (MemeMgr.AddMemeAlias(key, rep)) {
                        await MessageManager.SendGroupMessageAsync(q, $"已更新{key}");
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, $"添加{key}别名失败，请检查是否存在该meme以及该别名是否存在");
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^remmeme .+#.+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    string temp = text[8..].ToLower();
                    string key = temp.Split('#')[0].ToLower();
                    string val = temp.Split('#')[1].ToLower();

                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmeme .+")) {
                string temp = text[8..];
                var memes = MemeMgr.GetMeme(temp);
                if (memes != null) {
                    await MessageManager.SendGroupMessageAsync(q, memes);
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "未发现该meme");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^delmeme .+")) {
                if (PermissionMgr.HasPermissionOrAdmin(e.Sender.Id, "meme")) {
                    string temp = text[8..];
                    if (MemeMgr.RemoveMeme(temp)) {
                        await MessageManager.SendGroupMessageAsync(q, $"已移除{temp}");
                    } else {
                        await MessageManager.SendGroupMessageAsync(q, $"未发现{temp}");
                    }
                    MemeMgr.Save("meme.json");
                    File.Copy("meme.json", Program.Secret.MemeBackupDirectory + "/meme-" + DateTime.Now.Ticks + ".json");
                } else {
                    await MessageManager.SendFriendMessageAsync("2224899528", $"来自{e.Sender.Group.Name}的{e.Sender.Name}({e.Sender.Id})的建议\n{text}");
                    await MessageManager.SendGroupMessageAsync(q, "已将建议转发给Light_Quanta");
                }
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^getmemejson .+")) {
                string temp = text[12..];
                string json = MemeMgr.GetMemeJson(temp);
                if (json != null) {
                    await MessageManager.SendGroupMessageAsync(q, json);
                } else {
                    await MessageManager.SendGroupMessageAsync(q, "未发现该回复");
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
            if (text.StartsWith("findmeme ")) {
                string temp = text[9..].ToLower();
                if (temp.Length > 0) {
                    if (temp.Contains("#")) {
                        string key = temp.Split('#')[0];
                        string val = temp.Split('#')[1];
                        if (MemeMgr.HasReply(key)) {
                            string meme = MemeMgr.GetMeme(key);
                            List<string> rep = new List<string>();
                            foreach (var v in meme.Split("|")) {
                                if (v.ToLower().Contains(val) && rep.Count() < 8) {
                                    rep.Add(v);
                                }
                            }
                            if (rep.Count() > 0) {
                                await MessageManager.SendGroupMessageAsync(q, "查找到的回复：\n" + string.Join("\n", rep));
                            } else {
                                await MessageManager.SendGroupMessageAsync(q, "未查找到指定的回复内容！");
                            }
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "未查找到该关键词！");
                        }
                    } else {
                        List<MemeBase> memes = new List<MemeBase>();
                        foreach (var v in MemeMgr.Memes) {
                            if (v.Name.ToLower().Contains(temp) && memes.Count() < 8) {
                                memes.Add(v);
                            } else {
                                if (v.Alias != null) {
                                    foreach (var vv in v.Alias) {
                                        if (vv.ToLower().Contains(temp) && memes.Count() < 8 && !memes.Contains(v)) {
                                            memes.Add(v);
                                        }
                                    }
                                }
                            }
                        }
                        List<string> tmp = new List<string>();
                        foreach (var vvv in memes) {
                            string t = vvv.Name;
                            if (vvv.Alias != null && vvv.Alias.Count() > 0) {
                                t += "(别名：" + string.Join(",", vvv.Alias) + ")";
                            }
                            tmp.Add(t);
                        }

                        if (memes.Count() > 0) {
                            await MessageManager.SendGroupMessageAsync(q, "查找到的关键词：\n" + string.Join("\n", tmp));
                        } else {
                            await MessageManager.SendGroupMessageAsync(q, "未查找到指定的关键词！");
                        }
                    }
                }
                return true;
            }
            #endregion
            return false;
        }
    }
}