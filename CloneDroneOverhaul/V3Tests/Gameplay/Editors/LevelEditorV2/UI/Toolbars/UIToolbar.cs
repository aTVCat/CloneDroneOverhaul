using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.LevelEditor
{
    public class UIToolbar : UILevelEditorHUDBase
    {
        private Dictionary<LevelEditorToolType, Image> _tools = new Dictionary<LevelEditorToolType, Image>()
        {
            { LevelEditorToolType.DragFreely, null },
               { LevelEditorToolType.Move, null },
                  { LevelEditorToolType.Rotate, null },
                     { LevelEditorToolType.Scale, null },
                        { LevelEditorToolType.Select, null },
        };

        private void Start()
        {
            if (!HasModdedObject)
            {
                return;
            }

            int index = 0;
            foreach (GameObject gm in MyModdedObject.objects)
            {
                _tools[(LevelEditorToolType)index] = gm.GetComponent<Image>();
                index++;
            }

            MyModdedObject.GetObjectFromList<Transform>(0).GetComponent<Button>().onClick.AddListener(delegate
            {
                LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.DragFreely);
            });
            MyModdedObject.GetObjectFromList<Transform>(1).GetComponent<Button>().onClick.AddListener(delegate
            {
                LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.Move);
            });
            MyModdedObject.GetObjectFromList<Transform>(2).GetComponent<Button>().onClick.AddListener(delegate
            {
                LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.Scale);
            });
            MyModdedObject.GetObjectFromList<Transform>(3).GetComponent<Button>().onClick.AddListener(delegate
            {
                LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.Rotate);
            });
            MyModdedObject.GetObjectFromList<Transform>(4).GetComponent<Button>().onClick.AddListener(delegate
            {
                LevelEditorToolManager.Instance.SetActiveTool(LevelEditorToolType.Select);
            });

            V3_LevelEditorController.Patch(ELevelEditorBasicPatchType.DefaultToolsIsMove);
        }

        /// <summary>
        /// Set tool selected
        /// </summary>
        /// <param name="tool"></param>
        public void SetActiveTool(in LevelEditorToolType tool)
        {
            foreach (LevelEditorToolType type in _tools.Keys)
            {
                _tools[type].color = UILevelEditorV2.UIDefaultColor;
            }
            _tools[tool].color = UILevelEditorV2.UISelectedColor;
        }
    }
}
