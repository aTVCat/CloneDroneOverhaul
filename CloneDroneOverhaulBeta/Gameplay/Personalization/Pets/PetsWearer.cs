using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.Outfits;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsWearer : PersonalizationItemsWearer
    {
        private readonly List<PetInstanceBehaviour> m_SpawnedPets = new List<PetInstanceBehaviour>();

        private Transform m_HeadTransform;
        public Transform HeadTransform
        {
            get
            {
                if(!m_HeadTransform)
                    m_HeadTransform = Owner.GetBodyPartParent("Head");

                return m_HeadTransform;
            }
        }

        public override void Start()
        {
            base.Start();
            RefreshItems();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
            DestroyItems();
        }

        protected override void OnDeath()
        {
            DestroyItems();
        }

        private void Update()
        {
            if (!Owner.IsMainPlayer())
                return;

            if (Input.GetKeyDown(KeyCode.P))
                TogglePetsVisibility();
        }

        public override void RefreshItems()
        {
            DestroyItems();

            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsNewPersonalizationSystemEnabled)
                return;

            if (!Owner)
                return;

            string equippedItems = string.Empty;
            if (PlayerInformation)
            {
                Hashtable hashtable = PlayerInformation.Hashtable;
                if (hashtable != null && hashtable.ContainsKey("Pets.Equipped"))
                    equippedItems = hashtable["Pets.Equipped"].ToString();
            }
            else if (!GameModeManager.IsMultiplayer() && (PetsController.AllowEnemiesUsePets || IsOwnerPlayer()))
                equippedItems = PetsController.EquippedPets;

            foreach(PetItem petItem in PetsController.GetPetItemsBySaveString(equippedItems))
            {
                m_SpawnedPets.Add(PetInstanceBehaviour.CreateInstance(petItem, Owner));
            }
        }

        public void DestroyItems()
        {
            foreach (PetInstanceBehaviour b in m_SpawnedPets)
            {
                if (!b)
                    continue;

                b.DestroyGameObject();
            }
            m_SpawnedPets.Clear();
        }

        public void TogglePetsVisibility()
        {
            foreach (PetInstanceBehaviour b in m_SpawnedPets)
            {
                if (!b)
                    continue;

                b.gameObject.SetActive(!b.gameObject.activeSelf);
            }
        }
    }
}
