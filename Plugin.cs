using HarmonyLib;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace NWAPIBulletHoleVisualizer
{
    public class Plugin
    {
        public static Plugin Singleton { get; private set; }
        public static Harmony Harmony { get; private set; }

        public static PluginHandler PluginHandler { get; private set; }

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("NW Bullet Hole Visualizer", "1.1.1", "NW Bullet Hole Visualizer", "Steven4547466")]
        void LoadPlugin()
        {
            Singleton = this;
            PluginHandler = PluginHandler.Get(this);
            Harmony = new Harmony($"nw-bulletvisualizer-{DateTime.Now.Ticks}");
            Harmony.PatchAll();

            if (!Directory.Exists(Path.Combine(PluginHandler.PluginDirectoryPath, "rounds")))
            {
                Directory.CreateDirectory(Path.Combine(PluginHandler.PluginDirectoryPath, "rounds"));
            }

            EventManager.RegisterEvents(this);
        }

        [PluginUnload]
        void UnloadPlugin()
        {
            Harmony.UnpatchAll(Harmony.Id);
            Harmony = null;
            Singleton = null;
            PluginHandler = null;
        }

        [PluginEvent(ServerEventType.RoundStart)]
        public void StartingRound()
        {
            Utils.RoundStartTime = DateTime.Now;
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
            if (!Utils.LoadedRound && Config.SerializeToWebhook != string.Empty)
            {
                byte[] json = JsonSerializer.Serialize(Utils.Bullets);
                using (HttpClient httpClient = new HttpClient())
                {
                    MultipartFormDataContent form = new MultipartFormDataContent();
                    form.Add(new StringContent($"{{\"content\":\"{(Config.ServerNum != -1 ? $"Server {Config.ServerNum} " : "")}{(Config.ShowPort ? $"({Server.Port}) " : "")}Round at {Utils.RoundStartTime.ToString("dd-MM-yyyy H:mm:ss zzz")}\"}}"), "payload_json");
                    form.Add(new ByteArrayContent(json), "Document", $"round-{Utils.RoundStartTime.ToString().Replace("/", "-")}.json");
                    httpClient.PostAsync(Config.SerializeToWebhook, form).Wait();
                    httpClient.Dispose();
                }
            }

            Utils.SpawnedPrimitives.Clear();
            Utils.Bullets.Clear();
            Utils.LoadedRound = false;
        }

        [PluginConfig]
        public Config Config;
    }
}
