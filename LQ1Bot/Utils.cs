using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Mirai_CSharp.Models;

namespace LQ1Bot {
    static class Utils {
        /// <summary>
        /// 获取一个类在其所在的程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类</returns>
        public static List<Type> GetSubClass(Type ParentType) {
            var SubTypeList = new List<Type>();
            var Assembly = ParentType.Assembly;
            var AssemblyAllTypes = Assembly.GetTypes();
            foreach (var ItemType in AssemblyAllTypes) {
                if (ItemType.BaseType?.Name == ParentType.Name) {
                    SubTypeList.Add(ItemType);
                }
            }
            return SubTypeList.ToList();
        }

        public static string GetMessageText(IMessageBase[] msg) {
            string text = "";
            foreach (var v in msg) {
                //text += "\n" + v.Type;
                switch (v.Type) {
                    case "Plain":
                        text += ((PlainMessage) v).Message;
                        break;
                    case "At":
                        AtMessage am = (AtMessage) v;
                        if (am.Target == 1727089824 || am.Target == 2224899528)
                            text += "Light_Quanta";
                        text += am.Display;
                        break;
                    //case "Quote":
                    //    QuoteMessage qm = (QuoteMessage) v;
                    //    var oc = qm.OriginChain;
                    //    //text += qm.;
                    //    break;
                    case "Source":
                        //SourceMessage sm = (SourceMessage) v;
                        break;
                }
            }
            return text;
        }

        /// <summary>
        /// MD5加密(32位)
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <returns></returns>
        public static string GetMD5(string str) {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew) {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        /// <summary>
        /// 强随机字符串生成
        /// </summary>
        /// <param name="Length">字符串长度</param>
        /// <param name="Pattern">填充字符</param>
        /// <returns></returns>
        public static string GetStrongRandomString(int Length, string Pattern = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") {
            if (Length < 1 || Pattern.Length < 2) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Length; i++) {
                sb.Append(Pattern[RandomNumberGenerator.GetInt32(Pattern.Length)]);
            }
            return sb.ToString();
        }
        public static string RandomMsg(params string[] s) {
            Random r = new Random();
            return s[(int) Math.Round(r.NextDouble() * (s.Length - 1))];
        }
    }
}
