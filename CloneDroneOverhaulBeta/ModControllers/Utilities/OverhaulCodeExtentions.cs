using ModLibrary;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulCodeExtentions
    {
        /// <summary>
        /// Get a component of object with given index
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Type GetObject<Type>(this ModdedObject moddedObject, in int index) where Type : UnityEngine.Object
        {
            UnityEngine.Object @object = moddedObject.objects[index];
            UnityEngine.GameObject @gameObject = @object as UnityEngine.GameObject;

            return @gameObject.GetComponent<Type>();
        }

        /// <summary>
        /// Get color using hex
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static Color ConvertHexToColor(this string theString)
        {
            _ = ColorUtility.TryParseHtmlString(theString, out Color col);
            return col;
        }

        /// <summary>
        /// Check if weapon model is on fire
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool HasReplacedWithFireVariant(this WeaponModel model)
        {
            return model.GetPrivateField<bool>("_hasReplacedWithFireVariant");
        }

        public static void SetLogoAndRootButtonsVisible(this TitleScreenUI titleScreenUI, in bool value)
        {
            titleScreenUI.CallPrivateMethod("setLogoAndRootButtonsVisible", new object[] { value });
        }
    }
}
