#define UNITY_5_4_OR_NEWER

using UnityEngine;

public class SEGICascadedPreset : ScriptableObject
{
    public SEGICascaded.VoxelResolution voxelResolution = SEGICascaded.VoxelResolution.high;
    public bool voxelAA = true;
    [Range(0, 2)]
    public int innerOcclusionLayers = 1;
    public bool infiniteBounces = true;

    [Range(0.01f, 1.0f)]
    public float temporalBlendWeight = 0.103f;
    public bool useBilateralFiltering = false;
    public bool halfResolution = true; //false;
    public bool stochasticSampling = true;
    public bool doReflections = false;

    [Range(1, 128)]
    public int cones = 11;
    [Range(1, 32)]
    public int coneTraceSteps = 10;
    [Range(0.1f, 2.0f)]
    public float coneLength = 1.0f;
    [Range(0.5f, 6.0f)]
    public float coneWidth = 20.0f;
    [Range(0.0f, 4.0f)]
    public float coneTraceBias = 3f;
    [Range(0.0f, 4.0f)]
    public float occlusionStrength = 0.4f;//0.148f;
    [Range(0.0f, 4.0f)]
    public float nearOcclusionStrength = 0.4f; //0.19f;
    [Range(0.001f, 4.0f)]
    public float occlusionPower = 0.63f;
    [Range(0.0f, 4.0f)]
    public float nearLightGain = 0.36f;
    [Range(0.0f, 4.0f)]
    public float giGain = 3f; //1.0f;
    [Range(0.0f, 2.0f)]
    public float secondaryBounceGain = 0.7f;
    [Range(12, 128)]
    public int reflectionSteps = 64;
    [Range(0.001f, 4.0f)]
    public float reflectionOcclusionPower = 1.0f;
    [Range(0.0f, 1.0f)]
    public float skyReflectionIntensity = 1.0f;
    public bool gaussianMipFilter = false;

    [Range(0.1f, 4.0f)]
    public float farOcclusionStrength = 0.4f; //0.42f;
    [Range(0.1f, 4.0f)]
    public float farthestOcclusionStrength = 0.4f; //0.82f;

    [Range(3, 16)]
    public int secondaryCones = 6;
    [Range(0.1f, 4.0f)]
    public float secondaryOcclusionStrength = 0.271f;
}
