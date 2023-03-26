using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulExtentions
    {
        public static bool IsNullOrEmpty(this ICollection list)
        {
            return list == null || list.Count == 0;
        }

        public static bool TryRemove<T>(this ICollection<T> list, T item)
        {
            if (list.Contains(item))
            {
                _ = list.Remove(item);
                return true;
            }
            return false;
        }

        public static bool TryAdd<T>(this ICollection<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static bool IsNullOrEmpty(this Array array)
        {
            return array == null || array.Length == 0;
        }

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

        public static void AddColor(this HumanFactsManager manager, string colorName, Color color)
        {
            List<HumanFavouriteColor> list = HumanFactsManager.Instance.FavouriteColors.ToList();
            list.Add(new HumanFavouriteColor() { ColorName = colorName, ColorValue = color });
            HumanFactsManager.Instance.FavouriteColors = list.ToArray();
        }

        public static bool IsHeavyRobot(this CharacterModel model, out bool shouldUseLowerPitchValues, out bool shouldUseMSSounds, out bool shouldNotPlaySound)
        {
            shouldNotPlaySound = false;
            shouldUseMSSounds = false;
            shouldUseLowerPitchValues = false;
            if(model == null)
            {
                return false;
            }

            FirstPersonMover mover = model.GetOwner();
            if(mover == null)
            {
                return false;
            }

            bool m = GameModeManager.IsMultiplayer();
            shouldNotPlaySound = !m || (m && !mover.IsOnGroundServer());

            EnemyType type = mover.CharacterType;
            shouldUseLowerPitchValues = type == EnemyType.EmperorCombat || type == EnemyType.EmperorNonCombat;
            shouldUseMSSounds = mover.IsMindSpaceCharacter;
            return type == EnemyType.Hammer1 || type == EnemyType.Hammer2 ||
                type == EnemyType.Hammer3 || type == EnemyType.JetpackHammer ||
                type == EnemyType.EmperorCombat ||
                type == EnemyType.EmperorNonCombat || type == EnemyType.FireRaptor ||
                type == EnemyType.Hammer5 || type == EnemyType.LaserRaptor ||
                type == EnemyType.Spear3 || type == EnemyType.Spear4;
        }
    }
}
