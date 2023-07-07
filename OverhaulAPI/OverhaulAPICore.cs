using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace OverhaulAPI
{
    public class OverhaulAPICore : MonoBehaviour
    {
        /// <summary>
        /// The version of API
        /// </summary>
        public static readonly Version APIVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The instance of API
        /// </summary>
        public static OverhaulAPICore APIInstance;

        /// <summary>
        /// Throw an exception
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowException(in string message)
        {
            StackFrame f = new StackFrame(1);
            throw new Exception("OverhaulAPI Error " + APIVersion.ToString() + " [" + f.GetMethod().DeclaringType + "_" + f.GetMethod().Name + "]" + " - " + message);
        }

        /// <summary>
        /// Create new instance of API
        /// </summary>
        /// <returns></returns>
        public static OverhaulAPICore LoadAPI()
        {
            if (APIInstance)
                return APIInstance;

            APIInstance = new GameObject("OverhaulAPI Core v" + APIVersion.ToString() + " Instance").AddComponent<OverhaulAPICore>();

            OverhaulCameraEffect.Reset();
            return APIInstance;
        }

        private void Update()
        {
            OverhaulCameraEffect.APIUpdate();
        }
    }
}
