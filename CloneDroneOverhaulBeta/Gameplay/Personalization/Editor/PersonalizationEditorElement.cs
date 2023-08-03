using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorElement : OverhaulBehaviour
    {
        private PersonalizationEditorUI m_EditorUI;
        public PersonalizationEditorUI EditorUI
        {
            get
            {
                if (!m_EditorUI)
                    m_EditorUI = OverhaulController.GetController<PersonalizationEditorUI>();

                return m_EditorUI;
            }
        }

        public override void Start()
        {
            if (GetType() == typeof(PersonalizationEditorElement))
                throw new Exception("Override PersonalizationEditorElement in " + base.gameObject.name);

            OverhaulUIVer2.AssignVariables(this);
        }
    }
}
