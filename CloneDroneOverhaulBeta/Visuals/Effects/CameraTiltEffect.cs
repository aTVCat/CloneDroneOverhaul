using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class CameraTiltEffect : OverhaulCameraEffectBehaviour
    {
        public override void PatchCamera(Camera camera)
        {
            base.PatchCamera(camera);
            if (PreviousCamera && PreviousCamera != CurrentCamera)
            {
                CameraTiltController behaviour = PreviousCamera.GetComponent<CameraTiltController>();
                if (behaviour)
                    behaviour.Dispose(true);
            }
            if (CurrentCamera && CameraControllerBase.IsMainPlayerCamera(CurrentCamera))
            {
                CameraTiltController behaviour = CurrentCamera.GetComponent<CameraTiltController>();
                if (!behaviour)
                {
                    FirstPersonMover firstPersonMover = CameraControllerBase.GetCameraOwner(CurrentCamera) as FirstPersonMover;
                    if (!firstPersonMover)
                        return;

                    behaviour = CurrentCamera.gameObject.AddComponent<CameraTiltController>();
                    behaviour.SetOwner(firstPersonMover);
                }
            }
        }
    }
}
