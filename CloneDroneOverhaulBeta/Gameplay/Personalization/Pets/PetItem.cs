using CDOverhaul.Gameplay.Editors.Personalization;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetItem : PersonalizationItem
    {
        [PersonalizationEditorProperty("Visuals")]
        public OverhaulAssetInfo PetModel;

        [PersonalizationEditorProperty("Visuals")]
        public OverhaulVoxAssetInfo PetVoxModel;

        [PersonalizationEditorSubProperty]
        public PetBehaviourSettings BehaviourSettings;

        public string[] UserSettings; // ex: bool:Follow player, int:Position type
    }
}
