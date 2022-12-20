using AdminToys;
using Mirror;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NWAPIBulletHoleVisualizer
{
    public class Visualizer : MonoBehaviour
    {
        public bool IsAdmin { get; set; } = false;

        private bool Active = true;
        private Player Player;

        private float Timer = 0;

        public void Awake()
        {
            Player = Player.Get(gameObject);
            foreach (PrimitiveObjectToy primitive in Utils.SpawnedPrimitives)
            {
                NetworkServer.SendSpawnMessage(primitive.netIdentity, Player.Connection);
            }
        }

        public void Update()
        {
            if (!Active)
                return;
            Timer += Time.deltaTime;
            if (Timer > 0.5f)
            {
                Timer = 0;
                IEnumerable<Bullet> nearbyBullets = Utils.Bullets.Where(b => (b.Position - Player.Position).sqrMagnitude <= 100f);
                List<string> playerIdsAdded = new List<string>();
                List<string> added = new List<string>();
                foreach(Bullet bullet in nearbyBullets)
                {
                    if (!playerIdsAdded.Contains(bullet.UserId))
                    {
                        playerIdsAdded.Add(bullet.UserId);
                        added.Add($"\n<size=18><align=left><color={Utils.UserToColor[bullet.UserId].ToHex()}>{bullet.Name}{(IsAdmin ? $" ({bullet.UserId})" : "")}</color></align></size>");
                    }
                }

                Player.ReceiveHint($"<size=18><align=left>Nearby player bullets:</align></size>{string.Join("", added)}", 1f);
            }
        }

        public void Destroy()
        {
            Active = false;
            DestroyImmediate(this);
        }
    }
}
