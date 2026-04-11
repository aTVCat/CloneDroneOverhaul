using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class ConstructionPropsParticleReplacer : MonoBehaviour
    {
        public bool IsOutsideArena;

        public bool IsMultiplayerSpawnPoint;

        private Transform _welding1, _welding2;

        private Transform _prefab;

        private GameObject _replacedWelding1, _replacedWelding2;

        private void Start()
        {
            _prefab = ModResources.Prefab(AssetBundleConstants.VFX, "VFX_Welding").transform;
            getReferences();
            ReplaceWeldingParticles();
            RefreshParticles();
        }

        private void getReferences()
        {
            if (IsOutsideArena)
            {
                Transform bussinessWelder1 = TransformUtils.FindChildRecursive(transform, "BusinessWelder1");
                Transform bussinessWelder2 = TransformUtils.FindChildRecursive(transform, "BusinessWelder2");

                _welding1 = bussinessWelder1 ? TransformUtils.FindChildRecursive(bussinessWelder1, "WeldingVFX1") : null;
                _welding2 = bussinessWelder2 ? TransformUtils.FindChildRecursive(bussinessWelder2, IsMultiplayerSpawnPoint ? "WeldingVFX2" : "WeldingVFX1") : null;
            }
            else
            {
                _welding1 = TransformUtils.FindChildRecursive(transform, "WeldingVFX1");
                _welding2 = TransformUtils.FindChildRecursive(transform, "WeldingVFX2");
            }
        }

        public void RefreshParticles()
        {
            bool rework = ParticleManager.ReworkWeldingParticles;
            if (_welding1) _welding1.gameObject.SetActive(!rework);
            if (_replacedWelding1) _replacedWelding1.SetActive(rework);

            if (_welding2) _welding2.gameObject.SetActive(!rework);
            if (_replacedWelding2) _replacedWelding2.SetActive(rework);
        }

        public void ReplaceWeldingParticles()
        {
            if (_welding1 && !_replacedWelding1) _replacedWelding1 = ReplaceParticleSystem(_welding1).gameObject;
            if (_welding2 && !_replacedWelding2) _replacedWelding2 = ReplaceParticleSystem(_welding2).gameObject;
        }

        public ReplacedParticles ReplaceParticleSystem(Transform originalParticles)
        {
            Transform newWeldingVfx = Instantiate(_prefab, originalParticles.parent, false);
            newWeldingVfx.transform.localPosition = originalParticles.localPosition;
            newWeldingVfx.transform.localEulerAngles = originalParticles.localEulerAngles;
            newWeldingVfx.transform.localScale = Vector3.one;

            ReplacedParticles replacedParticles = newWeldingVfx.gameObject.AddComponent<ReplacedParticles>();
            replacedParticles.SetOriginalSystem(originalParticles.GetComponent<ParticleSystem>());
            replacedParticles.EmitIfOriginalSystemIsActive = IsOutsideArena;
            return replacedParticles;
        }
    }
}
