using System;
using System.Collections.Generic;
using System.Text;

namespace LQ1Bot.Meme {
    class MemeBase {
        /// <summary>
        /// 发言检测类型（普通检测，正则表达式匹配，正则表达式替换，指定内容开头检测）
        /// </summary>
        public enum MatchType {
            Equal, RegexMatch, RegexReplace, StartsWith
        }
        /// <summary>
        /// 定义文本检测的类型
        /// </summary>
        public MatchType DetectType { get; set; }
        /// <summary>
        /// meme的名称（也是要检测的文本）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// meme的别名
        /// </summary>
        public HashSet<string> Alias { get; set; }
        /// <summary>
        /// 要回复的内容
        /// </summary>
        public HashSet<string> ReplyContent { get; set; }
        /// <summary>
        /// 对于此meme的群白名单
        /// </summary>
        public HashSet<long> WhitelistGroups { get; set; }
        /// <summary>
        /// 对于此meme的群黑名单
        /// </summary>
        public HashSet<long> BlacklistGroups { get; set; }
    }
}
