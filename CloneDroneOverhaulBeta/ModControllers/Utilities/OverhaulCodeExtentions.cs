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
        /// Get component in parents, not only parent
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static T GetComponentInParents<T>(this GameObject gameObject) where T : Component
        {
            T result = null;
            Transform transform2 = gameObject.transform;
            while (result == null && transform2 != null)
            {
                result = transform2.GetComponent<T>();
                transform2 = transform2.parent;
            }
            return result;
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
