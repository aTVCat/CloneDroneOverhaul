using CDOverhaul.Gameplay;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// UI that lets you freely manage weapon skins
    /// </summary>
    public class UISkinSelection : UIBase
    {
        private ModdedObject _weaponEntry;
        private Transform _weaponContainer;

        private ModdedObject _skinEntry;
        private Transform _skinContainer;

        private ModdedObject _addSkin;
        private ModdedObject _skinAddingWindow;

        private readonly List<UIWeaponEntry> _spawnedWeaponEntries = new List<UIWeaponEntry>();
        private readonly List<UISkinEntry> _spawnedSkinEntries = new List<UISkinEntry>();

        private Transform _bg;

        private Vector3 _initCamPosition;

        public override void Initialize()
        {
            _ = OverhaulEventManager.AddEventListener(GamemodeSubstatesController.SubstateChangedEventString, onGamemodeSubstateUpdated);

            _weaponEntry = MyModdedObject.GetObject<ModdedObject>(0);
            _weaponEntry.gameObject.SetActive(false);
            _weaponContainer = MyModdedObject.GetObject<Transform>(1);

            _addSkin = MyModdedObject.GetObject<ModdedObject>(6);
            _addSkin.gameObject.SetActive(false);
            _skinEntry = MyModdedObject.GetObject<ModdedObject>(4);
            _skinEntry.gameObject.SetActive(false);
            _skinContainer = MyModdedObject.GetObject<Transform>(5);

            _skinAddingWindow = MyModdedObject.GetObject<ModdedObject>(7);
            //_skinAddingWindow.gameObject.AddComponent<DraggableUI>();

            _bg = MyModdedObject.GetObject<Transform>(8);

            MyModdedObject.GetObject<Button>(3).onClick.AddListener(tryWeapon);
            MyModdedObject.GetObject<Button>(2).onClick.AddListener(goBack);

            activate(false);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        /// <summary>
        /// Show all avaialble skins for weapon
        /// </summary>
        /// <param name="entry"></param>
        public void OnSelectWeapon(in UIWeaponEntry entry)
        {
            if (MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == EGamemodeSubstate.WeaponSkinCreation)
            {
                return;
            }
            foreach (UIWeaponEntry entry1 in _spawnedWeaponEntries)
            {
                entry1.VisualizeDeselect();
            }
            entry.VisualizeSelect();
            populateSkins(entry.Weapon);
        }

        /// <summary>
        /// Show skin description and variants
        /// </summary>
        /// <param name="entry"></param>
        public void SelectSkin(in UISkinEntry entry)
        {
            MainGameplayController.Instance.WeaponSkins.ConfirmSkinSelect(entry.WeaponType, entry.SkinName);
        }

        /// <summary>
        /// Populate all skins for weapon
        /// </summary>
        /// <param name="type"></param>
        private void populateSkins(in WeaponType type)
        {
            _spawnedSkinEntries.Clear();
            TransformUtils.DestroyAllChildren(_skinContainer);
            foreach (WeaponSkinModels model in MainGameplayController.Instance.WeaponSkins.GetAllSkins(type))
            {
                ModdedObject clone = Instantiate(_skinEntry, _skinContainer);
                clone.gameObject.SetActive(true);

                UISkinEntry entry = clone.gameObject.AddComponent<UISkinEntry>();
                entry.Initialize(model, this, type);
                _spawnedSkinEntries.Add(entry);
            }
            ModdedObject clone2 = Instantiate(_addSkin, _skinContainer);
            clone2.gameObject.SetActive(true);
            clone2.GetComponent<Button>().onClick.AddListener(startSkinAddingMode);

            FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
            if (m != null)
            {
                m.SetEquippedWeaponType(type, false);
            }
        }

        /// <summary>
        /// Populate all avaiable weapon entries
        /// </summary>
        private void populateWeapons()
        {
            _spawnedWeaponEntries.Clear();
            TransformUtils.DestroyAllChildren(_weaponContainer);
            int index = 0;
            foreach (string str in Enum.GetNames(typeof(WeaponType)))
            {
                if (index != 0 && index != 4 && index <= 6)
                {
                    ModdedObject clone = Instantiate(_weaponEntry, _weaponContainer);
                    clone.GetObject<Text>(1).text = str;
                    clone.gameObject.SetActive(true);

                    UIWeaponEntry entry = clone.gameObject.AddComponent<UIWeaponEntry>();
                    entry.Initialize(clone.GetObject<Transform>(0), clone.GetComponent<Button>(), this, (WeaponType)index);
                    _spawnedWeaponEntries.Add(entry);
                }

                index++;
            }
        }

        /// <summary>
        /// Switch UI visibility
        /// </summary>
        /// <param name="value"></param>
        private void activate(in bool value)
        {
            if (base.gameObject.activeSelf == value)
            {
                return;
            }

            base.gameObject.SetActive(value);

            FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
            if (m != null)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(!value);
            }

            if (value)
            {
                ShowCursor = true;
                populateWeapons();

                if (m != null && m.GetCameraMover() != null)
                {
                    Transform t = m.GetCameraMover().transform.parent.parent;
                    m.PauseRotationSmoothing();
                    t.localEulerAngles = new Vector3(5, 140f, 0);
                    _initCamPosition = t.localPosition;
                    t.localPosition = new Vector3(0, 1.3f, 2);
                }
            }
            else
            {
                ShowCursor = false;

                if (m != null && m.GetCameraMover() != null)
                {
                    Transform t = m.GetCameraMover().transform.parent.parent;
                    t.localEulerAngles = Vector3.zero;
                    t.localPosition = _initCamPosition;
                    m.ResumeRotationSmoothing();
                }
            }
        }

        /// <summary>
        /// Do something when substate of gamemode gets changed
        /// </summary>
        private void onGamemodeSubstateUpdated()
        {
            EGamemodeSubstate s = MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate;
            activate(s == EGamemodeSubstate.WeaponSkinSelection || s == EGamemodeSubstate.WeaponSkinCreation);
            _skinAddingWindow.gameObject.SetActive(s == EGamemodeSubstate.WeaponSkinCreation);
            _bg.gameObject.SetActive(s != EGamemodeSubstate.WeaponSkinCreation);
        }

        /// <summary>
        /// Start adding/changing a skin
        /// </summary>
        private void startSkinAddingMode()
        {
            MainGameplayController.Instance.WeaponSkins.EnterSkinCreationMode();
        }

        /// <summary>
        /// Exit the UI and test the skin
        /// </summary>
        private void tryWeapon()
        {
            if (MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == EGamemodeSubstate.WeaponSkinCreation)
            {
                MainGameplayController.Instance.WeaponSkins.ExitSkinCreationMode();
            }
            MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate = EGamemodeSubstate.None;
        }

        /// <summary>
        /// Exit to main menu
        /// </summary>
        private void goBack()
        {
            SceneTransitionManager.Instance.DisconnectAndExitToMainMenu();
        }
    }
}