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

        public void Update()
        {
            if (IsDisposedOrDestroyed())
                return;

            if(!Owner || !Owner.IsAlive())
            {
                DestroyGameObject();
                return;
            }

            float deltaTime = Time.deltaTime * 6f;

            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Owner.transform.rotation * Quaternion.Euler(BehaviourSettings.OffsetTargetRotation), deltaTime);

            Vector3 posTarget = Owner.transform.position + TargetPositionNodes.GetVector(BehaviourSettings.OffsetTargetPositionNodes, Owner);
            Vector3 pos = base.transform.position;
            pos.x = Mathf.Lerp(pos.x, posTarget.x, deltaTime);
            pos.y = Mathf.Lerp(pos.y, posTarget.y, deltaTime);
            pos.z = Mathf.Lerp(pos.z, posTarget.z, deltaTime);
            base.transform.position = pos;
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
    }
}
