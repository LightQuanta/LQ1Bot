
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LQ1Bot {
    class LQ1BotConfig {

        public string BaiduTranslateAppId { get; set; }
        public string BaiduTranslateSecret { get; set; }

        public string OsuApiSecret { get; set; }
        public string OsuApiId { get; set; }

        public string AliyunClientId { get; set; }
        public string AliyunClientSecret { get; set; }

        public string MiraiIp { get; set; }
        public string MiraiSecret { get; set; }
        public int MiraiPort { get; set; }
        public long QQ { get; set; }

        public string MemeBackupDirectory { get; set; }
        public void Init() {
            if (File.Exists("lq1bot.config")) {
                var v = JsonSerializer.Deserialize<LQ1BotConfig>(File.ReadAllText("lq1bot.config"));
                //我也不知道该怎么读取比较好了
                //你们有没有什么比较好的读取方式
                MemeBackupDirectory = v.MemeBackupDirectory;
                QQ = v.QQ;
                MiraiIp = v.MiraiIp;
                MiraiPort = v.MiraiPort;
                MiraiSecret = v.MiraiSecret;
                AliyunClientId = v.AliyunClientId;
                AliyunClientSecret = v.AliyunClientSecret;
                OsuApiId = v.OsuApiId;
                OsuApiSecret = v.OsuApiSecret;
                BaiduTranslateAppId = v.BaiduTranslateAppId;
                BaiduTranslateSecret = v.BaiduTranslateSecret;
            } else {
                File.Create("lq1bot.config");
                //格式化保存json，方便阅读
                var o = new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                File.WriteAllText("lq1bot.config", JsonSerializer.Serialize(this, o));
            }
        }
    }
}
