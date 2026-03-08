using System.Collections;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class GreatSwordPreviewController : MonoBehaviour
    {
        private GameObject _greatSwordModel;

        private WeaponModel _weaponModel;

        private bool _initialized;

        private void OnEnable()
        {
            if (!_initialized)
            {
                _ = base.StartCoroutine(initializeCoroutine());
            }
        }

        private IEnumerator initializeCoroutine()
        {
            FirstPersonMover firstPersonMover = base.GetComponent<FirstPersonMover>();
            while (firstPersonMover && !firstPersonMover.HasCharacterModel())
                yield return null;

            _initialized = true;
            if (!firstPersonMover)
                yield break;

            _weaponModel = firstPersonMover.GetCharacterModel().GetWeaponModel(WeaponType.Sword);
            yield break;
        }

        public void SetPreviewActivate(bool value)
        {
            if (!_initialized || !_weaponModel)
                return;

            if (value)
            {
                if (!_greatSwordModel)
                {
                    PhysicalWeaponModelType weaponModelType = WeaponManager.Instance.GetWeaponModelReplacementPrefab(WeaponType.Sword, false, true, false);
                    _greatSwordModel = Instantiate(WeaponManager.Instance.GetDefaultWeaponModel(weaponModelType), _weaponModel.transform, false).gameObject;
                    _greatSwordModel.transform.localScale = Vector3.one * 1.01f;
                }
                _greatSwordModel.SetActive(true);
            }
            else if (_greatSwordModel)
            {
                _greatSwordModel.SetActive(false);
            }
        }

        public bool IsPreviewActive()
        {
            return _greatSwordModel && _greatSwordModel.activeSelf;
        }
    }
}
