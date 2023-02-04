using System;
using System.Diagnostics;
using System.Reflection;

namespace OverhaulAPI
{
    public class API
    {
        /// <summary>
        /// The version of API
        /// </summary>
        public static readonly Version APIVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The instance of API
        /// </summary>
        public static API APIInstance;

        /// <summary>
        /// Throw an exception
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowException(in string message)
        {
            StackFrame f = new StackFrame(1);
            throw new Exception("OverhaulAPI " + APIVersion.ToString() + " [" + f.GetMethod().DeclaringType + "_" + f.GetMethod().Name + "]" + " - " + message);
        }

        public static API LoadAPI()
        {
            APIInstance = new API();

            GamemodeAPI.Reset();

            return APIInstance;
        }
    }
}
