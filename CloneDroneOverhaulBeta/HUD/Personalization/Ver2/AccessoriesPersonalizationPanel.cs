using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Outfits;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AccessoriesPersonalizationPanel : OverhaulPersonalizationPanel
    {
        private PrefabContainer m_BodyPartsContainer;
        private PrefabContainer m_OutfitItemsContainer;

        private bool m_HasPopulatedBodyParts;

        public string SelectedBodyPart
        {
            get;
            set;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_BodyPartsContainer = new PrefabContainer(MyModdedObject, 4, 5);
            m_OutfitItemsContainer = new PrefabContainer(MyModdedObject, 6, 7);

            SelectedBodyPart = "Head";
        }

        protected override void PopulateItems()
        {
            if (IsPopulatingItems)
                return;

            populateBodyPartsIfNeeded();
            m_OutfitItemsContainer.Clear();
            _ = StaticCoroutineRunner.StartStaticCoroutine(PopulateItemsCoroutine());
        }

        protected override IEnumerator PopulateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(false));

            OutfitsSystem controller = PersonalizationManager.reference?.outfits;
            if (controller)
            {
                List<PersonalizationItem> list = controller.GetOutfitItemsOfBodyPart(SelectedBodyPart);
                if (!list.IsNullOrEmpty())
                {
                    foreach (OutfitItem item in list)
                    {
                        ModdedObject outfitItemDisplay = m_OutfitItemsContainer.InstantiateEntry();
                        AccesoryItemDisplay displayComponent = outfitItemDisplay.gameObject.AddComponent<AccesoryItemDisplay>();
                        displayComponent.Initialize(item);
                    }
                }
            }

            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(true));
            IsPopulatingItems = false;
            yield break;
        }

        private void populateBodyPartsIfNeeded()
        {
            if (m_HasPopulatedBodyParts)
                return;

            OutfitsSystem controller = PersonalizationManager.reference?.outfits;
            if (!controller || controller.Items.IsNullOrEmpty())
                return;

            List<string> allBodyParts = new List<string>();
            List<PersonalizationItem> list = controller.Items;
            foreach (OutfitItem item in list)
            {
                if (!allBodyParts.Contains(item.BodyPart))
                    allBodyParts.Add(item.BodyPart);
            }

            foreach (string bodyPart in allBodyParts)
            {
                ModdedObject bodyPartDisplay = m_BodyPartsContainer.InstantiateEntry();
                bodyPartDisplay.GetObject<Transform>(0).gameObject.SetActive(bodyPart == SelectedBodyPart);
                bodyPartDisplay.GetObject<Text>(1).text = bodyPart;
            }
            m_HasPopulatedBodyParts = true;
        }
    }
}
