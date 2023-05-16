using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Multiplayer.Mods
{
    public class OverhaulMultiplayerCore
    {
        #region Instance

        /// <summary>
        /// The instance of the modded multiplayer core
        /// </summary>
        public OverhaulMultiplayerCore CoreInstance
        {
            get;
            private set;
        }

        public OverhaulMultiplayerCore()
        {
            CoreInstance = this;
            InitializeCore();
        }

        #endregion

        #region Static data 

        /// <summary>
        /// All prefabs that can work with this kind of multiplayer
        /// </summary>
        public static readonly Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

        #endregion

        public OverhaulMultiplayerEnvironment Environment
        {
            get;
            private set;
        }

        public void InitializeCore()
        {

        }
    }
}
