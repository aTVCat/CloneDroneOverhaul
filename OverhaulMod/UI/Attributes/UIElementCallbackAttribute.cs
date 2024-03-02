using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementCallbackAttribute : Attribute
    {
        public bool CallOnEndEdit;

        public UIElementCallbackAttribute(bool callOnEndEdit)
        {
            CallOnEndEdit = callOnEndEdit;
        }
    }
}
