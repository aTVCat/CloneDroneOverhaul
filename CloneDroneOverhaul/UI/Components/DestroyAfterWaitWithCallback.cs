using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public class DestroyAfterWaitWithCallback : MonoBehaviour
    {
        Action<GameObject> action;
        float timeToDestroy = -1;

        public void SetUp(float wait, Action<GameObject> act)
        {
            timeToDestroy = Time.unscaledTime + wait;
            action = act;
        }

        void FixedUpdate()
        {
            if(Time.unscaledTime >= timeToDestroy)
            {
                Destroy(this.gameObject);
            }
        }

        void OnDestroy()
        {
            action(this.gameObject);
        }
    }
}