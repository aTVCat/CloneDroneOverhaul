using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(ModdedObject), typeof(Collider))]
    public class LevelEditorImageBlock : MonoBehaviour
    {
        private ModdedObject _moddedObject;
        private RawImage _img;
        private RectTransform _canvasRectTransform;

        [IncludeInLevelEditor(false, true)]
        public string Path = "Path";

        [IncludeInLevelEditor(false, true)]
        public float X = 3f;
        [IncludeInLevelEditor(false, true)]
        public float Y = 3f;

        private string _lastPathString;

        private void Awake()
        {
            _moddedObject = base.GetComponent<ModdedObject>();
            _img = _moddedObject.GetObject<RawImage>(0);
            _canvasRectTransform = _moddedObject.GetObject<Canvas>(1).transform as RectTransform;

            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
        }

        private void Update()
        {
            _canvasRectTransform.sizeDelta = new Vector2(X, Y);
            if (_lastPathString != Path)
            {
                _lastPathString = Path;
                if (File.Exists(OverhaulMod.Core.ModFolder + Path))
                {
                    Texture2D texture2D = new Texture2D(1, 1);
                    _ = texture2D.LoadImage(File.ReadAllBytes(OverhaulMod.Core.ModFolder + Path));
                    _img.texture = texture2D;
                }
            }
        }
    }
}