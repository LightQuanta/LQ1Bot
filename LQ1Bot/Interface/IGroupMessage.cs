using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages.Receivers;

namespace LQ1Bot.Interface {
    interface IGroupMessage {
        Task<bool> GroupMessage(GroupMessageReceiver e);
    }
}
