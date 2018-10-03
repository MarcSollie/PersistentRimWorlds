﻿using System;
using System.Collections.Generic;
using Harmony;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace PersistentWorlds.Logic
{
    public class PersistentWorld
    {
        public string fileName;
        
        // Game.World is accessed.
        public Game Game = new Game();

        public PersistentWorldData WorldData = new PersistentWorldData();
        
        public List<Map> Maps = new List<Map>();
        public List<PersistentColony> Colonies = new List<PersistentColony>();

        public PersistentWorld()
        {
            Current.Game = this.Game;
            this.Game.World = new RimWorld.Planet.World();
        }

        public void LoadWorld()
        {
            Log.Message("Calling LoadWorld");
            LongEventHandler.SetCurrentEventText("LoadingPersistentWorld".Translate());
            
            this.Game.LoadGame();
            
            // At the end.. because Scribe doesn't run due to us not loading Game directly.
            this.Game.FinalizeInit();
        }

        public void ExposeAndFillGameSmallComponents()
        {
            Log.Message("Calling ExposeAndFillGameSmallComponents");
            
            var colony = Colonies[PersistentWorldManager.LoadColonyIndex];

            if (colony == null)
            {
                // Return to main menu.
                Log.Error("Colony is null. - Persistent Worlds");
                GenScene.GoToMainMenu();
                return;
            }

            AccessTools.Field(typeof(Game), "info").SetValue(this.Game, colony.ColonyData.info);
            AccessTools.Field(typeof(Game), "rules").SetValue(this.Game, colony.ColonyData.rules);
            this.Game.Scenario = colony.ColonyData.scenario;
            this.Game.tickManager = this.WorldData.TickManager;
            this.Game.playSettings = colony.ColonyData.playSettings;
            this.Game.storyWatcher = colony.ColonyData.storyWatcher;
            this.Game.gameEnder = colony.ColonyData.gameEnder;
            this.Game.letterStack = colony.ColonyData.letterStack;
            this.Game.researchManager = colony.ColonyData.researchManager;
            this.Game.storyteller = colony.ColonyData.storyteller;
            this.Game.history = colony.ColonyData.history;
            this.Game.taleManager = colony.ColonyData.taleManager;
            this.Game.playLog = colony.ColonyData.playLog;
            this.Game.battleLog = colony.ColonyData.battleLog;
            this.Game.outfitDatabase = colony.ColonyData.outfitDatabase;
            this.Game.drugPolicyDatabase = colony.ColonyData.drugPolicyDatabase;
            this.Game.tutor = colony.ColonyData.tutor;
            this.Game.dateNotifier = colony.ColonyData.dateNotifier;
            this.Game.uniqueIDsManager = colony.ColonyData.uniqueIDsManager;
            this.Game.components = colony.ColonyData.gameComponents;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Warning("LoadingVars - Experimental (PersistentWorld:PersistentWorld) in ExposeGameSmallComps.");
                AccessTools.Method(typeof(Game), "FillComponents", new Type[0]).Invoke(this.Game, new object[0]);
                BackCompatibility.GameLoadingVars(this.Game);
            }
        }

        public void LoadGameWorldAndMaps()
        {
            Log.Message("Calling LoadGameWorldAndMaps");

            this.ExposeGameWorldData();
            
            Log.Message("Calling World FinalizeInit.");
            this.Game.World.FinalizeInit();

            Log.Warning("LoadingMaps after FinalizeInit of World Persistent.");
            this.LoadMaps();

            Log.Message("Gonna call continue loading maps.");
            this.ContinueLoadingMaps();
        }

        public void ContinueLoadingMaps()
        {
            if (this.Maps.RemoveAll((Map x) => x == null) != 0)
            {
                Log.Warning("Custom - Some maps were null after loading.", false);
            }

            int num = -1;
            Log.Error("Stop here. Return.");

            while (true)
            {
            }
        }

        private void LoadMaps()
        {
            Log.Warning("LoadingMaps");
            PersistentWorldManager.WorldLoader.LoadMaps(this);
            Log.Message("map laoding done.,");
        }

        public void ExposeGameWorldData()
        {
            Log.Message("Calling ExposeGameWorldData");
            this.Game.World.info = this.WorldData.info;
            this.Game.World.grid = this.WorldData.grid;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Warning("LoadingVars - Experimental (PersistentWorld:PersistentWorld) in ExposeGameWorldData.");
                if (this.Game.World.grid == null || !this.Game.World.grid.HasWorldData)
                {
                    WorldGenerator.GenerateWithoutWorldData(this.Game.World.info.seedString);
                }
                else
                {
                    WorldGenerator.GenerateFromScribe(this.Game.World.info.seedString);
                }
            }
            else
            {
                this.ExposeAndFillGameWorldComponents();
            }
        }

        public void ConstructGameWorldComponentsAndExposeComponents()
        {
            Log.Message("Calling ConstructGameWorldComponentsAndExposeComponents");
            this.Game.World.ConstructComponents();

            this.ExposeAndFillGameWorldComponents();
        }

        public void ExposeAndFillGameWorldComponents()
        {
            Log.Message("Calling ExposeAndFillGameWorldComponents");
            
            this.Game.World.factionManager = this.WorldData.factionManager;
            this.Game.World.worldPawns = this.WorldData.worldPawns;
            this.Game.World.worldObjects = this.WorldData.worldObjectsHolder;
            this.Game.World.gameConditionManager = this.WorldData.gameConditionManager;
            this.Game.World.storyState = this.WorldData.storyState;
            this.Game.World.features = this.WorldData.worldFeatures;
            this.Game.World.components = this.WorldData.worldComponents;

            AccessTools.Method(typeof(RimWorld.Planet.World), "FillComponents", new Type[0]).Invoke(this.Game.World, new object[0]);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Log.Warning("LoadingVars - Experimental (PersistentWorld:PersistentWorld) in ExposeAndFillGameWorldComponents.");
                //BackCompatibility.WorldLoadingVars();
                if (this.WorldData.uniqueIDsManager != null)
                {
                    this.Game.uniqueIDsManager = this.WorldData.uniqueIDsManager;
                }
            }
        }

        public static PersistentWorld Convert(Game game)
        {
            Log.Warning("Run persistentworld convert.");
            PersistentWorld persistentWorld = new PersistentWorld();
            persistentWorld.Game = game;
            Current.Game = game;

            persistentWorld.WorldData = PersistentWorldData.Convert(game);

            Log.Warning("Converting colony.");
            persistentWorld.Colonies.Add(PersistentColony.Convert(game));
            
            Log.Warning("copying maps.");
            persistentWorld.Maps = game.Maps;
            
            return persistentWorld;
        }
    }
}