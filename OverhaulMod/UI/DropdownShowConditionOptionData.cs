using OverhaulMod.Content.Personalization;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class DropdownShowConditionOptionData : Dropdown.OptionData
    {
        public PersonalizationEditorObjectShowConditions Value;

        public DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions value)
        {
            switch (value)
            {
                case PersonalizationEditorObjectShowConditions.IsNormal:
                    text = "Normal";
                    break;
                case PersonalizationEditorObjectShowConditions.IsOnFire:
                    text = "Fire";
                    break;
                case PersonalizationEditorObjectShowConditions.IsNormalMultiplayer:
                    text = "Normal (Multiplayer)";
                    break;
                case PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer:
                    text = "Fire (Multiplayer)";
                    break;
                default:
                    text = value.ToString();
                    break;
            }
            Value = value;
        }
    }
}
