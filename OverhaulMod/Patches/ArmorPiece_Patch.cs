using HarmonyLib;
using OverhaulMod.Visuals;
using UnityEngine;

namespace OverhaulMod.Patches
{
    [HarmonyPatch(typeof(ArmorPiece))]
    internal static class ArmorPiece_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ArmorPiece.DestroyArmorPiece))]
        private static void DestroyArmorPiece_Postfix(ArmorPiece __instance, Vector3 impactDirection)
        {
            if (!ModCore.IsActive()) return;

            if (__instance.PiecesToDetach == null || __instance.PiecesToDetach.Length == 0) return;

            for (int i = 0; i < __instance.PiecesToDetach.Length; i++)
            {
                Transform piece = __instance.PiecesToDetach[i];
                if (piece)
                {
                    Rigidbody rigidbody = piece.GetComponent<Rigidbody>();
                    if (rigidbody)
                    {
                        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                        piece.gameObject.AddComponent<DisableRigidBodyInterpolation>().Initialize(rigidbody, 1f);
                    }
                }
            }
        }
    }
}
