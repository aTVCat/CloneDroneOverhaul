using UnityEngine;

namespace CDOverhaul
{
    public class ModController : MonoBehaviour
    {
        /// <summary>
        /// This variable doesn't affect anything
        /// </summary>
        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// This variable doesn't affect anything
        /// </summary>
        public bool HasAddedEventListeners { get; protected set; }

        /// <summary>
        /// This is called instantly when controller is added
        /// </summary>
        public virtual void Initialize()
        {

        }
    }
}
