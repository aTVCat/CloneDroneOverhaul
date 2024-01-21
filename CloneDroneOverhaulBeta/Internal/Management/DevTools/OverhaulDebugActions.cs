using ModLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CDOverhaul.DevTools
{
    public static class OverhaulDebugActions
    {
        private static readonly List<Tuple<string, MethodInfo>> s_DebugActions = new List<Tuple<string, MethodInfo>>();
        private static bool s_Initialized;

        public static List<Tuple<string, MethodInfo>> GetAllDebugActions() => s_DebugActions;

        public static void Initialize()
        {
            if (!OverhaulVersion.IsDebugBuild || s_Initialized)
                return;

            s_Initialized = true;
            Type[] allTypes = OverhaulMod.GetAllTypes();
            int typeIndex = 0;
            do
            {
                Type currentType = allTypes[typeIndex];
                MethodInfo[] allMethods = currentType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (allMethods.IsNullOrEmpty())
                {
                    typeIndex++;
                    continue;
                }

                int methodIndex = 0;
                do
                {
                    MethodInfo currentMethod = allMethods[methodIndex];

                    DebugActionAttribute mainAttribute = currentMethod.GetCustomAttribute<DebugActionAttribute>();
                    if (mainAttribute == null)
                    {
                        methodIndex++;
                        continue;
                    }

                    string displayName = mainAttribute.DisplayName;
                    MethodInfo methodInfo = currentMethod;
                    Tuple<string, MethodInfo> tuple = new Tuple<string, MethodInfo>(displayName, methodInfo);
                    s_DebugActions.Add(tuple);

                    methodIndex++;
                } while (methodIndex < allMethods.Length);

                typeIndex++;
            } while (typeIndex < allTypes.Length);
        }

        [DebugAction(Actions.KILL_PLAYER)]
        public static void KillPlayer()
        {
            if (!CharacterTracker.Instance || GameModeManager.IsMultiplayer())
                return;

            Character player = CharacterTracker.Instance.GetPlayer();
            if (!player)
                return;

            player.Kill(null, DamageSourceType.ScriptedKill);
        }

        [DebugAction(Actions.KILL_ENEMIES)]
        public static void KillEnemies()
        {
            if (!CharacterTracker.Instance || GameModeManager.IsMultiplayer() || (LevelManager.Instance && LevelManager.Instance.IsSpawningCurrentLevel()))
                return;

            DebugManager.Instance.KillAllEnemies();
        }

        [DebugAction(Actions.DISABLE_ENEMIES)]
        public static void DisableEnemies()
        {
            if (!CharacterTracker.Instance || GameModeManager.IsMultiplayer())
                return;

            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            foreach (Character character in characters)
            {
                if (!character || character.IsPlayerTeam)
                    continue;

                character.DisableAI();
            }
        }

        [DebugAction(Actions.ENABLE_ENEMIES)]
        public static void EnableEnemies()
        {
            if (!CharacterTracker.Instance || GameModeManager.IsMultiplayer())
                return;

            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            foreach (Character character in characters)
            {
                if (!character || character.IsPlayerTeam)
                    continue;

                character.ActivateAI();
            }
        }

        [DebugAction(Actions.DESTROY_LEVEL)]
        public static void DestroyLevel()
        {
            if (!LevelManager.Instance || GameModeManager.IsMultiplayer())
                return;

            LevelManager.Instance.CleanUpLevelRootsWaitingForDestruction();
        }

        [DebugAction(Actions.IGNORE_CRASH)]
        public static void IgnoreCrash()
        {
            if (!ErrorManager.Instance)
                return;

            ErrorManager.Instance.SetPrivateField("_hasCrashed", false);
            GameUIRoot.Instance.ErrorWindow.Hide();
            TimeManager.Instance.OnGameUnPaused();
        }

        [DebugAction(Actions.GET_100_POINTS)]
        public static void Get100Points()
        {
            if (!GameDataManager.Instance)
                return;

            GameDataManager.Instance.SetAvailableSkillPoints(100);
        }

        [DebugAction(Actions.DEUPGRADE_SELF)]
        public static void DeupgradePlayer()
        {
            if (!CharacterTracker.Instance || GameModeManager.IsMultiplayer())
                return;

            FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
            if (!player)
                return;

            UpgradeCollection collection = player.GetComponent<UpgradeCollection>();
            if (!collection)
                return;

            Dictionary<UpgradeType, int> dictionary = new Dictionary<UpgradeType, int>
            {
                { UpgradeType.SwordUnlock, 1 },
                { UpgradeType.BlockSwords, 1 },
                { UpgradeType.Dash, 1 }
            };
            collection.SetPrivateField("_upgradeLevels", dictionary);
            player.RefreshUpgrades();
            player.RefreshSizeUpgrades();
        }

        public static class Actions
        {
            public const string KILL_PLAYER = "Kill Player";

            public const string KILL_ENEMIES = "Kill Enemies";

            public const string DISABLE_ENEMIES = "Disable Enemies";

            public const string ENABLE_ENEMIES = "Enable Enemies";

            public const string DESTROY_LEVEL = "Destroy Level";

            public const string IGNORE_CRASH = "Ignore Recent Crash";

            public const string GET_100_POINTS = "Get 100 Upgrade Points";

            public const string DEUPGRADE_SELF = "\"De-upgrade\" player";
        }
    }
}
