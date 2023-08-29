using System;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUIElement : OverhaulBehaviour
    {
        private PersonalizationEditorUI m_EditorUI;
        public PersonalizationEditorUI EditorUI
        {
            get
            {
                /*
                if (!m_EditorUI)
                    m_EditorUI = OverhaulController.Get<PersonalizationEditorUI>();*/

                return m_EditorUI;
            }
        }

        public override void Start()
        {
            if (GetType() == typeof(PersonalizationEditorUIElement))
                throw new Exception("Override PersonalizationEditorElement in " + base.gameObject.name);

            if (AssignVariablesAutomatically())
                UIController.AssignValues(this);
        }

        protected virtual bool AssignVariablesAutomatically() => true;
    }
}
