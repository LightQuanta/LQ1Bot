using System;
using System.Collections.Generic;
using System.Linq;

namespace LQ1Bot.Plugins {

    internal class PluginController {
        public static readonly List<PluginBase> PluginInstance = new List<PluginBase>();

        public void LoadPlugins() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("已找到的插件");
            var Type = Utils.GetSubClass(typeof(PluginBase));
            foreach (var PluginType in Type.OrderBy(o => o.GetCustomAttributes(true))) {
                PluginBase Plugin = (PluginBase) Activator.CreateInstance(PluginType);
                PluginInstance.Add(Plugin);
            }
            foreach (var Plugin in PluginInstance.OrderByDescending(o => o.Priority)) {
                Console.WriteLine($"[{Plugin.Priority}]\t\t{Plugin.PluginName}");
            }
            Console.ResetColor();
        }

        public void ShowPlugins() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("所有已加载插件");
            foreach (var Plugin in PluginInstance.OrderByDescending(o => o.Priority)) {
                Console.WriteLine($"[{Plugin.Priority}]\t{Plugin.PluginName}");
            }
            Console.ResetColor();
        }
    }
}