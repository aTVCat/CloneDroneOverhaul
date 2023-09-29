using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyDisplay : PersonalizationEditorUIElement
    {
        [UIElementReferenceAttribute("Label")]
        public Text PropertyNameLabel;

        public PersonalizationEditorPropertyAttribute AttributeReference
        {
            get;
            set;
        }

        public override void Start()
        {
            UIController.AssignVariables(this);
        }
    }
}
