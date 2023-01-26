using UnityEngine;

namespace CDOverhaul.HUD
{
    [RequireComponent(typeof(ModdedObject))]
    public class UIBase : ModController
    {
        private ModdedObject _moddedObject;

        /// <summary>
        /// The instance of <see cref="ModdedObject"/>
        /// <b>Note: gameobject with this script must have <see cref="ModdedObject"/></b>
        /// </summary>
        public ModdedObject MyModdedObject
        {
            get
            {
                if (_moddedObject == null)
                {
                    _moddedObject = base.GetComponent<ModdedObject>();
                }
                return _moddedObject;
            }
        }

        protected byte EnableCursorConditionID { get; set; }
    }
}