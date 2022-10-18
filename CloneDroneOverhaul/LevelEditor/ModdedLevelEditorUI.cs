using CloneDroneOverhaul.UI;
using ModLibrary;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

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

        public ObjectsSelectedPanel ObjectsSelected;
        public Toolbar ToolBar;
        public LTEM_Toolbar LTEM_ToolBar;
        public Inspector InspectorPanel;

        public override void OnInstanceStart()
        {
            base.gameObject.SetActive(false);
            MyModdedObject = base.GetComponent<ModdedObject>();

            MyModdedObject.GetObjectFromList<Button>(0).onClick.AddListener(GameUIRoot.Instance.LevelEditorUI.OnPlayButtonClicked);
            ObjectsSelected = MyModdedObject.GetObjectFromList<RectTransform>(1).gameObject.AddComponent<ObjectsSelectedPanel>().SetUp<ObjectsSelectedPanel>();
            ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(2).gameObject.AddComponent<Toolbar>().SetUp<Toolbar>();
            LTEM_ToolBar = MyModdedObject.GetObjectFromList<RectTransform>(3).gameObject.AddComponent<LTEM_Toolbar>().SetUp<LTEM_Toolbar>();
            InspectorPanel = MyModdedObject.GetObjectFromList<RectTransform>(4).gameObject.AddComponent<Inspector>().SetUp<Inspector>();
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
