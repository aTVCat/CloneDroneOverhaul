using System;

namespace OverhaulMod.UI
{
    /// <summary>
    /// Marks the UI element not necessary to initialize
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UIElementIgnoreIfMissingAttribute : Attribute
    {
    }
}
