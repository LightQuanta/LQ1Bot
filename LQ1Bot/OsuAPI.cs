using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace LQ1Bot {

    public static class OsuAPI {
        public const string OSU = "osu";
        public const string MANIA = "mania";
        public const string CTB = "fruits";
        public const string TAIKO = "taiko";

        public static string mode = "osu";

        #region POST GET模块

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> dic) {
            string result = "";
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic) {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                result = reader.ReadToEnd();
            }
            return result;
        }

        #endregion

        public static string OsuApiId;
        public static string OsuApiSecret;

        public static string GetWithToken(string url, string token) {
            string result = "";
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Headers.Set("Authorization", "Bearer " + token);
            req.AllowAutoRedirect = false;

            req.Method = "GET";
            try {
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取内容
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                    result = reader.ReadToEnd();
                }
                return result;
            } catch (WebException e) {
                if (e.Response is HttpWebResponse response) {
                    switch (response.StatusCode) {
                        case HttpStatusCode.NotFound:
                            return "404";

                        case HttpStatusCode.Redirect:
                            return e.Response.Headers["Location"];

                        case HttpStatusCode.Unauthorized:
                            return "auth";
                    }
                } else {
                    return null;
                }
                return null;
            }
        }

        public static string GetOAuthToken(string OsuApiId, string OsuApiSecret) {
            Dictionary<string, string> d = new Dictionary<string, string> {
                { "client_id", OsuApiId },
                { "client_secret", OsuApiSecret },
                { "grant_type", "client_credentials" },
                { "scope", "public" }
            };
            string responce = Post(@"https://osu.ppy.sh/oauth/token", d);
            OAuthTokenResponce o = JsonConvert.DeserializeObject<OAuthTokenResponce>(responce);
            return o.access_token;
        }

        public static string GetUserInfo(string name, string token) {
            string rep = GetWithToken(@"https://osu.ppy.sh/api/v2/users/" + name + "/" + mode, token);
            if (rep != null) {
                return rep switch
                {
                    "auth" => "auth",
                    "404" => "404",
                    _ => rep,
                };
            } else {
                return null;
            }
        }

        public static PInfo Query(string name) {
            PInfo p;
            p.pp = 0; p.rank = 0; p.crank = 0; p.playcount = 0;
            try {
                SqliteConnection conn = new SqliteConnection("Data Source=osu.db");
                conn.Open();
                SqliteCommand cmd = new SqliteCommand("", conn) {
                    CommandText = "select pp,rank,crank,playcount from " + OsuAPI.mode + " where name=@name;"
                };

                cmd.Parameters.AddWithValue("@name", name);

                SqliteDataReader dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    p.pp = Int32.Parse(dr[0].ToString());
                    p.rank = Int32.Parse(dr[1].ToString());
                    p.crank = Int32.Parse(dr[2].ToString());
                    p.playcount = Int32.Parse(dr[3].ToString());
                }
                dr.Close();

                /*
                cmd.CommandText = "select rank from " + OsuAPI.mode + " where name=@name;";
                dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    p.rank = Int32.Parse(dr[0].ToString());
                }
                dr.Close();

                cmd.CommandText = "select crank from " + OsuAPI.mode + " where name=@name;";
                dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    p.crank = Int32.Parse(dr[0].ToString());
                }
                dr.Close();

                cmd.CommandText = "select playcount from " + OsuAPI.mode + " where name=@name;";
                dr = cmd.ExecuteReader();
                while (dr.Read()) {
                    p.playcount = Int32.Parse(dr[0].ToString());
                }
                dr.Close();
                */
                conn.Close();
                return p;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return p;
            }
        }

        public static PInfo QueryInjectable(ref string name) {
            PInfo p;
            p.pp = 0; p.rank = 0; p.crank = 0; p.playcount = 0;
            SqliteConnection conn = new SqliteConnection("Data Source=osuInject.db");
            conn.Open();
            SqliteCommand cmd = new SqliteCommand("", conn);
            cmd.CommandText = "select pp,rank,crank,playcount,name from " + OsuAPI.mode + " where name=\'" + name + "\';";
            SqliteDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) {
                p.pp = Int32.Parse(dr[0].ToString());
                p.rank = Int32.Parse(dr[1].ToString());
                p.crank = Int32.Parse(dr[2].ToString());
                p.playcount = Int32.Parse(dr[3].ToString());
                name = dr[4].ToString();
            }
            dr.Close();
            conn.Close();
            return p;
        }

        public struct PInfo {
            public int pp;
            public int rank;
            public int crank;
            public int playcount;
        }

        public static void Update(string name, int pp, int rank, int crank, int playcount) {
            try {
                using SqliteConnection conn = new SqliteConnection("Data Source=osu.db");
                conn.Open();
                SqliteCommand cmd = new SqliteCommand("", conn) {
                    CommandText = "select rank from " + OsuAPI.mode + " where name=@name;"
                };
                cmd.Parameters.AddWithValue("@name", name);

                SqliteDataReader dr = cmd.ExecuteReader();
                string s = "0";
                while (dr.Read()) {
                    s = dr[0].ToString();
                }
                dr.Close();

                if (s == "0") {
                    cmd.CommandText = "insert into " + OsuAPI.mode + " values (@name," + pp + "," + rank + "," + crank + "," + playcount + ");";
                    cmd.ExecuteNonQuery();
                } else {
                    cmd.CommandText = "update " + OsuAPI.mode + " set pp=" + pp + " where name=@name;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update " + OsuAPI.mode + " set rank=" + rank + " where name=@name;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update " + OsuAPI.mode + " set crank=" + crank + " where name=@name;";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "update " + OsuAPI.mode + " set playcount=" + playcount + " where name=@name;";
                    cmd.ExecuteNonQuery();
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public static string ReadToken() {
            string token = null;
            try {
                FileInfo fi = new FileInfo("osu.token");
                Console.WriteLine(fi.LastWriteTime);
                if (DateTime.Compare(fi.LastWriteTime, fi.LastWriteTime.AddDays(1.0)) >= 0) {
                    return UpdateToken();
                }
                using StreamReader sr = new StreamReader("osu.token");
                token = sr.ReadLine();
            } catch (Exception) {
                return UpdateToken();
            }
            return token;
        }

        public static string UpdateToken() {
            using (StreamWriter sw = new StreamWriter("osu.token")) {
                string token = OsuAPI.GetOAuthToken(OsuApiId, OsuApiSecret);
                sw.WriteLine(token);
                return token;
            }
        }
    }

    internal class OAuthTokenResponce {
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string access_token { get; set; }
    }
}