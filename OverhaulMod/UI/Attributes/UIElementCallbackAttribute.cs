using System;

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
