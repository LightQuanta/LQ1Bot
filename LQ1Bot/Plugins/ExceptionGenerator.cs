using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class ExceptionGenerator : PluginBase, IGroupMessage, IFriendMessage {
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
        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            switch (Utils.GetMessageText(e.Chain)) {
                #region 手动报错
                case "来点bug":
                case "来点异常":
                case "来点错误":
                case "来点exception":
                    var ExceptionType = ExceptionList[(new Random()).Next(ExceptionList.Count)];
                    try {
                        throw ((Exception) Activator.CreateInstance(ExceptionType));
                    } catch (Exception eee) {
                        await session.SendFriendMessageAsync(e.Sender.Id, new PlainMessage(eee.ToString()));
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            switch (Utils.GetMessageText(e.Chain)) {
                #region 手动报错
                case "来点bug":
                case "来点异常":
                case "来点错误":
                case "来点exception":
                    var ExceptionType = ExceptionList[(new Random()).Next(ExceptionList.Count)];
                    try {
                        throw ((Exception) Activator.CreateInstance(ExceptionType));
                    } catch (Exception eee) {
                        await session.SendGroupMessageAsync(e.Sender.Group.Id, new PlainMessage(eee.ToString()));
                    }
                    return true;
                #endregion
                default:
                    return false;
            }
        }
    }
}
