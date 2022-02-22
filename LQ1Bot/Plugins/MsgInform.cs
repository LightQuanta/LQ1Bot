using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LQ1Bot.Interface;
using Mirai.Net.Data.Messages.Receivers;

namespace LQ1Bot.Plugins {

    internal class MsgInform : PluginBase, IGroupMessage, IFriendMessage, ITempMessage {
        public override int Priority => 99999;

        public override string PluginName => "MsgInform";
        public override bool CanDisable => false;

        public async Task<bool> FriendMessage(FriendMessageReceiver e) {
            string q = e.Sender.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"FriendMessage\t{e.Sender.NickName}({q})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.NickName}[{e.Sender.Id}]\nContent:\t{Utils.GetMessageText(e.MessageChain)}");
            string text = Utils.GetMessageText(e.MessageChain).Trim();
            Console.WriteLine(text);
            return await Task.Run(() => false);
        }

        public async Task<bool> GroupMessage(GroupMessageReceiver e) {
            string q = e.Sender.Group.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now}\nGroupMessage\t{e.Sender.Group.Name}({e.Sender.Group.Id})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{Utils.GetMessageText(e.MessageChain)}");
            string text = Utils.GetMessageText(e.MessageChain).Trim();
            Console.WriteLine(text);
            return await Task.Run(() => false);
        }

        public async Task<bool> TempMessage(TempMessageReceiver e) {
            string q = e.Sender.Group.Id;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"TempMessage\t{e.Sender.Group.Name}({e.Sender.Group.Name})");
            Console.ResetColor();
            Console.WriteLine($"User:\t\t{e.Sender.Name}[{e.Sender.Id}]\nContent:\t{Utils.GetMessageText(e.MessageChain)}");
            string text = Utils.GetMessageText(e.MessageChain).Trim();
            Console.WriteLine(text);
            return await Task.Run(() => false);
        }
    }
}