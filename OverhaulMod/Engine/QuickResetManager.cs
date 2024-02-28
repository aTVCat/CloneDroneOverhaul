using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class QuickResetManager : Singleton<QuickResetManager>
    {
        public void RestartGame()
        {
            if (!BoltNetwork.IsSinglePlayer)
                return;

            GarbageManager.Instance.DestroyAllGarbage();
            GameFlowManager.Instance.SetRoundNotStarted();
            GameFlowManager.Instance._hasWonRound = false;
            GameFlowManager.Instance._hasPlayerDied = false;

            ProjectileManager.Instance.DestroyAllProjectiles();
            ReturnToElevatorManager.Instance.StopReturnSpeech();
            GarbageManager.Instance.ResetGarbageDoors();
            GarbageManager.Instance.DestroyAllGarbage();
            CutSceneManager.Instance.HideCutsceneGameObjects();
            ArenaLiftManager.Instance.SetToUpgradeRoom();

            PlayerAllyManager.Instance.DestroyAllPlayerAllies();
            CharacterTracker.Instance.DestroyExistingPlayer();

            Character[] characters = FindObjectsOfType<Character>();
            foreach (Character character in characters)
            {
                if(character is BattleCruiserController controller)
                {
                    controller.destroyLocationMarker();
                }

                Destroy(character.gameObject);
            }

            LevelManager.Instance.CleanUpLevelRootsWaitingForDestruction();

            LevelEditorBaseTrigger[] triggers = FindObjectsOfType<LevelEditorBaseTrigger>();
            foreach (LevelEditorBaseTrigger levelEditorBaseTrigger in triggers)
            {
                levelEditorBaseTrigger._hasTriggered = false;

                if(levelEditorBaseTrigger is EnableAITrigger trigger)
                {
                    trigger._seenClones.Clear();
                    trigger._allCharactersTouched.Clear();
                    trigger._enemiesTouched.Clear();
                    trigger._hasDispatchedDeathTrigger = false;
                }
                else if (levelEditorBaseTrigger is NobodyInsideTrigger trigger2)
                {
                    trigger2._allCharactersInsideArea.Clear();
                }
                else if (levelEditorBaseTrigger is LevelEditorUseButtonTrigger trigger3)
                {
                    trigger3._lastActivationTime = 0f;
                    trigger3._timeToRefreshHint = 0f;
                }
            }

            LevelEditorDestructibleBlock[] levelEditorDestructibleBlocks = FindObjectsOfType<LevelEditorDestructibleBlock>();
            foreach(LevelEditorDestructibleBlock levelEditorDestructibleBlock in levelEditorDestructibleBlocks)
            {
                if (!levelEditorDestructibleBlock._isBodyPartDestroyed)
                    continue;

                LevelEditorObjectAdvancedBehaviour levelObjectAdvancedBehaviour = levelEditorDestructibleBlock.GetComponent<LevelEditorObjectAdvancedBehaviour>();
                if (levelObjectAdvancedBehaviour)
                    levelObjectAdvancedBehaviour.RespawnObject();
            }

            SlidingDoorCloser[] slidingDoorClosers = FindObjectsOfType<SlidingDoorCloser>();
            foreach (SlidingDoorCloser slidingDoorCloser in slidingDoorClosers)
                slidingDoorCloser.ActivateFromTrigger(null);

            List<LevelEditorAnimation> levelEditorAnimations = LevelEditorAnimationManager.Instance.GetAnimationsInLevel();
            if (!levelEditorAnimations.IsNullOrEmpty())
            {
                foreach (var animation in levelEditorAnimations)
                {
                    if (!animation)
                        continue;

                    animation.StopPlaying();
                    animation.SetCurrentFrame(0);

                    if (animation.AutoPlay)
                        animation.Play();
                }
            }

            LevelEnemySpawner[] levelEnemySpawners = FindObjectsOfType<LevelEnemySpawner>();
            foreach (LevelEnemySpawner levelEnemySpawner in levelEnemySpawners)
            {
                levelEnemySpawner.Start();
            }

            ModActionUtils.DoInFrame(delegate
            {
                GameFlowManager gameFlowManager = GameFlowManager.Instance;
                GlobalEventManager.Instance.Dispatch("LevelSpawned");
                gameFlowManager.createPlayerAndSetLift();
                gameFlowManager.cleanupAfterRestart();
                GlobalEventManager.Instance.Dispatch("GameRestarted");
            });
        }
    }
}
