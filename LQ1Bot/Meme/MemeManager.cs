using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;

namespace LQ1Bot.Meme {

    internal class MemeManager {
        private HashSet<MemeBase> memes = new HashSet<MemeBase>();

        [JsonIgnore]
        public int MemeCount { get; }

        [JsonIgnore]
        public string SavePath { get; set; }

        public long LastUpdateTime { get; set; }
        public List<long> Admin { get; set; }

        public HashSet<MemeBase> Memes {
            get => memes;
            set {
                LastUpdateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                memes = value;
            }
        }

        public bool IsAdmin(long qq) => Admin?.Contains(qq) ?? false;

        public string GetMeme(string MemeName) {
            foreach (var m in memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    return string.Join("|", m.ReplyContent);
                }
            }
            return null;
        }

        public bool SetMeme(string MemeName, HashSet<string> Content, MemeBase.MatchType Type = MemeBase.MatchType.Equal) {
            RemoveMeme(MemeName);
            return AddNewMeme(MemeName, Content.ToHashSet(), Type);
        }

        public bool SetMeme(string MemeName, params string[] Content) {
            return SetMeme(MemeName, Content.ToHashSet());
        }

        public bool AddMemeReply(string MemeName, params string[] Content) {
            return AddMemeReply(MemeName, Content.ToHashSet());
        }

        public bool AddMemeReply(string MemeName, HashSet<string> Content) {
            bool modified = false;
            bool hasMeme = false;
            foreach (var m in Memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    hasMeme = true;
                    foreach (var v in Content) {
                        if (m.ReplyContent.Add(v)) {
                            modified = true;
                            LastUpdateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                        }
                    }
                }
            }
            if (!hasMeme) {
                Memes.Add(new MemeBase() { Name = MemeName, ReplyContent = Content });
                modified = true;
                LastUpdateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            }
            return modified;
        }

        public bool AddNewMeme(string MemeName, params string[] Content) {
            return AddNewMeme(MemeName, Content.ToHashSet());
        }

        public bool AddNewMeme(string MemeName, string Content, MemeBase.MatchType Type = MemeBase.MatchType.Equal, HashSet<string> MemeAlias = null, HashSet<long> Whitelist = null, HashSet<long> Blacklist = null) {
            return AddNewMeme(MemeName, new HashSet<string>() { Content }, Type, MemeAlias, Whitelist, Blacklist);
        }

        public bool AddNewMeme(string MemeName, HashSet<string> Content, MemeBase.MatchType Type = MemeBase.MatchType.Equal, HashSet<string> MemeAlias = null, HashSet<long> Whitelist = null, HashSet<long> Blacklist = null) {
            foreach (var m in Memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    return false;
                }
            }
            LastUpdateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return Memes.Add(new MemeBase() {
                Name = MemeName.ToLower(),
                DetectType = Type,
                ReplyContent = Content,
                Alias = MemeAlias,
                BlacklistGroups = Blacklist,
                WhitelistGroups = Whitelist,
            });
        }

        public bool RemoveMeme(string MemeName) {
            foreach (var m in Memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    LastUpdateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                    return Memes.Remove(m);
                }
            }
            return false;
        }

        public bool AddMemeAlias(string MemeName, params string[] Alias) {
            return AddMemeAlias(MemeName, Alias.ToHashSet());
        }

        public bool AddMemeAlias(string MemeName, HashSet<string> Alias) {
            foreach (var m in memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    foreach (var v in Alias) {
                        if (m.Alias == null) {
                            m.Alias = new HashSet<string>();
                        }
                        if (m.Name != v.ToLower()) {
                            m.Alias.Add(v.ToLower());
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool HasReply(string msg) {
            foreach (var m in memes) {
                switch (m.DetectType) {
                    case MemeBase.MatchType.RegexMatch:
                    case MemeBase.MatchType.RegexReplace:
                        if (Regex.IsMatch(msg.ToLower(), m.Name)) {
                            return true;
                        } else {
                            if (m.Alias != null) {
                                foreach (var a in m.Alias) {
                                    if (Regex.IsMatch(msg.ToLower(), a)) {
                                        return true;
                                    }
                                }
                            }
                        }
                        break;

                    case MemeBase.MatchType.StartsWith:
                        if (msg.ToLower().StartsWith(m.Name.ToLower())) {
                            return true;
                        } else {
                            if (m.Alias != null) {
                                foreach (var a in m.Alias) {
                                    if (msg.ToLower().StartsWith(a.ToLower())) {
                                        return true;
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        if (m.Name == msg.ToLower() || (m.Alias?.Contains(msg.ToLower()) ?? false)) {
                            return true;
                        }
                        break;
                }
            }
            return false;
        }

        public string GetReply(string msg, long group) {
            foreach (var m in memes) {
                if (m.WhitelistGroups != null) {
                    if (!m.WhitelistGroups.Contains(group)) {
                        continue;
                    }
                }
                if (m.BlacklistGroups?.Contains(group) ?? false) {
                    continue;
                }
                switch (m.DetectType) {
                    case MemeBase.MatchType.RegexMatch:
                        if (Regex.IsMatch(msg.ToLower(), m.Name)) {
                            List<string> rep = new List<string>();
                            foreach (var v in m.ReplyContent) {
                                rep.Add(v);
                            }
                            return rep[(new Random()).Next(rep.Count)];
                        }
                        break;

                    case MemeBase.MatchType.RegexReplace:
                        if (Regex.IsMatch(msg.ToLower(), m.Name)) {
                            List<string> rep = new List<string>();
                            foreach (var v in m.ReplyContent) {
                                rep.Add(v);
                            }
                            var match = Regex.Match(msg.ToLower(), m.Name);
                            string result = rep[(new Random()).Next(rep.Count)];
                            if (int.TryParse(match.Groups["id"].ToString(), out int id)) {
                                if (id >= 1) {
                                    if (id > rep.Count) {
                                        id %= rep.Count;
                                        id++;
                                    }
                                    result = rep[id - 1];
                                }
                            }
                            return Regex.Replace(msg, m.Name, result);
                        }
                        break;

                    case MemeBase.MatchType.StartsWith:
                        if (msg.ToLower().StartsWith(m.Name.ToLower())) {
                            List<string> rep = new List<string>();
                            foreach (var v in m.ReplyContent) {
                                rep.Add(v);
                            }
                            return rep[(new Random()).Next(rep.Count)];
                        } else {
                            if (m.Alias != null) {
                                foreach (var a in m.Alias) {
                                    if (msg.ToLower().StartsWith(a.ToLower())) {
                                        List<string> rep = new List<string>();
                                        foreach (var v in m.ReplyContent) {
                                            rep.Add(v);
                                        }
                                        return rep[(new Random()).Next(rep.Count)];
                                    }
                                }
                            }
                        }
                        break;

                    default:
                        if (m.Name == msg.ToLower() || (m.Alias?.Contains(msg.ToLower()) ?? false)) {
                            List<string> rep = new List<string>();
                            foreach (var v in m.ReplyContent) {
                                rep.Add(v);
                            }
                            return rep[(new Random()).Next(rep.Count)];
                        }
                        break;
                }
            }
            return null;
        }

        public string GetMemeJson(string MemeName) {
            foreach (var m in Memes) {
                if (m.Name == MemeName.ToLower() || (m.Alias?.Contains(MemeName.ToLower()) ?? false)) {
                    var o = new JsonSerializerOptions() {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                        WriteIndented = true
                    };
                    return JsonSerializer.Serialize(m, o);
                }
            }
            return null;
        }

        public override string ToString() {
            var o = new JsonSerializerOptions() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            };
            return JsonSerializer.Serialize(this, o);
        }

        public string ToIndentedString() {
            var o = new JsonSerializerOptions() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, o);
        }

        public static MemeManager Parse(string src) {
            return JsonSerializer.Deserialize<MemeManager>(src);
        }

        public static MemeManager ReadFromFile(string Path) {
            return Parse(File.ReadAllText(Path));
        }

        public void Save() {
            File.WriteAllText(SavePath, ToIndentedString());
        }

        public void Save(string Path) {
            File.WriteAllText(Path, ToIndentedString());
        }
    }
}