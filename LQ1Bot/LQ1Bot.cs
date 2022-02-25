using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Events.Concretes.Request;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot {

    partial class LQ1Bot {
        private static LQ1BotConfig Secret;

        //public async Task<bool> Disconnected(MiraiHttpSession session, IDisconnectedEventArgs e) {
        //    MiraiHttpSessionOptions options = new MiraiHttpSessionOptions(Secret.MiraiIp, Secret.MiraiPort, Secret.MiraiSecret);
        //    while (true) {
        //        try {
        //            await session.ConnectAsync(options, Secret.QQ); // 连到成功为止, QQ号自填, 你也可以另行处理重连的 behaviour
        //            return true;
        //        } catch (Exception) {
        //            await Task.Delay(1000);
        //        }
        //    }
        //}

        //public async Task<bool> NewFriendApply(MiraiHttpSession session, INewFriendApplyEventArgs e) {
        //    await session.HandleNewFriendApplyAsync(e, FriendApplyAction.Allow, "感谢加好友哼哼啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊");
        //    return false;
        //}

        public LQ1Bot(LQ1BotConfig config, MiraiBot bot) {
            Secret = config;
            OsuAPI.OsuApiId = config.OsuApiId;
            OsuAPI.OsuApiSecret = config.OsuApiSecret;

            bot.EventReceived
                .OfType<NewFriendRequestedEvent>()
                .Subscribe(receiver => {
                    RequestManager.HandleNewFriendRequestedAsync(receiver, Mirai.Net.Data.Shared.NewFriendRequestHandlers.Approve);
                }
                );
            bot.MessageReceived
                .OfType<TempMessageReceiver>()
                .Subscribe(receiver => {
                    MessageManager.SendTempMessageAsync(receiver.Sender.Id, "请先添加好友");
                });
            bot.EventReceived
                .OfType<NewInvitationRequestedEvent>()
                .Subscribe(receiver => {
                    RequestManager.HandleNewInvitationRequestedAsync(receiver, Mirai.Net.Data.Shared.NewInvitationRequestHandlers.Approve, "");
                });
        }
    }
}