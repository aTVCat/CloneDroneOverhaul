using HarmonyLib;
using UnityEngine;

namespace CDOverhaul.Patches
{
    [HarmonyPatch(typeof(EnemyFactory))]
    internal static class EnemyFactory_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyFactory), "SpawnEnemyWithRotation", new System.Type[] { typeof(Transform), typeof(Vector3), typeof(Vector3), typeof(CharacterModel) })]
        private static void SpawnEnemyWithRotation_Prefix(Transform enemyPrefab, Vector3 spawnPosition, Vector3 spawnRotation, CharacterModel characterModelOverride = null)
        {
            if (enemyPrefab != null && MultiplayerCharacterCustomizationManager.Instance != null && !MultiplayerCharacterCustomizationManager.Instance.CharacterModels.IsNullOrEmpty())
            {
                FirstPersonMover character = enemyPrefab.GetComponent<FirstPersonMover>();
                if (character != null && character.CharacterModelPrefab != null)
                {
                    switch (character.CharacterModelPrefab.name)
                    {
                        case "CharacterModel_Business1_NoJetpack":
                            character.CharacterModelPrefab = MultiplayerCharacterCustomizationManager.Instance.CharacterModels[24].CharacterModelPrefab;
                            break;
                            /*
                        case "CharacterModel_Zombie1":
                            character.CharacterModelPrefab = MultiplayerCharacterCustomizationManager.Instance.CharacterModels[26].CharacterModelPrefab;
                            break;*/
                    }
                }
            }
        }
    }
}
