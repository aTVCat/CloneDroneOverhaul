using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Utils
{
    public static class ModPrefabUtils
    {
        internal static Dictionary<string, Component> Storage = new Dictionary<string, Component>();

        /// <summary>
        /// Clone and hide a game object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public static T StoreObject<T>(Component @object, bool doClone = true) where T : UnityEngine.Component
        {
            if (Storage.ContainsKey(@object.name))
                return null;

            GameObject clone = doClone ? Object.Instantiate(@object.gameObject) : @object.gameObject;
            clone.SetActive(false);
            Object.DontDestroyOnLoad(clone);

            T component = clone.GetComponent<T>();
            Storage.Add(@object.name, component);
            return component;
        }

        /// <summary>
        /// Get loaded object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static T GetObject<T>(string objectName) where T : UnityEngine.Component
        {
            if (Storage.ContainsKey(objectName))
                return (T)Storage[objectName];

            T[] array = Resources.FindObjectsOfTypeAll<T>();
            foreach (T @object in array)
            {
                if (@object.name == objectName)
                {
                    return StoreObject<T>(@object);
                }
            }
            return null;
        }

        /// <summary>
        /// Instantiate loaded object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static T InstantiateObject<T>(string objectName) where T : UnityEngine.Component
        {
            T result = Object.Instantiate(GetObject<T>(objectName));
            result.gameObject.SetActive(true);
            return result;
        }

        /// <summary>
        /// Create a game object with component and save it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T CreateGameObjectWithComponent<T>(string name) where T : UnityEngine.Component
        {
            GameObject gameObject = new GameObject("Custom_" + name);
            return StoreObject<T>(gameObject.AddComponent<T>(), false);
        }

        public static T GetOrCreateGameObjectWithComponent<T>(string name) where T : UnityEngine.Component
        {
            string key = "Custom_" + name;
            return Storage.ContainsKey(key) ? (T)Storage[key] : CreateGameObjectWithComponent<T>(name);
        }

        private static Transform s_axe1Spawner;
        public static Transform axe1Spawner
        {
            get
            {
                if (!s_axe1Spawner)
                {
                    s_axe1Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Axe1", (EnemyType)700);
                }
                return s_axe1Spawner;
            }
        }

        private static Transform s_axe2Spawner;
        public static Transform axe2Spawner
        {
            get
            {
                if (!s_axe2Spawner)
                {
                    s_axe2Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Axe2", (EnemyType)701);
                }
                return s_axe2Spawner;
            }
        }

        private static Transform s_axe3Spawner;
        public static Transform axe3Spawner
        {
            get
            {
                if (!s_axe3Spawner)
                {
                    s_axe3Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Axe3", (EnemyType)702);
                }
                return s_axe3Spawner;
            }
        }

        private static Transform s_axe4Spawner;
        public static Transform axe4Spawner
        {
            get
            {
                if (!s_axe4Spawner)
                {
                    s_axe4Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Axe4", (EnemyType)703);
                }
                return s_axe4Spawner;
            }
        }

        private static Transform s_scythe1Spawner;
        public static Transform scythe1Spawner
        {
            get
            {
                if (!s_scythe1Spawner)
                {
                    s_scythe1Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Scythe1", (EnemyType)705);
                }
                return s_scythe1Spawner;
            }
        }

        private static Transform s_scythe2Spawner;
        public static Transform scythe2Spawner
        {
            get
            {
                if (!s_scythe2Spawner)
                {
                    s_scythe2Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Scythe2", (EnemyType)706);
                }
                return s_scythe2Spawner;
            }
        }

        private static Transform s_scythe3Spawner;
        public static Transform scythe3Spawner
        {
            get
            {
                if (!s_scythe3Spawner)
                {
                    s_scythe3Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Scythe3", (EnemyType)707);
                }
                return s_scythe3Spawner;
            }
        }

        private static Transform s_scythe4Spawner;
        public static Transform scythe4Spawner
        {
            get
            {
                if (!s_scythe4Spawner)
                {
                    s_scythe4Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Scythe4", (EnemyType)708);
                }
                return s_scythe4Spawner;
            }
        }

        private static Transform s_scytheSprinter1Spawner;
        public static Transform scytheSprinter1Spawner
        {
            get
            {
                if (!s_scytheSprinter1Spawner)
                {
                    s_scytheSprinter1Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("SprinterScythe1", (EnemyType)709);
                }
                return s_scytheSprinter1Spawner;
            }
        }

        private static Transform s_scytheSprinter2Spawner;
        public static Transform scytheSprinter2Spawner
        {
            get
            {
                if (!s_scytheSprinter2Spawner)
                {
                    s_scytheSprinter2Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("SprinterScythe2", (EnemyType)710);
                }
                return s_scytheSprinter2Spawner;
            }
        }

        private static Transform s_halberd1Spawner;
        public static Transform halberd1Spawner
        {
            get
            {
                if (!s_halberd1Spawner)
                {
                    s_halberd1Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Halberd1", (EnemyType)711);
                }
                return s_halberd1Spawner;
            }
        }

        private static Transform s_halberd2Spawner;
        public static Transform halberd2Spawner
        {
            get
            {
                if (!s_halberd2Spawner)
                {
                    s_halberd2Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Halberd2", (EnemyType)711);
                }
                return s_halberd2Spawner;
            }
        }

        private static Transform s_halberd3Spawner;
        public static Transform halberd3Spawner
        {
            get
            {
                if (!s_halberd3Spawner)
                {
                    s_halberd3Spawner = ModUnityUtils.CloneAndSetLevelEnemySpawnerUp("Halberd3", (EnemyType)712);
                }
                return s_halberd3Spawner;
            }
        }
    }
}
