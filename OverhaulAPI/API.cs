using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

namespace OverhaulAPI
{
    public class API : MonoBehaviour
    {
        /// <summary>
        /// The version of API
        /// </summary>
        public static readonly Version APIVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The instance of API
        /// </summary>
        public static API APIInstance = null;

        /// <summary>
        /// Throw an exception
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowException(in string message)
        {
            StackFrame f = new StackFrame(1);
            throw new Exception("OverhaulAPI " + APIVersion.ToString() + " [" + f.GetMethod().DeclaringType + "_" + f.GetMethod().Name + "]" + " - " + message);
        }

        /// <summary>
        /// Create new instance of API
        /// </summary>
        /// <returns></returns>
        public static API LoadAPI()
        {
            if(APIInstance != null)
            {
                return null;
            }

            APIInstance = new GameObject("OverhaulAPI v" + APIVersion.ToString()).AddComponent<API>();

            GamemodeAPI.Reset();
            OverhaulPostProcessBehaviour.Reset();

            return APIInstance;
        }

        private void Update()
        {
            OverhaulPostProcessBehaviour.APIUpdate();
        }
    }
}
