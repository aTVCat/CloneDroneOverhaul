using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class CameraFOVEffect : OverhaulCameraEffectBehaviour
    {
        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                CameraFOVController behaviour = PreviousCamera.GetComponent<CameraFOVController>();
                if (behaviour)
                    behaviour.Dispose(true);
            }
            if (CurrentCamera && CameraControllerBase.IsMainPlayerCamera(CurrentCamera))
            {
                CameraFOVController behaviour = CurrentCamera.GetComponent<CameraFOVController>();
                if (!behaviour)
                {
                    FirstPersonMover firstPersonMover = CameraControllerBase.GetCameraOwner(CurrentCamera) as FirstPersonMover;
                    if (!firstPersonMover)
                        return;

                    behaviour = CurrentCamera.gameObject.AddComponent<CameraFOVController>();
                    behaviour.SetOwner(firstPersonMover);
                }
            }
        }
    }
}
