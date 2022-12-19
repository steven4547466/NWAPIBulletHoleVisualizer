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

        private Dictionary<string, Color> UserToColor = new Dictionary<string, Color>();
        List<PrimitiveObjectToy> SpawnedObjects = new List<PrimitiveObjectToy>();

        public void Awake()
        {
            Player = Player.Get(gameObject);
            foreach (Bullet bullet in Utils.Bullets)
            {
                AddNewBullet(bullet);
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
                        added.Add($"\n<size=18><align=left><color={UserToColor[bullet.UserId].ToHex()}>{bullet.Name}{(IsAdmin ? $" ({bullet.UserId})" : "")}</color></align></size>");
                    }
                }

                Player.ReceiveHint($"<size=18><align=left>Nearby player bullets:</align></size>{string.Join("", added)}", 1f);
            }
        }

        public void AddNewBullet(Bullet bullet)
        {
            if (!UserToColor.ContainsKey(bullet.UserId))
            {
                UserToColor[bullet.UserId] = Utils.Colors[UserToColor.Count % Utils.Colors.Length];
            }
            SpawnBullet(bullet, UserToColor[bullet.UserId]);
        }

        private void SpawnBullet(Bullet bullet, Color color)
        {
            PrimitiveObjectToy primitive = Instantiate(Utils.PrimitiveBaseObject);
            SpawnedObjects.Add(primitive);
            primitive.transform.position = bullet.Position;
            primitive.transform.localScale = new Vector3(-0.05f, -0.05f, -0.05f);
            NetworkServer.Spawn(primitive.gameObject);
            foreach (Player player in Player.GetPlayers())
            {
                if (player != Player)
                    player.Connection.Send(new ObjectDestroyMessage() { netId = primitive.netId });
            }
            primitive.NetworkPosition = primitive.transform.position;
            primitive.NetworkScale = primitive.transform.localScale;
            primitive.NetworkPrimitiveType = PrimitiveType.Sphere;
            primitive.NetworkMaterialColor = color;
        }

        public void Destroy()
        {
            foreach(PrimitiveObjectToy primitive in SpawnedObjects)
            {
                NetworkServer.Destroy(primitive.gameObject);
            }
            Active = false;
            DestroyImmediate(this);
        }
    }
}
