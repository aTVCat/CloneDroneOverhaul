using OverhaulMod.Engine;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class CloneDroneLogoParticlesBehaviour : GamePatchBehaviour
    {
        private Transform _particlesParent;

        private GameObject[] _vfxObjects;

        public override void Patch()
        {
            Transform fireVfxTransform = TransformUtils.FindChildRecursive(ArenaCameraManager.Instance.TitleScreenLogo.transform, "fireVFX");
            if (!fireVfxTransform) return;

            _particlesParent = fireVfxTransform;

            if (fireVfxTransform.childCount != 0)
            {
                _vfxObjects = new GameObject[fireVfxTransform.childCount];
                for (int i = 0; i < fireVfxTransform.childCount; i++)
                    _vfxObjects[i] = fireVfxTransform.GetChild(i).gameObject;

                RefreshVisibility();
            }
        }

        public void RefreshVisibility()
        {
            if (_vfxObjects == null) return;

            for (int i = 0; i < _vfxObjects.Length; i++)
            {
                _vfxObjects[i].SetActive(TitleScreenCustomizationManager.ShowLogoFireParticles);
            }
        }

        public override void Update()
        {
            if (GameModeManager.IsOnTitleScreen() && _particlesParent && _particlesParent.gameObject.activeInHierarchy)
                _particlesParent.localPosition = new Vector3(-0.25f, 3.8f, -6.2f);
        }
    }
}