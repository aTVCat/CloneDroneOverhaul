using OverhaulMod.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
