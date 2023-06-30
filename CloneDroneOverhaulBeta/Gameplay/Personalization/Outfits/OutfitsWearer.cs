using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Graphics;
using OverhaulAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsWearer : OverhaulCharacterExpansion
    {
        private Dictionary<string, GameObject> m_SpawnedOutfitItems = new Dictionary<string, GameObject>();

        private OverhaulModdedPlayerInfo m_Info;
        public bool HasPlayerInfo => m_Info != null;

        private bool m_HasAddedEventListeners;

        public override void Start()
        {
            base.Start();

            m_Info = OverhaulModdedPlayerInfo.GetPlayerInfo(Owner);
            _ = OverhaulEventsController.AddEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
            m_HasAddedEventListeners = true;

            DelegateScheduler.Instance.Schedule(SpawnItems, 0.2f);
        }

        protected override void OnDeath()
        {
            DestroyAccessories();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DestroyAccessories();

            m_Info = null;
            m_SpawnedOutfitItems = null;
            if (m_HasAddedEventListeners)
            {
                m_HasAddedEventListeners = false;
                OverhaulEventsController.RemoveEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
            }
        }

        private void onGetData(Hashtable table)
        {
            SpawnItems();
        }

        public void SpawnItems()
        {
            DestroyAccessories();

            if (!OverhaulGamemodeManager.SupportsOutfits())
                return;

            if (Owner == null || !IsOwnerPlayer())
                return;

            string equippedItems = string.Empty;
            if (HasPlayerInfo)
            {
                Hashtable hashtable = m_Info.GetHashtable();
                if (hashtable != null && hashtable.ContainsKey("Outfits.Equipped"))
                    equippedItems = hashtable["Outfits.Equipped"].ToString();
            }
            else if (!GameModeManager.IsMultiplayer())
                equippedItems = OutfitsController.EquippedAccessories;

            List<OutfitItem> items = OutfitsController.GetOutfitItems(equippedItems);
            if (items.IsNullOrEmpty())
                return;

            foreach (OutfitItem accessoryItem in items)
            {
                if (!accessoryItem.Prefab)
                    continue;

                MechBodyPart bodyPart = Owner.GetBodyPart(accessoryItem.BodyPart);
                if (!bodyPart)
                    continue;

                Transform bodyPartTransform = bodyPart.transform.parent;
                if (!bodyPartTransform)
                    continue;

                GameObject instantiatedPrefab = Instantiate(accessoryItem.Prefab, bodyPartTransform);
                SetOffset(instantiatedPrefab, accessoryItem);
                m_SpawnedOutfitItems.Add(accessoryItem.Name, instantiatedPrefab);
            }
        }

        public void DestroyAccessories()
        {
            if (m_SpawnedOutfitItems.IsNullOrEmpty())
                return;

            foreach (string key in m_SpawnedOutfitItems.Keys)
            {
                GameObject toDestroy = m_SpawnedOutfitItems[key];
                if (toDestroy)
                    Destroy(toDestroy);
            }
            m_SpawnedOutfitItems.Clear();
        }

        public void SetOffset(GameObject outfitItem, OutfitItem item)
        {
            if (!outfitItem || item == null || !Owner || !Owner.HasCharacterModel())
                return;

            string characterModelName = Owner.GetCharacterModel().gameObject.name;
            if (string.IsNullOrEmpty(characterModelName) || item.Offsets.IsNullOrEmpty() || !item.Offsets.ContainsKey(characterModelName))
            {
                outfitItem.SetActive(false);
                return;
            }

            ModelOffset modelOffset = item.Offsets[characterModelName];
            if (modelOffset.OffsetLocalScale == Vector3.zero)
            {
                outfitItem.SetActive(false);
                return;
            }

            Transform outfitItemTransform = outfitItem.transform;
            outfitItemTransform.localPosition = modelOffset.OffsetPosition;
            outfitItemTransform.localEulerAngles = modelOffset.OffsetEulerAngles;
            outfitItemTransform.localScale = modelOffset.OffsetLocalScale;
        }

        private void Update()
        {
            if (Time.frameCount % 10 == 0 && !m_SpawnedOutfitItems.IsNullOrEmpty() && Owner.IsMainPlayer())
            {
                foreach (GameObject gm in m_SpawnedOutfitItems.Values)
                {
                    if (!gm)
                        continue;

                    gm.SetActive(!ViewModesController.IsFirstPersonModeEnabled || PhotoManager.Instance.IsInPhotoMode());
                }
            }
        }
    }
}