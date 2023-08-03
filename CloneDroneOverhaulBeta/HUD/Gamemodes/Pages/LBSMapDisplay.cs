using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class LBSMapDisplay : OverhaulBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [ObjectReference("MapPreview")]
        private readonly RawImage m_PreviewImage;

        [ObjectReference("SelectedIndicator")]
        private readonly GameObject m_SelectedIndicator;

        private Button m_Button;
        public LBSGameCustomization GameCustomizationUI;

        public bool ShowThumbnail
        {
            get;
            set;
        }
        public int Type
        {
            get;
            set;
        }

        public string LevelID
        {
            get;
            private set;
        }

        public string LevelTitle
        {
            get;
            private set;
        }

        public string LevelCreator
        {
            get;
            private set;
        }

        public bool IsSelected
        {
            get => GameCustomizationUI.SelectedLevels.Contains(LevelID);
            set
            {
                if (value && !IsSelected)
                {
                    GameCustomizationUI.SelectedLevels.Insert(0, LevelID);
                    onUpdateSelection(true);
                }
                else if (!value && IsSelected)
                {
                    _ = GameCustomizationUI.SelectedLevels.Remove(LevelID);
                    onUpdateSelection(false);
                }
            }
        }

        public void Initialize(LevelDescription levelDescription, LBSGameCustomization ui)
        {
            initializeDisplay(ui);
            LevelID = levelDescription.LevelID;
            if (ui.BrowsingLevelsType == 1)
            {
                Sprite levelPreview = Resources.Load<Sprite>("Data/LevelEditorLevels/" + levelDescription.PrefabName + "_thumbnail");
                if (levelPreview)
                {
                    m_PreviewImage.texture = levelPreview.texture;
                }
                LevelTitle = levelDescription.LevelID;
            }
            onUpdateSelection(IsSelected);
        }

        private void initializeDisplay(LBSGameCustomization ui)
        {
            GameCustomizationUI = ui;
            Type = ui.BrowsingLevelsType;

            m_Button = base.GetComponent<Button>();
            m_Button.AddOnClickListener(OnClick);

            OverhaulUIVer2.AssignVariables(this);
        }

        public void OnClick()
        {
            IsSelected = !IsSelected;
        }

        private void onUpdateSelection(bool newValue)
        {
            m_SelectedIndicator.SetActive(newValue);
            /*if (Type != 2)
            {
                if (newValue)
                {
                    base.transform.SetAsFirstSibling();
                }
                else
                {
                    base.transform.SetAsLastSibling();
                }
            }*/
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GameCustomizationUI.ShowMapInfoTooltip(true, base.transform.position, LevelTitle, LevelCreator);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GameCustomizationUI.ShowMapInfoTooltip(false, base.transform.position, LevelTitle, LevelCreator);
        }
    }
}
