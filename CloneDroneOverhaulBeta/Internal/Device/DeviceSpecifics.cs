using UnityEngine;

namespace CDOverhaul.Device
{
    public static class DeviceSpecifics
    {
        private static bool s_HasInitialized;

        public static int GPUMemorySize
        {
            get;
            private set;
        }

        public static int GPUShaderLevel
        {
            get;
            private set;
        }

        public static void Initialize()
        {
            if (s_HasInitialized)
                return;

            s_HasInitialized = true;

            GPUMemorySize = SystemInfo.graphicsMemorySize;
            GPUShaderLevel = SystemInfo.graphicsShaderLevel;
        }
    }
}
