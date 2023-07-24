using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Graphics;
using OverhaulAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Outfits
{
    public class OutfitsWearer : OverhaulCharacterExpansion
    {
        private Dictionary<string, GameObject> m_SpawnedOutfitItems = new Dictionary<string, GameObject>();

        private bool m_HasAddedEventListeners;

        public OverhaulPlayerInfo PlayerInformation
        {
            get;
            private set;
        }

        public override void Start()
        {
            base.Start();

            PlayerInformation = OverhaulPlayerInfo.GetOverhaulPlayerInfo(Owner);
            _ = OverhaulEventsController.AddEventListener<Hashtable>(OverhaulPlayerInfo.InfoReceivedEventString, onGetData);

            if (!IsOwnerMainPlayer())
            {
                DelegateScheduler.Instance.Schedule(SpawnItems, 0.2f);
            }
            else
            {
                SpawnItems();
            }
            m_HasAddedEventListeners = true;
        }

        protected override void OnDeath()
        {
            DestroyItems();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DestroyItems();

            PlayerInformation = null;
            m_SpawnedOutfitItems = null;
            if (m_HasAddedEventListeners)
            {
                OverhaulEventsController.RemoveEventListener<Hashtable>(OverhaulPlayerInfo.InfoReceivedEventString, onGetData);
                m_HasAddedEventListeners = false;
            }
        }

        private void onGetData(Hashtable table)
        {
            PlayerInformation = OverhaulPlayerInfo.GetOverhaulPlayerInfo(Owner);
            SpawnItems();
        }

        public void SpawnItems()
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
                if (!accessoryItem.Prefab)
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