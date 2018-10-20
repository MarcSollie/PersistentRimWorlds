﻿using Harmony;
using PersistentWorlds.SaveAndLoad;
using RimWorld;
using Verse;

namespace PersistentWorlds.Patches
{
    [HarmonyPatch(typeof(Game), "FinalizeInit")]
    public class Game_FinalizeInit_Patch
    {
        #region Methods
        static void Postfix(Game __instance)
        {
            if (!PersistentWorldManager.GetInstance().PersistentWorldNotNull())
                return;
            
            var flag = PersistentWorldManager.GetInstance()
                .PersistentWorldNotNullAndLoadStatusIsNot(PersistentWorldLoadSaver.PersistentWorldLoadStatus
                    .Converting);
            // Toggle colonies tab.
            DefDatabase<MainButtonDef>.GetNamed("Colonies").buttonVisible = flag;

            if(!flag)
                PersistentWorldManager.GetInstance().PersistentWorld.LoadSaver.Convert(__instance);
        }
        #endregion
    }
}