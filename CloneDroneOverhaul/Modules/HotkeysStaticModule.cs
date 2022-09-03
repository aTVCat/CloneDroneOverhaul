using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.Modules
{
    public class HotkeysModule : ModuleBase
    {
        private List<Hotkey> hotKeys = new List<Hotkey>();

        public override bool ShouldWork() // Add splashes/tooltips for it
        {
            return true;
        }

        public override void OnNewFrame()
        {
            if (hotKeys.Count == 0)
            {
                return;
            }

            for (int i = 0; i < hotKeys.Count; i++)
            {
                Hotkey hk = hotKeys[i];
                KeyCode Key1 = hk.Key1;
                KeyCode Key2 = hk.Key2;

                bool twoKeysUsed = Key1 != KeyCode.None && Key2 != KeyCode.None;
                if (!twoKeysUsed)
                {
                    KeyCode usedKey = Key1 != KeyCode.None ? Key1 : Key2;
                    if (Input.GetKeyDown(usedKey))
                    {
                        hk.Method();
                    }
                }
                else
                {
                    if (Input.GetKey(Key1) && Input.GetKeyDown(Key2))
                    {
                        hk.Method();
                    }
                }
            }
        }

        public void AddHotkey(Hotkey hotkey)
        {
            if ((hotkey.Key1 == KeyCode.None && hotkey.Key2 == KeyCode.None) || hotkey.Method == null)
            {
                return;
            }
            hotKeys.Add(hotkey); // Rewrite to make it simpler
        }
    }

    public class Hotkey
    {

        public KeyCode Key1 = KeyCode.None;
        public KeyCode Key2 = KeyCode.None;

        public Action Method;

    }
}
