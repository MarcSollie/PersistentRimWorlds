﻿using System.Collections.Generic;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace PersistentWorlds.Logic
{
    public class PersistentColonyGameData : IExposable
    {
        public sbyte currentMapIndex;
        
        private GameInfo info = new GameInfo();
        private GameRules rules = new GameRules();
        private Scenario scenario;
        private PlaySettings playSettings = new PlaySettings();
        private StoryWatcher storyWatcher = new StoryWatcher();
        private GameEnder gameEnder = new GameEnder();
        private LetterStack letterStack = new LetterStack();
        private ResearchManager researchManager = new ResearchManager();
        private Storyteller storyteller = new Storyteller();
        private History history = new History();
        private TaleManager taleManager = new TaleManager();
        private PlayLog playLog = new PlayLog();
        private BattleLog battleLog = new BattleLog();
        private OutfitDatabase outfitDatabase = new OutfitDatabase();
        private DrugPolicyDatabase drugPolicyDatabase = new DrugPolicyDatabase();
        private Tutor tutor = new Tutor();
        private DateNotifier dateNotifier = new DateNotifier();
        private List<GameComponent> gameComponents = new List<GameComponent>();

        /*
         * Camera Driver.
         */
        public Vector3 camRootPos;
        public float desiredSize;

        public void ExposeData()
        {
            if (PersistentWorldManager.PersistentWorld == null)
            {
                Log.Error("PersistentWorld is null.");

                GenScene.GoToMainMenu();

                return;
            }

            Scribe_Values.Look<sbyte>(ref currentMapIndex, "currentMapIndex", -1);

            Scribe_Deep.Look(ref info, "info");

            Scribe_Deep.Look(ref rules, "rules");

            Scribe_Deep.Look(ref scenario, "scenario");

            Scribe_Deep.Look(ref this.playSettings, "playSettings");

            Scribe_Deep.Look(ref this.storyWatcher, "storyWatcher");

            Scribe_Deep.Look(ref this.gameEnder, "gameEnder");

            Scribe_Deep.Look(ref this.letterStack, "letterStack");

            Scribe_Deep.Look(ref this.researchManager, "researchManager");

            Scribe_Deep.Look(ref this.storyteller, "storyteller");

            Scribe_Deep.Look(ref this.history, "history");

            Scribe_Deep.Look(ref this.taleManager, "taleManager");

            Scribe_Deep.Look(ref this.playLog, "playLog");

            Scribe_Deep.Look(ref this.battleLog, "battleLog");

            Scribe_Deep.Look(ref this.outfitDatabase, "outfitDatabase");

            Scribe_Deep.Look(ref this.drugPolicyDatabase, "drugPolicyDatabase");

            // Remove outfits and drug policies to prevent unneeded errors and wrong data.
            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                var crossReferencingExposables = (List<IExposable>) AccessTools
                    .Field(typeof(CrossRefHandler), "crossReferencingExposables").GetValue(Scribe.loader.crossRefs);
                
                foreach (var outfit in this.outfitDatabase.AllOutfits)
                {
                    crossReferencingExposables.Remove(outfit);
                }

                foreach (var drugPolicy in this.drugPolicyDatabase.AllPolicies)
                {
                    crossReferencingExposables.Remove(drugPolicy);
                }
            }

            Scribe_Deep.Look(ref this.tutor, "tutor");
            
            Scribe_Deep.Look(ref this.dateNotifier, "dateNotifier");
            
            Scribe_Collections.Look(ref this.gameComponents, "components", LookMode.Deep, new object[] { PersistentWorldManager.PersistentWorld.Game });
            
            Scribe_Values.Look(ref this.camRootPos, "camRootPos");
            Scribe_Values.Look(ref this.desiredSize, "desiredSize");
        }

        public void SetGame()
        {
            PersistentWorldManager.PersistentWorld.Game.currentMapIndex = this.currentMapIndex;
            
            AccessTools.Field(typeof(Game), "info").SetValue(PersistentWorldManager.PersistentWorld.Game, this.info);
            AccessTools.Field(typeof(Game), "rules").SetValue(PersistentWorldManager.PersistentWorld.Game, this.rules);
            
            PersistentWorldManager.PersistentWorld.Game.Scenario = this.scenario;
            PersistentWorldManager.PersistentWorld.Game.playSettings = this.playSettings;
            PersistentWorldManager.PersistentWorld.Game.storyWatcher = this.storyWatcher;
            PersistentWorldManager.PersistentWorld.Game.gameEnder = this.gameEnder;
            PersistentWorldManager.PersistentWorld.Game.letterStack = this.letterStack;
            PersistentWorldManager.PersistentWorld.Game.researchManager = this.researchManager;
            PersistentWorldManager.PersistentWorld.Game.storyteller = this.storyteller;
            PersistentWorldManager.PersistentWorld.Game.history = this.history;
            PersistentWorldManager.PersistentWorld.Game.taleManager = this.taleManager;
            PersistentWorldManager.PersistentWorld.Game.playLog = this.playLog;
            PersistentWorldManager.PersistentWorld.Game.battleLog = this.battleLog;
            PersistentWorldManager.PersistentWorld.Game.outfitDatabase = this.outfitDatabase;
            PersistentWorldManager.PersistentWorld.Game.drugPolicyDatabase = this.drugPolicyDatabase;
            PersistentWorldManager.PersistentWorld.Game.tutor = this.tutor;
            PersistentWorldManager.PersistentWorld.Game.dateNotifier = this.dateNotifier;
            PersistentWorldManager.PersistentWorld.Game.components = this.gameComponents;
            
            // Register outfits and drug policies to Cross-Referencer.
            foreach (var outfit in this.outfitDatabase.AllOutfits)
            {
                Scribe.loader.crossRefs.RegisterForCrossRefResolve(outfit);
            }

            foreach (var drugPolicy in this.drugPolicyDatabase.AllPolicies)
            {
                Scribe.loader.crossRefs.RegisterForCrossRefResolve(drugPolicy);
            }
        }

        public static PersistentColonyGameData Convert(Game game)
        {
            /*
             * Camera Driver.
             */
            var cameraDriverRootPos = (Vector3) AccessTools.Field(typeof(CameraDriver), "rootPos").GetValue(Find.CameraDriver);
            var cameraDriverDesiredSize = (float) AccessTools.Field(typeof(CameraDriver), "desiredSize").GetValue(Find.CameraDriver);
            
            var persistentColonyGameData = new PersistentColonyGameData
            {
                currentMapIndex = game.currentMapIndex,
                info = game.Info,
                scenario = game.Scenario,
                playSettings = game.playSettings,
                storyWatcher = game.storyWatcher,
                gameEnder = game.gameEnder,
                letterStack = game.letterStack,
                researchManager = game.researchManager,
                storyteller = game.storyteller,
                history = game.history,
                taleManager = game.taleManager,
                playLog = game.playLog,
                battleLog = game.battleLog,
                outfitDatabase = game.outfitDatabase,
                drugPolicyDatabase = game.drugPolicyDatabase,
                tutor = game.tutor,
                dateNotifier = game.dateNotifier,
                gameComponents = game.components,
                camRootPos = cameraDriverRootPos,
                desiredSize = cameraDriverDesiredSize
            };

            return persistentColonyGameData;
        }
    }
}