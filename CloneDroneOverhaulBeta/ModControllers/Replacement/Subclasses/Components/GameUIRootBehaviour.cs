using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class GameUIRootBehaviour : MonoBehaviour
    {
        private GameUIRoot _uiRoot;
        private GraphicRaycaster _raycaster;

        private bool _hasInitialized;

        private void Start()
        {
            _uiRoot = GameUIRoot.Instance;
            _raycaster = _uiRoot.GetComponent<GraphicRaycaster>();
            _hasInitialized = true;
        }

        private void Update()
        {
            if (_hasInitialized && Time.frameCount % 3 == 0)
            {
                _raycaster.enabled = Cursor.visible;
            }
        }
    }
}