using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CloneDroneOverhaul.V3Tests.Gameplay.Animations
{
    public class CustomAnimationEditorCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            float mouseX = Input.GetAxis("Mouse X") * 4;
            float mouseY = Input.GetAxis("Mouse Y") * 4;

            Vector3 direction = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                direction += base.transform.forward * 15;
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction -= base.transform.right * 15;
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction -= base.transform.forward * 15;
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += base.transform.right * 15;
            }

            base.transform.eulerAngles = new Vector3(Mathf.Clamp(base.transform.eulerAngles.x - mouseY, 0, 90), base.transform.eulerAngles.y + mouseX);
            base.transform.position += direction * Time.deltaTime;
        }
    }
}
