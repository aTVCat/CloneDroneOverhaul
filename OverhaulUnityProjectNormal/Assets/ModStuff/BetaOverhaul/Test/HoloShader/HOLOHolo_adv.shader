Shader "HOLO/Holo_adv" {
	Properties {
		_MainTex ("Not used confusing", 2D) = "white" {}
		_originalDiffuse ("Original Diffuse Map", 2D) = "white" {}
		_Diffuse ("Diffuse Map", 2D) = "white" {}
		[HDR] _diff_Color ("Diffuse Color Mult", Vector) = (1,1,1,1)
		_N_map ("Noise", 2D) = "white" {}
		_M_map ("Mask", 2D) = "white" {}
		[Toggle] _mask_type ("Use Map as Mask", Float) = 1
		_intensity ("Intensity", Range(0, 10)) = 0
		_deform ("Deformation intensity", Float) = 1
		[HDR] _Color ("Outline Color Mult", Vector) = (1,1,1,1)
		_Opacity ("Base Opacity", Range(0, 1)) = 0
		_Bias ("Bias", Range(0, 1)) = 0
		_Scale ("Scale ", Range(0, 10)) = 0
		_Power ("Power", Range(0, 3)) = 0
		_Speed ("Speed", Range(0, 1)) = 0
		_t ("Extra Option", Range(0, 1)) = 0
		[Toggle] _X ("Active X Axe", Float) = 1
		[Toggle] _Y ("Active Y Axe", Float) = 1
		[Toggle] _glitchColor ("Glitch/Diffuse Color", Float) = 1
		[HDR] _glitchColor_c ("G/H Color", Vector) = (1,1,1,1)
		[Toggle] _dist_chrom ("Chromatic ", Float) = 1
		_noise_details ("G/H Noise Details Amount", Range(1, 16)) = 0
		_cut_level ("Cut Level", Range(0, 6)) = 0
		_OrigineX ("OrigineX", Range(0, 1)) = 0
		_OrigineY ("OrigineY", Range(0, 1)) = 0
		_Circle_wave ("Wave Circles", Range(0, 100)) = 5
		_Speed_wave ("Wave Speed", Float) = 0
		_Zoom ("Zoom", Range(0.5, 200)) = 1
		_Speed_face ("_Speed_face", Range(0.01, 10)) = 1
		_Rotation ("Rotation", Range(0, 1)) = 0
		[Toggle] _monochrom ("Monochromatic", Float) = 1
		[Toggle] _OriginalUVSwitch ("Switch to Orginal UVs on/off", Float) = 0
	}
	SubShader {
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			GpuProgramID 25250
			Program "vp" {
				SubProgram "d3d11 " {
					"vs_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					layout(std140) uniform VGlobals {
						vec4 unused_0_0[6];
						vec4 _M_map_ST;
						float _intensity;
						float _deform;
						float _Bias;
						float _Scale;
						float _Power;
						float _Speed;
						float _t;
						float _noise_details;
						float _X;
						float _Y;
						float _mask_type;
						vec4 unused_0_13[2];
						float _OrigineX;
						float _OrigineY;
						float _Speed_wave;
						float _Circle_wave;
						vec4 unused_0_18;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_3_2[4];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_4[2];
					};
					uniform  sampler2D _M_map;
					in  vec4 in_POSITION0;
					in  vec4 in_TEXCOORD0;
					in  vec4 in_NORMAL0;
					out vec4 vs_TEXCOORD5;
					out vec4 vs_TEXCOORD0;
					out vec4 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_NORMAL0;
					out vec4 vs_TEXCOORD6;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					float u_xlat9;
					float u_xlat18;
					float u_xlat27;
					float u_xlat28;
					int u_xlati28;
					float u_xlat29;
					int u_xlati29;
					float u_xlat30;
					bool u_xlatb30;
					void main()
					{
					    u_xlat0.xyz = unity_ObjectToWorld[0].yyy * unity_MatrixV[1].xyz;
					    u_xlat0.xyz = unity_MatrixV[0].xyz * unity_ObjectToWorld[0].xxx + u_xlat0.xyz;
					    u_xlat0.xyz = unity_MatrixV[2].xyz * unity_ObjectToWorld[0].zzz + u_xlat0.xyz;
					    u_xlat0.xyz = unity_MatrixV[3].xyz * unity_ObjectToWorld[0].www + u_xlat0.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[1].yyy * unity_MatrixV[1].xyz;
					    u_xlat1.xyz = unity_MatrixV[0].xyz * unity_ObjectToWorld[1].xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_MatrixV[2].xyz * unity_ObjectToWorld[1].zzz + u_xlat1.xyz;
					    u_xlat1.xyz = unity_MatrixV[3].xyz * unity_ObjectToWorld[1].www + u_xlat1.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yyy * unity_MatrixV[1].xyz;
					    u_xlat2.xyz = unity_MatrixV[0].xyz * unity_ObjectToWorld[2].xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_MatrixV[2].xyz * unity_ObjectToWorld[2].zzz + u_xlat2.xyz;
					    u_xlat2.xyz = unity_MatrixV[3].xyz * unity_ObjectToWorld[2].www + u_xlat2.xyz;
					    u_xlat3.xyz = unity_ObjectToWorld[3].yyy * unity_MatrixV[1].xyz;
					    u_xlat3.xyz = unity_MatrixV[0].xyz * unity_ObjectToWorld[3].xxx + u_xlat3.xyz;
					    u_xlat3.xyz = unity_MatrixV[2].xyz * unity_ObjectToWorld[3].zzz + u_xlat3.xyz;
					    u_xlat3.xyz = unity_MatrixV[3].xyz * unity_ObjectToWorld[3].www + u_xlat3.xyz;
					    u_xlat27 = _Speed * _Time.x;
					    u_xlat27 = u_xlat27 * 60.0;
					    u_xlat4.x = sin(u_xlat27);
					    u_xlat5.x = cos(u_xlat27);
					    u_xlat27 = u_xlat4.x + 1.0;
					    u_xlat28 = _Speed_wave * _Time.x;
					    u_xlat4.xy = (-in_TEXCOORD0.xy) + vec2(_OrigineX, _OrigineY);
					    u_xlat4.xy = u_xlat4.xy * u_xlat4.xy;
					    u_xlat29 = u_xlat4.y + u_xlat4.x;
					    u_xlat28 = u_xlat29 * _Circle_wave + u_xlat28;
					    u_xlat28 = sin(u_xlat28);
					    u_xlat28 = u_xlat28 + 1.0;
					    u_xlat28 = u_xlat28 * 0.5;
					    u_xlat29 = (-_mask_type) + 1.0;
					    u_xlat30 = u_xlat5.x * _M_map_ST.y;
					    u_xlat27 = u_xlat27 * _M_map_ST.w;
					    u_xlat4.xy = in_TEXCOORD0.xy * vec2(u_xlat30) + vec2(u_xlat27);
					    u_xlat4 = textureLod(_M_map, u_xlat4.xy, 0.0);
					    u_xlat27 = u_xlat4.x * _mask_type;
					    u_xlat27 = u_xlat28 * u_xlat29 + u_xlat27;
					    u_xlati28 = int(_noise_details);
					    u_xlat4.xyz = _Time.xyz * vec3(0.100000001, 0.100000001, 0.100000001);
					    u_xlat5.xy = in_TEXCOORD0.xy;
					    u_xlat5.z = 0.0;
					    u_xlat6.x = float(0.0);
					    u_xlat6.y = float(0.0);
					    u_xlat6.z = float(0.0);
					    for(int u_xlati_loop_1 = u_xlati28 ; u_xlati_loop_1>0 ; u_xlati_loop_1 = u_xlati_loop_1 + int(0xFFFFFFFFu))
					    {
					        u_xlat7.xyz = floor(u_xlat5.xyz);
					        u_xlat8.xyz = u_xlat4.xyz * u_xlat7.xyz;
					        u_xlat7.xyz = u_xlat7.xxz * u_xlat7.yyy + u_xlat8.xyz;
					        u_xlat7.xyz = sin(u_xlat7.xyz);
					        u_xlat6.xyz = u_xlat6.xyz + u_xlat7.xyz;
					        u_xlat5.xyz = u_xlat5.xyz * vec3(2.5, 2.5, 2.5);
					    }
					    u_xlat4.xy = vec2(_deform, _intensity) * vec2(0.00100000005, 0.100000001);
					    u_xlat4.xzw = u_xlat4.xxx * u_xlat6.xyz;
					    u_xlat4.xzw = vec3(u_xlat27) * u_xlat4.xzw;
					    u_xlat4.xyz = u_xlat4.yyy * u_xlat4.xzw;
					    u_xlat4.xy = u_xlat4.xy * vec2(_X, _Y);
					    u_xlat4.xyz = u_xlat4.xyz * in_NORMAL0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(10.0, 10.0, 10.0) + in_POSITION0.xyz;
					    u_xlat5 = u_xlat4.yyyy * unity_ObjectToWorld[1];
					    u_xlat5 = unity_ObjectToWorld[0] * u_xlat4.xxxx + u_xlat5;
					    u_xlat5 = unity_ObjectToWorld[2] * u_xlat4.zzzz + u_xlat5;
					    u_xlat6 = u_xlat5 + unity_ObjectToWorld[3];
					    u_xlat7 = u_xlat6.yyyy * unity_MatrixVP[1];
					    u_xlat7 = unity_MatrixVP[0] * u_xlat6.xxxx + u_xlat7;
					    u_xlat7 = unity_MatrixVP[2] * u_xlat6.zzzz + u_xlat7;
					    gl_Position = unity_MatrixVP[3] * u_xlat6.wwww + u_xlat7;
					    u_xlat5.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat5.xyz;
					    u_xlat1.xyz = u_xlat1.xyz * in_NORMAL0.yyy;
					    u_xlat0.xyz = u_xlat0.xyz * in_NORMAL0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * in_NORMAL0.zzz + u_xlat0.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * in_NORMAL0.www + u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xy = vec2(u_xlat18) * u_xlat0.xy;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat18 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat1.xyz;
					    vs_TEXCOORD2.xy = u_xlat0.xy * vec2(0.5, 0.5) + vec2(0.5, 0.5);
					    u_xlat0.xyz = u_xlat5.xyz + (-_WorldSpaceCameraPos.xyz);
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat0.x = dot(u_xlat0.xyz, u_xlat1.xyz);
					    u_xlat0.x = u_xlat0.x + _t;
					    u_xlat9 = _Scale * _Bias;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Power;
					    u_xlat0.x = exp2(u_xlat0.x);
					    vs_TEXCOORD4.x = u_xlat0.x * u_xlat9;
					    vs_TEXCOORD5.xyz = u_xlat5.xyz;
					    vs_TEXCOORD5.w = 0.0;
					    vs_TEXCOORD0 = in_TEXCOORD0;
					    vs_TEXCOORD1 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD4.yzw = vec3(0.0, 0.0, 0.0);
					    vs_NORMAL0 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD6.w = in_POSITION0.w;
					    vs_TEXCOORD6.xyz = u_xlat4.xyz;
					    vs_TEXCOORD2.z = 0.0;
					    vs_TEXCOORD3.xyz = u_xlat1.xyz;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					"ps_4_0
					
					#version 330
					#extension GL_ARB_explicit_attrib_location : require
					#extension GL_ARB_explicit_uniform_location : require
					
					#define HLSLCC_ENABLE_UNIFORM_BUFFERS 1
					#if HLSLCC_ENABLE_UNIFORM_BUFFERS
					#define UNITY_UNIFORM
					#else
					#define UNITY_UNIFORM uniform
					#endif
					#define UNITY_SUPPORTS_UNIFORM_LOCATION 1
					#if UNITY_SUPPORTS_UNIFORM_LOCATION
					#define UNITY_LOCATION(x) layout(location = x)
					#define UNITY_BINDING(x) layout(binding = x, std140)
					#else
					#define UNITY_LOCATION(x)
					#define UNITY_BINDING(x) layout(std140)
					#endif
					layout(std140) uniform PGlobals {
						vec4 unused_0_0[2];
						vec4 _Color;
						vec4 _diff_Color;
						vec4 _Diffuse_ST;
						vec4 unused_0_4;
						vec4 _M_map_ST;
						float _intensity;
						vec4 unused_0_7;
						float _Speed;
						float _noise_details;
						float _cut_level;
						float _mask_type;
						float _dist_chrom;
						float _glitchColor;
						vec4 _glitchColor_c;
						float _Opacity;
						float _OrigineX;
						float _OrigineY;
						float _Speed_wave;
						float _Circle_wave;
						float _Zoom;
						float _Speed_face;
						float _Rotation;
						float _monochrom;
						float _OriginalUVSwitch;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 _SinTime;
						vec4 _CosTime;
						vec4 unused_1_3[3];
						vec4 _ScreenParams;
						vec4 unused_1_5[2];
					};
					uniform  sampler2D _N_map;
					uniform  sampler2D _M_map;
					uniform  sampler2D _Diffuse;
					uniform  sampler2D _originalDiffuse;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD4;
					in  vec4 vs_TEXCOORD6;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					int u_xlati2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					bool u_xlatb4;
					vec3 u_xlat5;
					bvec3 u_xlatb5;
					vec2 u_xlat6;
					vec2 u_xlat7;
					vec2 u_xlat10;
					bool u_xlatb10;
					vec2 u_xlat13;
					vec2 u_xlat14;
					float u_xlat15;
					int u_xlati17;
					void main()
					{
					vec4 hlslcc_FragCoord = vec4(gl_FragCoord.xyz, 1.0/gl_FragCoord.w);
					    u_xlat0.x = unused_0_7.y * _Time.x;
					    u_xlat0.x = u_xlat0.x * 60.0;
					    u_xlat1.x = cos(u_xlat0.x);
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat5.x = _Speed_wave * _Time.x;
					    u_xlat10.xy = (-vs_TEXCOORD0.xy) + vec2(_OrigineX, _OrigineY);
					    u_xlat10.xy = u_xlat10.xy * u_xlat10.xy;
					    u_xlat10.x = u_xlat10.y + u_xlat10.x;
					    u_xlat5.x = u_xlat10.x * _Circle_wave + u_xlat5.x;
					    u_xlat0.y = sin(u_xlat5.x);
					    u_xlat0.xy = u_xlat0.xy + vec2(1.0, 1.0);
					    u_xlat5.x = u_xlat0.y * 0.5;
					    u_xlat10.x = _Rotation * 1.57000005;
					    u_xlat2.x = sin(u_xlat10.x);
					    u_xlat3.x = cos(u_xlat10.x);
					    u_xlat4.x = (-u_xlat2.x);
					    u_xlat4.y = u_xlat3.x;
					    u_xlat3.y = dot(u_xlat4.yx, vs_TEXCOORD0.xy);
					    u_xlat4.z = u_xlat2.x;
					    u_xlat10.x = dot(u_xlat4.zy, vs_TEXCOORD0.xy);
					    u_xlat6.xy = hlslcc_FragCoord.xy / _ScreenParams.xy;
					    u_xlat15 = _CosTime.x * 100.0;
					    u_xlat2.x = u_xlat15 * _Speed_face;
					    u_xlat15 = _SinTime.x * 100.0;
					    u_xlat2.y = u_xlat15 * _Speed_face;
					    u_xlat2.xy = u_xlat2.xy / vec2(vec2(_Zoom, _Zoom));
					    u_xlat6.xy = u_xlat6.xy + u_xlat2.xy;
					    u_xlat2 = texture(_N_map, u_xlat6.xy);
					    u_xlat15 = (-_mask_type) + 1.0;
					    u_xlat1.x = u_xlat1.x * _M_map_ST.y;
					    u_xlat3.x = vs_TEXCOORD0.x;
					    u_xlat0.x = u_xlat0.x * _M_map_ST.w;
					    u_xlat1.xy = u_xlat3.xy * u_xlat1.xx + u_xlat0.xx;
					    u_xlat1 = texture(_M_map, u_xlat1.xy);
					    u_xlat0.x = u_xlat1.x * _mask_type;
					    u_xlat0.x = u_xlat5.x * u_xlat15 + u_xlat0.x;
					    u_xlat0.x = u_xlat0.x * u_xlat2.x;
					    u_xlatb5.xz = equal(vec4(vec4(_OriginalUVSwitch, _OriginalUVSwitch, _OriginalUVSwitch, _OriginalUVSwitch)), vec4(0.0, 0.0, 1.0, 1.0)).xz;
					    if(u_xlatb5.x){
					        u_xlat1.xy = vs_TEXCOORD2.xy * _Diffuse_ST.yy + _Diffuse_ST.ww;
					        u_xlat1 = texture(_Diffuse, u_xlat1.xy);
					    } else {
					        u_xlat1.x = float(0.0);
					        u_xlat1.y = float(0.0);
					        u_xlat1.z = float(0.0);
					        u_xlat1.w = float(0.0);
					    }
					    if(u_xlatb5.z){
					        u_xlat1 = texture(_originalDiffuse, u_xlat3.xy);
					    }
					    u_xlat0.x = u_xlat0.x * 0.841470957;
					    u_xlat5.x = u_xlat0.x * vs_TEXCOORD0.x;
					    u_xlat5.xz = u_xlat5.xx * vec2(_intensity) + u_xlat3.xy;
					    u_xlati2 = int(unused_0_7.w);
					    u_xlat7.xy = _Time.xy * vec2(0.100000001, 0.100000001);
					    u_xlat3.xy = u_xlat5.xz;
					    u_xlat13.x = float(0.0);
					    u_xlat13.y = float(0.0);
					    for(int u_xlati_loop_1 = u_xlati2 ; u_xlati_loop_1>0 ; u_xlati_loop_1 = u_xlati_loop_1 + int(0xFFFFFFFFu))
					    {
					        u_xlat4.xy = floor(u_xlat3.xy);
					        u_xlat14.xy = u_xlat7.xy * u_xlat4.xy;
					        u_xlat4.xy = u_xlat4.xx * u_xlat4.yy + u_xlat14.xy;
					        u_xlat4.xy = sin(u_xlat4.xy);
					        u_xlat13.xy = u_xlat13.xy + u_xlat4.xy;
					        u_xlat3.xy = u_xlat3.xy * vec2(2.5, 2.5);
					    }
					    u_xlat5.xz = u_xlat13.xy * vec2(vec2(_glitchColor, _glitchColor));
					    u_xlat0.xy = u_xlat5.xz * u_xlat0.xx;
					    u_xlat5.xz = u_xlat0.xy * vec2(_intensity);
					    u_xlat2.xy = u_xlat5.xz * vec2(0.25, 0.5) + _Color.xy;
					    u_xlat2.z = u_xlat0.x * _intensity + _Color.z;
					    u_xlat0.x = u_xlat2.y + u_xlat2.x;
					    u_xlat0.x = u_xlat2.z + u_xlat0.x;
					    u_xlat5.x = u_xlat0.x * 4.0;
					    u_xlat5.x = u_xlat5.x * _cut_level;
					    u_xlat5.x = (-u_xlat5.x) * u_xlat10.x + 1.0;
					    u_xlatb5.x = vs_TEXCOORD6.y<u_xlat5.x;
					    if(((int(u_xlatb5.x) * int(0xffffffffu)))!=0){discard;}
					    u_xlat0.x = u_xlat0.x * 0.333333343;
					    u_xlat1.w = u_xlat1.w * _Opacity;
					    u_xlat3 = u_xlat1 * _diff_Color;
					    u_xlat5.x = u_xlat3.y + u_xlat3.x;
					    u_xlat5.x = u_xlat1.z * _diff_Color.z + u_xlat5.x;
					    u_xlat5.x = u_xlat5.x * 0.333333343;
					    u_xlat10.x = (-_dist_chrom) + 1.0;
					    u_xlat15 = u_xlat10.x * u_xlat0.x;
					    u_xlat1.xyz = u_xlat2.xyz * vec3(_dist_chrom) + vec3(u_xlat15);
					    u_xlat15 = u_xlat10.x * u_xlat5.x;
					    u_xlat2.xyz = u_xlat3.xyz * vec3(_dist_chrom) + vec3(u_xlat15);
					    u_xlat4.xyz = _glitchColor_c.xyz * u_xlat10.xxx + vec3(_dist_chrom);
					    u_xlat1.xyz = u_xlat1.xyz * u_xlat4.xyz;
					    u_xlatb10 = _monochrom==1.0;
					    u_xlat3.xyz = (bool(u_xlatb10)) ? u_xlat5.xxx : u_xlat2.xyz;
					    u_xlat0.xyz = (bool(u_xlatb10)) ? u_xlat0.xxx : u_xlat1.xyz;
					    u_xlat0.xyz = (-u_xlat3.xyz) + u_xlat0.xyz;
					    u_xlat0.w = (-u_xlat3.w) + _Color.w;
					    SV_Target0 = vs_TEXCOORD4.xxxx * u_xlat0 + u_xlat3;
					    return;
					}"
				}
			}
		}
	}
	CustomEditor "Glitch_Editor"
}