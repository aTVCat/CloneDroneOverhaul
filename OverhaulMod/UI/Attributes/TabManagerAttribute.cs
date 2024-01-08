using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TabManagerAttribute : Attribute
    {
        public string PrefabFieldName, ContainerFieldName, OnTabCreatedCallbackMethodName, OnTabSelectedCallbackMethodName;
        public string[] Tabs;
        public Type ComponentType;

        public TabManagerAttribute(Type type, string prefabFieldName, string containerFieldName, string onTabCreatedCallbackMethodName, string onTabSelectedCallbackMethodName, string[] tabs = null)
        {
            if (onTabCreatedCallbackMethodName == null)
                onTabCreatedCallbackMethodName = string.Empty;

            if (onTabSelectedCallbackMethodName == null)
                onTabSelectedCallbackMethodName = string.Empty;

            PrefabFieldName = prefabFieldName;
            ContainerFieldName = containerFieldName;
            OnTabCreatedCallbackMethodName = onTabCreatedCallbackMethodName;
            OnTabSelectedCallbackMethodName = onTabSelectedCallbackMethodName;
            Tabs = tabs;
            ComponentType = type;
        }
    }
}
