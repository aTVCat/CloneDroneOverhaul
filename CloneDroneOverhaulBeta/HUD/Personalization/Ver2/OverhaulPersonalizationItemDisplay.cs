using CDOverhaul.Gameplay.Outfits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPersonalizationItemDisplay : OverhaulBehaviour
    {
        [ObjectReference("OutfitItemEntry")]
        protected Button ButtonComponent;
        [ObjectReference("OutfitItemEntry")]
        protected Animation AnimationComponent;

        [ObjectReference("Name")]
        protected Text ItemNameLabel;
        [ObjectReference("Author")]
        protected Text AuthorLabel;

        [ObjectReference("ExclusivityIndicator")]
        protected GameObject ExclusiveIndicator;
        [ObjectReference("Selected")]
        protected GameObject SelectedIndicator;

        public override void Start()
        {
            OverhaulUIVer2.FillVariables(this);
        }
    }
}
