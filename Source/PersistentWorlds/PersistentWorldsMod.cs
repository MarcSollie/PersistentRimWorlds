﻿using System;
using System.Reflection;
using Harmony;
using Verse;

using PersistentWorlds.UI;

namespace PersistentWorlds
{
    public class PersistentWorldsMod : Mod
    {
        // Must be public or IL transpiler throws error of ldsfld NULL.
        public static Delegate MainMenuButtonDelegate = new Action(PatchMainMenu);
        
        public PersistentWorldsMod(ModContentPack content) : base(content)
        {
            var harmony = HarmonyInstance.Create("PersistentWorlds");
            HarmonyInstance.DEBUG = true;
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private static void PatchMainMenu()
        {
            // Close main menu...
            Find.WindowStack.Add((Window) new Dialog_PersistentWorlds_Main());
        }
    }
}