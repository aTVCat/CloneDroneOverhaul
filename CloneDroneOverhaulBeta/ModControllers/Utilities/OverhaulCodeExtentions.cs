using ModLibrary;

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

        public static bool HasReplacedWithFireVariant(this WeaponModel model)
        {
            return model.GetPrivateField<bool>("_hasReplacedWithFireVariant");
        }
    }
}
