using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.Shared
{
    public class UIImagePalettePixel : MonoBehaviour, IPointerDownHandler
    {
        private UI3DImagePalette _palette;
        private Image _img;
        public int IndexInTexture;
        public int Layer;

        public void Initialize(in UI3DImagePalette palette, in Image component, in int index, in int layer)
        {
            _palette = palette;
            _img = component;
            IndexInTexture = index;
            Layer = layer;
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _img.color = _palette.TargetColor;
        }
    }
}