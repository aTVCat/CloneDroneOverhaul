using CDOverhaul.Gameplay.Multiplayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsWearer : OverhaulCharacterExpansion
    {
        public const string IDInHashtable = "Outfits.Equipped";

        private OverhaulModdedPlayerInfo m_Info;
        public bool HasPlayerInfo => m_Info != null;

        private Dictionary<string, GameObject> m_SpawnedAccessories;

        public override void Start()
        {
            base.Start();

            m_SpawnedAccessories = new Dictionary<string, GameObject>();

            if (IsOwnerMainPlayer())
            {
                m_Info = OverhaulModdedPlayerInfo.GetLocalPlayerInfo();
            }
            else
            {
                m_Info = OverhaulModdedPlayerInfo.GetPlayerInfo(Owner);
            }
            _ = OverhaulEventManager.AddEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);

            SpawnAccessories();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            OverhaulEventManager.RemoveEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
            m_Info = null;
            m_SpawnedAccessories.Clear();
            m_SpawnedAccessories = null;
        }

        private void onGetData(Hashtable table)
        {
            SpawnAccessories();
        }

        public void SpawnAccessories()
        {
            if (!m_SpawnedAccessories.IsNullOrEmpty())
            {
                foreach(string key in m_SpawnedAccessories.Keys)
                {
                    GameObject toDestroy = m_SpawnedAccessories[key];
                    if(toDestroy != null)
                    {
                        Destroy(toDestroy);
                    }
                }
                m_SpawnedAccessories.Clear();
            }

            if (Owner == null || !IsOwnerPlayer())
            {
                return;
            }

            string equippedItems = null;
            if (HasPlayerInfo)
            {
                Hashtable hashtable = m_Info.GetHashtable();
                if(hashtable != null && hashtable.ContainsKey(IDInHashtable))
                {
                    equippedItems = hashtable[IDInHashtable].ToString();
                }
            }
            else
            {
                equippedItems = OutfitsController.EquippedAccessories;
            }

            List<AccessoryItem> items = OutfitsController.GetAccessories(equippedItems);
            if (items.IsNullOrEmpty())
            {
                return;
            }

            foreach(AccessoryItem accessoryItem in items)
            {
                if(accessoryItem.Prefab == null)
                {
                    continue;
                }

                if (accessoryItem.Type.Equals(AccessoryType.Attached))
                {
                    MechBodyPart bodyPart = Owner.GetBodyPart(accessoryItem.BodyPart);
                    if(bodyPart == null)
                    {
                        continue;
                    }

                    Transform bodyPartTransform = bodyPart.transform.parent;
                    if(bodyPartTransform == null)
                    {
                        continue;
                    }

                    GameObject instantiatedPrefab = Instantiate(accessoryItem.Prefab, bodyPartTransform);
                    instantiatedPrefab.transform.localPosition = Vector3.zero;
                    instantiatedPrefab.transform.localEulerAngles = Vector3.zero;
                    instantiatedPrefab.transform.localScale = Vector3.one;
                    instantiatedPrefab.SetActive(true);
                    m_SpawnedAccessories.Add(accessoryItem.Name, instantiatedPrefab);
                }
            }
        }
    }
}