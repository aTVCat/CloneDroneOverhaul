using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetsWearer : PersonalizationItemsWearer
    {
        private readonly List<PetInstanceBehaviour> m_SpawnedPets = new List<PetInstanceBehaviour>();

        public Transform HeadTransform
        {
            get;
            private set;
        }

        public override void Start()
        {
            base.Start();

            HeadTransform = Owner.GetBodyPartParent("Head");
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
            //m_SpawnedPets.Add(PetInstanceBehaviour.CreateInstance(PetsController.Pets[1], Owner));
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
