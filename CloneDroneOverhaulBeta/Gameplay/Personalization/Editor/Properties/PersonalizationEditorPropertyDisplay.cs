using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorPropertyDisplay : OverhaulBehaviour
    {
        [ObjectReference("Label")]
        public Text PropertyNameLabel;

        public PersonalizationEditorPropertyAttribute AttributeReference
        {
            get;
            set;
        }

        public override void Start()
        {
            OverhaulUIVer2.AssignVariables(this);
        }
    }
}
