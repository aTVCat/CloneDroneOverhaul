using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Graphics;
using OverhaulAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsWearer : PersonalizationItemsWearer
    {
        private Dictionary<string, GameObject> m_SpawnedOutfitItems = new Dictionary<string, GameObject>();

        public override void Start()
        {
            base.Start();

            RefreshItems();
            _ = OverhaulEventsController.AddEventListener<Hashtable>(OverhaulPlayerInfo.InfoReceivedEventString, onGetData);
        }

        protected override void OnDeath()
        {
            DestroyItems();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DestroyItems();

            m_SpawnedOutfitItems = null;
            OverhaulEventsController.RemoveEventListener<Hashtable>(OverhaulPlayerInfo.InfoReceivedEventString, onGetData);
        }

        private void onGetData(Hashtable table)
        {
            RefreshItems();
        }

        public override void RefreshItems()
        {
            DestroyItems();

            if (!OverhaulGamemodeManager.SupportsOutfits())
                return;

            if (Owner == null || !IsOwnerPlayer())
                return;

            string equippedItems = string.Empty;
            if (PlayerInformation)
            {
                Hashtable hashtable = PlayerInformation.Hashtable;
                if (hashtable != null && hashtable.ContainsKey("Outfits.Equipped"))
                    equippedItems = hashtable["Outfits.Equipped"].ToString();
            }
            else if (!GameModeManager.IsMultiplayer() && (OutfitsController.AllowEnemiesWearAccesories || IsOwnerPlayer()))
                equippedItems = OutfitsController.EquippedAccessories;

            List<OutfitItem> items = OutfitsController.GetOutfitItemsBySaveString(equippedItems);
            if (items.IsNullOrEmpty())
                return;

            foreach (OutfitItem accessoryItem in items)
            {
                if (!accessoryItem.Prefab || m_SpawnedOutfitItems.ContainsKey(accessoryItem.Name))
                    continue;

                Transform bodyPartTransform = Owner.GetBodyPartParent(accessoryItem.BodyPart);
                if (!bodyPartTransform)
                    continue;

                GameObject instantiatedPrefab = Instantiate(accessoryItem.Prefab, bodyPartTransform);
                SetOffset(instantiatedPrefab, accessoryItem);
                m_SpawnedOutfitItems.Add(accessoryItem.Name, instantiatedPrefab);
            }
        }

        public void DestroyItems()
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