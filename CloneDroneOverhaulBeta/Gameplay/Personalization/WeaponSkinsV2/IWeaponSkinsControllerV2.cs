using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// Gonna test new way of controller impementation
    /// </summary>
    public interface IWeaponSkinsControllerV2
    {
        /// <summary>
        /// Create instance of weapon skin item. To set skin models, call <see cref="IWeaponSkinItemDefinition.SetModel(GameObject, bool, bool)"/>.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="skinName"></param>
        /// <returns></returns>
        IWeaponSkinItemDefinition NewSkinItem(WeaponType weapon, string skinName, WeaponSkinItemFilter filter);

        /// <summary>
        /// Get specific weapon skin item using <paramref name="weaponType"/> and <paramref name="filter"/>.
        /// If result is <b>NULL</b>, checking <paramref name="error"/> would be nice idea
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="skinName"></param>
        /// <param name="filter"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        IWeaponSkinItemDefinition GetSkinItem(WeaponType weaponType, string skinName, WeaponSkinItemFilter filter, out WeaponSkinItemNullReason error);
        /// <summary>
        /// Get multiple skins items. The way how you get them is the same as in <see cref="NewSkinItem(WeaponType, string)"/>
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="skinName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWeaponSkinItemDefinition[] GetSkinItems(WeaponType weaponType, WeaponSkinItemFilter filter);
        /// <summary>
        /// Get all weapon skins that match <paramref name="filter"/> parameter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        IWeaponSkinItemDefinition[] GetSkinItems(WeaponSkinItemFilter filter);
        /// <summary>
        /// Get all weapon skins that <paramref name="firstPersonMover"/> is allowed to use
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IWeaponSkinItemDefinition[] GetSkinItems(FirstPersonMover firstPersonMover);
    }
}
