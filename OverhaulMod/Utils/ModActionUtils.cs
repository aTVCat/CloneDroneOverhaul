using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Utils
{
    internal static class ModActionUtils
    {
        public static Coroutine RunCoroutine(IEnumerator enumerator, bool dontDestroyOnLoad = false)
        {
            GameObject gameObject = new GameObject("OverhaulCoroutine");
            if(dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(gameObject);
            return gameObject.AddComponent<CoroutineRunner>().RunCoroutine(enumerator);
        }

        public static void DoNextFrame(Action action)
        {
            _ = RunCoroutine(doNextFrameCoroutine(action));
        }

        public static bool IsGoBackKeyDown()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        private static IEnumerator doNextFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();
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
                Destroy(gameObject);
                yield break;
            }
        }
    }
}
