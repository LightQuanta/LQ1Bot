using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai.Net.Data.Messages.Receivers;

namespace LQ1Bot.Interface {
    interface ITempMessage {
        Task<bool> TempMessage(TempMessageReceiver e);
    }
}
