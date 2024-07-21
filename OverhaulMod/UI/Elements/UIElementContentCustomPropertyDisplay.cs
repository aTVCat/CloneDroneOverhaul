using OverhaulMod.Content;
using System.Reflection;

namespace OverhaulMod.UI
{
    public class UIElementContentCustomPropertyDisplay : OverhaulUIBehaviour
    {
        public FieldInfo fieldReference
        {
            get;
            private set;
        }

        public ExclusiveContentBase contentReference
        {
            get;
            private set;
        }

        public void Populate(FieldInfo fieldInfo, ExclusiveContentBase contentBase)
        {
            fieldReference = fieldInfo;
            contentReference = contentBase;
        }

        public virtual object GetValue()
        {
            return null;
        }
    }
}
