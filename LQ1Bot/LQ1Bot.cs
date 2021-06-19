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
    partial class LQ1Bot : ITempMessage, IBotInvitedJoinGroup, IFriendMessage,  INewFriendApply, IDisconnected {

        private static LQ1BotConfig Secret;

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
        }
        
        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            if (e.Sender.Id != 2224899528) {
                return true;
            } else {
                if (text.StartsWith("gm")) {
                    await session.SendGroupMessageAsync(235107078, new PlainMessage(text.Substring(2)));
                }
                return true;
            }
        }

        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e) {
            await session.SendTempMessageAsync(e.Sender.Id, e.Sender.Group.Id, new PlainMessage("请先添加好友"));
            return true;
        }

        public async Task<bool> BotInvitedJoinGroup(MiraiHttpSession session, IBotInvitedJoinGroupEventArgs e) {
            await session.HandleGroupApplyAsync(e, GroupApplyActions.Allow);
            return false;
        }
    }
}
