using CloneDroneOverhaul.UI;
using ModLibrary;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Threading;

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
                base.gameObject.SetActive(OverhaulDescription.LevelEditorToolsInstalled());
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
                for(int i = 0; i < 9; i++)
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
            public void populateEntries(EntryType type)
            {
                TransformUtils.DestroyAllChildren(base.ModdedObj.GetObjectFromList<Transform>(14));

                base.ModdedObj.GetObjectFromList<Transform>(16).gameObject.SetActive(false);

                if (type == EntryType.LevelFolder)
                {
                    List<LevelFileEntry> levelFilesAndFolders = Singleton<LevelEditorFilesManager>.Instance.GetLevelFilesAndFolders(true);
                    levelFilesAndFolders.Sort((LevelFileEntry entry, LevelFileEntry entryB) => string.Compare(entry.PathUnderLevelsFolder, entryB.PathUnderLevelsFolder, StringComparison.OrdinalIgnoreCase));

                    Dictionary<string, FolderWithLevels> dictionary = new Dictionary<string, FolderWithLevels>();
                    Dictionary<LevelFileEntry, Transform> dictionary2 = new Dictionary<LevelFileEntry, Transform>();

                    foreach (LevelFileEntry file in levelFilesAndFolders)
                    {
                        ModdedObject mObj = null;
                        if (file.IsFolder)
                        {
                            mObj = Instantiate<ModdedObject>(base.ModdedObj.GetObjectFromList<ModdedObject>(12), base.ModdedObj.GetObjectFromList<Transform>(14));
                            dictionary.Add(file.PathUnderLevelsFolder, mObj.gameObject.AddComponent<FolderWithLevels>().MakeUIFolder(file));
                        }
                        else
                        {
                            mObj = Instantiate<ModdedObject>(base.ModdedObj.GetObjectFromList<ModdedObject>(15), base.ModdedObj.GetObjectFromList<Transform>(14));
                            mObj.gameObject.AddComponent<UILevelEntry>().MakeLevelEntry(file);
                        }

                        if (mObj != null)
                        {
                            dictionary2.Add(file, mObj.transform);
                            mObj.gameObject.SetActive(true);
                        }
                    }

                    for (int j = 0; j < levelFilesAndFolders.Count; j++)
                    {
                        LevelFileEntry levelFileEntry2 = levelFilesAndFolders[j];
                        foreach (KeyValuePair<string, FolderWithLevels> keyValuePair in dictionary)
                        {
                            if (FolderContainsFile(keyValuePair.Key, levelFileEntry2))
                            {
                                Transform transform2 = dictionary2[levelFileEntry2];
                                keyValuePair.Value.PutLevelIntoFolder(transform2.gameObject);
                            }
                        }
                    }

                    OverhaulMain.Timer.AddNoArgAction(delegate
                    {
                        base.ModdedObj.GetObjectFromList<Transform>(16).gameObject.SetActive(true);
                    }, 0.1f, true);

                }
            }

            public void OnSelectedTab(TabButton button)
            {
                foreach(TabPage page in Pages)
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
                if(button.ID == "levels")
                {
                    populateEntries(EntryType.LevelFolder);
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
                public LevelFileEntry File;
            }
            public class FolderWithLevels : LevelsFileBase
            {
                public FolderWithLevels MakeUIFolder(LevelFileEntry path)
                {
                    File = path;
                    base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path.GetLevelName();
                    return this;
                }

                public void PutLevelIntoFolder(GameObject obj)
                {
                    obj.transform.SetParent(base.GetComponent<ModdedObject>().GetObjectFromList<Transform>(2));
                }
            }
            public class UILevelEntry : LevelsFileBase
            {
                public UILevelEntry MakeLevelEntry(LevelFileEntry path)
                {
                    File = path;
                    base.GetComponent<ModdedObject>().GetObjectFromList<Text>(0).text = path.GetLevelName();
                    return this;
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

            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelectedLETMod", new object[] { ObjectsSelected });
        }
    }
}
