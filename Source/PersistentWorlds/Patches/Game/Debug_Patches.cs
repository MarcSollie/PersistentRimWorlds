﻿using System;
using System.Collections.Generic;
using Harmony;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using UnityEngine.Video;
using Verse;

namespace PersistentWorlds.Patches
{
    public static class Debug_Patches
    {
        [HarmonyPatch(typeof(WorldObject), "SpawnSetup")]
        public static class WorldObject_SpawnSetup_Patch
        {
            [HarmonyPrefix]
            public static void SpawnSetup_Prefix(WorldObject __instance)
            {
                if (__instance.def == null)
                {
                    Log.Error("Def is null");
                }
                else
                {
                    Log.Message("Def not null");
                }
            }
        }
    }
}