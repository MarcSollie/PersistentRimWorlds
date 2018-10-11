using System.Collections.Generic;
using PersistentWorlds.Logic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace PersistentWorlds.World
{
    /// <summary>
    /// Colony world object class that handles colonies on the world map.
    /// </summary>
    public class Colony : MapParent
    {
        // TODO: Implement commands.
        //public static readonly Texture2D VisitCommand = ContentFinder<Texture2D>.Get("UI/Commands/Visit", true);

        public string Name;
        public PersistentColonyData PersistentColonyData;
        private Material cachedMat;
        
        /// <summary>
        /// Material for colonies, such as color properties that are set here depending on colony data.
        /// </summary>
        public override Material Material
        {
            get
            {
                if (this.cachedMat == null)
                    this.cachedMat = MaterialPool.MatFrom("World/WorldObjects/DefaultSettlement", ShaderDatabase.WorldOverlayTransparentLit, Color.green,
                        WorldMaterials.WorldObjectRenderQueue);

                return this.cachedMat;
            }
        }

        public override Texture2D ExpandingIcon => ContentFinder<Texture2D>.Get("World/WorldObjects/Expanding/Town", true);

        public override string Label => Name ?? base.Label;
        
        /// <summary>
        /// Saving/loading of colony world object instance.
        /// </summary>
        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look<string>(ref Name, "name");
            Scribe_References.Look<PersistentColonyData>(ref PersistentColonyData, "colony");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            // TODO: Add visit.
            
            return base.GetGizmos();
        }

        /// <summary>
        /// Inspect string that shows up in bottom-left corner of screen when world object is clicked.
        /// </summary>
        /// <returns></returns>
        public override string GetInspectString()
        {
            var inspectString = "";

            inspectString += "Colony: " + this.PersistentColonyData.ColonyFaction.Name;
            
            return inspectString;
        }

        public override IEnumerable<InspectTabBase> GetInspectTabs()
        {
            return base.GetInspectTabs();
        }
    }
}