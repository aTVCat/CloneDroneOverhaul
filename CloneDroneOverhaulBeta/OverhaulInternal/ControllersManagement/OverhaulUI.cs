using UnityEngine;

namespace CDOverhaul
{
    [RequireComponent(typeof(ModdedObject))]
    public class OverhaulUI : OverhaulController
    {
        private ModdedObject m_ModdedObject;
        /// <summary>
        /// The instance of <see cref="ModdedObject"/>
        /// <b>Note: gameobject with this script must have <see cref="ModdedObject"/></b>
        /// </summary>
        public ModdedObject MyModdedObject
        {
            get
            {
                if(IsDestroyed || IsDisposed)
                {
                    return null;
                }
                if (m_ModdedObject == null)
                {
                    m_ModdedObject = base.GetComponent<ModdedObject>();
                }
                return m_ModdedObject;
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

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }

        public override void Initialize()
        {
        }

        protected override void OnDisposed()
        {
            m_ModdedObject = null;
            ShowCursor = false;
        }
    }
}