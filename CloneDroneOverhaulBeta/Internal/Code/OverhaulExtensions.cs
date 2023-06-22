using CDOverhaul.Gameplay.Multiplayer;
using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul
{
    public static class OverhaulExtensions
    {
        private static readonly TextEditor s_TextEditor = new TextEditor();

        public static bool IsNullOrEmpty(this ICollection list) => list == null || list.Count == 0;
        public static bool IsNullOrEmpty(this Array array) => array == null || array.Length == 0;
        public static bool IsNullOrEmpty(this string @string) => string.IsNullOrEmpty(@string);

        public static bool IsAnOverhaulModUser(this FirstPersonMover firstPersonMover)
        {
            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(firstPersonMover);
            return info && info.GetHashtable() != null;
        }

        public static bool IsAnOverhaulModUser(this MultiplayerPlayerInfoState infoState)
        {
            if (!infoState || infoState.IsDetached() || string.IsNullOrEmpty(infoState.state.PlayFabID))
                return false;

            Character character = CharacterTracker.Instance.TryGetLivingCharacterWithPlayFabID(infoState.state.PlayFabID);
            return character && !character.IsDetached() && character is FirstPersonMover
&& IsAnOverhaulModUser(character as FirstPersonMover);
        }

        public static bool IsOverhaulDeveloper(this FirstPersonMover firstPersonMover)
        {
            return firstPersonMover
&& (GameModeManager.IsSinglePlayer()
                ? PlayFabDataController.GetLocalPlayFabID() == "883CC7F4CA3155A3"
                : firstPersonMover.GetPlayFabID() == "883CC7F4CA3155A3");
        }

        public static bool IsDebugBuildUser(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            if (GameModeManager.IsSinglePlayer())
                return OverhaulVersion.IsDebugBuild;

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(firstPersonMover);
            if (!info)
                return false;

            string flags = info.GetFlagsData();
            return !string.IsNullOrEmpty(flags) && flags.Contains("debug");
        }

        public static bool IsBlacklistedBuildUser(this FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover)
                return false;

            OverhaulModdedPlayerInfo info = OverhaulModdedPlayerInfo.GetPlayerInfo(firstPersonMover);
            if (!info)
                return false;

            Hashtable hashtable = info.GetHashtable();
            return hashtable != null && hashtable.ContainsKey("State.Version")
&& OverhaulVersion.BlacklistedVersions.Contains(hashtable["State.Version"]);
        }

        public static bool IsForcedToUseLockedStuff(this FirstPersonMover firstPersonMover)
        {
            return firstPersonMover
&& (!firstPersonMover.IsPlayer()
|| ((firstPersonMover.IsDebugBuildUser() || firstPersonMover.IsOverhaulDeveloper()) && !firstPersonMover.IsBlacklistedBuildUser()));
        }

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

        /// <summary>
        /// Todo: Finish this method
        /// </summary>
        /// <param name="levelManager"></param>
        /// <param name="index">0 - Story mode levels<br></br>1 - Endless mode levels</param>
        /// <returns></returns>
        public static List<LevelDescription> GetLevelDescriptions(this LevelManager levelManager, byte index)
        {
            switch (index)
            {
                case 0:
                    return levelManager.GetPrivateField<List<LevelDescription>>("_storyModeLevels");
            }
            return null;
        }

        #endregion

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

        public static UnityEngine.Object GetObject(this ModdedObject moddedObject, in int index)
        {
            UnityEngine.Object @object = moddedObject.objects[index];
            return @object;
        }

        #region Strings

        /// <summary>
        /// Get color using hex
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static Color ConvertToColor(this string theString)
        {
            _ = ColorUtility.TryParseHtmlString(theString, out Color col);
            return col;
        }

        public static void CopyToClipboard(this string @string)
        {
            s_TextEditor.text = @string;
            s_TextEditor.SelectAll();
            s_TextEditor.Copy();
            s_TextEditor.text = string.Empty;
        }

        #endregion

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

        /// <summary>
        /// Create sprite from texture
        /// </summary>
        /// <param name="texture2D"></param>
        /// <returns></returns>
        public static Sprite FastSpriteCreate(this Texture2D texture2D) => texture2D == null ? null : Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
    }
}
