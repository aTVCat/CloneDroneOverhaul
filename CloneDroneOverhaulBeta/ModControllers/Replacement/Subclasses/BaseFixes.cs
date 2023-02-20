using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        private Vector3? _ogPhysics;
        private Vector3? _overhaulPhysics;

        public override void Replace()
        {
            base.Replace();

            /*
            if(_ogPhysics == null)
            {
                _ogPhysics = PhysicsManager.Instance.GetDefaultGravity();

                _overhaulPhysics = PhysicsManager.Instance.GetPrivateField<Vector3>("_defaultRigidBodyGravity") - new Vector3(0, -6, 0);
                PhysicsManager.Instance.SetPrivateField("_defaultRigidBodyGravity", _overhaulPhysics);
                Physics.gravity = _overhaulPhysics.Value;
            }*/

            SeveredVolumeGenerator.Instance.SeveredPartVelocity = 6f;


            // Fix lines on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performacne a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
