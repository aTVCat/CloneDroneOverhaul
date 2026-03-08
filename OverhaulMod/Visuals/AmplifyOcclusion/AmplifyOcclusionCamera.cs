
// Amplify Occlusion 2 - Robust Ambient Occlusion for Unity
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEngine.Rendering;

namespace AmplifyOcclusion
{

    [RequireComponent(typeof(Camera))]
    public class AmplifyOcclusionEffect : MonoBehaviour
    {
        private static int _nextID = 0;
        private int _myID;
        private string _myIDstring;

        private readonly float _oneOverDepthScale = 1.0f / 65504.0f; // 65504.0f max half float

        public enum ApplicationMethod
        {
            PostEffect = 0,
            Deferred,
            Debug
        }

        public enum PerPixelNormalSource
        {
            None = 0,
            Camera,
            GBuffer,
            GBufferOctaEncoded,
        }

        [Header("Ambient Occlusion")]
        [Tooltip("How to inject the occlusion: Post Effect = Overlay, Deferred = Deferred Injection, Debug - Vizualize.")]
        public ApplicationMethod ApplyMethod = ApplicationMethod.PostEffect;

        [Tooltip("Number of samples per pass.")]
        public SampleCountLevel SampleCount = SampleCountLevel.Medium;

        [Tooltip("Source of per-pixel normals: None = All, Camera = Forward, GBuffer = Deferred.")]
        public PerPixelNormalSource PerPixelNormals = PerPixelNormalSource.Camera;

        [Tooltip("Final applied intensity of the occlusion effect.")]
        [Range(0, 1)]
        public float Intensity = 1.0f;

        [Tooltip("Color tint for occlusion.")]
        public Color Tint = Color.black;

        [Tooltip("Radius spread of the occlusion.")]
        public float Radius = 2.0f;

        [Tooltip("Power exponent attenuation of the occlusion.")]
        [Range(0, 16)]
        public float PowerExponent = 1.8f;

        [Tooltip("Controls the initial occlusion contribution offset.")]
        [Range(0, 0.99f)]
        public float Bias = 0.05f;

        [Tooltip("Controls the thickness occlusion contribution.")]
        [Range(0, 1.0f)]
        public float Thickness = 1.0f;

        [Tooltip("Compute the Occlusion and Blur at half of the resolution.")]
        public bool Downsample = true;

        [Tooltip("Cache optimization for best performance / quality tradeoff.")]
        public bool CacheAware = true;

        [Header("Distance Fade")]
        [Tooltip("Control parameters at faraway.")]
        public bool FadeEnabled = false;

        [Tooltip("Distance in Unity unities that start to fade.")]
        public float FadeStart = 100.0f;

        [Tooltip("Length distance to performe the transition.")]
        public float FadeLength = 50.0f;

        [Tooltip("Final Intensity parameter.")]
        [Range(0, 1)]
        public float FadeToIntensity = 0.0f;
        public Color FadeToTint = Color.black;

        [Tooltip("Final Radius parameter.")]
        public float FadeToRadius = 2.0f;

        [Tooltip("Final PowerExponent parameter.")]
        [Range(0, 16)]
        public float FadeToPowerExponent = 1.0f;

        [Tooltip("Final Thickness parameter.")]
        [Range(0, 1.0f)]
        public float FadeToThickness = 1.0f;

        [Header("Bilateral Blur")]
        public bool BlurEnabled = true;

        [Tooltip("Radius in screen pixels.")]
        [Range(1, 4)]
        public int BlurRadius = 3;

        [Tooltip("Number of times that the Blur will repeat.")]
        [Range(1, 4)]
        public int BlurPasses = 1;

        [Tooltip("Sharpness of blur edge-detection: 0 = Softer Edges, 20 = Sharper Edges.")]
        [Range(0, 20)]
        public float BlurSharpness = 15.0f;

        [Header("Temporal Filter")]
        [Tooltip("Accumulates the effect over the time.")]
        public bool FilterEnabled = true;
        public bool FilterDownsample = true;

        [Tooltip("Controls the accumulation decayment: 0 = More flicker with less ghosting, 1 = Less flicker with more ghosting.")]
        [Range(0, 1)]
        public float FilterBlending = 0.80f;

        [Tooltip("Controls the discard sensitivity based on the motion of the scene and objects.")]
        [Range(0, 1)]
        public float FilterResponse = 0.50f;

        // Current state variables
        private bool _HDR = true;
        private bool _MSAA = true;

        // Previous state variables
        private PerPixelNormalSource _prevPerPixelNormals;
        private ApplicationMethod _prevApplyMethod;
        private bool _prevDeferredReflections = false;
        private SampleCountLevel _prevSampleCount = SampleCountLevel.Low;
        private bool _prevDownsample = false;
        private bool _prevCacheAware = false;
        private bool _prevBlurEnabled = false;
        private int _prevBlurRadius = 0;
        private int _prevBlurPasses = 0;
        private bool _prevFilterEnabled = true;
        private bool _prevFilterDownsample = true;
        private bool _prevHDR = true;
        private bool _prevMSAA = true;

#if UNITY_EDITOR
    private bool _prevIsPlaying = false;
#endif

        private Camera _targetCamera;

        private readonly RenderTargetIdentifier[] applyDebugTargetsTemporal = new RenderTargetIdentifier[2];
        private readonly RenderTargetIdentifier[] applyDeferredTargets_Log_Temporal = new RenderTargetIdentifier[3];
        private readonly RenderTargetIdentifier[] applyDeferredTargetsTemporal = new RenderTargetIdentifier[3];
        private readonly RenderTargetIdentifier[] applyOcclusionTemporal = new RenderTargetIdentifier[2];
        private readonly RenderTargetIdentifier[] applyPostEffectTargetsTemporal = new RenderTargetIdentifier[2];

        // NOTE: MotionVectors are not supported in Deferred Injection mode due to 1 frame delay
        private bool UsingTemporalFilter => (_sampleStep > 0) && (FilterEnabled == true) && (_targetCamera.cameraType != UnityEngine.CameraType.SceneView);
        private bool UsingMotionVectors => UsingTemporalFilter && (ApplyMethod != ApplicationMethod.Deferred);
        private bool UsingFilterDownsample => (Downsample == true) && (FilterDownsample == true) && (UsingTemporalFilter == true);

        private bool useMRTBlendingFallback = false;
        private bool checkedforMRTBlendingFallback = false;

        // Command buffer
        private struct CmdBuffer
        {
            public CommandBuffer cmdBuffer;
            public CameraEvent cmdBufferEvent;
            public string cmdBufferName;
        }

        private CmdBuffer _commandBuffer_Parameters;
        private CmdBuffer _commandBuffer_Occlusion;
        private CmdBuffer _commandBuffer_Apply;

        private void createCommandBuffer(ref CmdBuffer aCmdBuffer, string aCmdBufferName, CameraEvent aCameraEvent)
        {
            if (aCmdBuffer.cmdBuffer != null)
            {
                cleanupCommandBuffer(ref aCmdBuffer);
            }

            aCmdBuffer.cmdBufferName = aCmdBufferName;

            aCmdBuffer.cmdBuffer = new CommandBuffer
            {
                name = aCmdBufferName
            };

            aCmdBuffer.cmdBufferEvent = aCameraEvent;

            _targetCamera.AddCommandBuffer(aCameraEvent, aCmdBuffer.cmdBuffer);
        }

        private void cleanupCommandBuffer(ref CmdBuffer aCmdBuffer)
        {
            CommandBuffer[] currentCBs = _targetCamera.GetCommandBuffers(aCmdBuffer.cmdBufferEvent);

            for (int i = 0; i < currentCBs.Length; i++)
            {
                if (currentCBs[i].name == aCmdBuffer.cmdBufferName)
                {
                    _targetCamera.RemoveCommandBuffer(aCmdBuffer.cmdBufferEvent, currentCBs[i]);
                }
            }

            aCmdBuffer.cmdBufferName = null;
            aCmdBuffer.cmdBufferEvent = 0;
            aCmdBuffer.cmdBuffer = null;
        }

        // Quad Mesh
        private static Mesh _quadMesh = null;

        private void createQuadMesh()
        {
            if (_quadMesh == null)
            {
                _quadMesh = new Mesh
                {
                    vertices = new Vector3[4] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) },
                    uv = new Vector2[4] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) },
                    triangles = new int[6] { 0, 1, 2, 0, 2, 3 },

                    normals = new Vector3[0],
                    tangents = new Vector4[0],
                    colors32 = new Color32[0],
                    colors = new Color[0]
                };
            }
        }

        private void PerformBlit(CommandBuffer cb, Material mat, int pass)
        {
            cb.DrawMesh(_quadMesh, Matrix4x4.identity, mat, 0, pass);
        }

        // Render Materials
        private static Material _occlusionMat;
        private static Material _blurMat;
        private static Material _applyOcclusionMat;

        private void checkMaterials(bool aThroughErrorMsg)
        {
            if (!_occlusionMat)
            {
                _occlusionMat = AmplifyOcclusionCommon.CreateMaterialWithShaderName("Hidden/Amplify Occlusion/Occlusion", aThroughErrorMsg);
            }
            if (!_blurMat)
            {
                _blurMat = AmplifyOcclusionCommon.CreateMaterialWithShaderName("Hidden/Amplify Occlusion/Blur", aThroughErrorMsg);
            }
            if (!_applyOcclusionMat)
            {
                _applyOcclusionMat = AmplifyOcclusionCommon.CreateMaterialWithShaderName("Hidden/Amplify Occlusion/Apply", aThroughErrorMsg);
            }

            if (_applyOcclusionMat != null)
            {
                if (checkedforMRTBlendingFallback == false)
                {
                    checkedforMRTBlendingFallback = true;

                    // some platforms still don't support MRT-blending; provide a fallback, if necessary
                    useMRTBlendingFallback = _applyOcclusionMat.GetTag("MRTBlending", false).ToUpper() != "TRUE";
                }
            }
        }

        private RenderTextureFormat _occlusionRTFormat = RenderTextureFormat.RGHalf;
        private readonly RenderTextureFormat _accumTemporalRTFormat = RenderTextureFormat.ARGB32;
        private readonly RenderTextureFormat _temporaryEmissionRTFormat = RenderTextureFormat.ARGB2101010;
        private readonly RenderTextureFormat _motionIntensityRTFormat = RenderTextureFormat.R8;


        private bool checkRenderTextureFormats()
        {
            // test the two fallback formats first
            if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32) && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
            {
                _occlusionRTFormat = RenderTextureFormat.RGHalf;
                if (!SystemInfo.SupportsRenderTextureFormat(_occlusionRTFormat))
                {
                    _occlusionRTFormat = RenderTextureFormat.RGFloat;
                    if (!SystemInfo.SupportsRenderTextureFormat(_occlusionRTFormat))
                    {
                        // already tested above
                        _occlusionRTFormat = RenderTextureFormat.ARGBHalf;
                    }
                }

                return true;
            }
            return false;
        }

        private void OnEnable()
        {
            _myID = _nextID;
            _myIDstring = _myID.ToString();
            _nextID++;

            if (!checkRenderTextureFormats())
            {
                //Debug.LogError("[AmplifyOcclusion] Target platform does not meet the minimum requirements for this effect to work properly.");

                enabled = false;

                return;
            }

            if (CacheAware == true)
            {
                if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RFloat) == false)
                {
                    CacheAware = false;
                    UnityEngine.Debug.LogWarning("[AmplifyOcclusion] System does not support RFloat RenderTextureFormat. CacheAware will be disabled.");
                }
                else
                {
                    if (SystemInfo.copyTextureSupport == CopyTextureSupport.None)
                    {
                        CacheAware = false;
                        UnityEngine.Debug.LogWarning("[AmplifyOcclusion] System does not support CopyTexture. CacheAware will be disabled.");
                    }
                    else
                    {
                        // AO-62 - some OpenGLES devices actually implement RFloat buffers using RHalf format.
                        if ((SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2) ||
                            (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3))
                        {
                            CacheAware = false;
                            UnityEngine.Debug.LogWarningFormat("[AmplifyOcclusion] CacheAware is not supported on {0} devices. CacheAware will be disabled.", SystemInfo.graphicsDeviceType);
                        }
                    }
                }
            }

            checkMaterials(false);
            createQuadMesh();

#if UNITY_2017_1_OR_NEWER
            if( GraphicsSettings.HasShaderDefine( Graphics.activeTier, BuiltinShaderDefine.SHADER_API_MOBILE ) )
            {
                // using 16376.0 for DepthScale for mobile due to precision issues
                _oneOverDepthScale = 1.0f / 16376.0f;
            }
#else
#if UNITY_IPHONE || UNITY_ANDROID
            _oneOverDepthScale = 1.0f / 16376.0f;
#endif
#endif
        }

        private void Reset()
        {
            if (_commandBuffer_Parameters.cmdBuffer != null)
            {
                cleanupCommandBuffer(ref _commandBuffer_Parameters);
            }

            if (_commandBuffer_Occlusion.cmdBuffer != null)
            {
                cleanupCommandBuffer(ref _commandBuffer_Occlusion);
            }

            if (_commandBuffer_Apply.cmdBuffer != null)
            {
                cleanupCommandBuffer(ref _commandBuffer_Apply);
            }

            AmplifyOcclusionCommon.SafeReleaseRT(ref _occlusionDepthRT);
            AmplifyOcclusionCommon.SafeReleaseRT(ref _depthMipmap);
            releaseTemporalRT();

            _tmpMipString = null;
        }

        private void OnDisable()
        {
            Reset();
        }

        private void OnDestroy()
        {
            if (_applyOcclusionMat)
                Destroy(_applyOcclusionMat);

            if (_blurMat)
                Destroy(_blurMat);

            if (_occlusionMat)
                Destroy(_occlusionMat);
        }

        private void releaseTemporalRT()
        {
            if (_temporalAccumRT != null)
            {
                for (int i = 0; i < _temporalAccumRT.Length; i++)
                {
                    AmplifyOcclusionCommon.SafeReleaseRT(ref _temporalAccumRT[i]);
                }
            }

            _temporalAccumRT = null;
        }

        private bool _paramsChanged = true;
        private bool _clearHistory = true;

        private void ClearHistory(CommandBuffer cb)
        {
            _clearHistory = false;

            if ((_temporalAccumRT != null) && (_occlusionDepthRT != null))
            {
                for (int i = 0; i < _temporalAccumRT.Length; i++)
                {
                    cb.SetRenderTarget(_temporalAccumRT[i]);
                    PerformBlit(cb, _occlusionMat, ShaderPass.ClearTemporal);
                }
            }
        }

        private void checkParamsChanged()
        {
            bool HDR = _targetCamera.allowHDR; // && tier?
            bool MSAA = _targetCamera.allowMSAA &&
                        _targetCamera.actualRenderingPath != RenderingPath.DeferredLighting &&
                        _targetCamera.actualRenderingPath != RenderingPath.DeferredShading &&
                        QualitySettings.antiAliasing >= 1;

            int antiAliasing = MSAA ? QualitySettings.antiAliasing : 1;

            if (_occlusionDepthRT != null)
            {
                if ((_occlusionDepthRT.width != _target.width) ||
                    (_occlusionDepthRT.height != _target.height) ||
                    (_prevMSAA != MSAA) ||
                    (!_occlusionDepthRT.IsCreated()) ||
                    (_prevFilterEnabled != FilterEnabled) ||
                    (_prevFilterDownsample != UsingFilterDownsample) ||
                    (_temporalAccumRT != null && (!_temporalAccumRT[0].IsCreated() || !_temporalAccumRT[1].IsCreated()))
#if UNITY_EDITOR
                || ( ( _prevIsPlaying == true ) && ( EditorApplication.isPlaying == false ) )
#endif
                    )
                {
                    AmplifyOcclusionCommon.SafeReleaseRT(ref _occlusionDepthRT);
                    AmplifyOcclusionCommon.SafeReleaseRT(ref _depthMipmap);
                    releaseTemporalRT();

                    _paramsChanged = true;
                }
            }

            if (_temporalAccumRT != null)
            {
                if (AmplifyOcclusionCommon.IsStereoMultiPassEnabled(_targetCamera) == true)
                {
                    if (_temporalAccumRT.Length != 4)
                    {
                        _temporalAccumRT = null;
                    }
                }
                else
                {
                    if (_temporalAccumRT.Length != 2)
                    {
                        _temporalAccumRT = null;
                    }
                }
            }

            if (_occlusionDepthRT == null)
            {
                _occlusionDepthRT = AmplifyOcclusionCommon.SafeAllocateRT("_AO_OcclusionDepthTexture",
                                                                            _target.width,
                                                                            _target.height,
                                                                            _occlusionRTFormat,
                                                                            RenderTextureReadWrite.Linear,
                                                                            FilterMode.Bilinear);
            }

            if (_temporalAccumRT == null && FilterEnabled)
            {
                _temporalAccumRT = AmplifyOcclusionCommon.IsStereoMultiPassEnabled(_targetCamera) == true ? (new RenderTexture[4]) : (new RenderTexture[2]);

                for (int i = 0; i < _temporalAccumRT.Length; i++)
                {
                    _temporalAccumRT[i] = AmplifyOcclusionCommon.SafeAllocateRT("_AO_TemporalAccu_" + i.ToString(),
                                                                                    _target.width,
                                                                                    _target.height,
                                                                                    _accumTemporalRTFormat,
                                                                                    RenderTextureReadWrite.Linear,
                                                                                    FilterMode.Bilinear,
                                                                                    antiAliasing);
                }

                _clearHistory = true;
            }

            if ((CacheAware == true) && (_depthMipmap == null))
            {
                _depthMipmap = AmplifyOcclusionCommon.SafeAllocateRT("_AO_DepthMipmap",
                                                                        _target.fullWidth >> 1,
                                                                        _target.fullHeight >> 1,
                                                                        RenderTextureFormat.RFloat,
                                                                        RenderTextureReadWrite.Linear,
                                                                        FilterMode.Point,
                                                                        1,
                                                                        true);

                int minSize = Mathf.Min(_target.fullWidth, _target.fullHeight);
                _numberMips = (int)(Mathf.Log(minSize, 2.0f) + 1.0f) - 1;

                _tmpMipString = null;
                _tmpMipString = new string[_numberMips];

                for (int i = 0; i < _numberMips; i++)
                {
                    _tmpMipString[i] = "_AO_TmpMip_" + i.ToString();
                }
            }
            else
            {
                if ((CacheAware == false) && (_depthMipmap != null))
                {
                    AmplifyOcclusionCommon.SafeReleaseRT(ref _depthMipmap);
                    _tmpMipString = null;
                }
            }

            if ((_prevSampleCount != SampleCount) ||
                (_prevDownsample != Downsample) ||
                (_prevCacheAware != CacheAware) ||
                (_prevBlurEnabled != BlurEnabled) ||
                (((_prevBlurPasses != BlurPasses) ||
                    (_prevBlurRadius != BlurRadius)) && (BlurEnabled == true)) ||
                (_prevFilterEnabled != FilterEnabled) ||
                (_prevFilterDownsample != UsingFilterDownsample) ||
                (_prevHDR != HDR) ||
                (_prevMSAA != MSAA))
            {
                _clearHistory |= _prevHDR != HDR;
                _clearHistory |= _prevMSAA != MSAA;

                _HDR = HDR;
                _MSAA = MSAA;

                _paramsChanged = true;
            }

#if UNITY_EDITOR
        _prevIsPlaying = EditorApplication.isPlaying;
#endif
        }


        private void updateParams()
        {
            _prevSampleCount = SampleCount;
            _prevDownsample = Downsample;
            _prevCacheAware = CacheAware;
            _prevBlurEnabled = BlurEnabled;
            _prevBlurPasses = BlurPasses;
            _prevBlurRadius = BlurRadius;
            _prevFilterEnabled = FilterEnabled;
            _prevFilterDownsample = UsingFilterDownsample;
            _prevHDR = _HDR;
            _prevMSAA = _MSAA;

            _paramsChanged = false;
        }

        private void Update()
        {
            if (_targetCamera != null)
            {
                if (_targetCamera.actualRenderingPath != RenderingPath.DeferredShading)
                {
                    if (PerPixelNormals != PerPixelNormalSource.None && PerPixelNormals != PerPixelNormalSource.Camera)
                    {
                        _paramsChanged = true;
                        PerPixelNormals = PerPixelNormalSource.Camera;

                        if (_targetCamera.cameraType != UnityEngine.CameraType.SceneView)
                        {
                            UnityEngine.Debug.LogWarning("[AmplifyOcclusion] GBuffer Normals only available in Camera Deferred Shading mode. Switched to Camera source.");
                        }
                    }

                    if (ApplyMethod == ApplicationMethod.Deferred)
                    {
                        _paramsChanged = true;
                        ApplyMethod = ApplicationMethod.PostEffect;

                        if (_targetCamera.cameraType != UnityEngine.CameraType.SceneView)
                        {
                            UnityEngine.Debug.LogWarning("[AmplifyOcclusion] Deferred Method requires a Deferred Shading path. Switching to Post Effect Method.");
                        }
                    }
                }
                else
                {
                    if (PerPixelNormals == PerPixelNormalSource.Camera)
                    {
                        _paramsChanged = true;
                        PerPixelNormals = PerPixelNormalSource.GBuffer;

                        if (_targetCamera.cameraType != UnityEngine.CameraType.SceneView)
                        {
                            UnityEngine.Debug.LogWarning("[AmplifyOcclusion] Camera Normals not supported for Deferred Method. Switching to GBuffer Normals.");
                        }
                    }
                }

                if ((_targetCamera.depthTextureMode & DepthTextureMode.Depth) == 0)
                {
                    _targetCamera.depthTextureMode |= DepthTextureMode.Depth;
                }

                if ((PerPixelNormals == PerPixelNormalSource.Camera) &&
                        (_targetCamera.depthTextureMode & DepthTextureMode.DepthNormals) == 0)
                {
                    _targetCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
                }

                if ((UsingMotionVectors == true) &&
                    (_targetCamera.depthTextureMode & DepthTextureMode.MotionVectors) == 0)
                {
                    _targetCamera.depthTextureMode |= DepthTextureMode.MotionVectors;
                }

            }
            else
            {
                _targetCamera = GetComponent<Camera>();
            }
        }

        private void OnPreRender()
        {
            checkMaterials(true);

            if (_targetCamera != null)
            {
                bool deferredReflections = GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredReflections) != BuiltinShaderMode.Disabled;

                if ((_prevPerPixelNormals != PerPixelNormals) ||
                    (_prevApplyMethod != ApplyMethod) ||
                    (_prevDeferredReflections != deferredReflections) ||
                    (_commandBuffer_Parameters.cmdBuffer == null) ||
                    (_commandBuffer_Occlusion.cmdBuffer == null) ||
                    (_commandBuffer_Apply.cmdBuffer == null)
                    )
                {
                    CameraEvent cameraStage = CameraEvent.BeforeImageEffectsOpaque;
                    if (ApplyMethod == ApplicationMethod.Deferred)
                    {
                        cameraStage = deferredReflections ? CameraEvent.BeforeReflections : CameraEvent.BeforeLighting;
                    }

                    createCommandBuffer(ref _commandBuffer_Parameters, "AmplifyOcclusion_Parameters_" + _myIDstring, cameraStage);
                    createCommandBuffer(ref _commandBuffer_Occlusion, "AmplifyOcclusion_Compute_" + _myIDstring, cameraStage);
                    createCommandBuffer(ref _commandBuffer_Apply, "AmplifyOcclusion_Apply_" + _myIDstring, cameraStage);

                    _prevPerPixelNormals = PerPixelNormals;
                    _prevApplyMethod = ApplyMethod;
                    _prevDeferredReflections = deferredReflections;

                    _paramsChanged = true;
                }

                if ((_commandBuffer_Parameters.cmdBuffer != null) &&
                    (_commandBuffer_Occlusion.cmdBuffer != null) &&
                    (_commandBuffer_Apply.cmdBuffer != null))
                {
                    if (AmplifyOcclusionCommon.IsStereoMultiPassEnabled(_targetCamera) == true)
                    {
                        uint curStepIdx = (_sampleStep >> 1) & 1;
                        uint curEyeIdx = _sampleStep & 1;
                        _curTemporalIdx = (curEyeIdx * 2) + 0 + curStepIdx;
                        _prevTemporalIdx = (curEyeIdx * 2) + (1 - curStepIdx);
                    }
                    else
                    {
                        uint curStepIdx = _sampleStep & 1;
                        _curTemporalIdx = 0 + curStepIdx;
                        _prevTemporalIdx = 1 - curStepIdx;
                    }

                    _commandBuffer_Parameters.cmdBuffer.Clear();

                    UpdateGlobalShaderConstants(_commandBuffer_Parameters.cmdBuffer);

                    UpdateGlobalShaderConstants_Matrices(_commandBuffer_Parameters.cmdBuffer);

                    UpdateGlobalShaderConstants_AmbientOcclusion(_commandBuffer_Parameters.cmdBuffer);

                    checkParamsChanged();

                    if (_paramsChanged)
                    {
                        _commandBuffer_Occlusion.cmdBuffer.Clear();

                        commandBuffer_FillComputeOcclusion(_commandBuffer_Occlusion.cmdBuffer);
                    }

                    _commandBuffer_Apply.cmdBuffer.Clear();

                    if (ApplyMethod == ApplicationMethod.Debug)
                    {
                        commandBuffer_FillApplyDebug(_commandBuffer_Apply.cmdBuffer);
                    }
                    else
                    {
                        if (ApplyMethod == ApplicationMethod.PostEffect)
                        {
                            commandBuffer_FillApplyPostEffect(_commandBuffer_Apply.cmdBuffer);
                        }
                        else
                        {
                            bool logTarget = !_HDR;

                            commandBuffer_FillApplyDeferred(_commandBuffer_Apply.cmdBuffer, logTarget);
                        }
                    }

                    updateParams();

                    _sampleStep++; // No clamp, free running counter
                }
            }
            else
            {
                _targetCamera = GetComponent<Camera>();
                Update();
            }
        }

        private void OnPostRender()
        {
            if (_occlusionDepthRT != null)
            {
                _occlusionDepthRT.MarkRestoreExpected();
            }
            if (_temporalAccumRT != null)
            {
                foreach (RenderTexture rt in _temporalAccumRT)
                {
                    rt.MarkRestoreExpected();
                }
            }
        }


        private RenderTexture _occlusionDepthRT = null;
        private RenderTexture[] _temporalAccumRT = null;
        private RenderTexture _depthMipmap = null;

        private uint _sampleStep = 0;
        private uint _curTemporalIdx = 0;
        private uint _prevTemporalIdx = 0;

        private string[] _tmpMipString = null;
        private int _numberMips = 0;
        private void commandBuffer_FillComputeOcclusion(CommandBuffer cb)
        {
            cb.BeginSample("AO 1 - ComputeOcclusion");

            if ((PerPixelNormals == PerPixelNormalSource.GBuffer) ||
                (PerPixelNormals == PerPixelNormalSource.GBufferOctaEncoded))
            {
                cb.SetGlobalTexture(PropertyID._AO_GBufferNormals, BuiltinRenderTextureType.GBuffer2);
            }

            Vector4 oneOverFullSize_Size = new Vector4(1.0f / _target.fullWidth,
                                                        1.0f / _target.fullHeight,
                                                        _target.fullWidth,
                                                        _target.fullHeight);

            int sampleCountPass = ((int)SampleCount) * AmplifyOcclusionCommon.PerPixelNormalSourceCount;

            int occlusionPass = ShaderPass.OcclusionLow_None +
                                    sampleCountPass +
                                    ((int)PerPixelNormals);

            if (CacheAware == true)
            {
                occlusionPass += ShaderPass.OcclusionLow_None_UseDynamicDepthMips;

                // Construct Depth mipmaps
                int previouslyTmpMipRT = 0;

                for (int i = 0; i < _numberMips; i++)
                {
                    int tmpMipRT;

                    int width = _target.fullWidth >> (i + 1);
                    int height = _target.fullHeight >> (i + 1);

                    tmpMipRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, _tmpMipString[i],
                                                                                width, height,
                                                                                RenderTextureFormat.RFloat,
                                                                                RenderTextureReadWrite.Linear,
                                                                                FilterMode.Bilinear);

                    // _AO_CurrDepthSource was previously set
                    cb.SetRenderTarget(tmpMipRT);

                    PerformBlit(cb, _occlusionMat, (i == 0) ? ShaderPass.ScaleDownCloserDepthEven_CameraDepthTexture : ShaderPass.ScaleDownCloserDepthEven);

                    cb.CopyTexture(tmpMipRT, 0, 0, _depthMipmap, 0, i);

                    if (previouslyTmpMipRT != 0)
                    {
                        AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, previouslyTmpMipRT);
                    }

                    previouslyTmpMipRT = tmpMipRT;

                    cb.SetGlobalTexture(PropertyID._AO_CurrDepthSource, tmpMipRT); // Set next MipRT ID
                }

                AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, previouslyTmpMipRT);

                cb.SetGlobalTexture(PropertyID._AO_SourceDepthMipmap, _depthMipmap);
            }

            if ((Downsample == true) && (UsingFilterDownsample == false))
            {
                int halfWidth = _target.fullWidth / 2;
                int halfHeight = _target.fullHeight / 2;

                int tmpSmallOcclusionRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_SmallOcclusionTexture",
                                                                    halfWidth, halfHeight,
                                                                    _occlusionRTFormat,
                                                                    RenderTextureReadWrite.Linear,
                                                                    FilterMode.Bilinear);

                cb.SetGlobalVector(PropertyID._AO_Source_TexelSize, oneOverFullSize_Size);
                cb.SetGlobalVector(PropertyID._AO_Target_TexelSize, new Vector4(1.0f / (_target.fullWidth / 2.0f),
                                                                                  1.0f / (_target.fullHeight / 2.0f),
                                                                                  _target.fullWidth / 2.0f,
                                                                                  _target.fullHeight / 2.0f));

                cb.SetRenderTarget(tmpSmallOcclusionRT);
                PerformBlit(cb, _occlusionMat, occlusionPass);

                cb.SetRenderTarget(default(RenderTexture));
                cb.EndSample("AO 1 - ComputeOcclusion");

                if (BlurEnabled == true)
                {
                    commandBuffer_Blur(cb, tmpSmallOcclusionRT, halfWidth, halfHeight);
                }

                // Combine
                cb.BeginSample("AO 2b - Combine");

                cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, tmpSmallOcclusionRT);

                cb.SetGlobalVector(PropertyID._AO_Target_TexelSize, oneOverFullSize_Size);

                cb.SetRenderTarget(_occlusionDepthRT);

                PerformBlit(cb, _occlusionMat, ShaderPass.CombineDownsampledOcclusionDepth);

                AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpSmallOcclusionRT);

                cb.SetRenderTarget(default(RenderTexture));
                cb.EndSample("AO 2b - Combine");
            }
            else
            {
                cb.SetGlobalVector(PropertyID._AO_Source_TexelSize, oneOverFullSize_Size);

                if (UsingFilterDownsample == true)
                {
                    // Must use proper float precision 2.0 division to avoid artefacts
                    cb.SetGlobalVector(PropertyID._AO_Target_TexelSize, new Vector4(1.0f / (_target.fullWidth / 2.0f),
                                                                                      1.0f / (_target.fullHeight / 2.0f),
                                                                                      _target.fullWidth / 2.0f,
                                                                                      _target.fullHeight / 2.0f));
                }
                else
                {
                    cb.SetGlobalVector(PropertyID._AO_Target_TexelSize, new Vector4(1.0f / _target.width,
                                                                                      1.0f / _target.height,
                                                                                      _target.width,
                                                                                      _target.height));
                }

                cb.SetRenderTarget(_occlusionDepthRT);
                PerformBlit(cb, _occlusionMat, occlusionPass);

                cb.SetRenderTarget(default(RenderTexture));
                cb.EndSample("AO 1 - ComputeOcclusion");

                if (BlurEnabled == true)
                {
                    commandBuffer_Blur(cb, _occlusionDepthRT, _target.width, _target.height);
                }
            }
        }

        private int commandBuffer_NeighborMotionIntensity(CommandBuffer cb, int aSourceWidth, int aSourceHeight)
        {
            int tmpRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_IntensityTmp",
                                                                        aSourceWidth / 4, aSourceHeight / 4,
                                                                        _motionIntensityRTFormat,
                                                                        RenderTextureReadWrite.Linear,
                                                                        FilterMode.Bilinear);


            cb.SetRenderTarget(tmpRT);
            cb.SetGlobalVector("_AO_Target_TexelSize", new Vector4(1.0f / (aSourceWidth / 4.0f),
                                                                     1.0f / (aSourceHeight / 4.0f),
                                                                     aSourceWidth / 4.0f,
                                                                     aSourceHeight / 4.0f));


            PerformBlit(cb, _occlusionMat, ShaderPass.NeighborMotionIntensity);

            int tmpBlurRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_BlurIntensityTmp",
                                                                            aSourceWidth / 4, aSourceHeight / 4,
                                                                            _motionIntensityRTFormat,
                                                                            RenderTextureReadWrite.Linear,
                                                                            FilterMode.Bilinear);

            // Horizontal
            cb.SetGlobalTexture(PropertyID._AO_CurrMotionIntensity, tmpRT);
            cb.SetRenderTarget(tmpBlurRT);
            PerformBlit(cb, _blurMat, ShaderPass.BlurHorizontalIntensity);

            // Vertical
            cb.SetGlobalTexture(PropertyID._AO_CurrMotionIntensity, tmpBlurRT);
            cb.SetRenderTarget(tmpRT);
            PerformBlit(cb, _blurMat, ShaderPass.BlurVerticalIntensity);

            AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpBlurRT);

            cb.SetGlobalTexture(PropertyID._AO_CurrMotionIntensity, tmpRT);

            return tmpRT;
        }

        private void commandBuffer_Blur(CommandBuffer cb, RenderTargetIdentifier aSourceRT, int aSourceWidth, int aSourceHeight)
        {
            cb.BeginSample("AO 2 - Blur");

            int tmpBlurRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_BlurTmp",
                                                                            aSourceWidth, aSourceHeight,
                                                                            _occlusionRTFormat,
                                                                            RenderTextureReadWrite.Linear,
                                                                            FilterMode.Bilinear);

            // Apply Cross Bilateral Blur
            for (int i = 0; i < BlurPasses; i++)
            {
                // Horizontal
                cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, aSourceRT);

                int blurHorizontalPass = ShaderPass.BlurHorizontal1 + ((BlurRadius - 1) * 2);

                cb.SetRenderTarget(tmpBlurRT);

                PerformBlit(cb, _blurMat, blurHorizontalPass);


                // Vertical
                cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, tmpBlurRT);

                int blurVerticalPass = ShaderPass.BlurVertical1 + ((BlurRadius - 1) * 2);

                cb.SetRenderTarget(aSourceRT);

                PerformBlit(cb, _blurMat, blurVerticalPass);
            }

            AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpBlurRT);

            cb.SetRenderTarget(default(RenderTexture));
            cb.EndSample("AO 2 - Blur");
        }

        private int getTemporalPass()
        {
            return ((UsingMotionVectors == true) && (_sampleStep > 1)) ? (1 << 0) : 0;
        }

        private void commandBuffer_TemporalFilter(CommandBuffer cb)
        {
            if (_clearHistory == true)
            {
                ClearHistory(cb);
            }

            // Temporal Filter
            float temporalAdj = Mathf.Lerp(0.01f, 0.99f, FilterBlending);

            cb.SetGlobalFloat(PropertyID._AO_TemporalCurveAdj, temporalAdj);
            cb.SetGlobalFloat(PropertyID._AO_TemporalMotionSensibility, (FilterResponse * FilterResponse) + 0.01f);

            cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, _occlusionDepthRT);
            cb.SetGlobalTexture(PropertyID._AO_TemporalAccumm, _temporalAccumRT[_prevTemporalIdx]);
        }

        private readonly RenderTargetIdentifier[] _applyDeferredTargets =
        {
        BuiltinRenderTextureType.GBuffer0,		// RGB: Albedo, A: Occ
        BuiltinRenderTextureType.CameraTarget,	// RGB: Emission, A: None
    };

        private readonly RenderTargetIdentifier[] _applyDeferredTargets_Log =
        {
        BuiltinRenderTextureType.GBuffer0,		// RGB: Albedo, A: Occ
        BuiltinRenderTextureType.GBuffer3		// RGB: Emission, A: None
    };

        private void commandBuffer_FillApplyDeferred(CommandBuffer cb, bool logTarget)
        {
            cb.BeginSample("AO 3 - ApplyDeferred");

            if (!logTarget)
            {
                if (UsingTemporalFilter)
                {
                    commandBuffer_TemporalFilter(cb);

                    int tmpMotionIntensityRT = 0;

                    if (UsingMotionVectors == true)
                    {
                        tmpMotionIntensityRT = commandBuffer_NeighborMotionIntensity(cb, _target.fullWidth, _target.fullHeight);
                    }

                    if (UsingFilterDownsample == false)
                    {
                        int applyOcclusionRT = 0;
                        if (useMRTBlendingFallback)
                        {
                            applyOcclusionRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_ApplyOcclusionTexture", _target.fullWidth, _target.fullHeight, RenderTextureFormat.ARGB32);

                            applyOcclusionTemporal[0] = applyOcclusionRT;
                            applyOcclusionTemporal[1] = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                            cb.SetRenderTarget(applyOcclusionTemporal, applyOcclusionTemporal[0] /* Not used, just to make Unity happy */ );
                            PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyPostEffectTemporal + getTemporalPass()); // re-use ApplyPostEffectTemporal pass to apply without Blend to the RT.
                        }
                        else
                        {
                            applyDeferredTargetsTemporal[0] = _applyDeferredTargets[0];
                            applyDeferredTargetsTemporal[1] = _applyDeferredTargets[1];
                            applyDeferredTargetsTemporal[2] = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                            cb.SetRenderTarget(applyDeferredTargetsTemporal, applyDeferredTargetsTemporal[0] /* Not used, just to make Unity happy */ );
                            PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredTemporal + getTemporalPass());
                        }

                        if (useMRTBlendingFallback)
                        {
                            cb.SetGlobalTexture("_AO_ApplyOcclusionTexture", applyOcclusionRT);

                            applyOcclusionTemporal[0] = _applyDeferredTargets[0];
                            applyOcclusionTemporal[1] = _applyDeferredTargets[1];

                            cb.SetRenderTarget(applyOcclusionTemporal, applyOcclusionTemporal[0] /* Not used, just to make Unity happy */ );
                            PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredTemporalMultiply);

                            AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, applyOcclusionRT);
                        }
                    }
                    else
                    {
                        // UsingFilterDownsample == true

                        RenderTargetIdentifier temporalRTid = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                        cb.SetRenderTarget(temporalRTid);
                        PerformBlit(cb, _occlusionMat, ShaderPass.Temporal + getTemporalPass());

                        cb.SetGlobalTexture(PropertyID._AO_TemporalAccumm, temporalRTid);
                        cb.SetRenderTarget(_applyDeferredTargets, _applyDeferredTargets[0] /* Not used, just to make Unity happy */ );
                        PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredCombineFromTemporal);
                    }

                    if (UsingMotionVectors == true)
                    {
                        AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpMotionIntensityRT);
                    }
                }
                else
                {
                    cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, _occlusionDepthRT);

                    // Multiply Occlusion
                    cb.SetRenderTarget(_applyDeferredTargets, _applyDeferredTargets[0] /* Not used, just to make Unity happy */ );
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferred);
                }
            }
            else
            {
                // Copy Albedo and Emission to temporary buffers
                int gbufferAlbedoRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_tmpAlbedo",
                                                                                        _target.fullWidth, _target.fullHeight,
                                                                                        RenderTextureFormat.ARGB32);

                int gbufferEmissionRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_tmpEmission",
                                                                                        _target.fullWidth, _target.fullHeight,
                                                                                        _temporaryEmissionRTFormat);

                cb.Blit(BuiltinRenderTextureType.GBuffer0, gbufferAlbedoRT);
                cb.Blit(BuiltinRenderTextureType.GBuffer3, gbufferEmissionRT);

                cb.SetGlobalTexture(PropertyID._AO_GBufferAlbedo, gbufferAlbedoRT);
                cb.SetGlobalTexture(PropertyID._AO_GBufferEmission, gbufferEmissionRT);

                if (UsingTemporalFilter)
                {
                    commandBuffer_TemporalFilter(cb);

                    int tmpMotionIntensityRT = 0;

                    if (UsingMotionVectors == true)
                    {
                        tmpMotionIntensityRT = commandBuffer_NeighborMotionIntensity(cb, _target.fullWidth, _target.fullHeight);
                    }

                    if (UsingFilterDownsample == false)
                    {
                        applyDeferredTargets_Log_Temporal[0] = _applyDeferredTargets_Log[0];
                        applyDeferredTargets_Log_Temporal[1] = _applyDeferredTargets_Log[1];
                        applyDeferredTargets_Log_Temporal[2] = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                        cb.SetRenderTarget(applyDeferredTargets_Log_Temporal, applyDeferredTargets_Log_Temporal[0] /* Not used, just to make Unity happy */ );
                        PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredLogTemporal + getTemporalPass());
                    }
                    else
                    {
                        // UsingFilterDownsample == true

                        RenderTargetIdentifier temporalRTid = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                        cb.SetRenderTarget(temporalRTid);
                        PerformBlit(cb, _occlusionMat, ShaderPass.Temporal + getTemporalPass());

                        cb.SetGlobalTexture(PropertyID._AO_TemporalAccumm, temporalRTid);
                        cb.SetRenderTarget(_applyDeferredTargets_Log, _applyDeferredTargets_Log[0] /* Not used, just to make Unity happy */ );
                        PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredLogCombineFromTemporal);
                    }

                    if (UsingMotionVectors == true)
                    {
                        AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpMotionIntensityRT);
                    }
                }
                else
                {
                    cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, _occlusionDepthRT);

                    cb.SetRenderTarget(_applyDeferredTargets_Log, _applyDeferredTargets_Log[0] /* Not used, just to make Unity happy */ );
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDeferredLog);
                }

                AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, gbufferAlbedoRT);
                AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, gbufferEmissionRT);
            }

            cb.SetRenderTarget(default(RenderTexture));
            cb.EndSample("AO 3 - ApplyDeferred");
        }

        private void commandBuffer_FillApplyPostEffect(CommandBuffer cb)
        {
            cb.BeginSample("AO 3 - ApplyPostEffect");

            if (UsingTemporalFilter)
            {
                commandBuffer_TemporalFilter(cb);

                int tmpMotionIntensityRT = 0;

                if (UsingMotionVectors == true)
                {
                    tmpMotionIntensityRT = commandBuffer_NeighborMotionIntensity(cb, _target.fullWidth, _target.fullHeight);
                }

                if (UsingFilterDownsample == false)
                {
                    int applyOcclusionRT = 0;
                    if (useMRTBlendingFallback)
                    {
                        applyOcclusionRT = AmplifyOcclusionCommon.SafeAllocateTemporaryRT(cb, "_AO_ApplyOcclusionTexture", _target.fullWidth, _target.fullHeight, RenderTextureFormat.ARGB32);
                        applyPostEffectTargetsTemporal[0] = applyOcclusionRT;
                    }
                    else
                    {
                        applyPostEffectTargetsTemporal[0] = BuiltinRenderTextureType.CameraTarget;
                    }

                    applyPostEffectTargetsTemporal[1] = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                    cb.SetRenderTarget(applyPostEffectTargetsTemporal, applyPostEffectTargetsTemporal[0] /* Not used, just to make Unity happy */ );
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyPostEffectTemporal + getTemporalPass());

                    if (useMRTBlendingFallback)
                    {
                        cb.SetGlobalTexture("_AO_ApplyOcclusionTexture", applyOcclusionRT);

                        cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                        PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyPostEffectTemporalMultiply);

                        AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, applyOcclusionRT);
                    }
                }
                else
                {
                    // UsingFilterDownsample == true

                    RenderTargetIdentifier temporalRTid = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                    cb.SetRenderTarget(temporalRTid);
                    PerformBlit(cb, _occlusionMat, ShaderPass.Temporal + getTemporalPass());

                    cb.SetGlobalTexture(PropertyID._AO_TemporalAccumm, temporalRTid);
                    cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyCombineFromTemporal);
                }

                if (UsingMotionVectors == true)
                {
                    AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpMotionIntensityRT);
                }
            }
            else
            {
                cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, _occlusionDepthRT);

                cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyPostEffect);
            }

            cb.SetRenderTarget(default(RenderTexture));
            cb.EndSample("AO 3 - ApplyPostEffect");
        }

        private void commandBuffer_FillApplyDebug(CommandBuffer cb)
        {
            cb.BeginSample("AO 3 - ApplyDebug");

            if (UsingTemporalFilter)
            {
                commandBuffer_TemporalFilter(cb);

                int tmpMotionIntensityRT = 0;

                if (UsingMotionVectors == true)
                {
                    tmpMotionIntensityRT = commandBuffer_NeighborMotionIntensity(cb, _target.fullWidth, _target.fullHeight);
                }

                if (UsingFilterDownsample == false)
                {
                    applyDebugTargetsTemporal[0] = BuiltinRenderTextureType.CameraTarget;
                    applyDebugTargetsTemporal[1] = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                    cb.SetRenderTarget(applyDebugTargetsTemporal, applyDebugTargetsTemporal[0] /* Not used, just to make Unity happy */ );
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDebugTemporal + getTemporalPass());
                }
                else
                {
                    // UsingFilterDownsample == true

                    RenderTargetIdentifier temporalRTid = new RenderTargetIdentifier(_temporalAccumRT[_curTemporalIdx]);

                    cb.SetRenderTarget(temporalRTid);
                    PerformBlit(cb, _occlusionMat, ShaderPass.Temporal + getTemporalPass());

                    cb.SetGlobalTexture(PropertyID._AO_TemporalAccumm, temporalRTid);
                    cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                    PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDebugCombineFromTemporal);
                }

                if (UsingMotionVectors == true)
                {
                    AmplifyOcclusionCommon.SafeReleaseTemporaryRT(cb, tmpMotionIntensityRT);
                }
            }
            else
            {
                cb.SetGlobalTexture(PropertyID._AO_CurrOcclusionDepth, _occlusionDepthRT);

                cb.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);
                PerformBlit(cb, _applyOcclusionMat, ShaderPass.ApplyDebug);
            }

            cb.SetRenderTarget(default(RenderTexture));
            cb.EndSample("AO 3 - ApplyDebug");
        }

        private TargetDesc _target = new TargetDesc();

        private void UpdateGlobalShaderConstants(CommandBuffer cb)
        {
            AmplifyOcclusionCommon.UpdateGlobalShaderConstants(cb, ref _target, _targetCamera, Downsample, UsingFilterDownsample);
        }

        private void UpdateGlobalShaderConstants_AmbientOcclusion(CommandBuffer cb)
        {
            // Ambient Occlusion
            cb.SetGlobalFloat(PropertyID._AO_Radius, Radius);
            cb.SetGlobalFloat(PropertyID._AO_PowExponent, PowerExponent);
            cb.SetGlobalFloat(PropertyID._AO_Bias, Bias * Bias);
            cb.SetGlobalColor(PropertyID._AO_Levels, new Color(Tint.r, Tint.g, Tint.b, Intensity));

            float invThickness = 1.0f - Thickness;
            cb.SetGlobalFloat(PropertyID._AO_ThicknessDecay, (1.0f - (invThickness * invThickness)) * 0.98f);

            float AO_BufDepthToLinearEye = _targetCamera.farClipPlane * _oneOverDepthScale;
            cb.SetGlobalFloat(PropertyID._AO_BufDepthToLinearEye, AO_BufDepthToLinearEye);

            if (BlurEnabled == true)
            {
                float AO_BlurSharpness = BlurSharpness * 100.0f * AO_BufDepthToLinearEye;

                cb.SetGlobalFloat(PropertyID._AO_BlurSharpness, AO_BlurSharpness);
            }

            // Distance Fade
            if (FadeEnabled == true)
            {
                FadeStart = Mathf.Max(0.0f, FadeStart);
                FadeLength = Mathf.Max(0.01f, FadeLength);

                float rcpFadeLength = 1.0f / FadeLength;

                cb.SetGlobalVector(PropertyID._AO_FadeParams, new Vector2(FadeStart, rcpFadeLength));
                float invFadeThickness = 1.0f - FadeToThickness;
                cb.SetGlobalVector(PropertyID._AO_FadeValues, new Vector4(FadeToIntensity, FadeToRadius, FadeToPowerExponent, (1.0f - (invFadeThickness * invFadeThickness)) * 0.98f));
                cb.SetGlobalColor(PropertyID._AO_FadeToTint, new Color(FadeToTint.r, FadeToTint.g, FadeToTint.b, 0.0f));
            }
            else
            {
                cb.SetGlobalVector(PropertyID._AO_FadeParams, new Vector2(0.0f, 0.0f));
            }

            if (UsingTemporalFilter == true)
            {
                AmplifyOcclusionCommon.CommandBuffer_TemporalFilterDirectionsOffsets(cb, _sampleStep);
            }
            else
            {
                cb.SetGlobalFloat(PropertyID._AO_TemporalDirections, 0);
                cb.SetGlobalFloat(PropertyID._AO_TemporalOffsets, 0);
            }
        }

        private readonly AmplifyOcclusionViewProjMatrix _viewProjMatrix = new AmplifyOcclusionViewProjMatrix();

        private void UpdateGlobalShaderConstants_Matrices(CommandBuffer cb)
        {
            _viewProjMatrix.UpdateGlobalShaderConstants_Matrices(cb, _targetCamera, UsingTemporalFilter);
        }
    }
}