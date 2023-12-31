using OverhaulMod.Combat.Weapons;
using OverhaulMod.Engine;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Utils
{
    internal static class ModExtensions
    {
        public static T GetObject<T>(this ModdedObject moddedObject, int index) where T : Object
        {
            if (!moddedObject)
                return null;

            GameObject gameObject = moddedObject.objects[index] as GameObject;
            return typeof(T) == typeof(GameObject) ? gameObject as T : (gameObject?.GetComponent<T>());
        }

        public static Object GetObject(this ModdedObject moddedObject, System.Type type, int index)
        {
            if (!moddedObject)
                return null;

            GameObject gameObject = moddedObject.objects[index] as GameObject;
            return type == typeof(GameObject) ? gameObject : (Object)(gameObject?.GetComponent(type));
        }

        public static T GetObject<T>(this ModdedObject moddedObject, string name) where T : Object
        {
            if (!moddedObject)
                return null;

            GameObject gameObject = null;
            foreach (Object @object in moddedObject.objects)
            {
                if (@object.name == name)
                {
                    gameObject = @object as GameObject;
                    break;
                }
            }
            return typeof(T) == typeof(GameObject) ? gameObject as T : (gameObject?.GetComponent<T>());
        }

        public static Object GetObject(this ModdedObject moddedObject, System.Type type, string name)
        {
            if (!moddedObject)
                return null;

            GameObject gameObject = null;
            foreach (Object @object in moddedObject.objects)
            {
                if (@object.name == name)
                {
                    gameObject = @object as GameObject;
                    break;
                }
            }
            return type == typeof(GameObject) ? gameObject : (Object)(gameObject?.GetComponent(type));
        }

        public static void SetLocalTransform(this Transform transform, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
        {
            transform.localPosition = localPosition;
            transform.localEulerAngles = localEulerAngles;
            transform.localScale = localScale;
        }

        public static void SetLocalTransform(this Transform transform, Vector3 localPosition, Vector3 localEulerAngles)
        {
            transform.localPosition = localPosition;
            transform.localEulerAngles = localEulerAngles;
        }

        public static void SetLocalTransform(this Transform transform, Vector3 localPosition)
        {
            transform.localPosition = localPosition;
        }

        public static void SetLocalTransform(this Transform transform, TransformInfo transformInfo)
        {
            transform.SetLocalTransform(transformInfo.Position, transformInfo.EulerAngles, transformInfo.LocalScale);
        }

        public static void RandomizeLocalTransform(this Transform transform, float min, float max, bool rPos, bool rRot, bool rLS)
        {
            transform.SetLocalTransform(transform.localPosition * (rPos ? Random.Range(min, max) : 1f), transform.localEulerAngles * (rRot ? Random.Range(min, max) : 1f), transform.localScale * (rLS ? Random.Range(min, max) : 1f));
        }

        public static Transform FindChildRecursive(this Transform transform, string name)
        {
            return TransformUtils.FindChildRecursive(transform, name);
        }

        public static List<ModWeaponModel> GetModWeaponModels(this FirstPersonMover firstPersonMover)
        {
            if (firstPersonMover && firstPersonMover.IsAttachedAndAlive())
            {
                CharacterModel characterModel = firstPersonMover.GetCharacterModel();
                if (characterModel && characterModel.WeaponModels != null)
                {
                    List<ModWeaponModel> list = new List<ModWeaponModel>();
                    foreach (WeaponModel weapon in characterModel.WeaponModels)
                    {
                        if (weapon is ModWeaponModel)
                        {
                            list.Add((ModWeaponModel)weapon);
                        }
                    }
                    return list;
                }
            }
            return null;
        }

        public static void RefreshModWeaponModels(this FirstPersonMover firstPersonMover)
        {
            List<ModWeaponModel> list = firstPersonMover.GetModWeaponModels();
            if (list == null)
                return;

            foreach (ModWeaponModel weapon in list)
            {
                weapon.OnUpgradesRefresh(firstPersonMover);
            }
        }

        public static bool IsModded(this WeaponModel weaponModel)
        {
            return weaponModel && weaponModel is ModWeaponModel;
        }

        public static ModWeaponModel ModdedReference(this WeaponModel weaponModel)
        {
            return weaponModel as ModWeaponModel;
        }
    }
}
