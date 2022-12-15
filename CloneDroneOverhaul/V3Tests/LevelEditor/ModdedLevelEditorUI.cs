using CloneDroneOverhaul.UI;
using ModLibrary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.LevelEditor
{
    public class ModdedLevelEditorUI : ModGUIBase
    {
        public static ModdedLevelEditorUI Instance;
        public static Color NormalColor
        {
            get
            {
                Color result;
                ColorUtility.TryParseHtmlString("#31373F", out result);
                return result; //286ea03e-b667-46ae-8c12-95eb08c412e4
            }
        }
        public static Color SelectedColor
        {
            get
            {
                Color result;
                ColorUtility.TryParseHtmlString("#5B6D85", out result);
                return result;
            }
        }

        public class UIPanel : MonoBehaviour
        {
            public ModdedObject ModdedObj;

            public T SetUp<T>() where T : UIPanel
            {
                ModdedObj = base.GetComponent<ModdedObject>();
                Config();
                return (T)this;
            }

            protected virtual void Config()
            {

            }
        }
        public class ObjectsSelectedPanel : UIPanel
        {
            public Text ObjectsSelected;
            public Text AdditionalText;

            protected override void Config()
            {
                ObjectsSelected = ModdedObj.GetObjectFromList<Text>(0);
                AdditionalText = ModdedObj.GetObjectFromList<Text>(1);
                base.gameObject.SetActive(false);
            }
        }
        public class Toolbar : UIPanel
        {
            public void RefreshSelected()
            {
                foreach (GameObject obj in ModdedObj.objects)
                {
                    obj.GetComponent<Image>().color = ModdedLevelEditorUI.NormalColor;
                }
                ModdedObj.GetObjectFromList<Image>((int)LevelEditorToolManager.Instance.GetActiveToolType()).color = ModdedLevelEditorUI.SelectedColor;

            }

            protected override void Config()
            {
                RefreshSelected();
            }
        }
        public class UpperToolbar : UIPanel
        {
            protected override void Config()
            {
                base.ModdedObj.GetObjectFromList<Button>(0).onClick.AddListener(openLevelsMenu);
            }

            private void openLevelsMenu()
            {
                ModdedLevelEditorUI.Instance.Menu.ShowMenu();
            }
        }
        public class LTEM_Toolbar : UIPanel
        {
            public void RefreshSelected()
            {
                base.gameObject.SetActive(OverhaulDescription.LevelEditorToolsEnabled());
            }
            protected override void Config()
            {
                RefreshSelected();
            }
        }
        public class Inspector : UIPanel
        {
            private bool _isUpdatingValues;
            private ObjectPlacedInLevel _selected;

            protected override void Config()
            {
                for (int i = 0; i < 9; i++)
                {
                    ModdedObj.GetObjectFromList<InputField>(i).onValueChanged.AddListener(onTransformValuesUpdated);
                }
            }

            public void Populate(ObjectPlacedInLevel placedObject)
            {
                _isUpdatingValues = true;
                _selected = placedObject;
                ModdedObj.GetObjectFromList<InputField>(0).text = (Mathf.Round(placedObject.transform.localPosition.x * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(1).text = (Mathf.Round(placedObject.transform.localPosition.y * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(2).text = (Mathf.Round(placedObject.transform.localPosition.z * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(3).text = (Mathf.Round(placedObject.transform.localEulerAngles.x * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(4).text = (Mathf.Round(placedObject.transform.localEulerAngles.y * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(5).text = (Mathf.Round(placedObject.transform.localEulerAngles.z * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(6).text = (Mathf.Round(placedObject.transform.localScale.x * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(7).text = (Mathf.Round(placedObject.transform.localScale.y * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                ModdedObj.GetObjectFromList<InputField>(8).text = (Mathf.Round(placedObject.transform.localScale.z * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                _isUpdatingValues = false;
            }
            public void OnDeslected(ObjectPlacedInLevel placedObject)
            {
                _selected = null;
            }

            private void onTransformValuesUpdated(string str)
            {
                if (_isUpdatingValues)
                {
                    return;
                }

                float num;
                float num2;
                float num3;
                _selected.OnPositionAboutToChange();
                _selected.OnRotationAboutToChange();
                _selected.OnScaleAboutToChange();

                if (float.TryParse(ModdedObj.GetObjectFromList<InputField>(0).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(1).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num2) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(2).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
                {
                    _selected.transform.localPosition = new Vector3(num, num2, num3);
                }
                if (float.TryParse(ModdedObj.GetObjectFromList<InputField>(3).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(4).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num2) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(5).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
                {
                    _selected.transform.localEulerAngles = new Vector3(num, num2, num3);
                }
                if (float.TryParse(ModdedObj.GetObjectFromList<InputField>(6).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(7).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num2) && float.TryParse(ModdedObj.GetObjectFromList<InputField>(8).text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out num3))
                {
                    _selected.transform.localScale = new Vector3(num, num2, num3);
                }

                _selected.OnPositionChanged();
                _selected.OnRotationChanged();
                _selected.OnScaleChanged();
            }
        }

        public class LibraryUI : UIPanel
        {
            protected override void Config()
            {
                base.ModdedObj.GetObjectFromList<Button>(0).onClick.AddListener(openLibrary);
                base.ModdedObj.GetObjectFromList<Button>(1).onClick.AddListener(closeLibrary);
                closeLibrary();
            }

            void openLibrary()
            {
                base.ModdedObj.gameObject.SetActive(true);
            }
            void closeLibrary()
            {
                base.ModdedObj.gameObject.SetActive(false);
            }
        }


        public class LevelsMenu : UIPanel
        {
            public TabButton[] Tabs;
            public TabPage[] Pages;
            public SelectedLevelControlsDisplay SelectedLevelDisplay;

            public string GetLevelsFolder()
            {
                return Application.persistentDataPath + "/LevelEditorLevels/";
            }

            protected override void Config()
            {
                Tabs = new TabButton[]
                {
                     base.ModdedObj.GetObjectFromList<Button>(0).gameObject.AddComponent<TabButton>().SetID("levels"),
                        base.ModdedObj.GetObjectFromList<Button>(1).gameObject.AddComponent<TabButton>().SetID("challenges"),
                           base.ModdedObj.GetObjectFromList<Button>(2).gameObject.AddComponent<TabButton>().SetID("adventures"),
                              base.ModdedObj.GetObjectFromList<Button>(3).gameObject.AddComponent<TabButton>().SetID("upload"),
                };
                Pages = new TabPage[]
                {
                    new TabPage() { ID = "levels", PageTransform = base.ModdedObj.GetObjectFromList<Transform>(9) },
                     new TabPage() { ID = "challenges", PageTransform = base.ModdedObj.GetObjectFromList<Transform>(10) },
                      new TabPage() { ID = "adventures", PageTransform = base.ModdedObj.GetObjectFromList<Transform>(10) },
                       new TabPage() { ID = "upload", PageTransform = base.ModdedObj.GetObjectFromList<Transform>(11) }
                };

                SelectedLevelDisplay = base.ModdedObj.GetObjectFromList<Transform>(9).gameObject.AddComponent<SelectedLevelControlsDisplay>();

                base.ModdedObj.GetObjectFromList<Button>(4).onClick.AddListener(CloseMenu);
                base.ModdedObj.GetObjectFromList<Button>(7).onClick.AddListener(delegate
                {
                    FileManagerStuff.OpenFolder(Application.persistentDataPath + "/LevelEditorLevels/");
                });
            }

            public void ShowMenu()
            {
                base.ModdedObj.gameObject.SetActive(true);
                OnSelectedTab(Tabs[0]);
            }
            public void CloseMenu()
            {
                base.ModdedObj.gameObject.SetActive(false);
            }
            public void PopulateEntries(EntryType type)
            {
                TransformUtils.DestroyAllChildren(base.ModdedObj.GetObjectFromList<Transform>(14));

                if (type == EntryType.LevelFolder)
                {
                    PopulateFolder((LevelEditorFilesManager.Instance.GetRootDataPath() + "/" + LevelEditorFilesManager.Instance.LocalLevelsFolder).Replace("/", "\\"), base.ModdedObj.GetObjectFromList<Transform>(14));
                }
            }

            public void RefreshContainerGameObject(GameObject obj)
            {
                obj.GetComponent<ContentSizeFitter>().CallPrivateMethod("SetDirty");
            }

            public List<Transform> PopulateFolder(string path, Transform parent)
            {
                List<Transform> result = new List<Transform>();

                List<string> allFiles = Directory.GetDirectories(path).ToList();
                List<string> files = Directory.GetFiles(path).ToList();
                foreach (string str in files)
                {
                    allFiles.Add(str);
                }

                foreach (string file in allFiles)
                {
                    LevelsFileBase entry = null;
                    if (Directory.Exists(file))
                    {
                        FolderWithLevels folder = Instantiate(ModdedObj.GetObjectFromList<Transform>(12), ModdedObj.GetObjectFromList<Transform>(14)).gameObject.AddComponent<FolderWithLevels>();
                        folder.transform.SetSiblingIndex(parent.GetSiblingIndex() + 1);
                        folder.MakeUIFolder(file);
                        folder.gameObject.SetActive(true);

                        if (parent.name != "Content")
                        {
                            folder.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                        }

                        entry = folder;
                    }
                    else if (file.EndsWith(".json"))
                    {
                        UILevelEntry folder = Instantiate(ModdedObj.GetObjectFromList<Transform>(15), ModdedObj.GetObjectFromList<Transform>(14)).gameObject.AddComponent<UILevelEntry>();
                        folder.transform.SetSiblingIndex(parent.GetSiblingIndex() + 1);
                        folder.MakeLevelEntry(file);
                        folder.gameObject.SetActive(true);

                        entry = folder;
                    }
                    if (entry != null) result.Add(entry.transform);
                }

                RefreshContainerGameObject(base.ModdedObj.GetObjectFromList<Transform>(14).gameObject);

                return result;
            }

            public void OnSelectedTab(TabButton button)
            {
                foreach (TabPage page in Pages)
                {
                    page.PageTransform.gameObject.SetActive(false);
                }
                foreach (TabPage page in Pages)
                {
                    if (page.ID == button.ID)
                    {
                        page.PageTransform.gameObject.SetActive(true);
                    }
                }
                foreach (TabButton button2 in Tabs)
                {
                    button2.SetDeselected();
                }
                foreach (TabButton button2 in Tabs)
                {
                    if (button2 == button)
                    {
                        button2.SetSelected();
                    }
                }
                if (button.ID == "levels")
                {
                    PopulateEntries(EntryType.LevelFolder);
                }
                if (button.ID == "challenges")
                {
                    PopulateEntries(EntryType.Challenge);
                }
                if (button.ID == "adventures")
                {
                    PopulateEntries(EntryType.Adventure);
                }
            }

            private static bool FolderContainsFile(string folderPath, LevelFileEntry levelFileEntry)
            {
                return levelFileEntry.PathUnderLevelsFolder.StartsWith(folderPath + "/") && LevelEditorFilesManager.GetPathDepth(folderPath) + 1 == LevelEditorFilesManager.GetPathDepth(levelFileEntry.PathUnderLevelsFolder);
            }

            public enum EntryType
            {
                LevelFolder,
                Level,
                Challenge,
                Adventure
            }

            public class LevelsFileBase : MonoBehaviour
            {
                public string File;

                protected virtual void OnClick()
                {

                }
            }
            public class FolderWithLevels : LevelsFileBase
            {
                List<Transform> _myEntries;

                public FolderWithLevels MakeUIFolder(string path)
                {
                    File = path;
                    if (path.Contains("/"))
                    {
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path;
                        string text = base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text;
                        text = text.Substring(text.LastIndexOf("/") + 1);
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = text;
                    }
                    else
                    {
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path;
                        string text = base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text;
                        text = text.Substring(text.LastIndexOf(@"\") + 1);
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = text;
                    }
                    base.GetComponent<ModdedObject>().GetObjectFromList<RectTransform>(2).gameObject.SetActive(false);
                    return this;
                }

                void Awake()
                {
                    base.GetComponent<ModdedObject>().GetObjectFromList<Button>(3).onClick.AddListener(OnClick);
                    base.GetComponent<ModdedObject>().GetObjectFromList<Image>(3).color = BaseUtils.ColorFromHex("#404448");
                }

                void OnDestroy()
                {
                    if (_myEntries != null && _myEntries.Count > 0)
                    {
                        this.SetIsOpen(false);
                    }
                }

                public void PutLevelIntoFolder(GameObject obj)
                {
                    obj.transform.SetParent(base.GetComponent<ModdedObject>().GetObjectFromList<Transform>(2));
                }

                public void SetIsOpen(bool refresh) // #404448 non opened  #42505E opened
                {
                    if (_myEntries == null || _myEntries.Count == 0)
                    {
                        _myEntries = ModdedLevelEditorUI.Instance.Menu.PopulateFolder(File, this.transform);
                        base.GetComponent<ModdedObject>().GetObjectFromList<Image>(3).color = BaseUtils.ColorFromHex("#42505E");
                    }
                    else
                    {
                        foreach (Transform t in _myEntries)
                        {
                            Destroy(t.gameObject);
                        }
                        _myEntries.Clear();
                        base.GetComponent<ModdedObject>().GetObjectFromList<Image>(3).color = BaseUtils.ColorFromHex("#404448");
                    }
                    if (refresh) ModdedLevelEditorUI.Instance.Menu.RefreshContainerGameObject(ModdedLevelEditorUI.Instance.Menu.ModdedObj.GetObjectFromList<Transform>(14).gameObject);
                }

                protected override void OnClick()
                {
                    SetIsOpen(true);
                }
            }
            public class UILevelEntry : LevelsFileBase
            {
                public UILevelEntry MakeLevelEntry(string path)
                {
                    File = path;
                    if (path.Contains("/"))
                    {
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path.Substring(path.LastIndexOf("/") + 1).Replace(".json", string.Empty);
                    }
                    else
                    {
                        base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path.Substring(path.LastIndexOf(@"\") + 1).Replace(".json", string.Empty);
                    }
                    return this;
                }

                void Awake()
                {
                    base.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        ModdedLevelEditorUI.Instance.Menu.SelectedLevelDisplay.SelectLevel(File);
                    });
                }
            }

            public class TabPage
            {
                public Transform PageTransform;
                public string ID;
            }

            public class TabButton : MonoBehaviour
            {
                public string ID;
                public TabButton SetID(string id)
                {
                    ID = id;
                    return this;
                }

                public void SetSelected()
                {
                    base.GetComponent<ModdedObject>().GetObjectFromList<RectTransform>(0).gameObject.SetActive(true);
                }

                public void SetDeselected()
                {
                    base.GetComponent<ModdedObject>().GetObjectFromList<RectTransform>(0).gameObject.SetActive(false);
                }

                void Awake()
                {
                    base.GetComponent<Button>().onClick.AddListener(select);
                }

                void select()
                {
                    ModdedLevelEditorUI.Instance.Menu.OnSelectedTab(this);
                }
            }

            public class SelectedLevelControlsDisplay : MonoBehaviour
            {
                public string SelectedLevelPath;

                void Awake()
                {
                    base.gameObject.AddComponent<LevelThumbnailSelector>();

                    ModdedObject obj = base.GetComponent<ModdedObject>();

                }

                /// <summary>
                /// Called, when user select a level
                /// </summary>
                /// <param name="levelPath"></param>
                public void SelectLevel(string levelPath)
                {
                    SelectedLevelPath = levelPath;

                    ModdedObject obj = base.GetComponent<ModdedObject>();
                    obj.GetObjectFromList<InputField>(8).text = levelPath.Substring(levelPath.LastIndexOf("\\") + 1).Replace(".json", string.Empty);
                    obj.GetObjectFromList<Button>(5).onClick.AddListener(delegate
                    {
                        EditSelectedLevel();
                    });
                }

                /// <summary>
                /// Opens currently selected level
                /// </summary>
                public void EditSelectedLevel()
                {
                    if (!string.IsNullOrEmpty(SelectedLevelPath) && File.Exists(SelectedLevelPath))
                    {
                        SceneTransitionController_OLD.SpawnTransitionScreen(delegate
                        {
                            ModdedLevelEditorUI.Instance.Menu.CloseMenu();
                            LevelEditorLevelData data = BaseUtils.TryLoad<LevelEditorLevelData>(SelectedLevelPath);
                            LevelEditorDataManager.Instance.SetPrivateField<LevelEditorLevelData>("_currentLevelData", data);
                            LevelEditorDataManager.Instance.CallPrivateMethod("PopulateCurrentLevel");
                        }, "Loading level...", SelectedLevelPath);
                    }
                }
            }

            public class LevelThumbnailSelector : MonoBehaviour
            {
                public Image Thumbnail;
                public Text ThumbnailPath;
                public Text ErrorLabel;
                public Button BrowseImage;

                void Awake()
                {
                    ModdedObject mObj = base.GetComponent<ModdedObject>();
                    Thumbnail = mObj.GetObjectFromList<Image>(0);
                    ThumbnailPath = mObj.GetObjectFromList<Text>(2);
                    ErrorLabel = mObj.GetObjectFromList<Text>(3);
                    ErrorLabel.gameObject.SetActive(false);
                    BrowseImage = mObj.GetObjectFromList<Button>(4);
                    BrowseImage.onClick.AddListener(delegate
                    {
                        this.onFileSelected(FileManagerStuff.OpenFileSelect("PNG files (*.png)|*.png|JPG files (*.jpg*)|*.jpg*|JPEG files (*.jpeg*)|*.jpeg*"));
                    });
                }

                void onFileSelected(string file)
                {
                    ErrorLabel.gameObject.SetActive(false);
                    if (string.IsNullOrEmpty(file))
                    {
                        ErrorLabel.gameObject.SetActive(true);
                        ErrorLabel.text = "Error: file selection was canceled";
                        return;
                    }
                    ThumbnailPath.text = "Path: " + file;
                    BaseUtils.ImageUtils.LoadSpriteFromFile(file, delegate (Sprite s)
                    {
                        Thumbnail.sprite = s;
                    });
                }
            }
        }

        public ObjectsSelectedPanel ObjectsSelected;
        public Toolbar ToolBar;
        public UpperToolbar UpperToolBar;
        public LTEM_Toolbar LTEM_ToolBar;
        public Inspector InspectorPanel;
        public LibraryUI LibraryPanel;
        public LevelsMenu Menu;

        public override void OnInstanceStart()
        {
            base.gameObject.SetActive(false);
            MyModdedObject = base.GetComponent<ModdedObject>();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(GameUIRoot.Instance.LevelEditorUI.OnPlayButtonClicked);
            ObjectsSelected = MyModdedObject.GetObjectFromList<RectTransform>(1).gameObject.AddComponent<ObjectsSelectedPanel>().SetUp<ObjectsSelectedPanel>();
            ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(2).gameObject.AddComponent<Toolbar>().SetUp<Toolbar>();
            LTEM_ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(3).gameObject.AddComponent<LTEM_Toolbar>().SetUp<LTEM_Toolbar>();
            InspectorPanel = MyModdedObject.GetObjectFromList<RectTransform>(4).gameObject.AddComponent<Inspector>().SetUp<Inspector>();
            LibraryPanel = MyModdedObject.GetObjectFromList<RectTransform>(5).gameObject.AddComponent<LibraryUI>().SetUp<LibraryUI>();
            Menu = MyModdedObject.GetObjectFromList<RectTransform>(6).gameObject.AddComponent<LevelsMenu>().SetUp<LevelsMenu>();
            UpperToolBar = MyModdedObject.GetObjectFromList<RectTransform>(7).gameObject.AddComponent<UpperToolbar>().SetUp<UpperToolbar>();

            Instance = this;

            base.gameObject.AddComponent<UIManager>();
        }

        public override void OnManagedUpdate()
        {
            LTEM_ToolBar.RefreshSelected();
            List<ObjectPlacedInLevel> list = LevelEditorObjectPlacementManager.Instance.GetSelectedSceneObjects();
            if (LevelEditorToolManager.Instance.IsMouseOverAnyTool() && list.Count > 0)
            {
                InspectorPanel.Populate(list[0]);
            }
        }

        public override void OnNewFrame()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!Menu.gameObject.activeSelf)
                {
                    Menu.ShowMenu();
                }
                else
                {
                    Menu.CloseMenu();
                }
            }
        }

        public void Show()
        {
            GameUIRoot.Instance.LevelEditorUI.SetPrivateField<RectTransform>("_rectTransform", GameUIRoot.Instance.LevelEditorUI.GetComponent<RectTransform>());
            OverhaulMain.Timer.AddNoArgAction(delegate
            {
                foreach (Camera cam in LevelEditorCameraManager.Instance.GetPrivateField<LevelEditorCameraController>("_cameraController").GetComponentsInChildren<Camera>())
                {
                    cam.rect = new Rect(0, 0, 1, 1);
                    cam.nearClipPlane = 1;
                }
            }, 0.2f, true);
            base.gameObject.SetActive(true);
        }

        public void RefreshSelected(List<ObjectPlacedInLevel> objects)
        {
            if (objects.Count == 1) InspectorPanel.Populate(objects[0]);
            ObjectsSelected.gameObject.SetActive(objects.Count > 0);
            if (objects.Count == 0)
            {
                return;
            }
            ObjectsSelected.ObjectsSelected.text = OverhaulMain.GetTranslatedString("LVLEdit_ObjectsSelected") + " " + objects.Count;
            ObjectsSelected.AdditionalText.text = "Install/Enable Level editor tools mod for advanced controls";

            /*
            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelectedLETMod", new object[] { ObjectsSelected });*/
        }
    }
}
