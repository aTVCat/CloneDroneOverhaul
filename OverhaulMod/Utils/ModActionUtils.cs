using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Utils
{
    internal static class ModActionUtils
    {
        private static GameObject s_dontDestroyOnLoadCoroutinesObject;
        public static GameObject dontDestroyOnLoadCoroutinesObject
        {
            get
            {
                if (!s_dontDestroyOnLoadCoroutinesObject)
                {
                    GameObject gameObject = new GameObject("OverhaulCoroutineRunner");
                    GameObject.DontDestroyOnLoad(gameObject);
                    s_dontDestroyOnLoadCoroutinesObject = gameObject;
                }
                return s_dontDestroyOnLoadCoroutinesObject;
            }
        }

        private static GameObject s_coroutinesObject;
        public static GameObject coroutinesObject
        {
            get
            {
                if (!s_coroutinesObject)
                {
                    GameObject gameObject = new GameObject("OverhaulCoroutineRunner");
                    s_coroutinesObject = gameObject;
                }
                return s_coroutinesObject;
            }
        }

        public static Coroutine RunCoroutine(IEnumerator enumerator, bool dontDestroyOnLoad = false)
        {
            GameObject gameObject = dontDestroyOnLoad ? dontDestroyOnLoadCoroutinesObject : coroutinesObject;
            return gameObject.AddComponent<CoroutineRunner>().RunCoroutine(enumerator);
        }

        public static void DoInFrame(Action action)
        {
            _ = RunCoroutine(doNextFrameCoroutine(action));
        }

        public static void DoInFrames(Action action, int frameCount)
        {
            _ = RunCoroutine(doNextFrameCoroutine(action, frameCount));
        }

        public static bool IsGoBackKeyDown()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        private static IEnumerator doNextFrameCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
            yield break;
        }

        private static IEnumerator doNextFrameCoroutine(Action action, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
                yield return null;

            action?.Invoke();
            yield break;
        }

        private class CoroutineRunner : MonoBehaviour
        {
            public Coroutine RunCoroutine(IEnumerator enumerator)
            {
                return StartCoroutine(theCoroutine(enumerator));
            }

            private IEnumerator theCoroutine(IEnumerator enumerator)
            {
                yield return StartCoroutine(enumerator);
                Destroy(this);
                yield break;
            }
        }
    }
}
