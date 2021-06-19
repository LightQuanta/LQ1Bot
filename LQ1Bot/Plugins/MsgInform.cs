using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class MsgInform : PluginBase, IGroupMessage, IFriendMessage, ITempMessage {
        public override int Priority => int.MaxValue;

        public override string PluginName => "MsgInform";
        public override bool CanDisable => false;

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            long q = e.Sender.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"FriendMessage\t{e.Sender.Name}({q})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{string.Join(null, (IEnumerable<IMessageBase>) e.Chain)}");
            string text = Utils.GetMessageText(e.Chain).Trim();
            Console.WriteLine(text);
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            long q = e.Sender.Group.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"GroupMessage\t{e.Sender.Group.Name}({e.Sender.Group.Id})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{string.Join(null, (IEnumerable<IMessageBase>) e.Chain)}");
            string text = Utils.GetMessageText(e.Chain).Trim();
            Console.WriteLine(text);
            return false;
        }

        public async Task<bool> TempMessage(MiraiHttpSession session, ITempMessageEventArgs e) {
            long q = e.Sender.Group.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"TempMessage\t{e.Sender.Group.Name}({e.Sender.Group.Name})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{string.Join(null, (IEnumerable<IMessageBase>) e.Chain)}");
            string text = Utils.GetMessageText(e.Chain).Trim();
            Console.WriteLine(text);
            return false;
        }
    }
}
