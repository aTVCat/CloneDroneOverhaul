using System;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUIElement : OverhaulBehaviour
    {
        public PersonalizationEditorUI EditorUI { get; }

        public override void Start()
        {
            if (GetType() == typeof(PersonalizationEditorUIElement))
                throw new Exception("Override PersonalizationEditorElement in " + base.gameObject.name);

            if (AssignVariablesAutomatically())
                UIController.AssignVariables(this);
        }

        protected virtual bool AssignVariablesAutomatically() => true;
    }
}
