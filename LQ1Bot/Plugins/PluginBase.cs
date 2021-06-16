using System;
using System.Collections.Generic;
using System.Text;

namespace LQ1Bot.Plugins {
    abstract class PluginBase : Mirai_CSharp.Plugin.IPlugin{
        public int Priority { get; }
        public bool HasConfigFile { get; }
    }
}
