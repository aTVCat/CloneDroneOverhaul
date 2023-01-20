using UnityEngine;

namespace CloneDroneOverhaul.V3.Utilities
{
    public static class GameObjectUtilities
    {
        public static GameObject[] Instantiate(this GameObject gameObject, GameObject original, Transform parent, int times, bool active)
        {
            GameObject[] result = new GameObject[times];
            for (int i = 0; i < times; i++)
            {
                result[i] = UnityEngine.Object.Instantiate(original, parent);
                result[i].SetActive(active);
            }
            return result;
        }

        public static GameObject[] Instantiate(this GameObject gameObject, GameObject original, Transform parent, int times)
        {
            GameObject[] result = new GameObject[times];
            for (int i = 0; i < times; i++)
            {
                result[i] = UnityEngine.Object.Instantiate(original, parent);
            }
            return result;
        }
    }
}
