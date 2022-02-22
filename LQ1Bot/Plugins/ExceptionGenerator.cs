using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;
using Mirai.Net.Sessions.Http.Managers;

namespace LQ1Bot.Plugins {

    internal class ExceptionGenerator : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9992;

        public override string PluginName => "ExceptionGenerator";

        public override bool CanDisable => true;

        private readonly List<Type> ExceptionList = new List<Type>();

        public ExceptionGenerator() {
            var err = new Exception();
            Stack<Type> s = new Stack<Type>();
            s.Push(err.GetType());
            while (s.Count > 0) {
                var v = s.Pop();
                var temp = Utils.GetSubClass(v);
                ExceptionList.AddRange(temp);
                temp.ForEach(o => s.Push(o));
            }
        }

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            switch (Utils.GetMessageText(e.MessageChain)) {
                #region 手动报错
                case "来点bug":
                case "来点异常":
                case "来点错误":
                case "来点exception":
                    var ExceptionType = ExceptionList[(new Random()).Next(ExceptionList.Count)];
                    try {
                        throw ((Exception) Activator.CreateInstance(ExceptionType));
                    } catch (Exception eee) {
                        await MessageManager.SendFriendMessageAsync(e.Sender.Id, eee.ToString());
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            switch (Utils.GetMessageText(e.MessageChain)) {
                #region 手动报错
                case "来点bug":
                case "来点异常":
                case "来点错误":
                case "来点exception":
                    var ExceptionType = ExceptionList[(new Random()).Next(ExceptionList.Count)];
                    try {
                        throw ((Exception) Activator.CreateInstance(ExceptionType));
                    } catch (Exception eee) {
                        await MessageManager.SendGroupMessageAsync(e.Sender.Group.Id, eee.ToString());
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }
    }
}