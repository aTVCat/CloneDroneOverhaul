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
        private PrefabAndContainer m_BodyPartsContainer;
        private PrefabAndContainer m_OutfitItemsContainer;

        private bool m_HasPopulatedBodyParts;

        public string SelectedBodyPart
        {
            get;
            set;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_BodyPartsContainer = new PrefabAndContainer(MyModdedObject, 4, 5);
            m_OutfitItemsContainer = new PrefabAndContainer(MyModdedObject, 6, 7);

            SelectedBodyPart = "Head";
        }

        protected override void PopulateItems()
        {
            if (IsPopulatingItems)
                return;

            populateBodyPartsIfNeeded();
            m_OutfitItemsContainer.ClearContainer();
            _ = StaticCoroutineRunner.StartStaticCoroutine(PopulateItemsCoroutine());
        }

        protected override IEnumerator PopulateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(false));

            OutfitsController controller = GetController<OutfitsController>();
            if (controller)
            {
                List<PersonalizationItem> list = controller.GetOutfitItemsOfBodyPart(SelectedBodyPart);
                if (!list.IsNullOrEmpty())
                {
                    foreach (OutfitItem item in list)
                    {
                        ModdedObject outfitItemDisplay = m_OutfitItemsContainer.CreateNew();
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

            OutfitsController controller = GetController<OutfitsController>();
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
                ModdedObject bodyPartDisplay = m_BodyPartsContainer.CreateNew();
                bodyPartDisplay.GetObject<Transform>(0).gameObject.SetActive(bodyPart == SelectedBodyPart);
                bodyPartDisplay.GetObject<Text>(1).text = bodyPart;
            }
            m_HasPopulatedBodyParts = true;
        }
    }
}
