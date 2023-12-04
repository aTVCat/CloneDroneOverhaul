using System;
using System.Collections;

namespace OverhaulMod.Patches
{
    public class RobotAddon : OverhaulBehaviour
    {
        private Character m_characterReference;
        public Character characterReference
        {
            get
            {
                if (!m_characterReference)
                {
                    m_characterReference = base.GetComponent<Character>();
                }
                return m_characterReference;
            }
        }

        private FirstPersonMover m_firstPersonMoverReference;
        public FirstPersonMover firstPersonMoverReference
        {
            get
            {
                if (!m_firstPersonMoverReference)
                {
                    m_firstPersonMoverReference = characterReference as FirstPersonMover;
                }
                return m_firstPersonMoverReference;
            }
        }

        public bool hasInitializedModel
        {
            get
            {
                FirstPersonMover firstPersonMover = firstPersonMoverReference;
                return firstPersonMover && firstPersonMover.HasCharacterModel();
            }
        }
    }
}
