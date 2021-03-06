﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Verse;

using PersistentWorlds.UI;
using FileLog = PersistentWorlds.Utils.FileLog;
using RimWorld.QuestGen;
using RimWorld;

namespace PersistentWorlds
{
    /// <summary>
    /// The main mod class that interacts with RimWorld when it is loaded.
    /// </summary>
    public sealed class PersistentWorldsMod : Mod
    {
        #region Fields
        // Must be public or IL transpiler throws error of ldsfld NULL.
        internal static Delegate MainMenuButtonDelegate = new Action(PatchMainMenu);
        
        internal static Delegate SaveMenuButtonDelegate = new Action(PatchSaveMenu);
        #endregion
        
        #region Constructors
        /// <summary>
        /// Main mod constructor that initializes Harmony patches.
        /// </summary>
        /// <param name="content"></param>
        public PersistentWorldsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("me.filunderscore.persistentrimworlds");

#if DEBUG
            Harmony.DEBUG = true;
            
            FileLog.StartLoggingToFile("debug_log.txt");
#endif

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Patches the button of "Persistent RimWorlds", more of a delegate called by "MainMenuButtonDelegate".
        /// </summary>
        private static void PatchMainMenu()
        {
            Find.WindowStack.Add(new Page_PersistentWorlds_LoadWorld_FileList());
        }

        private static void PatchSaveMenu()
        {
            Find.WindowStack.Add(new Dialog_PersistentWorlds_SaveWorld());
        }
        #endregion
    }
}