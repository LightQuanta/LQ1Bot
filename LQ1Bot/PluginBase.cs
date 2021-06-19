using System;
using System.Collections.Generic;
using System.Text;
using Mirai_CSharp.Plugin;

namespace LQ1Bot.Plugins {
    abstract class PluginBase : IPlugin {
        public abstract int Priority { get; }
        public string ConfigPath {
            get {
                return Environment.CurrentDirectory + "/" + PluginName;
            }
        }
        public abstract string PluginName { get; }

        public abstract bool CanDisable { get; }
    }
}
