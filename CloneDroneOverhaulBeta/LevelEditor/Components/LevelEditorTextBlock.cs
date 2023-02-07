using TMPro;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    [RequireComponent(typeof(ModdedObject), typeof(Collider))]
    public class LevelEditorTextBlock : MonoBehaviour
    {
        private ModdedObject _moddedObject;

        private TextMeshPro _text;

        [IncludeInLevelEditor(true, false)]
        public bool HasAttached;

        [IncludeInLevelEditor(false, true)]
        public string Text = "Write here something...";

        [IncludeInLevelEditor(false, false, true, true)]
        public bool RenderText = true;

        [IncludeInLevelEditor(false, true)]
        public Color TextColor = Color.white;

        [IncludeInLevelEditor(0f, 1f, false, false, true)]
        public float Transparency = 0f;

        private Color _tColor = Color.clear;

        private void Awake()
        {
            _moddedObject = base.GetComponent<ModdedObject>();
            _moddedObject.GetObject<Transform>(2).gameObject.SetActive(GameModeManager.IsInLevelEditor());
            _text = _moddedObject.GetObject<TextMeshPro>(0);

            Collider collider = base.GetComponent<Collider>();
            collider.enabled = GameModeManager.IsInLevelEditor();
        }

        private void Start()
        {
            if (!HasAttached)
            {
                ResetSize();

                HasAttached = true;
                base.GetComponent<ObjectPlacedInLevel>().SetCustomInspectorBoolValue("LevelEditorTextBlock", "HasAttached", true);
            }
        }

        private void Update()
        {
            _tColor.a = Transparency;

            if (_text.text != Text)
            {
                _text.text = Text;
            }

            if (_text.enabled != RenderText)
            {
                _text.enabled = RenderText;
            }

            _text.color = TextColor - new Color(0, 0, 0, _tColor.a);
        }

        [IncludeInLevelEditor]
        [CallFromAnimation]
        public void ResetSize()
        {
            ObjectPlacedInLevel o = base.GetComponent<ObjectPlacedInLevel>();
            o.OnScaleAboutToChange();
            base.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            o.OnScaleChanged();
        }
    }
}