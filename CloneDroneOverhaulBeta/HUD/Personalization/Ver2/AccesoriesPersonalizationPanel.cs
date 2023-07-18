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
        public override PersonalizationCategory GetCategory() => PersonalizationCategory.Outfits;

        protected override void PopulateItems()
        {

        }

        protected override IEnumerator populateItemsCoroutine()
        {
            IsPopulatingItems = true;

            IsPopulatingItems = false;
            yield break;
        }

        protected override Vector3 GetTargetPosition() => new Vector3(135f, 0f, 0f);
    }
}
