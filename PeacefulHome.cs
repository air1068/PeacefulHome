using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peaceful_Home {
    internal static class ModInfo {
        internal const string Guid = "air1068.elin.peacefulhome";
        internal const string Name = "Peaceful Home";
        internal const string Version = "0.0.3";
    }

    [BepInPlugin(ModInfo.Guid, ModInfo.Name, ModInfo.Version)]
    public class PeacefulHome : BaseUnityPlugin {
        private void Awake() {
            var harmony = new Harmony(ModInfo.Guid);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SpawnSetting), nameof(SpawnSetting.HomeWild))]
    class SpawnSetting_HomeWild_patch {
        static void Postfix(SpawnSetting __result) {
            __result.hostility = SpawnHostility.Neutral;
        }
    }

    [HarmonyPatch(typeof(Map), nameof(Map.CountHostile))]
    class Map_CountHostile_patch {
        static bool Prefix(Map __instance, ref int __result) {
            if (__instance.zone.IsPCFaction) {
                //set the reported enemy count to the area's spawn cap to prevent any from spawning
                __result = 4 + 4*(__instance.zone.Evalue(3705) + __instance.zone.Evalue(3710)*2);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TraitBaseSpellbook), nameof(TraitBaseSpellbook.ReadFailEffect))]
    class TraitBaseSpellbook_ReadFailEffect_patch {
        static bool Prefix(Chara c) {
            if (!c.IsPC && c.IsPCFaction && c.currentZone.IsPCFaction) {
                c.SayNothingHappans();
                return false;
            }
            return true;
        }
    }
}
