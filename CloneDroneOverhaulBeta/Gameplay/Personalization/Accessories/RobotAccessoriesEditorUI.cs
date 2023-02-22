using CDOverhaul.Gameplay;
using OverhaulAPI;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// This editor lets you pose accessories
    /// </summary>
    public class RobotAccessoriesEditorUI : UIBase
    {
        /// <summary>
        /// Check if user may use the editor
        /// </summary>
        public static bool MayUseEditor => OverhaulVersion.IsDebugBuild;

        private Dropdown CharacterModelsDropdown;
        private Dropdown AccessoriesDropdown;

        /// <summary>
        /// Position related input fields
        /// </summary>
        public InputField[] Position = new InputField[3];
        /// <summary>
        /// Rotation related input fields
        /// </summary>
        public InputField[] Rotation = new InputField[3];
        /// <summary>
        /// Scale related input fields
        /// </summary>
        public InputField[] Scale = new InputField[3];

        /// <summary>
        /// Currently editing accessory
        /// </summary>
        public RobotAccessoryBehaviour EditingAccessory;

        /// <summary>
        /// TBA?
        /// </summary>
        public override void Initialize()
        {
            Hide();

            CharacterModelsDropdown = MyModdedObject.GetObject<Dropdown>(14);
            CharacterModelsDropdown.onValueChanged.AddListener(SwitchCharacterModel);

            AccessoriesDropdown = MyModdedObject.GetObject<Dropdown>(15);
            AccessoriesDropdown.onValueChanged.AddListener(SwitchAccessory);

            MyModdedObject.GetObject<Button>(1).onClick.AddListener(SaveAccessoryTransform);
            MyModdedObject.GetObject<Button>(0).onClick.AddListener(SetAccessoryDefaultTransform);
            MyModdedObject.GetObject<Button>(3).onClick.AddListener(Hide);

            Position[0] = MyModdedObject.GetObject<InputField>(5);
            Position[0].text = "0";
            Position[1] = MyModdedObject.GetObject<InputField>(6);
            Position[1].text = "0";
            Position[2] = MyModdedObject.GetObject<InputField>(7);
            Position[2].text = "0";

            Rotation[0] = MyModdedObject.GetObject<InputField>(8);
            Rotation[0].text = "0";
            Rotation[1] = MyModdedObject.GetObject<InputField>(9);
            Rotation[1].text = "0";
            Rotation[2] = MyModdedObject.GetObject<InputField>(10);
            Rotation[2].text = "0";

            Scale[0] = MyModdedObject.GetObject<InputField>(11);
            Scale[0].text = "1";
            Scale[1] = MyModdedObject.GetObject<InputField>(12);
            Scale[1].text = "1";
            Scale[2] = MyModdedObject.GetObject<InputField>(13);
            Scale[2].text = "1";

            HasAddedEventListeners = true;
            HasInitialized = true;
        }

        /// <summary>
        /// TBA
        /// </summary>
        /// <param name="value"></param>
        public void SetActive(in bool value)
        {
            if (value)
            {
                Show();
                return;
            }
            Hide();
        }

        /// <summary>
        /// Show the editor UI
        /// </summary>
        public void Show()
        {
            base.gameObject.SetActive(true);
            ShowCursor = true;

            AccessoriesDropdown.options = RobotAccessoriesController.GetAllAccessoriesDropdownOptions();
            CharacterModelsDropdown.options = MultiplayerCharacterCustomizationManager.Instance.GetCharacterModelDropdownOptions(true);
        }

        /// <summary>
        /// Hide the editor UI
        /// </summary>
        public void Hide()
        {
            base.gameObject.SetActive(false);
            if (MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == EGamemodeSubstate.EditingAccessories)
            {
                MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate = EGamemodeSubstate.None;
            }

            ShowCursor = false;
        }

        /// <summary>
        /// Equip other character model and respawn player
        /// </summary>
        /// <param name="index"></param>
        public void SwitchCharacterModel(int index)
        {
            if (!base.gameObject.activeSelf)
            {
                return;
            }

            SettingsManager.Instance.SetMultiplayerCharacterModelIndex(index);
            SettingsManager.Instance.SetUseSkinInSingleplayer(true);

            GameObject gm = new GameObject("TempSpawnpoint");
            gm.transform.position = CharacterTracker.Instance.GetPlayerTransform().position;
            gm.transform.rotation = CharacterTracker.Instance.GetPlayerTransform().rotation;

            CharacterTracker.Instance.GetPlayerRobot().DestroyIfAlive();
            _ = GameFlowManager.Instance.SpawnPlayer(gm.transform, true, true);

            DelegateScheduler.Instance.Schedule(delegate
            {
                EditingAccessory = CharacterTracker.Instance.GetPlayerTransform().GetComponent<RobotAccessoriesWearer>().GetEquipedAccessory(0);
                UpdateInputFields();
            }, 0.2f);

            Destroy(gm);
        }

        /// <summary>
        /// Equip accessory and respawn player
        /// </summary>
        /// <param name="index"></param>
        public void SwitchAccessory(int index)
        {
            SetAccessory(RobotAccessoriesController.GetAllAccessoriesDropdownOptions()[index].text);
        }

        /// <summary>
        /// TBA
        /// </summary>
        /// <param name="accsName"></param>
        public void SetAccessory(in string accsName)
        {
            RobotAccessoriesController.PlayerData.Accessories.Clear();
            RobotAccessoriesController.PlayerData.Accessories.Add(accsName);

            SwitchCharacterModel(SettingsManager.Instance.GetMultiplayerCharacterModelIndex());

            EditingAccessory = null;
            DelegateScheduler.Instance.Schedule(delegate
            {
                EditingAccessory = CharacterTracker.Instance.GetPlayerTransform().GetComponent<RobotAccessoriesWearer>().GetEquipedAccessory(0);
                UpdateInputFields();
            }, 0.2f);
        }

        /// <summary>
        /// Set default positions on accessory
        /// </summary>
        public void SetAccessoryDefaultTransform()
        {
            if (EditingAccessory == null)
            {
                OverhaulDebugController.PrintError("Editing accessory is NULL!");
                return;
            }

            EditingAccessory.TargetTransform = new SerializeTransform()
            {
                Position = EditingAccessory.Item.TransformInfo.Position,
                EulerAngles = EditingAccessory.Item.TransformInfo.EulerAngles,
                LocalScale = EditingAccessory.Item.TransformInfo.LocalScale,
            };
            SerializeTransform.ApplyOnTransform(EditingAccessory.TargetTransform, EditingAccessory.transform);
            UpdateInputFields();
        }

        /// <summary>
        /// Save transform
        /// </summary>
        public void SaveAccessoryTransform()
        {
            if (EditingAccessory == null)
            {
                OverhaulDebugController.PrintError("Editing accessory is NULL!");
                return;
            }

            EditingAccessory.Item.SaveTransformForAccessory(EditingAccessory);
        }

        /// <summary>
        /// Update all input fields
        /// </summary>
        public void UpdateInputFields()
        {
            if (EditingAccessory == null)
            {
                OverhaulDebugController.PrintError("Editing accessory is NULL!");
                return;
            }

            Position[0].text = EditingAccessory.transform.localPosition.x.ToString();
            Position[1].text = EditingAccessory.transform.localPosition.y.ToString();
            Position[2].text = EditingAccessory.transform.localPosition.z.ToString();

            Rotation[0].text = EditingAccessory.transform.localEulerAngles.x.ToString();
            Rotation[1].text = EditingAccessory.transform.localEulerAngles.y.ToString();
            Rotation[2].text = EditingAccessory.transform.localEulerAngles.z.ToString();

            Scale[0].text = EditingAccessory.transform.localScale.x.ToString();
            Scale[1].text = EditingAccessory.transform.localScale.y.ToString();
            Scale[2].text = EditingAccessory.transform.localScale.z.ToString();
        }

        private void Update()
        {
            if (EditingAccessory != null && Time.frameCount % 30 == 0)
            {
                _ = float.TryParse(Position[0].text, out float px);
                _ = float.TryParse(Position[1].text, out float py);
                _ = float.TryParse(Position[2].text, out float pz);
                _ = float.TryParse(Rotation[0].text, out float rx);
                _ = float.TryParse(Rotation[1].text, out float ry);
                _ = float.TryParse(Rotation[2].text, out float rz);
                _ = float.TryParse(Scale[0].text, out float sx);
                _ = float.TryParse(Scale[1].text, out float sy);
                _ = float.TryParse(Scale[2].text, out float sz);

                EditingAccessory.TargetTransform.Position = new Vector3(px, py, pz);
                EditingAccessory.TargetTransform.EulerAngles = new Vector3(rx, ry, rz);
                EditingAccessory.TargetTransform.LocalScale = new Vector3(sx, sy, sz);
            }
        }
    }
}
