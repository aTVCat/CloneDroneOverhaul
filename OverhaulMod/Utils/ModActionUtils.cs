using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Utils
{
    internal static class ModActionUtils
    {
        private static CoroutineRunner s_persistentRoutineRunner;
        public static CoroutineRunner PersistentRoutineRunner
        {
            get
            {
                if (!s_persistentRoutineRunner)
                {
                    GameObject gameObject = new GameObject("Overhaul Coroutine Runner");
                    UnityEngine.Object.DontDestroyOnLoad(gameObject);
                    s_persistentRoutineRunner = gameObject.AddComponent<CoroutineRunner>();
                }
                return s_persistentRoutineRunner;
            }
        }

        private static CoroutineRunner s_routineRunner;
        public static CoroutineRunner RoutineRunner
        {
            get
            {
                if (!s_routineRunner)
                {
                    GameObject gameObject = new GameObject("Overhaul Coroutine Runner");
                    s_routineRunner = gameObject.AddComponent<CoroutineRunner>();
                }
                return s_routineRunner;
            }
        }

        public static Coroutine Run(this IEnumerator enumerator, bool dontDestroyOnLoad = false)
        {
            CoroutineRunner runner = dontDestroyOnLoad ? PersistentRoutineRunner : RoutineRunner;
            return runner.StartCoroutine(enumerator);
        }

        public static void DoInFrame(Action action)
        {
            doNextFrameCoroutine(action).Run();
        }

        public static void DoInFrames(Action action, int frameCount)
        {
            doInFramesCoroutine(action, frameCount).Run();
        }

        public static void DoInTime(Action action, float time)
        {
            doInScaledTimeCoroutine(action, time).Run();
        }

        public static void DoInUnscaledTime(Action action, float time)
        {
            doInUnscaledScaledTimeCoroutine(action, time).Run();
        }

        private static IEnumerator doNextFrameCoroutine(Action action)
        {
            yield return null;
            if (action != null) action();
            yield break;
        }

        private static IEnumerator doInFramesCoroutine(Action action, int frameCount)
        {
            for (int i = 0; i < frameCount; i++) yield return null;
            if (action != null) action();
            yield break;
        }

        private static IEnumerator doInScaledTimeCoroutine(Action action, float time)
        {
            float targetTime = Time.time + time;
            while (Time.time < targetTime) yield return null;
            if (action != null) action();
            yield break;
        }

        private static IEnumerator doInUnscaledScaledTimeCoroutine(Action action, float time)
        {
            float targetTime = Time.unscaledTime + time;
            while (Time.unscaledTime < targetTime) yield return null;
            if (action != null) action();
            yield break;
        }

        public class CoroutineRunner : MonoBehaviour { }
    }
}