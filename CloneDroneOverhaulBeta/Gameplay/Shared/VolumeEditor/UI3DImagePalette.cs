using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CDOverhaul.Gameplay;
using Newtonsoft.Json;
using System.IO;

namespace CDOverhaul.Shared
{
    public class UI3DImagePalette : MonoBehaviour
    {
        private Transform _container;
        private Transform _prefab;

        public Button NextLayerButton;
        public Button PrevLayerButton;

        public Color TargetColor;
        public int CurrentLayer;

        public UI3DImagePalette Initialize(in Transform container, in Transform prefab)
        {
            _container = container;
            _prefab = prefab;
            return this;
        }

        public void SetButtons(in Button next, in Button prev)
        {
            NextLayerButton = next;
            PrevLayerButton = prev;

            NextLayerButton.onClick.AddListener(NextLayer);
            PrevLayerButton.onClick.AddListener(PrevLayer);

            RefreshButtons();
        }

        public void NextLayer()
        {
            CurrentLayer++;
            RefreshButtons();
            PopulatePalette(CurrentLayer);
        }

        public void PrevLayer()
        {
            if (CurrentLayer == 0)
            {
                return;
            }
            CurrentLayer--;
            RefreshButtons();
            PopulatePalette(CurrentLayer);
        }

        public void RefreshButtons()
        {
            NextLayerButton.interactable = CurrentLayer < 0;
            PrevLayerButton.interactable = CurrentLayer > 0;
        }

   

        public void PopulatePalette(int layer)
        {
            TransformUtils.DestroyAllChildren(_container);

            AltVolumeEditorController controller = ModControllerManager.GetController<AltVolumeEditorController>();

            Color[,] array = null;//

            GridLayoutGroup grid = _container.GetComponent<GridLayoutGroup>();
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 10; //

            int index = 0;
            foreach (Color c in array)
            {
                Image img = Instantiate<Image>(_prefab.GetComponent<Image>(), _container);
                img.gameObject.SetActive(true);
                img.color = c;
                img.gameObject.AddComponent<UIImagePalettePixel>().Initialize(this, img, index, layer);
                index++;
            }
        }
    }
}