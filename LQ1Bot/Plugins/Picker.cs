using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class Picker : PluginBase,IGroupMessage {
        public override int Priority => 9991;

        public override bool CanDisable => true;
        public override string PluginName => "Picker";
        struct Pick {
            public long qq;
            public string Name;
            public int PickCount;
            public int Total;
            public Dictionary<long, string> Player;
        }

        private readonly List<Pick> PickRecord = new List<Pick>();
        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain).ToLower();
            long q = e.Sender.Group.Id;
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
                return true;
            }
            if (Regex.IsMatch(text.ToLower(), @"^[!！]pick join \S+$")) {
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
                return true;
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
                return true;
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
                return true;
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
                return true;
            }
            #endregion
            return false;
        }
    }
}
