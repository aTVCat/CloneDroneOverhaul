using AmplifyOcclusion;
using CloneDroneOverhaul.UI.Components;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewPhotoModeUI : ModGUIBase
    {
        public static NewPhotoModeUI Instance;
        private bool _showDebug;
        private BetterSlider _camTilt;
        private BetterSlider _camZoom;
        private ToggleWithDesc _showHUD;
        private ToggleWithDesc _overrideVisuals;
        private List<GarbageTarget> garbageTargets = new List<GarbageTarget>();

        private string _curPage = "Robots";
        private int _curPageInt;

        private string[] _pages = new string[]
        {
            "Robots",
            "Bloom",
            "Amplify Occlusion",
            "Noise",
            "Shadows"
        };
        private Dictionary<string, GameObject[]> _effects = new Dictionary<string, GameObject[]>();

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            Hide();

            _camTilt = MyModdedObject.GetObjectFromList<ModdedObject>(2).AddAndConfigBetterSlider(new BetterSlider.Settings()
            {
                UseInt = true,
                MinValue = 0,
                MaxValue = 360
            }, setCamTilt);
            _camZoom = MyModdedObject.GetObjectFromList<ModdedObject>(3).AddAndConfigBetterSlider(new BetterSlider.Settings()
            {
                UseInt = true,
                MinValue = 1,
                MaxValue = 180
            }, setCamZoom);
            _showHUD = MyModdedObject.GetObjectFromList<ModdedObject>(11).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, refreshHUD);
            _overrideVisuals = MyModdedObject.GetObjectFromList<ModdedObject>(17).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, setOverrideVisuals);

            _effects.Add(_pages[0], new GameObject[]
            {
                MyModdedObject.GetObjectFromList<ModdedObject>(12).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, refreshPlayerModel).gameObject,
                     MyModdedObject.GetObjectFromList<ModdedObject>(13).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, refreshEnemiesModel).gameObject,
                          MyModdedObject.GetObjectFromList<ModdedObject>(14).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, setGarbageVisible).gameObject,
                                  MyModdedObject.GetObjectFromList<ModdedObject>(19).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, refreshProjectiles).gameObject
            });

            _effects.Add(_pages[1], new GameObject[]
            {
                    MyModdedObject.GetObjectFromList<ModdedObject>(22).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, null).gameObject,
                         MyModdedObject.GetObjectFromList<ModdedObject>(23).AddAndConfigBetterSlider(new BetterSlider.Settings()
                         {
                              UseInt = true,
                              MinValue = 1,
                              MaxValue = 10
                         }, null).gameObject,
                              MyModdedObject.GetObjectFromList<ModdedObject>(24).AddAndConfigBetterSlider(new BetterSlider.Settings()
                         {
                              UseInt = false,
                              MinValue = 0.1f,
                              MaxValue = 3f
                         }, null).gameObject,
                                   MyModdedObject.GetObjectFromList<ModdedObject>(25).AddAndConfigBetterSlider(new BetterSlider.Settings()
                         {
                              UseInt = false,
                              MinValue = 0.1f,
                              MaxValue = 10f
                         }, null).gameObject,
            });

            _effects.Add(_pages[2], new GameObject[]
            {
                MyModdedObject.GetObjectFromList<ModdedObject>(16).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, setAOEnabled).gameObject,
                MyModdedObject.GetObjectFromList<ModdedObject>(18).AddAndConfigBetterSlider(new BetterSlider.Settings()
                {
                MinValue = 0.1f,
                MaxValue = 3f
                }, setAOIntensity).gameObject
            });

            _effects.Add(_pages[3], new GameObject[]
           {
                MyModdedObject.GetObjectFromList<ModdedObject>(20).gameObject.AddComponent<ToggleWithDesc>().SetUp(null, setNoiseEnabled).gameObject,
                         MyModdedObject.GetObjectFromList<ModdedObject>(21).AddAndConfigBetterSlider(new BetterSlider.Settings()
                         {
                              UseInt = false,
                              MinValue = 0.5f,
                              MaxValue = 3f
                         }, setNoiseIntensity).gameObject,
           });

            _effects.Add(_pages[4], new GameObject[]
           {

           });

            MyModdedObject.GetObjectFromList<Button>(5).onClick.AddListener(nextPage);
            MyModdedObject.GetObjectFromList<Button>(6).onClick.AddListener(prevPage);

            Instance = this;
        }
        public override void OnNewFrame()
        {
            if (base.gameObject.activeSelf)
            {
                GameUIRoot.Instance.RefreshCursorEnabled();

                if (Input.GetKeyDown(KeyCode.C))
                {
                    MyModdedObject.GetObjectFromList<Transform>(15).gameObject.SetActive(!MyModdedObject.GetObjectFromList<Transform>(15).gameObject.activeSelf);
                }
            }
        }
        public void Show()
        {
            base.gameObject.SetActive(true);

            foreach (string str in _effects.Keys)
            {
                foreach (GameObject gObj in _effects[str])
                {
                    gObj.SetActive(false);
                }
            }

            refreshDebugInfoEnabled();
            refreshCameraSettings();
            refreshGameplayInportantThings();
            setActivePage(_pages[0]);
            MyModdedObject.GetObjectFromList<Transform>(15).gameObject.SetActive(true);
            _overrideVisuals.SetValue(true);


        }
        public void Hide()
        {
            base.gameObject.SetActive(false);
            refreshHUD(!CutSceneManager.Instance.IsInCutscene());
            if (Time.timeSinceLevelLoad > 3)
            {
                refreshGameplayInportantThings();
                _overrideVisuals.SetValue(false);
            }
        }
        public bool ShouldShowCursor()
        {
            return base.gameObject.activeSelf && MyModdedObject.GetObjectFromList<Transform>(15).gameObject.activeSelf && !Input.GetMouseButton(1);
        }


        private void setActivePage(string name)
        {
            foreach (GameObject gObj in _effects[_curPage])
            {
                gObj.SetActive(false);
            }
            _curPage = name;
            MyModdedObject.GetObjectFromList<Text>(4).text = name;
            foreach (GameObject gObj in _effects[_curPage])
            {
                gObj.SetActive(true);
            }
        }
        private void nextPage()
        {
            _curPageInt++;
            if (_curPageInt == _pages.Length)
            {
                _curPageInt = 0;
            }
            setActivePage(_pages[_curPageInt]);
        }
        private void prevPage()
        {
            _curPageInt--;
            if (_curPageInt == -1)
            {
                _curPageInt = _pages.Length - 1;
            }
            setActivePage(_pages[_curPageInt]);
        }
        private void setOverrideVisuals(bool val)
        {
            OverhaulMain.Visuals.OverrideSettings = val;
            OverhaulMain.Visuals.RefreshVisuals();
        }

        //
        //
        //
        private void refreshDebugInfoEnabled()
        {
            bool isEnabled = GUIManagement.Instance.GetGUI<NewEscMenu>().PhotoModeDebugInfo.isOn;
            _showDebug = isEnabled;

            MyModdedObject.GetObjectFromList<RectTransform>(10).gameObject.SetActive(isEnabled);
        }
        private void refreshCameraSettings()
        {
            _camTilt.SetValue(0);
            _camZoom.SetValue(60);
            _showHUD.SetValue(!CutSceneManager.Instance.IsInCutscene());
        }
        private void refreshHUD(bool val)
        {
            if (Time.timeSinceLevelLoad > 3)
            {
                GameUIRoot.Instance.SetPlayerHUDVisible(val);
            }
        }
        private void refreshGameplayInportantThings()
        {
            if (!OverhaulDescription.TEST_FEATURES_ENABLED)
            {
                return;
            }
            MyModdedObject.GetObjectFromList<ToggleWithDesc>(12).SetValue(true);
            MyModdedObject.GetObjectFromList<ToggleWithDesc>(13).SetValue(true);
            MyModdedObject.GetObjectFromList<ToggleWithDesc>(14).SetValue(true);
            MyModdedObject.GetObjectFromList<ToggleWithDesc>(19).SetValue(true);
        }

        //
        //
        //
        private void setCamZoom(float val)
        {
            FlyingCameraController cntrl = PhotoManager.Instance.GetCurrentFlyingCameraController();
            if (cntrl != null)
            {
                Camera cam = cntrl.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    cam.fieldOfView = val;
                }
            }
        }
        private void setCamTilt(float val)
        {
            FlyingCameraController cntrl = PhotoManager.Instance.GetCurrentFlyingCameraController();
            if (cntrl != null)
            {
                Camera cam = cntrl.GetComponentInChildren<Camera>();
                if (cam != null)
                {
                    cam.transform.localEulerAngles = new Vector3(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, val);
                }
            }
        }
        private void setAOEnabled(bool val)
        {
            OverhaulMain.Visuals.Override_AOEnabled = val;
            if (val)
            {
                OverhaulMain.Visuals.Override_AOIntensity = 1f;
                OverhaulMain.Visuals.Override_AOSampleCount = (int)SampleCountLevel.Medium;
            }
            OverhaulMain.Visuals.RefreshVisuals();
        }
        private void setAOIntensity(float val)
        {
            OverhaulMain.Visuals.Override_AOIntensity = val;
            OverhaulMain.Visuals.RefreshVisuals();
        }
        private void setNoiseEnabled(bool val)
        {
            OverhaulMain.Visuals.Override_NoiseEnabled = val;
            OverhaulMain.Visuals.RefreshVisuals();
        }
        private void setNoiseIntensity(float val)
        {
            OverhaulMain.Visuals.Override_NoiseMultipler = val;
            OverhaulMain.Visuals.RefreshVisuals();
        }
        private void setGarbageVisible(bool val)
        {
            if (!val)
            {
                garbageTargets = GarbageManager.Instance.GetAllGarbageReadyForCollection();
            }
            foreach (GarbageTarget t in garbageTargets)
            {
                t.gameObject.SetActive(val);
            }
        }
        private void refreshPlayerModel(bool show = true) // Code was copied from v0.1.5
        {
            bool flag = !show;
            if (flag)
            {
                bool flag2 = Singleton<CharacterTracker>.Instance.GetPlayerRobot() != null && Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetCharacterModel() != null;
                if (flag2)
                {
                    Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetCharacterModel().HideAllBodyPartsandArmor();
                    bool flag3 = Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetEquippedWeaponModel() != null;
                    if (flag3)
                    {
                        Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetEquippedWeaponModel().gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                bool flag4 = Singleton<CharacterTracker>.Instance.GetPlayerRobot() != null && Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetCharacterModel() != null;
                if (flag4)
                {
                    Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetCharacterModel().ShowAllHiddenBodyPartsAndArmor();
                    bool flag5 = Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetEquippedWeaponModel() != null;
                    if (flag5)
                    {
                        Singleton<CharacterTracker>.Instance.GetPlayerRobot().GetEquippedWeaponModel().gameObject.SetActive(true);
                    }
                }
            }
        }
        private void refreshEnemiesModel(bool show = true)
        {
            bool flag = Singleton<CharacterTracker>.Instance.GetAllLivingCharacters() == null;
            if (!flag)
            {
                foreach (Character character in Singleton<CharacterTracker>.Instance.GetAllLivingCharacters())
                {
                    bool flag2 = character is FirstPersonMover && !character.IsMainPlayer() && (character as FirstPersonMover).GetCharacterModel() != null;
                    if (flag2)
                    {
                        bool flag3 = (character as FirstPersonMover).GetEquippedWeaponModel() != null;
                        if (flag3)
                        {
                            (character as FirstPersonMover).GetEquippedWeaponModel().gameObject.SetActive(show);
                        }
                        bool flag4 = !show;
                        if (flag4)
                        {
                            (character as FirstPersonMover).GetCharacterModel().HideAllBodyPartsandArmor();
                        }
                        else
                        {
                            (character as FirstPersonMover).GetCharacterModel().ShowAllHiddenBodyPartsAndArmor();
                        }
                    }
                }
            }
        }

        private void refreshProjectiles(bool show = true)
        {
            bool flag = Singleton<ProjectileManager>.Instance != null && Singleton<ProjectileManager>.Instance.ArrowPool.GetAllActiveObjects() != null;
            if (flag)
            {
                foreach (Transform transform in Singleton<ProjectileManager>.Instance.ArrowPool.GetAllActiveObjects())
                {
                    bool flag2 = transform != null;
                    if (flag2)
                    {
                        transform.gameObject.SetActive(show);
                    }
                }
            }
        }
    }

    public struct EffectInfo
    {
        public string Name;

    }
}
