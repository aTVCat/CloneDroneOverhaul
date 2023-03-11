using ModLibrary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OverhaulAPI
{
    public static class WeaponsAdder 
    {
        internal const int Start_WeaponType_Index = 20;
        private static int m_NextID = Start_WeaponType_Index;

        private static Transform m_WeaponsStorage;
        private static readonly List<AddedWeaponModel> m_AddedWeapons = new List<AddedWeaponModel>(); 

        internal static void Init()
        {
            m_NextID = Start_WeaponType_Index;
            m_AddedWeapons.Clear();
            m_WeaponsStorage = new GameObject("OverhaulAPI WeaponsStorage").transform;
            m_WeaponsStorage.gameObject.SetActive(false);
        }

        public static T AddWeaponModel<T>(Transform prefab, ModelOffset offset, MechBodyPartType bodyPart = MechBodyPartType.RightArm) where T : AddedWeaponModel
        {
            Transform transform = Object.Instantiate(prefab, m_WeaponsStorage);

            AddedWeaponModel addedWeaponModel = transform.gameObject.AddComponent<T>();
            addedWeaponModel.WeaponType = (WeaponType)m_NextID;
            addedWeaponModel.ModelOffset = offset;
            addedWeaponModel.BodyPartType = bodyPart;
            m_AddedWeapons.Add(addedWeaponModel);
            m_NextID++;

            return (T)addedWeaponModel;
        }

        /// <summary>
        /// Call this in your mod to add
        /// </summary>
        /// <param name="mover"></param>
        public static void AddWeaponModelsToFirstPersonMover(FirstPersonMover mover, List<AddedWeaponModel> weapons, bool equip, out List<AddedWeaponModel> spawnedWeapons)
        {
            if(mover == null)
            {
                spawnedWeapons = null;
                return;
            }
            if (!mover.HasCharacterModel())
            {
                API.ThrowException("NullReferenceExcepton: Make sure that FirstPersonMover's CharacterModel is already initialized at the moment you add weapons. Call this method after robot's model is initialized.\n");
            }
            if(weapons == null || weapons.Count == 0)
            {
                API.ThrowException("NullReferenceExcepton: You're trying to add 0 new weapons to robots.");
            }

            spawnedWeapons = new List<AddedWeaponModel>();
            CharacterModel model = mover.GetCharacterModel();
            List<WeaponModel> weaponModels = model.WeaponModels.ToList();
            List<WeaponType> weaponTypes = mover.GetPrivateField<List<WeaponType>>("_equippedWeapons");

            foreach(AddedWeaponModel addedModel in weapons)
            {
                MechBodyPartType bodyPartToUse = addedModel.BodyPartType;
                MechBodyPart bodyPart = mover.GetBodyPart(bodyPartToUse);
                if(bodyPart == null)
                {
                    continue;
                }
                Transform parent = bodyPart.transform.parent;
                if(parent == null)
                {
                    continue;
                }

                AddedWeaponModel spawnedModel = GameObject.Instantiate(addedModel, parent);
                spawnedModel.WeaponType = addedModel.WeaponType;
                spawnedModel.BodyPartType = addedModel.BodyPartType;
                spawnedModel.gameObject.layer = Layers.BodyPart;
                spawnedModel.transform.localPosition = addedModel.ModelOffset.OffsetPosition;
                spawnedModel.transform.localEulerAngles = addedModel.ModelOffset.OffsetEulerAngles;
                spawnedModel.transform.localScale = addedModel.ModelOffset.OffsetLocalScale;
                spawnedModel.ModelOffset = addedModel.ModelOffset;
                spawnedModel.gameObject.SetActive(mover.GetEquippedWeaponType() == spawnedModel.WeaponType);
                spawnedModel.Initialize(mover);
                HSBColor hsbcolor2 = new HSBColor(mover.GetCharacterModel().GetFavouriteColor())
                {
                    b = 1f,
                    s = 0.75f
                };
                spawnedModel.ApplyOwnersFavouriteColor(hsbcolor2.ToColor() * 2.5f);
                spawnedWeapons.Add(spawnedModel);

                weaponModels.Add(spawnedModel);
                if(equip) weaponTypes.Add(spawnedModel.WeaponType);
            }

            model.WeaponModels = weaponModels.ToArray();
            if (equip) mover.SetPrivateField("_equippedWeapons", weaponTypes);
        }
    }
}