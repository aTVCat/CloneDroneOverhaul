using ModLibrary;
using OverhaulAPI.UnsortedClasses;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulAPI
{
    public static class MultiplayerAPI
    {
        private static Dictionary<string, Action<string[]>> _dataIDs = new Dictionary<string, Action<string[]>>();
        private static Dictionary<string, Action<string[]>> _dataAnswers = new Dictionary<string, Action<string[]>>();

        public const string UnknownDataIDError = "Error: Unknown data ID";
        public const string PlayfabIDNullError = "Error: PlayfabID is empty";

        public static string[] LatestStringData { get; set; }
        public static string Answer { get; set; }

        private static bool _added;

        internal static void Init()
        {
            new GameObject("MultiplayerAPI, EventListener").AddComponent<MultiplayerEventListener>();

            return;
            if (_added) return;
            MultiplayerEventCallback.AddEventListener<GenericStringForModdingEvent>(OnEvent);
            _added = true;
        }

        public static void RegisterRequestAndAnswerListener(string id, Action<string[]> action, Action<string[]> onAnswer)
        {
            if (_dataIDs.ContainsKey(id))
            {
                return;
            }

            _dataAnswers.Add(id, onAnswer);
            _dataIDs.Add(id, action);
        }

        public static void RequestDataFromPlayer(FirstPersonMover player, string dataID, Action<string> errorString)
        {
            if (player == null || string.IsNullOrEmpty(player.GetPlayFabID()))
            {
                ThrowError(errorString, PlayfabIDNullError);
                return;
            }

            if (!_dataIDs.ContainsKey(dataID))
            {
                ThrowError(errorString, UnknownDataIDError + ": " + dataID);
                return;
            }

            ThrowError(errorString, "Requesting...");

            MultiplayerMessageSender.SendToAllClients(GetEventString(false, dataID, player.GetPlayFabID()));
        }

        internal static void OnEvent(GenericStringForModdingEvent moddedEvent)
        {
            Debug.Log("EVENT EVENT!!: " + moddedEvent.EventData);
            string eventData = moddedEvent.EventData;
            if (eventData.Contains("[OverhaulAPI]"))
            {
                string localPlayFabID = MultiplayerLoginManager.Instance.GetLocalPlayFabID();
                string[] split = eventData.Split('-');
                if (split[2] == bool.FalseString && split[3] == localPlayFabID)
                {
                    TryCallDataIDFunction(split[1], split, false);
                    MultiplayerMessageSender.SendToAllClients(GetEventString(true, split[1], split[3], Answer));
                }
                else if (split[2] == bool.TrueString && split[3] == localPlayFabID)
                {
                    TryCallDataIDFunction(split[1], split, true);
                }
            }
        }

        internal static void TryCallDataIDFunction(string dataID, string[] splitData, bool isAnswer)
        {
            LatestStringData = splitData;
            if (isAnswer)
            {
                _dataAnswers[dataID].Invoke(splitData);
            }
            else
            {
                _dataIDs[dataID].Invoke(splitData);
            }
        }

        internal static string GetEventString(bool isAnswer, string dataID, string playfabID, string additData = "")
        {
            return "[OverhaulAPI]-" + dataID + "-" + isAnswer + "-" + playfabID + "-" + additData;
        }

        internal static void ThrowError(Action<string> action, string str)
        {
            if (action != null)
            {
                action(str);
            }
        }
    }
}
