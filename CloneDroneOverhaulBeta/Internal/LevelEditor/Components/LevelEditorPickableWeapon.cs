using ModLibrary;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public class LevelEditorPickableWeapon : MonoBehaviour
    {
        private bool m_HasPickedUp;
        private int m_WeaponIndexLastCheck = -1;

        [IncludeInLevelEditor(false, false)]
        public int WeaponIndex;

        [IncludeInLevelEditor(false, false)]
        public float Distance;

        private void Start()
        {
            GetComponent<ModdedObject>().GetObject<Transform>(0).gameObject.SetActive(GameModeManager.IsInLevelEditor());
            RefreshWeapon();
        }

        public void RefreshWeapon()
        {
            TransformUtils.DestroyAllChildren(GetComponent<ModdedObject>().GetObject<Transform>(1));
            Transform t = WeaponManager.Instance.GetWeaponModelReplacementPrefab((WeaponType)WeaponIndex, true, false, false); // Todo: replace this with better variant that supports normal models
            if (t == null)
            {
                return;
            }

            Transform spawned = Instantiate(t, GetComponent<ModdedObject>().GetObject<Transform>(1));
            spawned.localPosition = Vector3.zero;
            spawned.localEulerAngles = Vector3.zero;
            spawned.localScale = Vector3.one;
        }

        public void GiveWeapon(FirstPersonMover player)
        {
            GetComponent<LevelEditorTriggerPlayAnimation>().PlayAnimation();
            TransformUtils.DestroyAllChildren(GetComponent<ModdedObject>().GetObject<Transform>(1));
            m_HasPickedUp = true;
            List<WeaponType> list = player.GetPrivateField<List<WeaponType>>("_equippedWeapons");
            if (!list.Contains((WeaponType)WeaponIndex))
            {
                list.Add((WeaponType)WeaponIndex);
                player.SetEquippedWeaponType((WeaponType)WeaponIndex, false);
            }
        }

        private void LateUpdate()
        {
            if (GameModeManager.IsInLevelEditor())
            {
                int newIndex = WeaponIndex;
                if (newIndex != m_WeaponIndexLastCheck)
                {
                    RefreshWeapon();
                }
                m_WeaponIndexLastCheck = newIndex;
            }
            else
            {
                if (m_HasPickedUp)
                {
                    return;
                }

                FirstPersonMover player = CharacterTracker.Instance.GetPlayerRobot();
                if (player == null)
                {
                    return;
                }

                float distance = Vector3.Distance(player.transform.position, base.transform.position);
                _ = distance < Distance;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    GiveWeapon(player);
                }
            }
        }
    }
}