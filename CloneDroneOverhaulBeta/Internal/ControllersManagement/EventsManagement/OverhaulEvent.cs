using System;

namespace CDOverhaul
{
    /// <summary>
    /// A definition of event that has listeners
    /// </summary>
    public class OverhaulEvent
    {
        /// <summary>
        /// Event name
        /// </summary>
        public string EventName;

        /// <summary>
        /// Is it not a vanilla event?
        /// </summary>
        public bool WithPrefix;

        /// <summary>
        /// What function should be called
        /// </summary>
        public Action EventAction;

        /// <summary>
        /// What function with arguments should be called
        /// </summary>
        public object EventActionWithArgument;

        public OverhaulEvent(in string eventName, in Action @delegate, in bool withPrefix)
        {
            EventName = eventName;
            EventAction = @delegate;
            EventActionWithArgument = null;
            WithPrefix = withPrefix;
        }

        /// <summary>
        /// Mark event as one that requires arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delegate"></param>
        public void SetArgument<T>(Action<T> @delegate)
        {
            EventAction = null;
            EventActionWithArgument = @delegate;
        }

        /// <summary>
        /// Compare 2 events
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Equals(in OverhaulEvent a, in OverhaulEvent b) => (a.EventName, a.EventAction, a.EventActionWithArgument) == (b.EventName, b.EventAction, b.EventActionWithArgument);
    }
}
