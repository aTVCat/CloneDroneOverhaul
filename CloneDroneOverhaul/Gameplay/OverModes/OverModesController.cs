using UnityEngine;
using System;
using UnityEngine.SceneManagement;

namespace CloneDroneOverhaul.Gameplay.OverModes
{
    public static class OverModesController
    {
        private static GameObject _overModesGameObject;

        private static int _prevSceneID;

        /// <summary>
        /// Usually all monobehaviours remove when switching scenes
        /// </summary>
        public static void InitializeForCurrentScene()
        {
            GameObject gameObject = new GameObject("Overhaul_OverModes");
            _overModesGameObject = gameObject;

            InstantiateManager<EndlessModeOverhaul>("Endless_Overhaul");
        }

        /// <summary>
        /// Create an instance of over mode manager
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T InstantiateManager<T>(string name) where T : OverModeBase
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(_overModesGameObject.transform);
            T result = obj.AddComponent<T>();
            result.Initialize();
            return result;
        }
    }
}
