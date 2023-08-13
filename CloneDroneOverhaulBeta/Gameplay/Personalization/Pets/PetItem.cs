using CDOverhaul.Gameplay.Editors.Personalization;
using CDOverhaul.Gameplay.Outfits;
using System;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetItem : PersonalizationItem
    {
        [PersonalizationEditorProperty("Visuals")]
        public OverhaulAssetInfo PetModel;

        [PersonalizationEditorSubProperty]
        public PetBehaviourSettings BehaviourSettings;

        public string[] UserSettings; // ex: bool:Follow player, int:Position type
    }
}
