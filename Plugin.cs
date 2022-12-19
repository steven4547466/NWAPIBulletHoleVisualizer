using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWAPIBulletHoleVisualizer
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }
        public static Harmony Harmony { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("NW Bullet Hole Visualizer", "1.0.0", "NW Bullet Hole Visualizer", "Steven4547466")]
        void LoadPlugin()
        {
            Singleton = this;
            Harmony = new Harmony($"nw-bulletvisualizer-{DateTime.Now.Ticks}");
            Harmony.PatchAll();
        }

        [PluginUnload]
        void UnloadPlugin()
        {
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
            Singleton = null;
        }

        [PluginEvent(ServerEventType.RoundRestart)]
        public void RestartingRound()
        {
            foreach (Player player in Utils.Visualizers.Keys)
            {

                if (player.GameObject.TryGetComponent(out Visualizer behaviour))
                {
                    behaviour.Destroy();
                }
            }
            Utils.Visualizers.Clear();
            Utils.Bullets.Clear();
        }
    }
}
