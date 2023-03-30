using CDOverhaul.Gameplay.Multiplayer;
using CDOverhaul.HUD;
using ModLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinsWearer : OverhaulCharacterExpansion
    {
        public readonly Dictionary<IWeaponSkinItemDefinition, WeaponSkinSpawnInfo> WeaponSkins = new Dictionary<IWeaponSkinItemDefinition, WeaponSkinSpawnInfo>();
        private bool m_WaitingToSpawnSkins;

        private OverhaulModdedPlayerInfo m_Info;

        public bool IsMultiplayerControlled { get; private set; }
        private bool m_HasAddedListeners;

        public override void Start()
        {
            base.Start();
            SpawnSkins();

            DelegateScheduler.Instance.Schedule(delegate
            {
                if(FirstPersonMover != null && MultiplayerPlayerInfoManager.Instance != null)
                {
                    MultiplayerPlayerInfoState pInfo = MultiplayerPlayerInfoManager.Instance.GetPlayerInfoState(FirstPersonMover.GetPlayFabID());
                    if (pInfo != null)
                    {
                        m_Info = pInfo.GetComponent<OverhaulModdedPlayerInfo>();
                        if(m_Info != null)
                        {
                            onGetData(m_Info.GetHashtable());

                            m_HasAddedListeners = true;
                            _ = OverhaulEventManager.AddEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
                        }
                    }
                }
            }, 0.5f);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            if (!m_HasAddedListeners)
            {
                return;
            }
            OverhaulEventManager.RemoveEventListener<Hashtable>(OverhaulModdedPlayerInfo.InfoReceivedEventString, onGetData);
        }

        protected override void OnRefresh()
        {
            SpawnSkins();
        }

        private void onGetData(Hashtable hash)
        {
            IsMultiplayerControlled = true;
            OnRefresh();
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
                m_WaitingToSpawnSkins = false;
                try
                {
                    spawnSkins();
                }
                catch
                {

                }
            }, 0.2f);
        }

        private void spawnSkins()
        {
            if (!WeaponSkinsController.IsFirstPersonMoverSupported(FirstPersonMover))
            {
                return;
            }

            SetDefaultModelsActive();
            if (!IsOwnerPlayer() && !WeaponSkinsController.AllowEnemiesWearSkins)
            {
                return;
            }
            if(GameModeManager.IsMultiplayer() && (m_Info == null || !m_Info.HasReceivedData || m_Info.GetData("ID").Equals(string.Empty)))
            {
                return;
            }

            WeaponSkinsController controller = OverhaulController.GetController<WeaponSkinsController>();
            IWeaponSkinItemDefinition[] skins;
            if (IsMultiplayerControlled)
            {
                skins = new IWeaponSkinItemDefinition[4];
                skins[0] = controller.Interface.GetSkinItem(WeaponType.Sword, m_Info.GetData("Skin.Sword"), ItemFilter.Everything, out _);
                skins[1] = controller.Interface.GetSkinItem(WeaponType.Bow, m_Info.GetData("Skin.Bow"), ItemFilter.Everything, out _);
                skins[2] = controller.Interface.GetSkinItem(WeaponType.Hammer, m_Info.GetData("Skin.Hammer"), ItemFilter.Everything, out _);
                skins[3] = controller.Interface.GetSkinItem(WeaponType.Spear, m_Info.GetData("Skin.Spear"), ItemFilter.Everything, out _);
            }
            else
            {
                skins = controller.Interface.GetSkinItems(FirstPersonMover);
            }
            if (skins == null)
            {
                return;
            }

            if (!WeaponSkins.Values.IsNullOrEmpty())
            {
                foreach(WeaponSkinSpawnInfo info in WeaponSkins.Values)
                {
                    if(info.Type == WeaponType.Bow && !OverhaulGamemodeManager.SupportsBowSkins())
                    {
                        continue;
                    }

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
                    if(weaponModel1.PartsToDrop != null)
                    {
                        List<Transform> t1 = weaponModel1.PartsToDrop.ToList();
                        _ = t1.Remove(transformToRemove);
                        weaponModel1.PartsToDrop = t1.ToArray();
                    }
                }
                else
                {
                    SetDefaultModelsVisible(true, weaponModel1);
                }
            }
            WeaponModel weaponModel2 = model.GetWeaponModel(WeaponType.Bow);
            if (weaponModel2 != null && OverhaulGamemodeManager.SupportsBowSkins())
            {
                if (transformToRemove != null)
                {
                    if (weaponModel2.PartsToDrop != null)
                    {
                        List<Transform> t1 = weaponModel2.PartsToDrop.ToList();
                        _ = t1.Remove(transformToRemove);
                        weaponModel2.PartsToDrop = t1.ToArray();
                    }
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
                    if (weaponModel3.PartsToDrop != null)
                    {
                        List<Transform> t1 = weaponModel3.PartsToDrop.ToList();
                        _ = t1.Remove(transformToRemove);
                        weaponModel3.PartsToDrop = t1.ToArray();
                    }
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
                    if (weaponModel4.PartsToDrop != null)
                    {
                        List<Transform> t1 = weaponModel4.PartsToDrop.ToList();
                        _ = t1.Remove(transformToRemove);
                        weaponModel4.PartsToDrop = t1.ToArray();
                    }
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
            if(weaponModel == null || (weaponModel.WeaponType.Equals(WeaponType.Bow) && !OverhaulGamemodeManager.SupportsBowSkins()))
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
            
            WeaponSkinModel newModel = item.GetModel(fire, multiplayer, 0);            
            if (newModel != null)
            {
                Transform spawnedModel = Instantiate(newModel.Model, weaponModel.transform).transform;
                spawnedModel.localPosition = newModel.Offset.OffsetPosition;
                spawnedModel.localEulerAngles = newModel.Offset.OffsetEulerAngles;
                spawnedModel.localScale = newModel.Offset.OffsetLocalScale;
                spawnedModel.gameObject.layer = Layers.BodyPart;
                if((fire && !(item as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenFire) || (!fire && !(item as WeaponSkinItemDefinitionV2).DontUseCustomColorsWhenNormal))
                {
                    Color? forcedColor = null;
                    if(fire && (item as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor != -1)
                    {
                        forcedColor = HumanFactsManager.Instance.GetFavColor((item as WeaponSkinItemDefinitionV2).IndexOfForcedFireVanillaColor).ColorValue;
                    }
                    else if(!fire && (item as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor != -1)
                    {
                        forcedColor = HumanFactsManager.Instance.GetFavColor((item as WeaponSkinItemDefinitionV2).IndexOfForcedNormalVanillaColor).ColorValue;
                    }

                    SetModelColor(spawnedModel.gameObject, fire, (item as WeaponSkinItemDefinitionV2).Saturation, forcedColor);
                }
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
                        bowStringLower.GetChild(0).localScale = new Vector3(0.1f, 1.3f, 0.1f);
                        bowStringUpper.GetChild(0).localScale = new Vector3(0.1f, 1.3f, 0.1f);
                        if ((item as WeaponSkinItemDefinitionV2).UseVanillaBowStrings)
                        {
                            m.GetObject<Transform>(0).gameObject.SetActive(false);
                            m.GetObject<Transform>(1).gameObject.SetActive(false);
                            bowStringLower.GetChild(0).gameObject.SetActive(true);
                            bowStringLower.GetChild(0).localScale = new Vector3(0.05f, 1.3f, 0.05f);
                            bowStringUpper.GetChild(0).gameObject.SetActive(true);
                            bowStringUpper.GetChild(0).localScale = new Vector3(0.05f, 1.3f, 0.05f);
                        }
                        else
                        {
                            m.GetObject<Transform>(0).SetParent(bowStringUpper, true);
                            m.GetObject<Transform>(1).SetParent(bowStringLower, true);
                        }
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
            if(model == null)
            {
                return;
            }

            Transform[] partsToDropArray = model.PartsToDrop;
            if (!partsToDropArray.IsNullOrEmpty())
            {
                foreach (Transform part in partsToDropArray)
                {
                    if(part != null)
                    part.gameObject.SetActive(value);
                }
            }
        }

        public void SetModelColor(GameObject model, bool fire, float saturation, Color? forceColor = null)
        {
            Renderer renderer = model.GetComponent<Renderer>();
            if (renderer == null || renderer.material == null)
            {
                return;
            }
            Material material = renderer.material;

            Color? color;
            if (forceColor == null)
            {
                color = FirstPersonMover.GetCharacterModel().GetFavouriteColor();
            }
            else
            {
                color = forceColor;
            }

            HSBColor hsbcolor2 = new HSBColor(color.Value)
            {
                b = 1f,
                s = saturation
            };
            material.SetColor("_EmissionColor", hsbcolor2.ToColor() * 2.5f);

            if (model.GetComponent<WeaponSkinFireAnimator>())
            {
                model.GetComponent<WeaponSkinFireAnimator>().TargetColor = hsbcolor2.ToColor();
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

#if DEBUG

        private void Update()
        {
            if (!IsOwnerMainPlayer())
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftAlt))
            {
                Transform model = getTransform();
                if(model == null)
                {
                    return;
                }
                copyVector(model.localPosition);
            }
            if (Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.LeftAlt))
            {
                Transform model = getTransform();
                if (model == null)
                {
                    return;
                }
                copyVector(model.localEulerAngles);
            }
            if (Input.GetKeyDown(KeyCode.C) && Input.GetKey(KeyCode.LeftAlt))
            {
                Transform model = getTransform();
                if (model == null)
                {
                    return;
                }
                copyVector(model.localScale);
            }
        }

        private void copyVector(Vector3 vector)
        {
            string toCopy = vector[0].ToString().Replace(',', '.') + "f, " + vector[1].ToString().Replace(',', '.') + "f, " + vector[2].ToString().Replace(',', '.') + "f";
            TextEditor editor = new TextEditor();
            editor.text = toCopy;
            editor.SelectAll();
            editor.Copy();
        }

        private Transform getTransform()
        {
            if(FirstPersonMover == null || WeaponSkins.IsNullOrEmpty())
            {
                return null;
            }

            WeaponType weaponType = FirstPersonMover.GetEquippedWeaponType();
            if (!WeaponSkinsMenu.SupportedWeapons.Contains(weaponType))
            {
                return null;
            }

            WeaponSkinsController controller = OverhaulController.GetController<WeaponSkinsController>();
            if (controller == null || controller.Interface == null)
            {
                return null;
            }

            string w = null;
            switch (weaponType)
            {
                case WeaponType.Sword:
                    w = WeaponSkinsController.EquippedSwordSkin;
                    break;
                case WeaponType.Hammer:
                    w = WeaponSkinsController.EquippedHammerSkin;
                    break;
                case WeaponType.Bow:
                    w = WeaponSkinsController.EquippedBowSkin;
                    break;
                case WeaponType.Spear:
                    w = WeaponSkinsController.EquippedSpearSkin;
                    break;
            }
            if (string.IsNullOrEmpty(w))
            {
                return null;
            }

            IWeaponSkinItemDefinition item = controller.Interface.GetSkinItem(weaponType, w, ItemFilter.Everything, out _);
            if (item == null)
            {
                return null;
            }

            WeaponSkins.TryGetValue(item, out WeaponSkinSpawnInfo model);
            if (model == null)
            {
                return null;
            }

            return model.Model.transform;
        }
#endif
    }
}