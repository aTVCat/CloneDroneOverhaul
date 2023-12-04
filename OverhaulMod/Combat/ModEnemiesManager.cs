using OverhaulMod.Combat.AIBehaviours;
using OverhaulMod.Combat.Weapons;
using System.Collections.Generic;

namespace OverhaulMod
{
    public class ModEnemiesManager : Singleton<ModEnemiesManager>, IModLoadListener
    {
        public void OnModLoaded()
        {
            AddsBots();
        }

        public void AddsBots()
        {
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer1, 500);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer2, 501);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer3, 502);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer2, EnemyType.Hammer3, 503);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer2, EnemyType.Hammer5, 504);

            // Scythe 1
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 505);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)705), 0.35f, 6f, 0.07f, new List<AIBehaviour>()
            {
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomSprintAIBehaviour(false),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)705, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 }
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)705, new UnityEngine.Color(1f, 0.1f, 0f, 0.95f));

            // Scythe 2
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 506);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)706), 0.4f, 6f, 0.075f, new List<AIBehaviour>()
            {
                GetBackDashAIBehaviour(false),
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomSprintAIBehaviour(false),
                GetKickWhenRequiredAIBehaviour(),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)706, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 },
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.KickUnlock, Level = 1},
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)706, new UnityEngine.Color(0f, 0.3f, 1f, 0.9f));

            // Scythe 3
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 507);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)707), 0.45f, 5f, 0.075f, new List<AIBehaviour>()
            {
                GetBackDashAIBehaviour(false),
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomDashAIBehaviour(),
                GetRandomSprintAIBehaviour(true),
                GetKickWhenRequiredAIBehaviour(),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)707, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 },
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.KickUnlock, Level = 1}
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)707, new UnityEngine.Color(1f, 0.25f, 0.05f, 0.8f));

            // Scythe 4
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 508);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)708), 0.45f, 5f, 0.1f, new List<AIBehaviour>()
            {
                GetBackDashAIBehaviour(true),
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomDashAIBehaviour(),
                GetRandomSprintAIBehaviour(true),
                GetKickWhenRequiredAIBehaviour(),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)708, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 },
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.KickUnlock, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = ScytheWeaponModel.FireUnlockUpgradeType, Level = 1}
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)708, new UnityEngine.Color(1f, 0.35f, 0.05f, 0.7f));

            // Sprinter Scythe 
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Sprinter1, 509);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)709), 0.5f, 5f, 0.1f, new List<AIBehaviour>()
            {
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomDashAIBehaviour(),
                GetRandomSprintAIBehaviour(true),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)709, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 },
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)709, new UnityEngine.Color(1f, 0.6f, 0f, 0.9f));

            // Sprinter Scythe 2
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Sprinter1, 510);
            ConfigureAIController(ModBotAPI.EnemyAPI.SetEnemyAIController<AIComposableBehaviourController>((EnemyType)710), 0.5f, 5f, 0.1f, new List<AIBehaviour>()
            {
                GetAttackForwardAIBehaviour(),
                GetAttackLeftAIBehaviour(),
                GetAttackRightAIBehaviour(),
                GetRandomDashAIBehaviour(),
                GetLongRandomSprintAIBehaviour(),
            });
            ModBotAPI.EnemyAPI.SetEnemyUpgrades((EnemyType)710, new UpgradeTypeAndLevel[]
            {
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.Dash, Level = 1},
                new UpgradeTypeAndLevel() { UpgradeType = UpgradeType.EnergyCapacity, Level = 3 },
                new UpgradeTypeAndLevel() { UpgradeType = ScytheWeaponModel.FireUnlockUpgradeType, Level = 1},
            });
            ModBotAPI.EnemyAPI.ForceOverrideEnemyColor((EnemyType)710, new UnityEngine.Color(1f, 0.75f, 0f, 0.85f));

            // Halberd 1
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear2, 511);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear3, 512);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear4, 513);
        }

        public void ConfigureAIController(AIController aiController, float constantTurnSpeed = 0.3f, float distanceToMoveTowardsOpponent = 5f, float nearTurnSpeed = 0.05f, List<AIBehaviour> aiBehaviours = null)
        {
            if (aiController is AIComposableBehaviourController composable)
            {
                composable.ConstantTurnSpeed = constantTurnSpeed;
                composable.DistanceToMoveTowardsOpponent = distanceToMoveTowardsOpponent;
                composable.NearTurnSpeed = nearTurnSpeed;
                composable.DefaultMoveAndRotate = true;
                composable.AIBehaviourPrefabs = aiBehaviours;
            }
        }

        public AttackAIBehaviour GetAttackForwardAIBehaviour()
        {
            return PrefabStorage.GetObject<AttackAIBehaviour>("AttackForward_Zombie");
        }

        public AttackAIBehaviour GetAttackLeftAIBehaviour()
        {
            return PrefabStorage.GetObject<AttackAIBehaviour>("AttackLeft_Zombie");
        }

        public AttackAIBehaviour GetAttackRightAIBehaviour()
        {
            return PrefabStorage.GetObject<AttackAIBehaviour>("AttackRight_Zombie");
        }

        public RandomDashAIBehaviour GetRandomDashAIBehaviour()
        {
            return PrefabStorage.GetOrCreateGameObjectWithComponent<RandomDashAIBehaviour>("RandomDash");
        }

        public RandomSprintAIBehaviour GetRandomSprintAIBehaviour(bool insane)
        {
            return insane ? PrefabStorage.GetOrCreateGameObjectWithComponent<InsaneRandomSprintAIBehaviour>("InsaneRandomSprint") : PrefabStorage.GetOrCreateGameObjectWithComponent<RandomSprintAIBehaviour>("RandomSprint");
        }

        public InsaneLongRandomSprintAIBehaviour GetLongRandomSprintAIBehaviour()
        {
            return PrefabStorage.GetOrCreateGameObjectWithComponent<InsaneLongRandomSprintAIBehaviour>("InsaneLongRandomSprint");
        }

        public BackDashAIBehaviour GetBackDashAIBehaviour(bool insane)
        {
            return insane ? PrefabStorage.GetOrCreateGameObjectWithComponent<InsaneBackDashAIBehaviour>("InsaneBackDash") : PrefabStorage.GetOrCreateGameObjectWithComponent<BackDashAIBehaviour>("BackDash");
        }

        public KickWhenRequiredAIBehaviour GetKickWhenRequiredAIBehaviour()
        {
            return PrefabStorage.GetOrCreateGameObjectWithComponent<KickWhenRequiredAIBehaviour>("KickWhenRequired");
        }
    }
}
