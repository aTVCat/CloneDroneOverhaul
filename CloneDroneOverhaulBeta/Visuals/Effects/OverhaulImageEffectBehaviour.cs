﻿using CDOverhaul.Gameplay;
using OverhaulAPI.SharedMonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CDOverhaul.Visuals
{
    public class OverhaulImageEffectBehaviour : OverhaulBehaviour
    {
        public static readonly string[] IgnoredCameras = new string[]
        {
            "TitleScreenLogoCamera",
            "UICamera",
            "ArenaCamera"
        };

        protected Camera PreviousCamera;
        protected Camera CurrentCamera;

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public virtual void PatchCamera(Camera camera)
        {
            PreviousCamera = CurrentCamera;
            CurrentCamera = camera;
        }

        public static OverhaulImageEffect GetImageEffect(Camera camera, string name)
        {
            if (!camera || string.IsNullOrEmpty(name))
                return null;

            OverhaulImageEffect[] overhaulImageEffects = camera.GetComponents<OverhaulImageEffect>();
            foreach (OverhaulImageEffect effect in overhaulImageEffects)
                if (effect.effectName == name)
                    return effect;

            return null;
        }
    }
}
