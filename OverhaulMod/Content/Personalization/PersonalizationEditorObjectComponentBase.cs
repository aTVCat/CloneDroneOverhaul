using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectComponentBase : MonoBehaviour
    {
        private PersonalizationEditorObjectBehaviour _objectBehaviour;
        public PersonalizationEditorObjectBehaviour objectBehaviour
        {
            get
            {
                if (!_objectBehaviour)
                {
                    _objectBehaviour = base.GetComponent<PersonalizationEditorObjectBehaviour>();
                }
                return _objectBehaviour;
            }
        }
    }
}
