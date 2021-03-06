﻿using System.IO;
using Verse;

namespace PersistentWorlds.Logic
{
    /// <summary>
    /// Stores information about colonies.
    /// </summary>
    public class PersistentColony : IExposable
    {
        #region Fields
        /// <summary>
        /// File info used to fetch information such as last write time.
        /// </summary>
        public FileInfo FileInfo;
        
        /// <summary>
        /// Main source of data for colony information.
        /// </summary>
        public PersistentColonyData ColonyData = new PersistentColonyData();

        /// <summary>
        /// Main source of data for colony game information.
        /// </summary>
        public PersistentColonyGameData GameData = new PersistentColonyGameData();
        #endregion
        
        #region Methods
        /// <summary>
        /// Saving/loading of colony data.
        /// </summary>
        public void ExposeData()
        {
            Scribe_Deep.Look(ref ColonyData, "data");
            
            Scribe_Deep.Look(ref GameData, "gameData");
        }
        
        /// <summary>
        /// Used in conversion/saving of the current colony in-game.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="colonyColonyData"></param>
        /// <returns></returns>
        public static PersistentColony Convert(Game game, PersistentColonyData colonyColonyData = null)
        {
            var persistentColony = new PersistentColony
            {
                ColonyData = PersistentColonyData.Convert(game, colonyColonyData),
                GameData = PersistentColonyGameData.Convert(game)
            };

            return persistentColony;
        }

        public override bool Equals(object obj)
        {
            if (obj is PersistentColony colony)
                return this.GetHashCode() == colony.GetHashCode();

            return false;
        }

        /// <summary>
        /// GetHashCode uses colony unique ID as it shouldn't ever change.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ColonyData.UniqueId;
        }

        public override string ToString()
        {
            return $"{nameof(PersistentColony)} " +
                   $"({nameof(FileInfo)}={FileInfo}, " +
                   $"{nameof(ColonyData)}={ColonyData}, " +
                   $"{nameof(GameData)}={GameData})";
        }
        #endregion
    }
}