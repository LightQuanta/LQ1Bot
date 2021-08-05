using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mirai_CSharp;
using Mirai_CSharp.Models;
using Mirai_CSharp.Plugin.Interfaces;

namespace LQ1Bot.Plugins {
    class HomoCalc : PluginBase, IGroupMessage, IFriendMessage {
        public override int Priority => 9986;

        public override string PluginName => "HomoCalc";

        public override bool CanDisable => true;

        private readonly Dictionary<int, string> BinSet = new Dictionary<int, string>() {
            {1073741824,"114514*((1+1)*4514+(11*(45-14)+(11-4+5-1-4)))+(11*4514+(114*5*14+(-(1-14)*5*14+(11-4-5+14))))" },
            {536870912,"114514*((1145+1)*4+(114-5-1-4))+(114*51*4+(114*51+4+(11*4*5-14)))" },
            {268435456,"114514*(114*5*1*4+(11*4+5*1*4))+(1+14514+(-1-14*(5-14)))" },
            {134217728,"114514*(1145+14+(1*14-5/1+4))+(1+14*514+(114-5+14))" },
            {67108864,"114514*(114*5+14+(-11+4-5+14))+((1+1)*451*4+(11+45/1-4))" },
            {33554432,"114514*(11+4*5*14+(-11+4-5+14))+(114*(5-1)*4+(1-14+5+14))" },
            {16777216,"114514*(11+45*-(1-4))+(11*4514+(114*5*14+(1+14+514+(11-4+5+1-4))))" },
            {8388608,"114514*(1*14*5-1+4)+(114*51*4+(114*51+4+(-11+4+5+14)))" },
            {4194304,"114514*(11+4*5+1+4)+(114*514+(11451+4+(114*-5*(1-4)+(-11+45+1+4))))" },
            {2097152,"114514*(1+1+4*5*1-4)+(114*51*4+(11451+4+(1145+14+(1*-1+45-14))))" },
            {1048576,"114514*(11-4+5+1-4)+(1145*14+((11+451)*4+(-1-1+4+5*14)))" },
            {524288,"114514*(-11-4+5+14)+(114*514+(1+14*514+((1+145)*-(1-4)+(11/(45-1)*4))))" },
            {262144,"114514*(-11+4-5+14)+(114*51*4+((1+1)*4514+(11+4*51*4+(11-4*5+14))))" },
            {131072,"114514+(1145*14+(1*14+514))" },
            {65536,"114*514+(11*45*14+(-11/4+51/4))" },
            {32768,"114*51*4+((1+1)*4514+(11*45-14+(11*-4+51-4)))" },
            {16384,"1145*14+((11-4)*51-4+(11/(45-1)*4))" },
            {8192,"114*5*14+((1+1)*4+51*4)" },
            {4096,"(1+1)*451*4+(11*(45-1)+4)" },
            {2048,"-11+4*514+(11*-4+51-4)" },
            {1024,"1*(1+4)*51*4+(-11-4+5+14)" },
            {512,"1+1-4+514" },
            {256,"(114-51)*4+(-11-4+5+14)" },
            {128,"1+1+(4+5)*14" },
            {64,"1-1+4*(5-1)*4" },
            {32,"1+1*45-14" },
            {16,"1+1+4+5+1+4" },
            {8,"1+1+4+5+1-4" },
            {4,"1+1+4-5-1+4" },
            {2,"(-1)*(1+(4+5)/(1-4))" },
            {1,"1+1*4*(5-1-4)" },
        };

        //这么屑的算法有什么存在的必要吗
        //现在这生成的算式又臭又长
        //之后再考虑该怎么优化
        private string Num2Homo(int n) {
            if (n == 0)
                return "1+1-4+5+1-4";
            if (n == -2147483648)
                return "-114514*(1145*14+((1-14)*51*-4+((1+14)*5-1*4)))+(1*(1+4)*514+(11+4*5+1+4))";
            bool isNegative = false;
            if (n < 0) {
                isNegative = true;
                n = -n;
            }
            List<string> nums = new List<string>();
            while (n != 0) {
                foreach (var v in BinSet) {
                    if (v.Key <= n) {
                        nums.Add(BinSet.GetValueOrDefault(v.Key));
                        n -= v.Key;
                        break;
                    }
                }
            }
            if (isNegative)
                return $"-({string.Join("+", nums)})";
            else
                return $"{string.Join("+", nums)}";
        }

        public async Task<bool> FriendMessage(MiraiHttpSession session, IFriendMessageEventArgs e) {
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Id;

            if (text.StartsWith("恶臭论证 ")) {
                text = text[5..];
                if (int.TryParse(text, out int num)) {
                    await session.SendFriendMessageAsync(q, new PlainMessage(num + " = " + Num2Homo(num)));
                } else {
                    await session.SendFriendMessageAsync(q, new PlainMessage("在？你管这叫int32整数？"));
                }
                return true;
            }
            return false;
        }

        public async Task<bool> GroupMessage(MiraiHttpSession session, IGroupMessageEventArgs e) {
            if (!FunctionSwitch.IsEnabled(e.Sender.Group.Id, PluginName)) {
                return false;
            }
            string text = Utils.GetMessageText(e.Chain);
            long q = e.Sender.Group.Id;

            if (text.StartsWith("恶臭论证 ")) {
                text = text[5..];
                if (int.TryParse(text, out int num)) {
                    await session.SendGroupMessageAsync(q, new PlainMessage(Num2Homo(num)));
                } else {
                    await session.SendGroupMessageAsync(q, new PlainMessage("在？你管这叫int32整数？"));
                }
                return true;
            }
            return false;
        }
    }
}
