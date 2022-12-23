using System;
using UnityEngine;

namespace CloneDroneOverhaul.UI.Components
{
    public class DestroyAfterWaitWithCallback : MonoBehaviour
    {
        private Action<GameObject> action;
        private float timeToDestroy = -1;

        public void SetUp(float wait, Action<GameObject> act)
        {
            timeToDestroy = Time.unscaledTime + wait;
            action = act;
        }

        private void FixedUpdate()
        {
            if (Time.unscaledTime >= timeToDestroy)
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            action(gameObject);
        }
    }
}