﻿using System.Collections.Generic;
using PersistentWorlds.Logic;
using PersistentWorlds.SaveAndLoad;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Profile;

namespace PersistentWorlds.UI
{
    [StaticConstructorOnStartup]
    public sealed class Page_PersistentWorlds_LoadWorld_ColonySelection : Page
    {        
        #region Fields
        private static readonly Texture2D Town = ContentFinder<Texture2D>.Get("World/WorldObjects/Expanding/Town");

        private readonly PersistentWorld persistentWorld;

        private bool normalClose = true;
        #endregion
        
        #region Properties
        public override Vector2 InitialSize => new Vector2(600f, 700f);
        #endregion
        
        #region Constructors
        public Page_PersistentWorlds_LoadWorld_ColonySelection(PersistentWorld persistentWorld)
        {
            this.persistentWorld = persistentWorld;
            persistentWorld.LoadSaver.LoadColonies();
            
            this.doCloseButton = true;
            this.doCloseX = true;
            this.forcePause = true;
            this.absorbInputAroundWindow = true;
            this.closeOnAccept = false;
        }
        #endregion
        
        #region Methods
        public override void PostClose()
        {
            base.PostClose();
            
            ColonyUI.Reset();
            
            if (!normalClose) return;

            this.DoBack();
            PersistentWorldManager.GetInstance().Clear();
        }

        public override void DoWindowContents(Rect inRect)
        {
            /*
            GUI.BeginGroup(inRect);

            var rect1 = new Rect((inRect.width - 170f) / 2, 0.0f, 170f, inRect.height);

            var optList = new List<ListableOption>
            {
                new ListableOption("NewColony".Translate(),
                    delegate
                    {
                        normalClose = false;
                        
                        PersistentWorldManager.GetInstance().PersistentWorld = this.persistentWorld;

                        this.next = new Page_SelectScenario {prev = this};
                        this.DoNext();
                    })
            };

            var num1 = (double) OptionListingUtility.DrawOptionListing(rect1, optList);
            
            var rect2 = new Rect(0, (float) num1, inRect.width, inRect.height);
            
            GUI.BeginGroup(rect2);
            GUI.EndGroup();
            
            GUI.EndGroup();
            */
            
            ColonyUI.DrawColoniesList(ref inRect, this.Margin, this.persistentWorld.Colonies, this.Load);
        }

        private void Load(int index)
        {
            var colony = this.persistentWorld.Colonies[index];
            
            normalClose = false;
                        
            PersistentWorldManager.GetInstance().PersistentWorld = this.persistentWorld;
                        
            this.persistentWorld.LoadSaver.LoadColony(ref colony);
            this.persistentWorld.Colonies[index] = colony;

            // This line cause UIRoot_Play to throw one error due to null world/maps, can be patched to check if null before running.
            MemoryUtility.ClearAllMapsAndWorld();

            this.persistentWorld.PatchPlayerFaction();
            this.persistentWorld.LoadSaver.TransferToPlayScene();
        }
        #endregion
    }
}