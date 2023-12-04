using UnityEngine;

namespace CDOverhaul.HUD
{
    public static class UIUtility
    {
        public static UIElementDropdown InitDropdown(GameObject target)
        {
            if (!target)
                return null;

            UIElementDropdown result = target.GetComponent<UIElementDropdown>();
            if (!result)
            {
                result = target.AddComponent<UIElementDropdown>();
                result.Initialize();
            }
            return result;
        }
    }
}
