﻿using PersistentWorlds.Logic;
using PersistentWorlds.SaveAndLoad;
using Verse;

namespace PersistentWorlds
{
    /// <summary>
    /// Mainly a static class that keeps track of the current persistent world as well as the loader for the persistent world.
    /// Also has methods to help with keeping track of current game state.
    /// </summary>
    public static class PersistentWorldManager
    {
        public static PersistentWorld PersistentWorld;
        public static PersistentWorldLoadSaver WorldLoadSaver;

        public static bool Active()
        {
            return PersistentWorld != null && WorldLoadSaver != null && PersistentWorld.Colony != null;
        }

        public static bool NotNull()
        {
            return PersistentWorld != null && WorldLoadSaver != null;
        }

        // TODO: May break current game if Persistent Worlds menu is accessed in game then quit on colony page.
        public static void Clear()
        {
            PersistentWorld = null;
            WorldLoadSaver = null;
            
            ScribeVars.Clear();
            ScribeMultiLoader.Clear();
            ReferenceSaveLoader.ClearReferences();
            
            Scribe.ForceStop();
        }
    }
}