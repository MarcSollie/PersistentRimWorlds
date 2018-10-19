using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using PersistentWorlds.Logic;
using RimWorld;
using Verse;

namespace PersistentWorlds.SaveAndLoad
{
    public static class DynamicMapLoader
    {
        #region Fields
        private static readonly FieldInfo reservedDestinationsField =
            AccessTools.Field(typeof(PawnDestinationReservationManager), "reservedDestinations");
        #endregion
        
        #region Properties
        private static PersistentWorld PersistentWorld => PersistentWorldManager.GetInstance().PersistentWorld;
        #endregion
        
        #region Methods
        #region Map Loading Methods
        public static IEnumerable<Map> LoadMaps(params int[] tiles)
        {
            Current.ProgramState = ProgramState.MapInitializing;

            var maps = new List<Map>(PersistentWorld.LoadSaver.LoadMaps(tiles));

            foreach (var map in maps)
            {
                Current.Game.Maps.Add(map);

                SetupLoadedMap(map);
            }

            Current.ProgramState = ProgramState.Playing;

            return maps.AsEnumerable();
        }
        
        public static Map LoadMap(int tile)
        {
            return LoadMaps(tile).First();    
        }
        
        public static IEnumerable<Map> LoadColonyMaps(PersistentColony colony)
        {
            var maps = LoadMaps(colony.ColonyData.ActiveWorldTiles.ToArray());
            
            foreach (var map in maps)
            {
                if (PersistentWorld.Maps.ContainsKey(colony))
                {
                    PersistentWorld.Maps[colony].Add(map.Tile);
                }
                else
                {
                    PersistentWorld.Maps.Add(colony, new List<int>() { map.Tile });
                }
                
                yield return map;
            }
        }
        #endregion
        
        #region Map Data Loading Methods
        private static void SetupLoadedMap(Map map)
        {
            /*
            * Register in case as to not cause problems.
            */
            
            var reservedDestinations =
                (Dictionary<Faction, PawnDestinationReservationManager.PawnDestinationSet>)
                reservedDestinationsField.GetValue(map.pawnDestinationReservationManager);

            foreach (var faction in Find.FactionManager.AllFactions)
            {
                if (!reservedDestinations.ContainsKey(faction))
                {
                    map.pawnDestinationReservationManager.RegisterFaction(faction);
                }
            }
            
            /*
             * Regenerate map data and finalize loading.
             */
            
            map.mapDrawer.RegenerateEverythingNow();
            map.FinalizeLoading();
            map.Parent.FinalizeLoading();
        }
        #endregion
        #endregion
    }
}