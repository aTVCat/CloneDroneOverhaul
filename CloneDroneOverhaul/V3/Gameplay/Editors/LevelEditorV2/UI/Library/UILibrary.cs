using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.LevelEditor
{
    public class UILibrary : UILevelEditorHUDBase
    {
        private (Transform, Transform) _placeholders;
        private Button _openLibraryButton;

        private bool _libraryIsOpen;

        private void Start()
        {
            if (!HasModdedObject)
            {
                return;
            }

            _placeholders.Item1 = MyModdedObject.GetObjectFromList<Transform>(0);
            _placeholders.Item2 = MyModdedObject.GetObjectFromList<Transform>(1);

            _openLibraryButton = MyModdedObject.GetObjectFromList<Button>(2);
            _openLibraryButton.onClick.AddListener(toggleLibrary);

            SetActive(false);
        }

        public void SetActive(in bool value)
        {
            _libraryIsOpen = value;
            SetPlaceholdersVisible(value);



            base.gameObject.SetActive(value);
        }

        /// <summary>
        /// Toggle library visibility
        /// </summary>
        private void toggleLibrary()
        {
            SetActive(!_libraryIsOpen);
        }

        /// <summary>
        /// Move toolbar to right
        /// </summary>
        /// <param name="value"></param>
        public void SetPlaceholdersVisible(in bool value)
        {
            _placeholders.Item1.gameObject.SetActive(value);
            _placeholders.Item2.gameObject.SetActive(value);
        }
    }
}
