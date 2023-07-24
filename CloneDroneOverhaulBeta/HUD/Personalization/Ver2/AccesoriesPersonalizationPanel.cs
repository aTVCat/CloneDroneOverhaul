using CDOverhaul.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class AccesoriesPersonalizationPanel : OverhaulPersonalizationPanel
    {
        private PrefabAndContainer m_BodyPartsContainer;
        private PrefabAndContainer m_OutfitItemsContainer;

        public override void Initialize()
        {
            base.Initialize();

            m_BodyPartsContainer = new PrefabAndContainer(MyModdedObject, 4, 5);
            m_OutfitItemsContainer = new PrefabAndContainer(MyModdedObject, 6, 7);
        }

        protected override void PopulateItems()
        {
            StaticCoroutineRunner.StartStaticCoroutine(PopulateItemsCoroutine());
        }

        protected override IEnumerator PopulateItemsCoroutine()
        {
            IsPopulatingItems = true;
            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(false));

            yield return StaticCoroutineRunner.StartStaticCoroutine(PlayFadeAnimation(true));
            IsPopulatingItems = false;
            yield break;
        }
    }
}
