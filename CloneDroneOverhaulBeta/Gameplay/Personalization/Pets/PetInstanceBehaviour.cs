using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    public class PetInstanceBehaviour : OverhaulBehaviour
    {
        public PetItem Item;
        public PetBehaviourSettings BehaviourSettings => Item != null ? Item.BehaviourSettings : null;

        public FirstPersonMover Owner;
        public PetsWearer OwnerPets;

        public float YOffset;

        public void Update()
        {
            if (IsDisposedOrDestroyed())
                return;

            if (!OwnerPets || !Owner || !Owner.IsAlive())
            {
                DestroyGameObject();
                return;
            }

            float deltaTime = GetDeltaTime();
            YOffset = Mathf.Sin(GetTime() * BehaviourSettings.FlyEffectMultiplier1) * BehaviourSettings.FlyEffectMultiplier2 * Time.timeScale;

            Vector3 posTarget = Owner.transform.position + TargetPositionNodes.GetVector(BehaviourSettings.OffsetTargetPositionNodes, Owner);
            Vector3 pos = base.transform.position;
            pos.x = Mathf.Lerp(pos.x, posTarget.x, deltaTime);
            pos.y = Mathf.Lerp(pos.y, posTarget.y, deltaTime) + YOffset;
            pos.z = Mathf.Lerp(pos.z, posTarget.z, deltaTime);
            base.transform.position = pos;
            base.transform.rotation = Quaternion.Lerp(base.transform.rotation, GetTargetRotation(), deltaTime);
        }

        public Quaternion GetTargetRotation()
        {
            if (BehaviourSettings == null)
                return default;

            if (BehaviourSettings.RangeToLookAtEnemy > 0f)
            {
                FirstPersonMover firstPersonMover = GetNearestEnemyRobot(BehaviourSettings.RangeToLookAtEnemy);
                if (firstPersonMover && Vector3.Distance(Owner.transform.position, firstPersonMover.transform.position) <= BehaviourSettings.RangeToLookAtEnemy)
                {
                    Vector3 lookPos = firstPersonMover.transform.position + (Vector3.up * 2f) - base.transform.position;
                    return Quaternion.LookRotation(lookPos) * Quaternion.Euler(BehaviourSettings.OffsetTargetRotation);
                }
            }
            return BehaviourSettings.FollowHeadRotation && OwnerPets.HeadTransform
                ? OwnerPets.HeadTransform.rotation * Quaternion.Euler(BehaviourSettings.OffsetTargetRotation)
                : Owner.transform.rotation * Quaternion.Euler(BehaviourSettings.OffsetTargetRotation);
        }

        public static PetInstanceBehaviour CreateInstance(PetItem pet, FirstPersonMover firstPersonMover)
        {
            if (pet == null || pet.PetModel == null || pet.PetVoxModel == null)
                return null;

            GameObject gameObject;
            if (!pet.PetModel.LoadAsset())
            {
                if (pet.PetVoxModel.IsNone())
                    return null;

                gameObject = Instantiate(OverhaulUniquePrefabs.EmptyVolume.gameObject);
                gameObject.SetActive(true);
                _ = pet.PetVoxModel.TryLoad(gameObject);
            }
            else
            {
                gameObject = Instantiate(pet.PetModel.Asset as GameObject);
            }

            gameObject.transform.position = firstPersonMover.transform.position + new Vector3(0, 5, 0);
            gameObject.transform.localScale = pet.BehaviourSettings.OffsetScale;
            PetInstanceBehaviour petInstanceBehaviour = gameObject.AddComponent<PetInstanceBehaviour>();
            petInstanceBehaviour.Item = pet;
            petInstanceBehaviour.Owner = firstPersonMover;
            petInstanceBehaviour.OwnerPets = firstPersonMover.GetComponent<PetsWearer>();
            return petInstanceBehaviour;
        }

        public static float GetTime()
        {
            return BoltNetwork.IsConnected ? BoltNetwork.ServerTime : Time.time;
        }

        public static float GetDeltaTime()
        {
            return (BoltNetwork.IsConnected ? BoltNetwork.FrameDeltaTime : Time.deltaTime) * 4.5f;
        }

        public FirstPersonMover GetNearestEnemyRobot(float maxRange)
        {
            List<Character> characters = CharacterTracker.Instance.GetAllLivingCharacters();
            Transform ownerTransform = Owner.transform;

            FirstPersonMover result = null;
            float range = maxRange;
            foreach (Character character in characters)
            {
                if (!character || !(character is FirstPersonMover) || character == Owner)
                    continue;

                FirstPersonMover firstPersonMover = character as FirstPersonMover;
                if (!GameModeManager.IsNonCoopMultiplayer() && firstPersonMover.IsPlayerTeam == Owner.IsPlayerTeam)
                    continue;

                Transform transform = character.transform;
                float newDistance = Vector3.Distance(ownerTransform.position, transform.position);
                if (newDistance < range)
                {
                    result = character as FirstPersonMover;
                    range = newDistance;
                }
            }
            return result;
        }
    }
}
