using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CloneDroneOverhaul.Modules;
using CloneDroneOverhaul.UI;
using ModLibrary;

namespace CloneDroneOverhaul.LevelEditor
{
    public class ModdedLevelEditorUI : ModGUIBase
    {
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
            }
        }
        public class Toolbar : UIPanel
        {
            public void RefreshSelected()
            {
                foreach(GameObject obj in ModdedObj.objects)
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
        public class LTEM_Toolbar : UIPanel
        {
            public void RefreshSelected()
            {
            }
            protected override void Config()
            {
                RefreshSelected();
                base.gameObject.SetActive(OverhaulDescription.LevelEditorToolsInstalled());
            }
        }
        public class Inspector : UIPanel
        {
            protected override void Config()
            {
            }
        }

        public ObjectsSelectedPanel ObjectsSelected;
        public Toolbar ToolBar;
        public LTEM_Toolbar LTEM_ToolBar;

        public override void OnInstanceStart()
        {
            base.gameObject.SetActive(false);
            MyModdedObject = base.GetComponent<ModdedObject>();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(GameUIRoot.Instance.LevelEditorUI.OnPlayButtonClicked);
            ObjectsSelected = MyModdedObject.GetObjectFromList<RectTransform>(1).gameObject.AddComponent<ObjectsSelectedPanel>().SetUp<ObjectsSelectedPanel>();
            ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(2).gameObject.AddComponent<Toolbar>().SetUp<Toolbar>();
            LTEM_ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(3).gameObject.AddComponent<LTEM_Toolbar>().SetUp<LTEM_Toolbar>();
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
            ObjectsSelected.gameObject.SetActive(objects.Count > 0);
            if(objects.Count == 0)
            {
                return;
            }
            ObjectsSelected.ObjectsSelected.text = OverhaulMain.GetTranslatedString("LVLEdit_ObjectsSelected") + " " + objects.Count;
            ObjectsSelected.AdditionalText.text = "Install/Enable Level editor tools mod for advanced controls";

            CrossModManager.DoAction("ModdedLevelEditor.RefreshSelectedLETMod", new object[] { ObjectsSelected });
        }
    }
}
