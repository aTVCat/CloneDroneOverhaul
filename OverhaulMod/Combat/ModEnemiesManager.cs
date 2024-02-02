using OverhaulMod.Combat.Enemies;
using OverhaulMod.Combat.Weapons;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Combat
{
    public class ModEnemiesManager : Singleton<ModEnemiesManager>, IModLoadListener
    {
        public void OnModLoaded()
        {
            AddsBots();
        }

        public void AddsBots()
        {
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer1, 500, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer2, 501, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Hammer3, 502, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer2, EnemyType.Hammer3, 503, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer2, EnemyType.Hammer5, 504, out _);

            // Scythe 1
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 505, out bool hasAdded);
            if (!hasAdded)
            {
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
            }

            // Scythe 2
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 506, out hasAdded);
            if (!hasAdded)
            {
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
            }

            // Scythe 3
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 507, out hasAdded);
            if (!hasAdded)
            {
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
            }

            // Scythe 4
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Swordsman3, 508, out hasAdded);
            if (!hasAdded)
            {
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
            }

            // Sprinter Scythe 
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Sprinter1, 509, out hasAdded);
            if (!hasAdded)
            {
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
            }

            // Sprinter Scythe 2
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman3, EnemyType.Sprinter1, 510, out hasAdded);
            if (!hasAdded)
            {
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
            }

            // Halberd 1
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear2, 511, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear3, 512, out _);
            _ = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Hammer1, EnemyType.Spear4, 513, out _);

            EnemyConfiguration guardBotConfig = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.ImperialRepairBot, EnemyType.ImperialRepairBot, 514, out hasAdded);
            if (!hasAdded)
            {
                Transform transform = guardBotConfig.EnemyPrefab;
                transform.localScale = Vector3.one * 4f;
            }

            EnemyConfiguration chibiMk2SwordConfig = ModBotAPI.EnemyAPI.DuplicateFirstPersonMoverInternal(EnemyType.Swordsman2, EnemyType.Swordsman2, 515, out hasAdded);
            if (!hasAdded)
            {
                Transform transform = chibiMk2SwordConfig.EnemyPrefab;
                transform.localScale = Vector3.one * 0.75f;
            }
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
            return ModPrefabUtils.GetObject<AttackAIBehaviour>("AttackForward_Zombie");
        }

        public AttackAIBehaviour GetAttackLeftAIBehaviour()
        {
            return ModPrefabUtils.GetObject<AttackAIBehaviour>("AttackLeft_Zombie");
        }

        public AttackAIBehaviour GetAttackRightAIBehaviour()
        {
            return ModPrefabUtils.GetObject<AttackAIBehaviour>("AttackRight_Zombie");
        }

        public DashAIBehaviour GetRandomDashAIBehaviour()
        {
            return ModPrefabUtils.GetOrCreateGameObjectWithComponent<DashAIBehaviour>("RandomDash");
        }

        public SprintAIBehaviour GetRandomSprintAIBehaviour(bool insane)
        {
            return insane ? ModPrefabUtils.GetOrCreateGameObjectWithComponent<InsaneSprintAIBehaviour>("InsaneRandomSprint") : ModPrefabUtils.GetOrCreateGameObjectWithComponent<SprintAIBehaviour>("RandomSprint");
        }

        public InsaneLongSprintAIBehaviour GetLongRandomSprintAIBehaviour()
        {
            return ModPrefabUtils.GetOrCreateGameObjectWithComponent<InsaneLongSprintAIBehaviour>("InsaneLongRandomSprint");
        }

        public BackwardDashAIBehaviour GetBackDashAIBehaviour(bool insane)
        {
            return insane ? ModPrefabUtils.GetOrCreateGameObjectWithComponent<InsaneBackwardDashAIBehaviour>("InsaneBackDash") : ModPrefabUtils.GetOrCreateGameObjectWithComponent<BackwardDashAIBehaviour>("BackDash");
        }

        public UseKickAIBehaviour GetKickWhenRequiredAIBehaviour()
        {
            return ModPrefabUtils.GetOrCreateGameObjectWithComponent<UseKickAIBehaviour>("KickWhenRequired");
        }
    }
}
