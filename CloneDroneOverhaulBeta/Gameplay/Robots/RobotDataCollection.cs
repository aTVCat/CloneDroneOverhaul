using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// The main script with all temp data about <see cref="FirstPersonMover"/>
    /// </summary>
    public class RobotDataCollection : FirstPersonMoverExtention
    {
        public const string DataCollectionInitializedEventString = "FPMDataCollectionInitialized";
        private List<(GameObject, string, bool, bool)> _skinsCollection;

        protected override void Initialize(FirstPersonMover owner)
        {
            _skinsCollection = new List<(GameObject, string, bool, bool)>();

            OverhaulEventManager.DispatchEvent<RobotDataCollection>(DataCollectionInitializedEventString, this);
        }

        /// <summary>
        /// Let game understand that we already spawned specific skin
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id"></param>
        /// <param name="fire"></param>
        /// <param name="multiplayer"></param>
        public void RegisterSpawnedSkin(in GameObject model, in string id, in bool fire, in bool multiplayer)
        {
            (GameObject, string, bool, bool) newData = (model, id, fire, multiplayer);
            _skinsCollection.Add(newData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fire"></param>
        /// <param name="multiplayer"></param>
        /// <returns>Returns <b>True</b> if we haven't spawned specific skin before, otherwise <b>False</b></returns>
        public bool AllowSkinRegistration(in string id, in bool fire, in bool multiplayer)
        {
            foreach ((GameObject, string, bool, bool) data in _skinsCollection)
            {
                if (data.Item2 == id && data.Item3 == fire && data.Item4 == multiplayer)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Hide all skin models
        /// </summary>
        public void HideAllSkins()
        {
            foreach ((GameObject, string, bool, bool) data in _skinsCollection)
            {
                data.Item1.SetActive(false);
            }
        }

        /// <summary>
        /// D e s t r o y  a l l  s k i n s
        /// </summary>
        public void DestroyAllSkins()
        {
            foreach ((GameObject, string, bool, bool) data in _skinsCollection)
            {
                Destroy(data.Item1);
            }
            _skinsCollection.Clear();
        }

        /// <summary>
        /// Show specific skin model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fire"></param>
        /// <param name="multiplayer"></param>
        public void ShowSkin(in string id, in bool fire, in bool multiplayer)
        {
            foreach ((GameObject, string, bool, bool) data in _skinsCollection)
            {
                if (data.Item2 == id && data.Item3 == fire && data.Item4 == multiplayer)
                {
                    data.Item1.SetActive(true);
                }
            }
        }
    }
}