using CDOverhaul.CustomMultiplayer;
using CDOverhaul.Gameplay;
using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.Gameplay.QualityOfLife;
using ModLibrary;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul
{
    public static class OverhaulExtensions
    {
        public static bool IsNullOrEmpty(this ICollection list) => list == null || list.Count == 0;
        public static bool IsNullOrEmpty(this Array array) => array == null || array.Length == 0;
        public static bool IsNullOrEmpty(this string @string) => string.IsNullOrEmpty(@string);

        #region Strings

        /// <summary>
        /// Get color using hex
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static Color ToColor(this string theString)
        {
            _ = ColorUtility.TryParseHtmlString(theString, out Color col);
            return col;
        }

        public static void CopyToClipboard(this string @string)
        {
            TextEditor s_TextEditor = new TextEditor
            {
                text = @string
            };
            s_TextEditor.SelectAll();
            s_TextEditor.Copy();
        }

        #endregion

        #region Texture

        /// <summary>
        /// Create sprite from texture
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Sprite ToSprite(this Texture2D texture2D) => texture2D == null ? null : Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

        #endregion

        #region Transform

        public static Transform FindChildRecursive(this Transform transform, string childName) => TransformUtils.FindChildRecursive(transform, childName);
        public static RectTransform FindRectChildRecursive(this Transform transform, string childName) => TransformUtils.FindChildRecursive(transform, childName) as RectTransform;

        #endregion

        #region ModdedObject

        /// <summary>
        /// Get a component of object with given index
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="moddedObject"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Type GetObject<Type>(this ModdedObject moddedObject, in int index) where Type : UnityEngine.Object
        {
            UnityEngine.GameObject @gameObject = moddedObject.objects[index] as UnityEngine.GameObject;
            return @gameObject.GetComponent<Type>();
        }

        public static UnityEngine.Object GetObject(this ModdedObject moddedObject, in int index, Type type)
        {
            UnityEngine.GameObject @gameObject = moddedObject.objects[index] as UnityEngine.GameObject;
            return @gameObject.GetComponent(type);
        }

        public static UnityEngine.Object GetObject(this ModdedObject moddedObject, in int index)
        {
            return moddedObject.objects[index];
        }

        #endregion

        #region Button

        public static void AddOnClickListener(this Button button, UnityAction unityAction)
        {
            button.onClick.AddListener(unityAction);
        }

        public static void RemoveAllOnClickListeners(this Button button)
        {
            button.onClick = new Button.ButtonClickedEvent();
        }

        #endregion

        #region LevelManager

        /// <summary>
        /// Get all levels related to challenges
        /// </summary>
        /// <param name="levelManager"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Dictionary<string, List<LevelDescription>> GetAllChallengeLevelDescriptions(this LevelManager levelManager)
        {
            return levelManager.GetPrivateField<Dictionary<string, List<LevelDescription>>>("_challengeLevels");
        }

        /// <summary>
        /// Get all challenge levels
        /// </summary>
        /// <param name="levelManager"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<LevelDescription> GetChallengeLevelDescriptions(this LevelManager levelManager, string challengeID)
        {
            Dictionary<string, List<LevelDescription>> all = levelManager.GetAllChallengeLevelDescriptions();
            challengeID = challengeID.Replace("Coop", string.Empty);

            _ = all.TryGetValue(challengeID, out List<LevelDescription> result);
            return result;
        }

        #endregion

        #region UpgradeManager

        public static bool RevertUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgradeDescription)
        {
            if (!upgradeDescription || !upgradeManager)
                return false;

            UpgradeType upgradeToRevert = upgradeDescription.UpgradeType;
            int upgradeLevelToRevert = upgradeDescription.Level;

            Dictionary<UpgradeType, int> dictionary = GameDataManager.Instance.GetPlayerUpgradeDictionary();
            if (!upgradeDescription.CanBeReverted())
                return false;

            if (upgradeLevelToRevert > 1)
                GameDataManager.Instance.SetUpgradeLevel(upgradeToRevert, upgradeLevelToRevert - 1);
            else
                _ = dictionary.Remove(upgradeToRevert);

            upgradeManager.SetAvailableSkillPoints(upgradeManager.GetAvailableSkillPoints() + upgradeDescription.GetSkillPointCost());
            GlobalEventManager.Instance.Dispatch("UpgradeCompleted", upgradeDescription);
            return true;
        }

        public static List<UpgradeDescription> GetUpgradesRequiringUpgrade(this UpgradeManager upgradeManager, UpgradeDescription upgradeDescription)
        {
            List<UpgradeDescription> result = new List<UpgradeDescription>();
            if (!upgradeDescription || !upgradeManager)
                return result;

            foreach (UpgradeDescription description in upgradeManager.UpgradeDescriptions)
            {
                if (!description || description.IsTheSameUpgrade(upgradeDescription))
                    continue;

                if (description.Requirement == upgradeDescription || description.Requirement2 == upgradeDescription)
                    result.Add(description);
            }
            return result;
        }

        #endregion

        #region MultiplayerPlayerInfoState

        public static bool IsAnOverhaulModUser(this MultiplayerPlayerInfoState infoState)
        {
            if (!infoState || infoState.IsDetached() || string.IsNullOrEmpty(infoState.state.PlayFabID))
                return false;

            Character character = CharacterTracker.Instance.TryGetLivingCharacterWithPlayFabID(infoState.state.PlayFabID);
            return character && !character.IsDetached() && character is FirstPersonMover
&& IsAnOverhaulModUser(character as FirstPersonMover);
        }

        public static bool IsLocalPlayer(this MultiplayerPlayerInfoState infoState)
        {
            return infoState && !infoState.IsDetached() && !string.IsNullOrEmpty(infoState.state.PlayFabID)
&& OverhaulPlayerIdentifier.GetLocalPlayFabID() == infoState.state.PlayFabID;
        }

        #endregion

        #region FirstPersonMover

        public static bool IsAnOverhaulModUser(this FirstPersonMover firstPersonMover)
        {
            OverhaulPlayerInfo info = OverhaulPlayerInfo.GetOverhaulPlayerInfo(firstPersonMover);
            return info && info.Hashtable != null;
        }

        public static bool IsOverhaulDeveloper(this FirstPersonMover firstPersonMover)
        {
            return firstPersonMover
&& (GameModeManager.IsSinglePlayer()
                ? OverhaulPlayerIdentifier.GetLocalPlayFabID() == "883CC7F4CA3155A3"
                : firstPersonMover.GetPlayFabID() == "883CC7F4CA3155A3");
        }

        public static bool IsDebugBuildUser(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            if (GameModeManager.IsSinglePlayer())
                return OverhaulVersion.IsDebugBuild;

            OverhaulPlayerInfo info = OverhaulPlayerInfo.GetOverhaulPlayerInfo(firstPersonMover);
            if (!info)
                return false;

            string flags = info.GetUserFlags();
            return !string.IsNullOrEmpty(flags) && flags.Contains("debug");
        }

        public static bool IsBlacklistedBuildUser(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            OverhaulPlayerInfo info = OverhaulPlayerInfo.GetOverhaulPlayerInfo(firstPersonMover);
            if (!info)
                return false;

            Hashtable hashtable = info.Hashtable;
            return hashtable != null && hashtable.ContainsKey("State.Version") && OverhaulVersion.IsBlacklistedVersion(hashtable["State.Version"].ToString());
        }

        public static bool IsForcedToUseLockedStuff(this FirstPersonMover firstPersonMover)
        {
            return firstPersonMover && (!firstPersonMover.IsPlayer() || firstPersonMover.IsDebugBuildUser() || firstPersonMover.IsOverhaulDeveloper());
        }

        public static bool IsMultiplayerSandboxPlayer(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || !OverhaulGamemodeManager.IsMultiplayerSandbox())
                return false;

            //PlayerSync sync = firstPersonMover.GetComponent<PlayerSync>();
            //return sync;
            return false;
        }

        public static bool IsMultiplayerSandboxHost(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || !OverhaulGamemodeManager.IsMultiplayerSandbox())
                return false;

            //PlayerSync sync = firstPersonMover.GetComponent<PlayerSync>();
            //return sync && sync.OwnerSteamID.IsMultiplayerSandboxHost();
            return false;
        }

        public static bool IsFireWeapon(this FirstPersonMover firstPersonMover, WeaponType type)
        {
            if (!firstPersonMover || !firstPersonMover.IsAlive())
                return false;

            if (type == WeaponType.Sword)
            {
                if (firstPersonMover.HasUpgrade(UpgradeType.FireSword))
                {
                    return true;
                }
            }
            else if (type == WeaponType.Hammer)
            {
                if (firstPersonMover.HasUpgrade(UpgradeType.FireHammer))
                {
                    return true;
                }
            }
            else if (type == WeaponType.Spear)
            {
                if (firstPersonMover.HasUpgrade(UpgradeType.FireSpear))
                {
                    return true;
                }
            }
            else if (type == WeaponType.Bow)
            {
                if (firstPersonMover.HasUpgrade(UpgradeType.FireArrow))
                {
                    return true;
                }
            }
            return false;
        }

        public static void GoToPreviousWeapon(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return;

            int equippedWeaponsCount = firstPersonMover._equippedWeapons.Count;

            List<WeaponType> list = new List<WeaponType>(firstPersonMover._equippedWeapons);
            if (list.Contains(WeaponType.Shield))
            {
                _ = list.Remove(WeaponType.Shield);
                equippedWeaponsCount--;
            }
            if (list.IsNullOrEmpty())
                return;

            int index = list.IndexOf(firstPersonMover._currentWeapon) - 1;
            if (list.Count == 1 && firstPersonMover._droppedWeapons.Count != 0)
                FirstPersonMover.dispatchAttemptedChangeToDroppedWeapon(firstPersonMover._droppedWeapons[0]);

            if (index < 0)
                index = equippedWeaponsCount - 1;

            WeaponType weaponType = list[index];
            if (GameModeManager.IsMultiplayer())
            {
                switch (weaponType)
                {
                    case WeaponType.Sword:
                        firstPersonMover.SetWeapon1ButtonDown(true);
                        break;
                    case WeaponType.Bow:
                        firstPersonMover.SetWeapon2ButtonDown(true);
                        break;
                    case WeaponType.Hammer:
                        firstPersonMover.SetWeapon3ButtonDown(true);
                        break;
                    case WeaponType.Spear:
                        firstPersonMover.SetWeapon4ButtonDown(true);
                        break;
                }
            }
            else
            {
                firstPersonMover.SetEquippedWeaponType(weaponType, true);
            }
        }

        public static void RefreshPersonalizationItems(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return;

            WeaponSkinsWearer legacySkinsWearer = firstPersonMover.GetComponent<WeaponSkinsWearer>();
            if (legacySkinsWearer)
                legacySkinsWearer.SpawnSkins();

            foreach (PersonalizationItemsWearer itemsWearer in firstPersonMover.GetComponents<PersonalizationItemsWearer>())
                itemsWearer.RefreshItems();
        }

        #endregion

        #region CSteamID

        public static bool IsMultiplayerSandboxHost(this CSteamID steamId)
        {
            return OverhaulGamemodeManager.IsMultiplayerSandbox() && steamId != CSteamID.Nil && OverhaulMultiplayerManager.Lobby.OwnerUserID == steamId;
        }

        public static bool IsNil(this CSteamID steamID)
        {
            return steamID == CSteamID.Nil;
        }

        #endregion

        #region CharacterModel

        public static bool IsHeavyRobot(this CharacterModel model, out bool shouldUseLowerPitchValues, out bool shouldUseMSSounds, out bool shouldNotPlaySound)
        {
            shouldNotPlaySound = false;
            shouldUseMSSounds = false;
            shouldUseLowerPitchValues = false;

            if (model == null)
                return false;

            FirstPersonMover mover = model.GetOwner();
            if (mover == null)
                return false;
            _ = GameModeManager.IsMultiplayer();
            shouldNotPlaySound = mover.GetPrivateField<Vector3>("_velocity").y < -1.5f;

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

        #endregion

        #region UpgradeUIIcon

        public static UpgradeUIIcon GetUpgradeUIIcon(this UpgradeUI upgradeUI, UpgradeType upgradeType, int upgradeLevel)
        {
            List<UpgradeUIIcon> icons = upgradeUI.GetPrivateField<List<UpgradeUIIcon>>("_icons");
            if (icons.IsNullOrEmpty())
                return null;

            UpgradeUIIcon result = null;
            int index = 0;
            do
            {
                UpgradeUIIcon icon = icons[index];
                if (!icon)
                {
                    index++;
                    continue;
                }

                UpgradeDescription upgradeDescription = icon.GetDescription();
                if (!upgradeDescription)
                {
                    index++;
                    continue;
                }

                if (upgradeDescription.UpgradeType == upgradeType && upgradeDescription.Level == upgradeLevel)
                {
                    result = icon;
                    break;
                }
                index++;
            } while (index < icons.Count);

            return result;
        }

        #endregion


        #region ChallengeDefinition

        public static int GetNumberOfBeatenLevels(this ChallengeDefinition challengeDefinition)
        {
            bool hadData = DataRepository.Instance.TryLoad<GameData>("ChallengeData" + challengeDefinition.ChallengeID, out GameData gameData, false);
            return !hadData || gameData == null ? 0 : gameData.LevelIDsBeatenThisPlaythrough.Count;
        }

        public static int GetNumberOfTotalLevels(this ChallengeDefinition challengeDefinition)
        {
            string id = challengeDefinition.ChallengeID.Replace("Coop", string.Empty);

            List<LevelDescription> list = LevelManager.Instance.GetChallengeLevelDescriptions(id);
            return !list.IsNullOrEmpty() ? list.Count : int.MaxValue;
        }

        #endregion

        #region UpgradeDescription

        public static bool IsUpgraded(this UpgradeDescription upgradeDescription)
        {
            if (!CharacterTracker.Instance)
                return false;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            if (!firstPersonMover)
                return false;

            List<UpgradeTypeAndLevel> upgrades = firstPersonMover.GetUpgradeTypesAndLevels();
            if (upgrades.IsNullOrEmpty())
                return false;

            foreach (UpgradeTypeAndLevel upgradeTypeAndLevel in upgrades)
            {
                if (upgradeTypeAndLevel.IsTheSameUpgrade(upgradeDescription))
                    return true;
            }
            return false;
        }

        public static bool CanBeReverted(this UpgradeDescription upgradeDescription)
        {
            if (!CharacterTracker.Instance)
                return false;

            FirstPersonMover firstPersonMover = CharacterTracker.Instance.GetPlayerRobot();
            if (!firstPersonMover)
                return false;

            Dictionary<UpgradeType, int> dictionary = GameDataManager.Instance.GetPlayerUpgradeDictionary();
            if (UpgradeModesSystem.IsUnrevertableUpgrade(upgradeDescription.UpgradeType, upgradeDescription.Level) || dictionary.IsNullOrEmpty() || upgradeDescription.IsRepeatable || !dictionary.ContainsKey(upgradeDescription.UpgradeType))
                return false;

            int playerLevel = dictionary[upgradeDescription.UpgradeType];
            if (playerLevel != upgradeDescription.Level)
                return false;

            List<UpgradeDescription> otherUpgrades = UpgradeManager.Instance.GetUpgradesRequiringUpgrade(upgradeDescription);
            if (!otherUpgrades.IsNullOrEmpty())
            {
                foreach (UpgradeDescription upgrade in otherUpgrades)
                {
                    if (upgrade.IsUpgraded())
                        return false;
                }
            }
            return true;
        }

        public static bool IsTheSameUpgrade(this UpgradeDescription upgradeDescription, UpgradeDescription other) => (upgradeDescription.UpgradeType, upgradeDescription.Level) == (other.UpgradeType, other.Level);

        #endregion

        #region UpgradeTypeAndLevel

        public static bool IsTheSameUpgrade(this UpgradeTypeAndLevel upgradeTypeAndLevel, UpgradeDescription other) => (upgradeTypeAndLevel.UpgradeType, upgradeTypeAndLevel.Level) == (other.UpgradeType, other.Level);

        #endregion

        #region Object

        public static byte[] SerializeObject(this object @object)
        {
            if (@object == null)
                return Array.Empty<byte>();

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, @object);
                return memoryStream.ToArray();
            }
        }

        public static T DeserializeObject<T>(this byte[] array)
        {
            if (array.IsNullOrEmpty())
                return default;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                _ = memoryStream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(memoryStream);
            }
        }

        #endregion

        #region Type

        public static List<FieldInfo> GetFieldsWithAttribute<T>(this System.Type type, BindingFlags bindingFlags) where T : Attribute
        {
            List<FieldInfo> result = new List<FieldInfo>();

            FieldInfo[] fields = type.GetFields(bindingFlags);
            foreach (FieldInfo info in fields)
            {
                if (info.GetCustomAttribute<T>() != null)
                    result.Add(info);
            }
            return result;
        }

        public static List<T> GetFieldReferencingAttributes<T>(this System.Type type, BindingFlags bindingFlags) where T : FieldReferencingAttribute
        {
            List<T> result = new List<T>();

            FieldInfo[] fields = type.GetFields(bindingFlags);
            foreach (FieldInfo info in fields)
            {
                T attribute = info.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    attribute.FieldReference = info;
                    result.Add(attribute);
                }
            }
            return result;
        }

        public static List<MethodInfo> GetMethodsWithAttribute<T>(this System.Type type, BindingFlags bindingFlags) where T : Attribute
        {
            List<MethodInfo> result = new List<MethodInfo>();

            MethodInfo[] methods = type.GetMethods(bindingFlags);
            foreach (MethodInfo info in methods)
            {
                if (info.GetCustomAttribute<T>() != null)
                    result.Add(info);
            }
            return result;
        }

        public static List<T> GetMethodReferencingAttributes<T>(this System.Type type, BindingFlags bindingFlags) where T : MethodReferencingAttribute
        {
            List<T> result = new List<T>();

            MethodInfo[] methods = type.GetMethods(bindingFlags);
            foreach (MethodInfo info in methods)
            {
                T attribute = info.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    attribute.MethodReference = info;
                    result.Add(attribute);
                }
            }
            return result;
        }

        #endregion
    }
}
