using CDOverhaul.HUD.Tooltips;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Combat
{
    public class PlayerTooltipsBehaviour : OverhaulCharacterExpansion
    {
        private MeshRenderer m_Renderer;

        private OverhaulTooltipsController m_Controller;
        private OverhaulCurrentWeaponTooltip m_CurrentWeaponTooltip;
        private OverhaulClosestPlayerTooltip m_ClosestPlayerTooltip;

        private WeaponSkinsWearer m_SkinsWearer;
        public WeaponSkinsWearer SkinsWearer
        {
            get
            {
                if (!m_SkinsWearer)
                    m_SkinsWearer = GetComponent<WeaponSkinsWearer>();

                return m_SkinsWearer;
            }
        }

        private WeaponType m_EquippedWeponType;

        public override void Start()
        {
            base.Start();

            m_Controller = OverhaulController.GetController<OverhaulTooltipsController>();
            m_CurrentWeaponTooltip = m_Controller.GetTooltip<OverhaulCurrentWeaponTooltip>();
            m_ClosestPlayerTooltip = m_Controller.GetTooltip<OverhaulClosestPlayerTooltip>();

            if (GameModeManager.IsMultiplayer() && IsOwnerMultiplayerNotMainPlayer())
            {
                m_Renderer = base.gameObject.AddComponent<MeshRenderer>();
            }
        }

        private void Update()
        {
            if (!Owner || !Owner.IsMainPlayer())
                return;

            if (!Owner.IsAttachedAndAlive())
            {
                base.enabled = false;
                return;
            }

            if (!m_CurrentWeaponTooltip || !m_ClosestPlayerTooltip)
                return;

            if (OverhaulTooltipsUI.ShowPlayerInfos && Time.frameCount % 15 == 0 && GameModeManager.IsNonCoopMultiplayer())
            {
                float maxRange = 30f;
                Character character = null;
                List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
                if (!characters.IsNullOrEmpty())
                {
                    for (int i = 0; i < characters.Count; i++)
                    {
                        Character toCheck = characters[i];
                        if (!toCheck || toCheck.IsMainPlayer() || !toCheck.IsPlayer())
                            continue;

                        float mag = (toCheck.transform.position - base.transform.position).magnitude;
                        if (mag < maxRange)
                        {
                            MeshRenderer renderer = toCheck.GetComponent<MeshRenderer>();
                            if (!renderer || !renderer.isVisible)
                                continue;

                            maxRange = mag;
                            character = toCheck;
                        }
                    }

                    m_ClosestPlayerTooltip.ShowTooltip(character);
                }
            }

            WeaponType newEquippedWeaponType = Owner.GetEquippedWeaponType();
            if (newEquippedWeaponType != m_EquippedWeponType)
            {
                m_CurrentWeaponTooltip.ShowTooltip(newEquippedWeaponType, Owner.IsFireWeapon(newEquippedWeaponType));
            }
            m_EquippedWeponType = newEquippedWeaponType;
        }
    }
}
