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

        private byte _enableCursorConditionID = 0;
        protected bool ShowCursor
        {
            set
            {
                if (value)
                {
                    _enableCursorConditionID = EnableCursorController.AddCondition();
                }
                else
                {
                    if (_enableCursorConditionID != 0)
                    {
                        EnableCursorController.RemoveCondition(_enableCursorConditionID);
                        _enableCursorConditionID = 0;
                    }
                }
            }
        }
    }
}