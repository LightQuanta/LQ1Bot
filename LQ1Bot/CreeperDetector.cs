using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TinyPinyin;

namespace LQ1Bot {

    internal static class CreeperDetector {
        #region 终极爬行者检测模块

        public static bool IsCreeper(string msg) {
            msg = CharReplace(msg);
            msg += Reverse(msg);
            if (Regex.IsMatch(msg, @"^.*([机木]{1,}.{0,6}器{1,}.{0,6}人{1,}.{0,300}爬)|(爬{1,}.{0,300}[机木]{1,}.{0,6}器{1,}.{0,6}人).*$") ||
                Regex.IsMatch(msg, @"^.*(b{1,}[^\w]{0,10}o{1,}[^\w]{0,10}t{1,}.{0,50}爬)|(爬{1,}.{0,50}b{1,}[^\w]{0,10}o{1,}[^\w]{0,10}t).*$")) {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(msg);
                Console.ResetColor();
                return true;
            } else {
                return false;
            }
        }

        private static string CharReplace(string s) {
            s = s.Trim();
            #region base64解码
            foreach (Match match in Regex.Matches(s, @"((([a-zA-Z0-9\\\+]{4})*([a-zA-Z0-9\\\+]{3})=)|(([a-zA-Z0-9\\\+]{4})*([a-zA-Z0-9\\\+]{2})==)|[a-zA-Z0-9\\\+]{4})+")) {
                try {
                    s += Encoding.UTF8.GetString(Convert.FromBase64String(match.Value));
                } catch (Exception) { }
            }
            #endregion
            #region 熊曰解密
            foreach (Match match in Regex.Matches(s, @"熊曰：[\u4e00-\u9fa5]+")) {
                string result = "";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(@"http://hi.pcmoe.net/bear.php");
                req.Timeout = 1000;
                req.Headers.Add("X-Token", "D398E4D76D4E");
                req.Headers.Add("X-Requested-With", "XMLHttpRequest");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = "http://hi.pcmoe.net/index.html";
                req.Method = "POST";
                byte[] bs = Encoding.UTF8.GetBytes("mode=Bear&code=Decode&txt=" + HttpUtility.UrlEncode(match.Value));
                req.ContentLength = bs.Length;
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                } catch (Exception) { }
                s += result;
            }
            #endregion
            #region 佛曰解密
            foreach (Match match in Regex.Matches(s, @"新佛曰：[\u4e00-\u9fa5]+")) {
                string result = "";
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(@"http://hi.pcmoe.net/bear.php");
                req.Timeout = 1000;
                req.Headers.Add("X-Token", "D398E4D76D4E");
                req.Headers.Add("X-Requested-With", "XMLHttpRequest");
                req.ContentType = "application/x-www-form-urlencoded";
                req.Referer = "http://hi.pcmoe.net/index.html";
                req.Method = "POST";
                byte[] bs = Encoding.UTF8.GetBytes("mode=Buddha&code=Decode&txt=" + HttpUtility.UrlEncode(match.Value));
                req.ContentLength = bs.Length;
                Stream reqStream = req.GetRequestStream();
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
                try {
                    HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(stream)) {
                        result = reader.ReadToEnd();
                    }
                    stream.Close();
                } catch (Exception) { }
                s += result;
            }
            #endregion
            #region 摩斯电码解码
            s = s.Replace("----.", "9")
                .Replace("---..", "8")
                .Replace("--...", "7")
                .Replace("-....", "6")
                .Replace(".....", "5")
                .Replace("....-", "4")
                .Replace("...--", "3")
                .Replace("..---", "2")
                .Replace(".----", "1")
                .Replace("-----", "0")
                .Replace("..-..", "E")
                .Replace("--..", "Z")
                .Replace("-.--", "Y")
                .Replace("-..-", "X")
                .Replace("...-", "V")
                .Replace("--.-", "Q")
                .Replace(".--.", "P")
                .Replace(".-..", "L")
                .Replace(".---", "J")
                .Replace("....", "H")
                .Replace("..-..", "F")
                .Replace("-.-.", "C")
                .Replace("-...", "B")
                .Replace(".--", "W")
                .Replace("..-", "U")
                .Replace("...", "S")
                .Replace(".-.", "R")
                .Replace("---", "O")
                .Replace("-.-", "K")
                .Replace("--.", "G")
                .Replace("-..", "D")
                .Replace("-.", "N")
                .Replace("--", "M")
                .Replace("..", "I")
                .Replace(".-", "A")
                .Replace("-", "T")
                .Replace(".", "E");
            #endregion
            #region UNICODE
            foreach (Match match in Regex.Matches(s, @"\\u[0-9a-f]{4}")) {
                int code = Convert.ToInt32(match.Value[2..], 16);
                s += (char) code;
            }
            #endregion
            s = s.ToLower();
            s = s.Replace(" ", "");
            s = s.Replace("[1]", "");
            s = s.Replace("Π", "机");
            s = Regex.Replace(s, @"[ΩㄇΠ♬]", "机");
            s = s.Replace("_(•̀ω•́」∠)_", "爬");
            s = s.Replace("表情117", "爬");
            s = s.Replace("表情208", "爬");
            s = s.Replace("Õ", "o");
            s = Regex.Replace(s, @"[\n ,.?!。，？！_-]", "");
            s = s.Replace("machine", "机器");
            s = s.Replace("chicken", "机");
            s = s.Replace("jqrp", "机器人爬");
            s = s.Replace("gas", "器");
            s = s.Replace("man", "人");
            s = s.Replace("亻", "人");
            s = s.Replace("ㄐㄧ", "机");
            s = s.Replace("ㄑㄧˋ", "器");
            s = s.Replace("ㄖㄣˊ", "人");
            s = s.Replace("ㄆㄚˊ", "爬");
            s = s.Replace("ㄅㄚ", "爬");
            s = s.Replace("怕", "");
            s = Regex.Replace(s, "zhua.{0,2}ba", "爬");
            s = Regex.Replace(s, @"[口ロ]+.?[ボ木术]+.?ッ+.?[ト卜ド]|🤖|篮.?球|🏀|lq|basketball|lq|lightquanta", "机器人");
            s = Regex.Replace(s, @"[啪钯杷耙琶葩趴扒耙鈀🧗啪🚼🦂巴巳パぱ⑧筢䯲掱苩]|c.*r.*a.*w.*l|c.*r.*e.*p|c.*l.*i.*m.*b|pa|滚|匍.*匐|g.*u.*n|pa", "爬");
            s = Regex.Replace(s, @"[g機幾几🐓萝罗螺j🐔稽𓄿🐣🐤🐥じⓙ羁激]|j[i1]", "机");
            s = Regex.Replace(s, @"[🏇噐博卜伯q械戒⑦7七柒犬💭🚴🚵☁气氣💨哭Ⓠ屑ち球求♟️]|大[·`‘’']|q[i1]", "器");
            s = Regex.Replace(s, @"[👨人亼特口rɹ仌秂亽仁👤👦👨👥👴银淫亻慢曼漫🧍Ⓡ入恁]|れん|[rl][eᴱ3][и∩n]", "人");
            string temp = "";
            var c = new List<char>(s.ToCharArray());
            c.ForEach(x => {
                if (PinyinHelper.IsChinese(x)) {
                    string rep = PinyinHelper.GetPinyin(x).ToLower();
                    temp += rep switch
                    {
                        "ji" => "机",
                        "qi" => "器",
                        "ren" => "人",
                        "pa" => "爬",
                        "bo" => "bo",
                        "te" => "te",
                        _ => x,
                    };
                } else {
                    temp += x;
                }
            });
            s += temp;
            s = s.Replace("ᗷ", "b");
            s = Regex.Replace(s, @"13|[βⒷ6Ь♭вΒ🅱𝒃𝕓𝓫𝔟𝐛𝗯𝘣𝙗𝚋𝖇🄱🅑ⓑᵇｂ🇧​𝒷ꃳꋰ฿ᵇც]", "b");
            s = Regex.Replace(s, @"[口0oοоОΟ🅾〇⚪⭕〇⚫Ⓞ𝒐𝕠𝓸𝔬𝐨𝗼𝘰𝙤𝚘𝖔🄾🅞ⓞᵒｏ𝚘𝑜ꄲŐθꂦ⊙•́⚽Ø🏀ÕᵒõΟơ]", "o");
            s = Regex.Replace(s, @"[Ⓣᵀт丅𝒕𝕥𝓽𝐭𝔱𝘁𝘵𝙩𝚝𝖙🆃🅃🅣ⓣᵗｔ𝚝𝓉꓄Ťꋖ☂ɬ₮┬ᵗΤΓτť]", "t");
            s = Regex.Replace(s, @"[Ⓟ𝒑𝓹𝔭𝕡𝐩𝗽𝘱𝙥𝚙𝖕🅿🄿🅟ⓟᵖｐ𝚙𝓅ᴘρþ]", "p");
            s = Regex.Replace(s, @"[α𝒂𝕒𝓪𝔞𝐚𝗮𝘢𝙖𝚊𝖆🅰🄰🅐ⓐᵃａ𝚊𝒶ⒶᴀΛáã𠆢Â]", "a");
            s = Regex.Replace(s, @"p.{0,5}a", "爬");
            if (Regex.IsMatch(s, @"^机器人.{0,10}p$"))
                return "机器人爬";
            return s;
        }

        #endregion

        public static string Reverse(string ReverseString) {
            String output = string.Empty;
            for (int i = ReverseString.Length; i > 0; i--) {
                output += ReverseString.Substring(i - 1, 1);
            }
            return output;
        }
    }
}