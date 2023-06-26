using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetInstanceBehaviour : OverhaulBehaviour
    {
        public PetItem Item;
        public PetBehaviourSettings BehaviourSettings => Item != null ? Item.BehaviourSettings : null;

        public FirstPersonMover Owner;

        public float YOffset;

        public void Update()
        {
            if (IsDisposedOrDestroyed())
                return;

            if(!Owner || !Owner.IsAlive())
            {
                DestroyGameObject();
                return;
            }

            float deltaTime = (BoltNetwork.IsConnected ? BoltNetwork.FrameDeltaTime : Time.deltaTime) * 6f;
            YOffset = Mathf.Sin(GetTime() * BehaviourSettings.FlyEffectMultiplier1) * BehaviourSettings.FlyEffectMultiplier2 * Time.timeScale;

            Vector3 posTarget = Owner.transform.position + TargetPositionNodes.GetVector(BehaviourSettings.OffsetTargetPositionNodes, Owner);
            Vector3 pos = base.transform.position;
            pos.x = Mathf.Lerp(pos.x, posTarget.x, deltaTime);
            pos.y = Mathf.Lerp(pos.y, posTarget.y, deltaTime) + YOffset;
            pos.z = Mathf.Lerp(pos.z, posTarget.z, deltaTime);
            base.transform.position = pos;
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Owner.transform.rotation * Quaternion.Euler(BehaviourSettings.OffsetTargetRotation), deltaTime);
        }

        public static PetInstanceBehaviour CreateInstance(PetItem pet, FirstPersonMover firstPersonMover)
        {
            GameObject gameObject = Instantiate(pet.Model);
            gameObject.transform.position = firstPersonMover.transform.position + new Vector3(0, 5, 0);
            gameObject.transform.localScale = pet.BehaviourSettings.OffsetScale;
            PetInstanceBehaviour petInstanceBehaviour = gameObject.AddComponent<PetInstanceBehaviour>();
            petInstanceBehaviour.Item = pet;
            petInstanceBehaviour.Owner = firstPersonMover;
            return petInstanceBehaviour;
        }

        public static float GetTime()
        {
            return BoltNetwork.IsConnected ? BoltNetwork.ServerTime : Time.time;
        }
    }
}
