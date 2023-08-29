using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Pets;
using System.Collections;
using System.Collections.Generic;

namespace CDOverhaul.HUD
{
    public class MountsPersonalizationPanel : OverhaulPersonalizationPanel
    {
        private PrefabAndContainer m_PetItemsContainer;

        public override void Initialize()
        {
            base.Initialize();
            m_PetItemsContainer = new PrefabAndContainer(MyModdedObject, 6, 7);
        }

        protected override void PopulateItems()
        {
            if (IsPopulatingItems)
                return;

            m_PetItemsContainer.ClearContainer();
            _ = StaticCoroutineRunner.StartStaticCoroutine(PopulateItemsCoroutine());
        }

        protected override IEnumerator PopulateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(false));

            PetSystem controller = PersonalizationManager.reference?.pets;
            if (controller)
            {
                List<PersonalizationItem> list = controller.Items;
                if (!list.IsNullOrEmpty())
                {
                    foreach (PetItem item in list)
                    {
                        ModdedObject outfitItemDisplay = m_PetItemsContainer.CreateNew();
                        MountItemDisplay displayComponent = outfitItemDisplay.gameObject.AddComponent<MountItemDisplay>();
                        displayComponent.Initialize(item);
                    }
                }
            }

            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(true));
            IsPopulatingItems = false;
            yield break;
        }
    }
}
