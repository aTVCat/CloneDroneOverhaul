using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class ControlInstructionEntry : MonoBehaviour
    {
        public Transform NonAvailableHUD;
        public Text Name;
        public Text Key;
        public Animator Animator;

        public KeyCode Keycode { get; private set; }
        public bool MouseTrigger { get; private set; }
        public int MouseButtonID { get; private set; }
        public bool IsAvailable = true;

        public static ControlInstructionEntry AddComponent(in ModdedObject moddedObject)
        {
            ControlInstructionEntry entry = moddedObject.gameObject.AddComponent<ControlInstructionEntry>();
            entry.NonAvailableHUD = moddedObject.GetObjectFromList<Transform>(4);
            entry.Name = moddedObject.GetObjectFromList<Text>(1);
            entry.Key = moddedObject.GetObjectFromList<Text>(2);
            entry.Animator = moddedObject.GetComponent<Animator>();
            return entry;
        }

        /// <summary>
        /// Make control darker
        /// </summary>
        /// <param name="value"></param>
        public void SetIsAvailable(in bool value)
        {
            NonAvailableHUD.gameObject.SetActive(value);
            IsAvailable = value;
        }

        /// <summary>
        /// Set control visible
        /// </summary>
        /// <param name="value"></param>
        public void SetVisible(in bool value)
        {
            base.gameObject.SetActive(value);
        }

        /// <summary>
        /// Set control key
        /// </summary>
        /// <param name="key"></param>
        public void SetUseTrigger(in KeyCode key, in bool useMouse = false, in int mouseButtonID = 0)
        {
            Key.text = key.ToString();
            MouseTrigger = useMouse;
            MouseButtonID = mouseButtonID;
            Keycode = key;
        }

        public void PlayUseAnimation()
        {
            base.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            iTween.ScaleTo(base.gameObject, Vector3.one, 0.5f);
            return;
            Animator.Play("UseControl");
            base.gameObject.SetActive(false);
            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
            {
                base.gameObject.SetActive(true);
            });
        }

        private void Update()
        {
            if (IsAvailable)
            {
                if (!MouseTrigger)
                {
                    if (Input.GetKeyDown(Keycode))
                    {
                        PlayUseAnimation();
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(MouseButtonID))
                    {
                        PlayUseAnimation();
                    }
                }
            }
        }
    }
}
