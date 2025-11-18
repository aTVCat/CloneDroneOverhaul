using System.Collections;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class GreatSwordPreviewController : MonoBehaviour
    {
        private GameObject m_greatSwordModel;

        private WeaponModel m_weaponModel;

        private bool m_initialized;

        private void OnEnable()
        {
            if (!m_initialized)
            {
                _ = base.StartCoroutine(initializeCoroutine());
            }
        }

        private IEnumerator initializeCoroutine()
        {
            FirstPersonMover firstPersonMover = base.GetComponent<FirstPersonMover>();
            while (firstPersonMover && !firstPersonMover.HasCharacterModel())
                yield return null;

            m_initialized = true;
            if (!firstPersonMover)
                yield break;

            m_weaponModel = firstPersonMover.GetCharacterModel().GetWeaponModel(WeaponType.Sword);
            yield break;
        }

        public void SetPreviewActivate(bool value)
        {
            if (!m_initialized || !m_weaponModel)
                return;

            if (value)
            {
                if (!m_greatSwordModel)
                {
                    PhysicalWeaponModelType weaponModelType = WeaponManager.Instance.GetWeaponModelReplacementPrefab(WeaponType.Sword, false, true, false);
                    m_greatSwordModel = Instantiate(WeaponManager.Instance.GetDefaultWeaponModel(weaponModelType), m_weaponModel.transform, false).gameObject;
                    m_greatSwordModel.transform.localScale = Vector3.one * 1.01f;
                }
                m_greatSwordModel.SetActive(true);
            }
            else if (m_greatSwordModel)
            {
                m_greatSwordModel.SetActive(false);
            }
        }

        public bool IsPreviewActive()
        {
            return m_greatSwordModel && m_greatSwordModel.activeSelf;
        }
    }
}
