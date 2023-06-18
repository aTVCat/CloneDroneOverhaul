using InjectionClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.HUD.Gamemodes
{
    public class FullscreenWindowPageBase : OverhaulBehaviour
    {
        private static List<FullscreenWindowPageBase> s_AllPages = new List<FullscreenWindowPageBase>();
        public virtual Vector2 GetWindowSize() => OverhaulGamemodesUIFullscreenWindow.DefaultWindowSize;

        protected OverhaulGamemodesUIFullscreenWindow FullscreenWindow;
        protected ModdedObject MyModdedObject;

        public virtual void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            MyModdedObject = base.GetComponent<ModdedObject>();
            FullscreenWindow = fullscreenWindow;
            if (!s_AllPages.Contains(this))
                s_AllPages.Add(this);
        }

        public virtual void Show()
        {
            base.gameObject.SetActive(true);
        }

        public virtual void Hide(bool justHide)
        {
            base.gameObject.SetActive(false);
            FullscreenWindow.Hide();
        }

        public static void Reset()
        {
            s_AllPages.Clear();
        }
    }
}