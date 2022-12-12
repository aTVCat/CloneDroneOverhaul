using CloneDroneOverhaul.V3Tests.Base;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Utilities
{
    /// <summary>
    /// A system that allows patching game objects mostly without code
    /// </summary>
    public static class FakePrefabSystem
    {
        public static ModDataController DataController;

        /// <summary>
        /// Patch a gameobject if its id is registered in the system
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="id"></param>
        public static void PatchGameObject(in GameObject gameObject, string id)
        {

        }
    }
}
