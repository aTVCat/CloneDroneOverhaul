using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorCamera : MonoBehaviour
    {
        public static bool IsControllingTheCamera;

        public bool IsScreenshotStageCamera;

        private UIManager m_uiManager;

        private InputManager m_inputManager;

        private bool m_wasMouseButtonDownPrevFrame;
        private bool m_prevValue;

        private void Start()
        {
            m_uiManager = UIManager.Instance;
            m_inputManager = InputManager.Instance;
            m_inputManager.SetCursorEnabled(true);
        }

        private void OnDestroy()
        {
            IsControllingTheCamera = false;
        }

        private void Update()
        {
            bool newValue = IsControllingTheCamera;
            if (newValue != m_prevValue)
            {
                m_inputManager.SetCursorEnabled(!newValue);
                m_prevValue = newValue;
            }
        }

        private void LateUpdate()
        {
            bool isMouseOverUIElement = m_uiManager.IsMouseOverUIElement();
            bool control = Input.GetMouseButton(1);
            bool willControl = (control && !m_wasMouseButtonDownPrevFrame && !isMouseOverUIElement) || (m_wasMouseButtonDownPrevFrame && control && IsControllingTheCamera);

            m_wasMouseButtonDownPrevFrame = control;

            if (!willControl)
            {
                IsControllingTheCamera = false;
                return;
            }
            IsControllingTheCamera = true;

            float deltaTime = Time.deltaTime;
            Vector3 vector = Vector3.zero;

            bool forward = Input.GetKey(KeyCode.W);
            bool backward = Input.GetKey(KeyCode.S);
            bool right = Input.GetKey(KeyCode.D);
            bool left = Input.GetKey(KeyCode.A);
            bool up = Input.GetKey(KeyCode.Space);
            bool down = Input.GetKey(KeyCode.LeftControl);

            bool rollLeft = false;
            bool rollRight = false;
            if (IsScreenshotStageCamera)
            {
                rollLeft = Input.GetKey(KeyCode.Q);
                rollRight = Input.GetKey(KeyCode.E);
            }

            float y = Input.GetAxis("Mouse X");
            float x = -Input.GetAxis("Mouse Y");
            float z = (rollRight ? 45f : (rollLeft ? -45f : 0f)) * Time.unscaledDeltaTime;

            if (forward)
                vector += base.transform.forward;
            if (backward)
                vector += -base.transform.forward;
            if (right)
                vector += base.transform.right;
            if (left)
                vector += -base.transform.right;
            if (up)
                vector += base.transform.up;
            if (down)
                vector += -base.transform.up;

            if (Input.GetKey(KeyCode.LeftShift))
                vector *= 15f;
            else
                vector *= 5f;

            Vector3 currentEulerAngles = base.transform.eulerAngles;
            float newX = currentEulerAngles.x + x;
            float newY = currentEulerAngles.y + y;
            float newZ = currentEulerAngles.z + z;

            if (newX > 180f)
                newX -= 360f;
            if (newX < -180f)
                newX += 360f;
            newX = Mathf.Max(-90f, newX);
            newX = Mathf.Min(90f, newX);

            base.transform.position += vector * deltaTime;
            base.transform.eulerAngles = new Vector3(newX, newY, newZ);
        }
    }
}
