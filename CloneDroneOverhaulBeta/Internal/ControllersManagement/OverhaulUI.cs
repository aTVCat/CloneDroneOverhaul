using UnityEngine;

namespace CDOverhaul
{
    /// <summary>
    /// The base class of any HUD in the mod
    /// </summary>
    [RequireComponent(typeof(ModdedObject), typeof(RectTransform))]
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
                if (IsDisposedOrDestroyed())
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
        /// <summary>
        /// Make the game to force show system cursor
        /// </summary>
        protected bool ShowCursor
        {
            set
            {
                if (value && !IsDisposedOrDestroyed())
                {
                    if (_enableCursorConditionID != 0)
                    {
                        return;
                    }
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
            base.OnDisposed();
            m_ModdedObject = null;
            ShowCursor = false;
        }
    }
}