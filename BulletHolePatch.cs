using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NWAPIBulletHoleVisualizer
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBulletholeDecal))]
    internal class BulletHolePatch
    {
        public static void Postfix(StandardHitregBase __instance, Ray ray, RaycastHit hit)
        {
            Player shooter = Player.Get(__instance.Hub);
            Utils.AddBulletHole(shooter, hit.point);
        }
    }
}
