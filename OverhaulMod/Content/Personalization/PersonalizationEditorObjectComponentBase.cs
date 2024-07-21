using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectComponentBase : MonoBehaviour
    {
        private PersonalizationEditorObjectBehaviour m_objectBehaviour;
        public PersonalizationEditorObjectBehaviour objectBehaviour
        {
            get
            {
                if (!m_objectBehaviour)
                {
                    m_objectBehaviour = base.GetComponent<PersonalizationEditorObjectBehaviour>();
                }
                return m_objectBehaviour;
            }
        }
    }
}
