using ModLibrary;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsWearer : OverhaulCharacterExpansion
    {
        public readonly Dictionary<IWeaponSkinItemDefinition, WeaponSkinSpawnInfo> WeaponSkins = new Dictionary<IWeaponSkinItemDefinition, WeaponSkinSpawnInfo>();
        private bool m_WaitingToSpawnSkins;

        public override void Start()
        {
            base.Start();
            SpawnSkins();
        }

        protected override void OnRefresh()
        {
            SpawnSkins();
        }

        public void SpawnSkins()
        {
            if (IsDisposedOrDestroyed())
            {
                return;
            }
            if (m_WaitingToSpawnSkins)
            {
                return;
            }

            m_WaitingToSpawnSkins = true;
            DelegateScheduler.Instance.Schedule(delegate
            {
                spawnSkins();
                m_WaitingToSpawnSkins = false;
            }, 0.2f);
        }

        private void spawnSkins()
        {
            if(GameModeManager.IsMultiplayer() && !IsOwnerMainPlayer() && !WeaponSkinsController.AllowEnemiesWearSkins)
            {
                return;
            }
            if (!IsOwnerPlayer() && !WeaponSkinsController.AllowEnemiesWearSkins)
            {
                return;
            }

            IWeaponSkinItemDefinition[] skins = OverhaulController.GetController<WeaponSkinsController>().Interface.GetSkinItems(FirstPersonMover);
            if (skins == null)
            {
                return;
            }

            SetDefaultModelsActive();

            if (!WeaponSkins.Values.IsNullOrEmpty())
            {
                foreach(WeaponSkinSpawnInfo info in WeaponSkins.Values)
                {
                    SetDefaultModelsActive(info.Model.transform);

                    if (info.Type == WeaponType.Bow)
                    {
                        ModdedObject m = info.Model.GetComponent<ModdedObject>();
                        Destroy(m.GetObject<Transform>(0).gameObject);
                        Destroy(m.GetObject<Transform>(1).gameObject);
                    }
                    info.DestroyModel();
                }
                WeaponSkins.Clear();
            }

            foreach (IWeaponSkinItemDefinition skin in skins)
            {
                SpawnSkin(skin);
            }
        }

        public void SetDefaultModelsActive(Transform transformToRemove = null)
        {
            if (!FirstPersonMover.HasCharacterModel())
            {
                return;
            }
            CharacterModel model = FirstPersonMover.GetCharacterModel();

            WeaponModel weaponModel1 = model.GetWeaponModel(WeaponType.Sword);
            if (weaponModel1 != null)
            {
                if(transformToRemove != null)
                {
                    List<Transform> t1 = weaponModel1.PartsToDrop.ToList();
                    _ = t1.Remove(transformToRemove);
                    weaponModel1.PartsToDrop = t1.ToArray();
                }
                else
                {
                    SetDefaultModelsVisible(true, weaponModel1);
                }
            }
            WeaponModel weaponModel2 = model.GetWeaponModel(WeaponType.Bow);
            if (weaponModel2 != null)
            {
                if (transformToRemove != null)
                {
                    List<Transform> t1 = weaponModel2.PartsToDrop.ToList();
                    _ = t1.Remove(transformToRemove);
                    weaponModel2.PartsToDrop = t1.ToArray();
                }
                else
                {
                    SetDefaultModelsVisible(true, weaponModel2);
                }
            }
            WeaponModel weaponModel3 = model.GetWeaponModel(WeaponType.Hammer);
            if (weaponModel3 != null)
            {
                if (transformToRemove != null)
                {
                    List<Transform> t1 = weaponModel3.PartsToDrop.ToList();
                    _ = t1.Remove(transformToRemove);
                    weaponModel3.PartsToDrop = t1.ToArray();
                }
                else
                {
                    SetDefaultModelsVisible(true, weaponModel3);
                }
            }
            WeaponModel weaponModel4 = model.GetWeaponModel(WeaponType.Spear);
            if (weaponModel4 != null)
            {
                if (transformToRemove != null)
                {
                    List<Transform> t1 = weaponModel4.PartsToDrop.ToList();
                    _ = t1.Remove(transformToRemove);
                    weaponModel4.PartsToDrop = t1.ToArray();
                }
                else
                {
                    SetDefaultModelsVisible(true, weaponModel4);
                }
            }
        }

        public void SpawnSkin(IWeaponSkinItemDefinition item)
        {
            if (item == null || FirstPersonMover == null || !FirstPersonMover.HasCharacterModel())
            {
                return;
            }
            WeaponModel weaponModel = FirstPersonMover.GetCharacterModel().GetWeaponModel(item.GetWeaponType());
            if(weaponModel == null)
            {
                return;
            }
            SetDefaultModelsVisible(false, weaponModel);
            if (item.GetItemName() == "Default" || weaponModel.WeaponType != item.GetWeaponType())
            {
                SetDefaultModelsVisible(true, weaponModel);
                return;
            }
            bool fire = IsFireVariant(weaponModel) && item.GetWeaponType() != WeaponType.Bow;
            bool multiplayer = GameModeManager.UsesMultiplayerSpeedMultiplier() && item.GetWeaponType() == WeaponType.Sword;
            WeaponVariant variant = WeaponSkinsController.GetVariant(fire, multiplayer);

            /*
            foreach(WeaponSkinSpawnInfo wsInfo in WeaponSkins.Values)
            {
                if (wsInfo.Type == item.GetWeaponType() && wsInfo != null)
                {
                    wsInfo.DestroyModel();
                    break;
                }
            }
            _ = WeaponSkins.Remove(item);*/
            
            WeaponSkinModel newModel = item.GetModel(fire, multiplayer);            
            /*OverhaulDebugController.Print("Fire: " + fire);
            OverhaulDebugController.Print("Multiplayer: " + multiplayer);
            OverhaulDebugController.Print((newModel == null).ToString());*/
            if (newModel != null)
            {
                Transform spawnedModel = Instantiate(newModel.Model, weaponModel.transform).transform;
                spawnedModel.localPosition = newModel.Offset.OffsetPosition;
                spawnedModel.localEulerAngles = newModel.Offset.OffsetEulerAngles;
                spawnedModel.localScale = newModel.Offset.OffsetLocalScale;
                spawnedModel.gameObject.layer = Layers.BodyPart;
                SetModelColor(spawnedModel.gameObject, fire);
                WeaponSkinSpawnInfo newInfo = new WeaponSkinSpawnInfo
                {
                    Model = spawnedModel.gameObject,
                    Type = item.GetWeaponType(),
                    Variant = variant
                };
                WeaponSkins.Add(item, newInfo);

                BoxCollider collider = spawnedModel.gameObject.AddComponent<BoxCollider>();
                collider.size *= 0.5f;

                List<Transform> t1 = weaponModel.PartsToDrop.ToList();
                t1.Add(spawnedModel);
                weaponModel.PartsToDrop = t1.ToArray();

                if(weaponModel.WeaponType == WeaponType.Bow)
                {
                    ModdedObject m = spawnedModel.GetComponent<ModdedObject>();
                    Transform bowStringUpper = TransformUtils.FindChildRecursive(weaponModel.transform, "BowStringUpper");
                    Transform bowStringLower = TransformUtils.FindChildRecursive(weaponModel.transform, "BowStringLower");
                    if(bowStringLower != null && bowStringUpper != null)
                    {
                        m.GetObject<Transform>(0).SetParent(bowStringUpper, true);
                        m.GetObject<Transform>(1).SetParent(bowStringLower, true);
                    }
                }
            }
            else
            {
                SetDefaultModelsVisible(true, weaponModel);
            }
        }

        public void SetDefaultModelsVisible(bool value, WeaponModel model)
        {
            Transform[] partsToDropArray = model.PartsToDrop;
            if (!partsToDropArray.IsNullOrEmpty())
            {
                foreach (Transform part in partsToDropArray)
                {
                    part.gameObject.SetActive(value);
                }
            }
        }

        public void SetModelColor(GameObject model, bool fire)
        {
            if (!fire)
            {
                Renderer renderer = model.GetComponent<Renderer>();
                if(renderer == null || renderer.material == null)
                {
                    return;
                }
                Material material = renderer.material;
                HSBColor hsbcolor2 = new HSBColor(FirstPersonMover.GetCharacterModel().GetFavouriteColor())
                {
                    b = 1f,
                    s = 0.75f
                };
                material.SetColor("_EmissionColor", hsbcolor2.ToColor() * 2.5f);
            }
        }

        public bool IsFireVariant(WeaponModel model)
        {
            if(model.WeaponType == WeaponType.Sword)
            {
                if (FirstPersonMover.HasUpgrade(UpgradeType.FireSword))
                {
                    return true;
                }
            }
            else if (model.WeaponType == WeaponType.Hammer)
            {
                if (FirstPersonMover.HasUpgrade(UpgradeType.FireHammer))
                {
                    return true;
                }
            }
            else if (model.WeaponType == WeaponType.Spear)
            {
                if (FirstPersonMover.HasUpgrade(UpgradeType.FireSpear))
                {
                    return true;
                }
            }
            return false;
        }

    }
}