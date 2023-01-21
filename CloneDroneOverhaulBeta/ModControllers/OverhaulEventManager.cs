using System;
using System.Collections.Generic;

namespace CDOverhaul
{
    /// <summary>
    /// A simplified interaction with <see cref="GlobalEventManager"/> (not really)
    /// </summary>
    public static class OverhaulEventManager
    {
        public const string EventPrefix = "Overhaul_";
        public static Action EmptyDelegate = delegate { };

        private static GlobalEventManager _globalEventManager;

        private static List<SOverhaulEventEntry> _events;

        /// <summary>
        /// Check if <see cref="GlobalEventManager"/> instance is not <b>Null</b> (for some reason)
        /// </summary>
        public static bool MayAddListeners => _globalEventManager != null && _events != null;

        internal static void Initialize()
        {
            _globalEventManager = GlobalEventManager.Instance;

            if (_events == null)
            {
                _events = new List<SOverhaulEventEntry>();
            }
            _events.Clear();

            OverhaulEventManager.AddEvent(OverhaulBase.ModDeactivatedEventString);
            AddListenerToEvent(OverhaulBase.ModDeactivatedEventString, Deconstruct);
        }

        internal static void Deconstruct()
        {
            if (MayAddListeners)
            {
                for (int i = _events.Count - 1; i > -1; i--)
                {
                    RemoveEventListener(_events[i].EventName, _events[i].EventAction);
                }
            }
        }


        public static void AddEvent(in string name, in bool dontAddPrefix = false)
        {
            AddEventListener(name, EmptyDelegate, dontAddPrefix);
        }

        public static void RemoveEvent(in string name, in bool dontAddPrefix = false)
        {
            RemoveEventListener(name, EmptyDelegate, dontAddPrefix);

            if (MayAddListeners)
            {
                string finalString = !dontAddPrefix ? EventPrefix + name : name;
                foreach (SOverhaulEventEntry entry in _events)
                {
                    if (entry.EventName == finalString)
                    {
                        RemoveEventListener(entry.EventName, entry.EventAction);
                    }
                }
            }
        }

        public static void AddListenerToEvent(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            AddEventListener(name, callback, dontAddPrefix);
        }

        public static void AddListenerToEvent<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            AddEventListener(name, callback, dontAddPrefix);
        }

        public static void RemoveListenerFromEvent(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            RemoveEventListener(name, callback, dontAddPrefix);
        }

        public static void RemoveListenerFromEvent<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            RemoveEventListener(name, callback, dontAddPrefix);
        }


        public static SOverhaulEventEntry AddEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return default(SOverhaulEventEntry);
            }

            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.AddEventListener(finalString, callback);

            SOverhaulEventEntry newEvent = new SOverhaulEventEntry(finalString, callback, !dontAddPrefix);
            _events.Add(newEvent);

            return newEvent;
        }

        public static SOverhaulEventEntry AddEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return default(SOverhaulEventEntry);
            }


            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.AddEventListener<T>(finalString, callback);

            SOverhaulEventEntry newEvent = new SOverhaulEventEntry(finalString, null, !dontAddPrefix);
            newEvent.SetArgument<T>(callback);
            _events.Add(newEvent);

            return newEvent;
        }

        public static void RemoveEventListener(in string name, in Action callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return;
            }

            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.RemoveEventListener(finalString, callback);

            int removeIndex = -1;
            SOverhaulEventEntry eventToRemove = new SOverhaulEventEntry(finalString, callback, !dontAddPrefix);
            foreach (SOverhaulEventEntry entry in _events)
            {
                removeIndex++;
                if (entry.Equals(entry, eventToRemove))
                {
                    break;
                }
            }
            if (removeIndex > -1)
            {
                _events.RemoveAt(removeIndex);
            }
        }

        public static void RemoveEventListener<T>(in string name, in Action<T> callback, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return;
            }

            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.RemoveEventListener<T>(finalString, callback);

            int removeIndex = -1;
            SOverhaulEventEntry eventToRemove = new SOverhaulEventEntry(finalString, null, !dontAddPrefix);
            eventToRemove.SetArgument<T>(callback);
            foreach (SOverhaulEventEntry entry in _events)
            {
                removeIndex++;
                if (entry.Equals(entry, eventToRemove))
                {
                    break;
                }
            }
            if (removeIndex > -1)
            {
                _events.RemoveAt(removeIndex);
            }
        }

        public static void DispatchEvent(in string name, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return;
            }

            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.Dispatch(finalString);
        }

        public static void DispatchEvent<T>(in string name, in T argument, in bool dontAddPrefix = false)
        {
            if (!MayAddListeners)
            {
                return;
            }

            string finalString = !dontAddPrefix ? EventPrefix + name : name;
            _globalEventManager.Dispatch(finalString, argument);
        }
    }
}
