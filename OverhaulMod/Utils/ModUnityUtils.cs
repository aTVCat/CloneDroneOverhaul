using UnityEngine;

namespace OverhaulMod.Utils
{
    internal static class ModUnityUtils
    {
        public static Transform CloneLevelEnemySpawner(string name)
        {
            GameObject toInstantiate = Resources.Load<GameObject>("Prefabs/LevelObjects/EnemySpawns/Bow1");
            toInstantiate.SetActive(false);
            GameObject gameObject = UnityEngine.Object.Instantiate(toInstantiate);
            gameObject.name = name;
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            toInstantiate.SetActive(true);
            return gameObject.transform;
        }

        public static Transform CloneAndSetLevelEnemySpawnerUp(string name, EnemyType enemyType)
        {
            GameObject toInstantiate = Resources.Load<GameObject>("Prefabs/LevelObjects/EnemySpawns/Bow1");
            toInstantiate.SetActive(false);
            GameObject gameObject = UnityEngine.Object.Instantiate(toInstantiate);
            gameObject.name = name;
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            toInstantiate.SetActive(true);

            LevelEnemySpawner levelEnemySpawner = gameObject.GetComponent<LevelEnemySpawner>();
            if (levelEnemySpawner)
                levelEnemySpawner.EnemyPrefab = EnemyFactory.Instance.GetEnemyPrefab(enemyType).transform;
            return gameObject.transform;
        }

        public static void SetEmissionEnabled(this ParticleSystem particleSystem, bool enabled)
        {
            if (!particleSystem)
                return;

            ParticleSystem.EmissionModule em = particleSystem.emission;
            em.enabled = enabled;
        }
    }
}
