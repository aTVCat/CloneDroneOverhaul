using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (UIManager.Instance.IsMouseOverUIElement())
                return;

            bool control = Input.GetMouseButton(1);
            Cursor.visible = !control;
            Cursor.lockState = control ? CursorLockMode.Locked : CursorLockMode.None;
            if (!control)
                return;

            float deltaTime = Time.deltaTime;
            Vector3 vector = Vector3.zero;

            bool forward = Input.GetKey(KeyCode.W);
            bool backward = Input.GetKey(KeyCode.S);
            bool right = Input.GetKey(KeyCode.D);
            bool left = Input.GetKey(KeyCode.A);
            bool up = Input.GetKey(KeyCode.Space);
            bool down= Input.GetKey(KeyCode.LeftControl);

            float y = Input.GetAxis("Mouse X");
            float x = -Input.GetAxis("Mouse Y");

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

            if (newX > 180f)
                newX -= 360f;
            if (newX < -180f)
                newX += 360f;
            newX = Mathf.Max(-90f, newX);
            newX = Mathf.Min(90f, newX);

            base.transform.position += vector * deltaTime;
            base.transform.eulerAngles = new Vector3(newX, newY, 0f);
        }
    }
}
