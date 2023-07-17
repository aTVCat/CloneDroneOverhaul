using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.Device
{
    public static class Recommendations
    {
        #region SSAO

        public static int GetSSAORequiredShaderLevel() => 30;
        public static string GetSSAORequiredShaderLevelStiring() => "3.0";

        public static int GetSSAORequiredGPUMemorySize() => 2048;

        public static RecommendationLevel GetSSAORecommendation()
        {
            if (DeviceSpecifics.GPUShaderLevel < GetSSAORequiredShaderLevel())
                return RecommendationLevel.Unsupported;

            if (DeviceSpecifics.GPUMemorySize < GetSSAORequiredGPUMemorySize())
                return RecommendationLevel.BelowReqirements;

            return RecommendationLevel.Recommended;
        }

        public static string GetSSAORecommendationString()
        {
            switch (GetSSAORecommendation())
            {
                case RecommendationLevel.Recommended:
                    return string.Format("Your graphics card has more than {0}Mb of memory, Shader Model {1} is supported.", new object[] { GetSSAORequiredGPUMemorySize(), GetSSAORequiredShaderLevelStiring() });
                case RecommendationLevel.BelowReqirements:
                    return string.Format("Your graphics card has less than {0}Mb of memory, Shader Model {1} is supported. This may result FPS drops.", new object[] { GetSSAORequiredGPUMemorySize(), GetSSAORequiredShaderLevelStiring() });
                case RecommendationLevel.Unsupported:
                    return string.Format("Your graphics card doesn't support Shader Model {0}.", new object[] { GetSSAORequiredShaderLevelStiring() });
            }
            return "Unknown error";
        }

        public static bool GetSSAOSettingDefaultValue()
        {
            if (GetSSAORecommendation() == RecommendationLevel.Recommended)
                return true;

            return false;
        }

        #endregion
    }
}
