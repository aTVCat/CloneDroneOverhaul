﻿using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    /// <summary>
    /// A simplified interaction with <see cref="GlobalEventManager"/> (not really)
    /// </summary>
    public static class OverhaulEventsController
    {
        /// <summary>
        /// This prefix is automatically added to any of mod events
        /// </summary>
        public const string EventPrefix = "Overhaul_";

        private static GlobalEventManager _globalEventManager;

        /// <summary>
        /// Check if <see cref="GlobalEventManager"/> instance is not <b>Null</b> (for some reason)
        /// </summary>
        public static bool MayAddListeners => _globalEventManager != null;
        private static readonly List<OverhaulEvent> _events = new List<OverhaulEvent>();

        internal static void Initialize()
        {
            _globalEventManager = GlobalEventManager.Instance;
            _events.Clear();

            _ = AddEventListener(OverhaulMod.ModDeactivatedEventString, Deconstruct);
        }

        internal static void Deconstruct()
        {
            if (MayAddListeners)
                for (int i = _events.Count - 1; i > -1; i--)
                    RemoveEventListener(_events[i].EventName, _events[i].EventAction);
        }

        internal static string GetString(in string eventName, in bool prefix) => !prefix ? EventPrefix + eventName : eventName;

        public static OverhaulEvent AddEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.AddEventListener(finalString, callback);

            OverhaulEvent newEvent = new OverhaulEvent(finalString, callback, !dontAddPrefix);
            _events.Add(newEvent);
            return newEvent;
        }

        public static OverhaulEvent AddEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.AddEventListener<T>(finalString, callback);

            OverhaulEvent newEvent = new OverhaulEvent(finalString, null, !dontAddPrefix);
            newEvent.SetArgument<T>(callback);
            _events.Add(newEvent);

            return newEvent;
        }

        public static void RemoveEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.RemoveEventListener(finalString, callback);

            int removeIndex = -1;
            OverhaulEvent eventToRemove = new OverhaulEvent(finalString, callback, !dontAddPrefix);
            foreach (OverhaulEvent entry in _events)
            {
                removeIndex++;
                if (entry.Equals(entry, eventToRemove))
                    break;
            }

            if (removeIndex > -1)
                _events.RemoveAt(removeIndex);
        }

        public static void RemoveEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.RemoveEventListener<T>(finalString, callback);

            int removeIndex = -1;
            OverhaulEvent eventToRemove = new OverhaulEvent(finalString, null, !dontAddPrefix);
            eventToRemove.SetArgument<T>(callback);
            foreach (OverhaulEvent entry in _events)
            {
                removeIndex++;
                if (entry.Equals(entry, eventToRemove))
                    break;
            }

            if (removeIndex > -1)
                _events.RemoveAt(removeIndex);
        }

        public static void DispatchEvent(in string name, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.Dispatch(finalString);
        }

        public static void DispatchEvent<T>(in string name, in T argument, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
                OverhaulExceptions.ThrowException(OverhaulExceptions.Exc_EventControllerUsedTooEarly);

            string finalString = GetString(name, dontAddPrefix);
            _globalEventManager.Dispatch(finalString, argument);
        }
    }
}
