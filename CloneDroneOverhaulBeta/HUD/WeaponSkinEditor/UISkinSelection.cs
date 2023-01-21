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

        private List<UIWeaponEntry> _spawnedWeaponEntries = new List<UIWeaponEntry>();
        private List<UISkinEntry> _spawnedSkinEntries = new List<UISkinEntry>();

        private Vector3 _initCamPosition;

        public override void Initialize()
        {
            OverhaulEventManager.AddListenerToEvent(GamemodeSubstatesController.SubstateChangedEventString, onGamemodeSubstateUpdated);

            _weaponEntry = MyModdedObject.GetObject<ModdedObject>(0);
            _weaponEntry.gameObject.SetActive(false);
            _weaponContainer = MyModdedObject.GetObject<Transform>(1);

            _skinEntry = MyModdedObject.GetObject<ModdedObject>(4);
            _skinEntry.gameObject.SetActive(false);
            _skinContainer = MyModdedObject.GetObject<Transform>(5);

            MyModdedObject.GetObject<Button>(3).onClick.AddListener(tryWeaponOut);
            setActive(false);

            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        /// <summary>
        /// Show all avaialble skins for weapon
        /// </summary>
        /// <param name="entry"></param>
        public void OnSelectWeapon(in UIWeaponEntry entry)
        {
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
        public void OnSelectSkin(in UISkinEntry entry)
        {
            MainGameplayController.Instance.WeaponSkins.ConfirmSkinSelect(entry.WeaponType, entry.SkinName);
        }

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

            FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
            if (m != null)
            {
                m.SetEquippedWeaponType(type, false);
            }
        }

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

        private void setActive(in bool value)
        {
            base.gameObject.SetActive(value);

            if (value)
            {
                EnableCursorConditionID = EnableCursorController.AddCondition();
                populateWeapons();

                FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
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
                EnableCursorController.RemoveCondition(EnableCursorConditionID);

                FirstPersonMover m = CharacterTracker.Instance.GetPlayerRobot();
                if (m != null && m.GetCameraMover() != null)
                {
                    Transform t = m.GetCameraMover().transform.parent.parent;
                    t.localEulerAngles = Vector3.zero;
                    t.localPosition = _initCamPosition;
                    m.ResumeRotationSmoothing();
                }
            }
        }

        private void onGamemodeSubstateUpdated()
        {
            setActive(MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate == EGamemodeSubstate.WeaponSkinSelection);
        }

        private void tryWeaponOut()
        {
            MainGameplayController.Instance.GamemodeSubstates.GamemodeSubstate = EGamemodeSubstate.None;
        }
    }
}