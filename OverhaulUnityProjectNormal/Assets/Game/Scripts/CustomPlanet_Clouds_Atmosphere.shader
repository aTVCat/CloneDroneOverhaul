Shader "Custom/Planet_Clouds_Atmosphere" {
	Properties {
		_MainTex ("Clouds Base (RGB)", 2D) = "white" {}
		_AlphaScale ("Clouds alpha Scale", Range(0, 2)) = 1
		[NoScaleOffset] _BumpMap ("Clouds normal Map", 2D) = "bump" {}
		_BumpScale ("Clouds bump Scale", Range(0, 1)) = 1
		_DisplacementClouds ("Clouds displacement", Range(0, 0.05)) = 0.005
		_Highatmospherecolor ("High atmosphere color", Vector) = (0.5,0.5,0.5,0)
		_Lowatmospherecolor ("Low atmosphere color", Vector) = (0.5,0.5,0.5,1)
		_Inneratmosphere ("Inner atmosphere", Vector) = (0.5,0.5,0.5,1)
		_Outeratmospherelimit ("Outer atmosphere limit", Range(0, 1)) = 0.548887
		_Outeratmopsheredensity ("Outer atmopshere density", Range(0, 1)) = 1
		_Inneratmopsheredensity ("Inner atmopshere density", Range(0, 1)) = 0.08092485
		_Innerouterlimit ("Inner outer limit", Range(0, 1)) = 0.6647399
		_Inneroutersmoothness ("Inner outer smoothness", Range(0, 1)) = 0.2572428
		_DisplacementAtmosphere ("Displacement atmosphere", Range(0, 0.1)) = 0.1
	}
	SubShader {
		LOD 300
		Tags { "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend One OneMinusSrcAlpha, One OneMinusSrcAlpha
			ColorMask RGB -1
			ZWrite Off
			GpuProgramID 17623
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat0.x = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_1_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = u_xlat0.xxxx * u_xlat2.xyzz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.wxy;
					    u_xlat3.xyz = u_xlat2.ywx * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat0.x = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.z = u_xlat2.x;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD2.z = u_xlat2.y;
					    vs_TEXCOORD3.z = u_xlat2.w;
					    u_xlat0.x = u_xlat2.y * u_xlat2.y;
					    u_xlat0.x = u_xlat2.x * u_xlat2.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat2.ywzx * u_xlat2;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD4.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_1_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_1_11[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					float u_xlat15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat15 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat1.xyz;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat16 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat2 = vec4(u_xlat16) * u_xlat2.xyzz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.wxy;
					    u_xlat3.xyz = u_xlat2.ywx * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat3.xyz = vec3(u_xlat15) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.z = u_xlat2.x;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD2.z = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat2.w;
					    u_xlat1 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat3 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat4 = u_xlat2.yyyy * u_xlat3;
					    u_xlat3 = u_xlat3 * u_xlat3;
					    u_xlat3 = u_xlat1 * u_xlat1 + u_xlat3;
					    u_xlat1 = u_xlat1 * u_xlat2.xxxx + u_xlat4;
					    u_xlat1 = u_xlat0 * u_xlat2.wwzw + u_xlat1;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat3;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat3 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat3;
					    u_xlat1 = max(u_xlat1, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat1;
					    u_xlat1.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat1.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat1.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat2.y * u_xlat2.y;
					    u_xlat15 = u_xlat2.x * u_xlat2.x + (-u_xlat15);
					    u_xlat2 = u_xlat2.ywzx * u_xlat2;
					    u_xlat3.x = dot(unity_SHBr, u_xlat2);
					    u_xlat3.y = dot(unity_SHBg, u_xlat2);
					    u_xlat3.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD4.xyz = u_xlat0.xyz * u_xlat1.xyz + u_xlat2.xyz;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD5;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD5 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2.xyz = u_xlat0.xxx * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat0.x = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_1_5[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD5;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD5 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat1.xyz = u_xlat0.xxx * u_xlat1.xyz;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat0.x = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat2 = u_xlat0.xxxx * u_xlat2.xyzz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.wxy;
					    u_xlat3.xyz = u_xlat2.ywx * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat0.x = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.z = u_xlat2.x;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD2.z = u_xlat2.y;
					    vs_TEXCOORD3.z = u_xlat2.w;
					    u_xlat0.x = u_xlat2.y * u_xlat2.y;
					    u_xlat0.x = u_xlat2.x * u_xlat2.x + (-u_xlat0.x);
					    u_xlat1 = u_xlat2.ywzx * u_xlat2;
					    u_xlat2.x = dot(unity_SHBr, u_xlat1);
					    u_xlat2.y = dot(unity_SHBg, u_xlat1);
					    u_xlat2.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD4.xyz = unity_SHC.xyz * u_xlat0.xxx + u_xlat2.xyz;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_1_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_1_11[2];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_2_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD5;
					out vec4 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD7;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					float u_xlat15;
					float u_xlat16;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD5 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat15 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat1.xyz;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.w = u_xlat0.x;
					    u_xlat15 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat16 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat16 = inversesqrt(u_xlat16);
					    u_xlat2 = vec4(u_xlat16) * u_xlat2.xyzz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.wxy;
					    u_xlat3.xyz = u_xlat2.ywx * u_xlat1.yzx + (-u_xlat3.xyz);
					    u_xlat3.xyz = vec3(u_xlat15) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.z = u_xlat2.x;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.w = u_xlat0.y;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD2.z = u_xlat2.y;
					    vs_TEXCOORD3.w = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat2.w;
					    u_xlat1 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat3 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat4 = u_xlat2.yyyy * u_xlat3;
					    u_xlat3 = u_xlat3 * u_xlat3;
					    u_xlat3 = u_xlat1 * u_xlat1 + u_xlat3;
					    u_xlat1 = u_xlat1 * u_xlat2.xxxx + u_xlat4;
					    u_xlat1 = u_xlat0 * u_xlat2.wwzw + u_xlat1;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat3;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat3 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat1 = u_xlat1 * u_xlat3;
					    u_xlat1 = max(u_xlat1, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat1;
					    u_xlat1.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat1.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat1.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat1.xyz = u_xlat0.xyz * u_xlat1.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat2.y * u_xlat2.y;
					    u_xlat15 = u_xlat2.x * u_xlat2.x + (-u_xlat15);
					    u_xlat2 = u_xlat2.ywzx * u_xlat2;
					    u_xlat3.x = dot(unity_SHBr, u_xlat2);
					    u_xlat3.y = dot(unity_SHBg, u_xlat2);
					    u_xlat3.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD4.xyz = u_xlat0.xyz * u_xlat1.xyz + u_xlat2.xyz;
					    vs_TEXCOORD7 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityReflectionProbes {
						vec4 unity_SpecCube0_BoxMax;
						vec4 unity_SpecCube0_BoxMin;
						vec4 unity_SpecCube0_ProbePosition;
						vec4 unity_SpecCube0_HDR;
						vec4 unity_SpecCube1_BoxMax;
						vec4 unity_SpecCube1_BoxMin;
						vec4 unity_SpecCube1_ProbePosition;
						vec4 unity_SpecCube1_HDR;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec4 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					vec3 u_xlat10;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					bvec3 u_xlatb12;
					vec3 u_xlat14;
					vec3 u_xlat15;
					float u_xlat27;
					float u_xlat39;
					float u_xlat40;
					float u_xlat41;
					bool u_xlatb41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.x = vs_TEXCOORD1.w;
					    u_xlat0.y = vs_TEXCOORD2.w;
					    u_xlat0.z = vs_TEXCOORD3.w;
					    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat39 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat39 = inversesqrt(u_xlat39);
					    u_xlat2.xyz = vec3(u_xlat39) * u_xlat1.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat40 = u_xlat3.w * _AlphaScale;
					    u_xlat40 = clamp(u_xlat40, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat41 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat41 = min(u_xlat41, 1.0);
					    u_xlat41 = (-u_xlat41) + 1.0;
					    u_xlat4.z = sqrt(u_xlat41);
					    u_xlat41 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat41 = inversesqrt(u_xlat41);
					    u_xlat4.xyz = vec3(u_xlat41) * u_xlat4.xyz;
					    u_xlatb41 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb41){
					        u_xlatb41 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb41)) ? u_xlat5.xyz : u_xlat0.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat41 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat42 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat41, u_xlat42);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat41 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat41 = clamp(u_xlat41, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat42 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat4.xyz = vec3(u_xlat42) * u_xlat5.xyz;
					    u_xlat42 = dot((-u_xlat2.xyz), u_xlat4.xyz);
					    u_xlat42 = u_xlat42 + u_xlat42;
					    u_xlat5.xyz = u_xlat4.xyz * (-vec3(u_xlat42)) + (-u_xlat2.xyz);
					    u_xlat6.xyz = vec3(u_xlat41) * _LightColor0.xyz;
					    u_xlatb41 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb41){
					        u_xlat41 = dot(u_xlat5.xyz, u_xlat5.xyz);
					        u_xlat41 = inversesqrt(u_xlat41);
					        u_xlat7.xyz = vec3(u_xlat41) * u_xlat5.xyz;
					        u_xlat8.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					        u_xlat9.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					        u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat8;
					            hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					            hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					            hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					            u_xlat8 = hlslcc_movcTemp;
					        }
					        u_xlat41 = min(u_xlat8.y, u_xlat8.x);
					        u_xlat41 = min(u_xlat8.z, u_xlat41);
					        u_xlat8.xyz = u_xlat0.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat7.xyz = u_xlat7.xyz * vec3(u_xlat41) + u_xlat8.xyz;
					    } else {
					        u_xlat7.xyz = u_xlat5.xyz;
					    }
					    u_xlat7 = textureLod(unity_SpecCube0, u_xlat7.xyz, 6.0);
					    u_xlat41 = u_xlat7.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat8.xyz = u_xlat7.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat5.xyz, u_xlat5.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat9.xyz = vec3(u_xlat42) * u_xlat5.xyz;
					            u_xlat10.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat9.xyz;
					            u_xlat11.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat11.xyz = u_xlat11.xyz / u_xlat9.xyz;
					            u_xlatb12.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat9.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat10;
					                hlslcc_movcTemp.x = (u_xlatb12.x) ? u_xlat10.x : u_xlat11.x;
					                hlslcc_movcTemp.y = (u_xlatb12.y) ? u_xlat10.y : u_xlat11.y;
					                hlslcc_movcTemp.z = (u_xlatb12.z) ? u_xlat10.z : u_xlat11.z;
					                u_xlat10 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat10.y, u_xlat10.x);
					            u_xlat42 = min(u_xlat10.z, u_xlat42);
					            u_xlat0.xyz = u_xlat0.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat5.xyz = u_xlat9.xyz * vec3(u_xlat42) + u_xlat0.xyz;
					        }
					        u_xlat5 = textureLod(unity_SpecCube1, u_xlat5.xyz, 6.0);
					        u_xlat0.x = u_xlat5.w + -1.0;
					        u_xlat0.x = unity_SpecCube1_HDR.w * u_xlat0.x + 1.0;
					        u_xlat0.x = u_xlat0.x * unity_SpecCube1_HDR.x;
					        u_xlat0.xyz = u_xlat5.xyz * u_xlat0.xxx;
					        u_xlat5.xyz = vec3(u_xlat41) * u_xlat7.xyz + (-u_xlat0.xyz);
					        u_xlat8.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat0.xyz;
					    }
					    u_xlat0.xyz = vec3(u_xlat40) * u_xlat3.xyz;
					    SV_Target0.w = u_xlat40 * 0.779083729 + 0.220916271;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat39) + _WorldSpaceLightPos0.xyz;
					    u_xlat39 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat39 = max(u_xlat39, 0.00100000005);
					    u_xlat39 = inversesqrt(u_xlat39);
					    u_xlat1.xyz = vec3(u_xlat39) * u_xlat1.xyz;
					    u_xlat39 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat40 = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat40 = clamp(u_xlat40, 0.0, 1.0);
					    u_xlat1.x = dot(_WorldSpaceLightPos0.xyz, u_xlat1.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat14.x = dot(u_xlat1.xx, u_xlat1.xx);
					    u_xlat14.x = u_xlat14.x + -0.5;
					    u_xlat27 = (-u_xlat40) + 1.0;
					    u_xlat2.x = u_xlat27 * u_xlat27;
					    u_xlat2.x = u_xlat2.x * u_xlat2.x;
					    u_xlat27 = u_xlat27 * u_xlat2.x;
					    u_xlat27 = u_xlat14.x * u_xlat27 + 1.0;
					    u_xlat2.x = -abs(u_xlat39) + 1.0;
					    u_xlat15.x = u_xlat2.x * u_xlat2.x;
					    u_xlat15.x = u_xlat15.x * u_xlat15.x;
					    u_xlat2.x = u_xlat2.x * u_xlat15.x;
					    u_xlat14.x = u_xlat14.x * u_xlat2.x + 1.0;
					    u_xlat14.x = u_xlat14.x * u_xlat27;
					    u_xlat14.x = u_xlat40 * u_xlat14.x;
					    u_xlat39 = abs(u_xlat39) + u_xlat40;
					    u_xlat39 = u_xlat39 + 9.99999975e-06;
					    u_xlat0.w = 0.5 / u_xlat39;
					    u_xlat0 = u_xlat0 * vec4(0.779083729, 0.779083729, 0.779083729, 0.999999881);
					    u_xlat39 = max(u_xlat0.w, 9.99999975e-05);
					    u_xlat39 = sqrt(u_xlat39);
					    u_xlat39 = u_xlat40 * u_xlat39;
					    u_xlat14.xyz = u_xlat14.xxx * u_xlat6.xyz;
					    u_xlat15.xyz = u_xlat6.xyz * vec3(u_xlat39);
					    u_xlat39 = (-u_xlat1.x) + 1.0;
					    u_xlat1.x = u_xlat39 * u_xlat39;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat39 = u_xlat39 * u_xlat1.x;
					    u_xlat39 = u_xlat39 * 0.779083729 + 0.220916301;
					    u_xlat15.xyz = vec3(u_xlat39) * u_xlat15.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat14.xyz + u_xlat15.xyz;
					    u_xlat1.xyz = u_xlat8.xyz * vec3(0.720000029, 0.720000029, 0.720000029);
					    u_xlat39 = u_xlat2.x * -2.98023224e-08 + 0.220916301;
					    SV_Target0.xyz = u_xlat1.xyz * vec3(u_xlat39) + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityReflectionProbes {
						vec4 unity_SpecCube0_BoxMax;
						vec4 unity_SpecCube0_BoxMin;
						vec4 unity_SpecCube0_ProbePosition;
						vec4 unity_SpecCube0_HDR;
						vec4 unity_SpecCube1_BoxMax;
						vec4 unity_SpecCube1_BoxMin;
						vec4 unity_SpecCube1_ProbePosition;
						vec4 unity_SpecCube1_HDR;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec4 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					vec3 u_xlat10;
					vec3 u_xlat11;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					bvec3 u_xlatb13;
					vec3 u_xlat15;
					vec3 u_xlat16;
					float u_xlat29;
					float u_xlat42;
					float u_xlat43;
					float u_xlat44;
					bool u_xlatb44;
					float u_xlat45;
					bool u_xlatb45;
					float u_xlat46;
					float u_xlat47;
					void main()
					{
					    u_xlat0.x = vs_TEXCOORD1.w;
					    u_xlat0.y = vs_TEXCOORD2.w;
					    u_xlat0.z = vs_TEXCOORD3.w;
					    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat42 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat2.xyz = vec3(u_xlat42) * u_xlat1.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat43 = u_xlat3.w * _AlphaScale;
					    u_xlat43 = clamp(u_xlat43, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat44 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat44 = min(u_xlat44, 1.0);
					    u_xlat44 = (-u_xlat44) + 1.0;
					    u_xlat4.z = sqrt(u_xlat44);
					    u_xlat44 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat44 = inversesqrt(u_xlat44);
					    u_xlat4.xyz = vec3(u_xlat44) * u_xlat4.xyz;
					    u_xlatb44 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb44){
					        u_xlatb45 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb45)) ? u_xlat5.xyz : u_xlat0.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat45 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat46 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat45, u_xlat46);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat45 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat45 = clamp(u_xlat45, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat4.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat4.x = inversesqrt(u_xlat4.x);
					    u_xlat4.xyz = u_xlat4.xxx * u_xlat5.xyz;
					    u_xlat5.x = dot((-u_xlat2.xyz), u_xlat4.xyz);
					    u_xlat5.x = u_xlat5.x + u_xlat5.x;
					    u_xlat5.xyz = u_xlat4.xyz * (-u_xlat5.xxx) + (-u_xlat2.xyz);
					    u_xlat6.xyz = vec3(u_xlat45) * _LightColor0.xyz;
					    if(u_xlatb44){
					        u_xlatb44 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat7.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat7.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat7.xyz;
					        u_xlat7.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat7.xyz;
					        u_xlat7.xyz = u_xlat7.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat7.xyz = (bool(u_xlatb44)) ? u_xlat7.xyz : u_xlat0.xyz;
					        u_xlat7.xyz = u_xlat7.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat7.yzw = u_xlat7.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat44 = u_xlat7.y * 0.25;
					        u_xlat45 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat47 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat44 = max(u_xlat44, u_xlat45);
					        u_xlat7.x = min(u_xlat47, u_xlat44);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat7.xzw);
					        u_xlat9.xyz = u_xlat7.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat9 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat7.xyz = u_xlat7.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat7.xyz);
					        u_xlat4.w = 1.0;
					        u_xlat8.x = dot(u_xlat8, u_xlat4);
					        u_xlat8.y = dot(u_xlat9, u_xlat4);
					        u_xlat8.z = dot(u_xlat7, u_xlat4);
					    } else {
					        u_xlat4.w = 1.0;
					        u_xlat8.x = dot(unity_SHAr, u_xlat4);
					        u_xlat8.y = dot(unity_SHAg, u_xlat4);
					        u_xlat8.z = dot(unity_SHAb, u_xlat4);
					    }
					    u_xlat7.xyz = u_xlat8.xyz + vs_TEXCOORD4.xyz;
					    u_xlat7.xyz = max(u_xlat7.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat7.xyz = log2(u_xlat7.xyz);
					    u_xlat7.xyz = u_xlat7.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat7.xyz = exp2(u_xlat7.xyz);
					    u_xlat7.xyz = u_xlat7.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat7.xyz = max(u_xlat7.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlatb44 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb44){
					        u_xlat44 = dot(u_xlat5.xyz, u_xlat5.xyz);
					        u_xlat44 = inversesqrt(u_xlat44);
					        u_xlat8.xyz = vec3(u_xlat44) * u_xlat5.xyz;
					        u_xlat9.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					        u_xlat10.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					        u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat9;
					            hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					            hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					            hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					            u_xlat9 = hlslcc_movcTemp;
					        }
					        u_xlat44 = min(u_xlat9.y, u_xlat9.x);
					        u_xlat44 = min(u_xlat9.z, u_xlat44);
					        u_xlat9.xyz = u_xlat0.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat8.xyz = u_xlat8.xyz * vec3(u_xlat44) + u_xlat9.xyz;
					    } else {
					        u_xlat8.xyz = u_xlat5.xyz;
					    }
					    u_xlat8 = textureLod(unity_SpecCube0, u_xlat8.xyz, 6.0);
					    u_xlat44 = u_xlat8.w + -1.0;
					    u_xlat44 = unity_SpecCube0_HDR.w * u_xlat44 + 1.0;
					    u_xlat44 = u_xlat44 * unity_SpecCube0_HDR.x;
					    u_xlat9.xyz = u_xlat8.xyz * vec3(u_xlat44);
					    u_xlatb45 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb45){
					        u_xlatb45 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb45){
					            u_xlat45 = dot(u_xlat5.xyz, u_xlat5.xyz);
					            u_xlat45 = inversesqrt(u_xlat45);
					            u_xlat10.xyz = vec3(u_xlat45) * u_xlat5.xyz;
					            u_xlat11.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat11.xyz = u_xlat11.xyz / u_xlat10.xyz;
					            u_xlat12.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat12.xyz = u_xlat12.xyz / u_xlat10.xyz;
					            u_xlatb13.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat10.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat11;
					                hlslcc_movcTemp.x = (u_xlatb13.x) ? u_xlat11.x : u_xlat12.x;
					                hlslcc_movcTemp.y = (u_xlatb13.y) ? u_xlat11.y : u_xlat12.y;
					                hlslcc_movcTemp.z = (u_xlatb13.z) ? u_xlat11.z : u_xlat12.z;
					                u_xlat11 = hlslcc_movcTemp;
					            }
					            u_xlat45 = min(u_xlat11.y, u_xlat11.x);
					            u_xlat45 = min(u_xlat11.z, u_xlat45);
					            u_xlat0.xyz = u_xlat0.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat5.xyz = u_xlat10.xyz * vec3(u_xlat45) + u_xlat0.xyz;
					        }
					        u_xlat5 = textureLod(unity_SpecCube1, u_xlat5.xyz, 6.0);
					        u_xlat0.x = u_xlat5.w + -1.0;
					        u_xlat0.x = unity_SpecCube1_HDR.w * u_xlat0.x + 1.0;
					        u_xlat0.x = u_xlat0.x * unity_SpecCube1_HDR.x;
					        u_xlat0.xyz = u_xlat5.xyz * u_xlat0.xxx;
					        u_xlat5.xyz = vec3(u_xlat44) * u_xlat8.xyz + (-u_xlat0.xyz);
					        u_xlat9.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat0.xyz;
					    }
					    u_xlat0.xyz = vec3(u_xlat43) * u_xlat3.xyz;
					    SV_Target0.w = u_xlat43 * 0.779083729 + 0.220916271;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat42) + _WorldSpaceLightPos0.xyz;
					    u_xlat42 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat42 = max(u_xlat42, 0.00100000005);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat1.xyz = vec3(u_xlat42) * u_xlat1.xyz;
					    u_xlat42 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat43 = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat43 = clamp(u_xlat43, 0.0, 1.0);
					    u_xlat1.x = dot(_WorldSpaceLightPos0.xyz, u_xlat1.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat15.x = dot(u_xlat1.xx, u_xlat1.xx);
					    u_xlat15.x = u_xlat15.x + -0.5;
					    u_xlat29 = (-u_xlat43) + 1.0;
					    u_xlat2.x = u_xlat29 * u_xlat29;
					    u_xlat2.x = u_xlat2.x * u_xlat2.x;
					    u_xlat29 = u_xlat29 * u_xlat2.x;
					    u_xlat29 = u_xlat15.x * u_xlat29 + 1.0;
					    u_xlat2.x = -abs(u_xlat42) + 1.0;
					    u_xlat16.x = u_xlat2.x * u_xlat2.x;
					    u_xlat16.x = u_xlat16.x * u_xlat16.x;
					    u_xlat2.x = u_xlat2.x * u_xlat16.x;
					    u_xlat15.x = u_xlat15.x * u_xlat2.x + 1.0;
					    u_xlat15.x = u_xlat15.x * u_xlat29;
					    u_xlat15.x = u_xlat43 * u_xlat15.x;
					    u_xlat42 = abs(u_xlat42) + u_xlat43;
					    u_xlat42 = u_xlat42 + 9.99999975e-06;
					    u_xlat0.w = 0.5 / u_xlat42;
					    u_xlat0 = u_xlat0 * vec4(0.779083729, 0.779083729, 0.779083729, 0.999999881);
					    u_xlat42 = max(u_xlat0.w, 9.99999975e-05);
					    u_xlat42 = sqrt(u_xlat42);
					    u_xlat42 = u_xlat43 * u_xlat42;
					    u_xlat15.xyz = u_xlat6.xyz * u_xlat15.xxx + u_xlat7.xyz;
					    u_xlat16.xyz = u_xlat6.xyz * vec3(u_xlat42);
					    u_xlat42 = (-u_xlat1.x) + 1.0;
					    u_xlat1.x = u_xlat42 * u_xlat42;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat42 = u_xlat42 * u_xlat1.x;
					    u_xlat42 = u_xlat42 * 0.779083729 + 0.220916301;
					    u_xlat16.xyz = vec3(u_xlat42) * u_xlat16.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat15.xyz + u_xlat16.xyz;
					    u_xlat1.xyz = u_xlat9.xyz * vec3(0.720000029, 0.720000029, 0.720000029);
					    u_xlat42 = u_xlat2.x * -2.98023224e-08 + 0.220916301;
					    SV_Target0.xyz = u_xlat1.xyz * vec3(u_xlat42) + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unity_FogColor;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityReflectionProbes {
						vec4 unity_SpecCube0_BoxMax;
						vec4 unity_SpecCube0_BoxMin;
						vec4 unity_SpecCube0_ProbePosition;
						vec4 unity_SpecCube0_HDR;
						vec4 unity_SpecCube1_BoxMax;
						vec4 unity_SpecCube1_BoxMin;
						vec4 unity_SpecCube1_ProbePosition;
						vec4 unity_SpecCube1_HDR;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					vec3 u_xlat10;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					bvec3 u_xlatb12;
					vec3 u_xlat14;
					vec3 u_xlat15;
					float u_xlat27;
					float u_xlat39;
					float u_xlat40;
					float u_xlat41;
					bool u_xlatb41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.x = vs_TEXCOORD1.w;
					    u_xlat0.y = vs_TEXCOORD2.w;
					    u_xlat0.z = vs_TEXCOORD3.w;
					    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat39 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat39 = inversesqrt(u_xlat39);
					    u_xlat2.xyz = vec3(u_xlat39) * u_xlat1.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat40 = u_xlat3.w * _AlphaScale;
					    u_xlat40 = clamp(u_xlat40, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat41 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat41 = min(u_xlat41, 1.0);
					    u_xlat41 = (-u_xlat41) + 1.0;
					    u_xlat4.z = sqrt(u_xlat41);
					    u_xlat41 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat41 = inversesqrt(u_xlat41);
					    u_xlat4.xyz = vec3(u_xlat41) * u_xlat4.xyz;
					    u_xlatb41 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb41){
					        u_xlatb41 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb41)) ? u_xlat5.xyz : u_xlat0.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat41 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat42 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat41, u_xlat42);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat41 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat41 = clamp(u_xlat41, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat42 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat4.xyz = vec3(u_xlat42) * u_xlat5.xyz;
					    u_xlat42 = dot((-u_xlat2.xyz), u_xlat4.xyz);
					    u_xlat42 = u_xlat42 + u_xlat42;
					    u_xlat5.xyz = u_xlat4.xyz * (-vec3(u_xlat42)) + (-u_xlat2.xyz);
					    u_xlat6.xyz = vec3(u_xlat41) * _LightColor0.xyz;
					    u_xlatb41 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb41){
					        u_xlat41 = dot(u_xlat5.xyz, u_xlat5.xyz);
					        u_xlat41 = inversesqrt(u_xlat41);
					        u_xlat7.xyz = vec3(u_xlat41) * u_xlat5.xyz;
					        u_xlat8.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					        u_xlat9.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					        u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat8;
					            hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					            hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					            hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					            u_xlat8 = hlslcc_movcTemp;
					        }
					        u_xlat41 = min(u_xlat8.y, u_xlat8.x);
					        u_xlat41 = min(u_xlat8.z, u_xlat41);
					        u_xlat8.xyz = u_xlat0.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat7.xyz = u_xlat7.xyz * vec3(u_xlat41) + u_xlat8.xyz;
					    } else {
					        u_xlat7.xyz = u_xlat5.xyz;
					    }
					    u_xlat7 = textureLod(unity_SpecCube0, u_xlat7.xyz, 6.0);
					    u_xlat41 = u_xlat7.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat8.xyz = u_xlat7.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat5.xyz, u_xlat5.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat9.xyz = vec3(u_xlat42) * u_xlat5.xyz;
					            u_xlat10.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat9.xyz;
					            u_xlat11.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat11.xyz = u_xlat11.xyz / u_xlat9.xyz;
					            u_xlatb12.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat9.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat10;
					                hlslcc_movcTemp.x = (u_xlatb12.x) ? u_xlat10.x : u_xlat11.x;
					                hlslcc_movcTemp.y = (u_xlatb12.y) ? u_xlat10.y : u_xlat11.y;
					                hlslcc_movcTemp.z = (u_xlatb12.z) ? u_xlat10.z : u_xlat11.z;
					                u_xlat10 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat10.y, u_xlat10.x);
					            u_xlat42 = min(u_xlat10.z, u_xlat42);
					            u_xlat0.xyz = u_xlat0.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat5.xyz = u_xlat9.xyz * vec3(u_xlat42) + u_xlat0.xyz;
					        }
					        u_xlat5 = textureLod(unity_SpecCube1, u_xlat5.xyz, 6.0);
					        u_xlat0.x = u_xlat5.w + -1.0;
					        u_xlat0.x = unity_SpecCube1_HDR.w * u_xlat0.x + 1.0;
					        u_xlat0.x = u_xlat0.x * unity_SpecCube1_HDR.x;
					        u_xlat0.xyz = u_xlat5.xyz * u_xlat0.xxx;
					        u_xlat5.xyz = vec3(u_xlat41) * u_xlat7.xyz + (-u_xlat0.xyz);
					        u_xlat8.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat0.xyz;
					    }
					    u_xlat0.xyz = vec3(u_xlat40) * u_xlat3.xyz;
					    SV_Target0.w = u_xlat40 * 0.779083729 + 0.220916271;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat39) + _WorldSpaceLightPos0.xyz;
					    u_xlat39 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat39 = max(u_xlat39, 0.00100000005);
					    u_xlat39 = inversesqrt(u_xlat39);
					    u_xlat1.xyz = vec3(u_xlat39) * u_xlat1.xyz;
					    u_xlat39 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat40 = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat40 = clamp(u_xlat40, 0.0, 1.0);
					    u_xlat1.x = dot(_WorldSpaceLightPos0.xyz, u_xlat1.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat14.x = dot(u_xlat1.xx, u_xlat1.xx);
					    u_xlat14.x = u_xlat14.x + -0.5;
					    u_xlat27 = (-u_xlat40) + 1.0;
					    u_xlat2.x = u_xlat27 * u_xlat27;
					    u_xlat2.x = u_xlat2.x * u_xlat2.x;
					    u_xlat27 = u_xlat27 * u_xlat2.x;
					    u_xlat27 = u_xlat14.x * u_xlat27 + 1.0;
					    u_xlat2.x = -abs(u_xlat39) + 1.0;
					    u_xlat15.x = u_xlat2.x * u_xlat2.x;
					    u_xlat15.x = u_xlat15.x * u_xlat15.x;
					    u_xlat2.x = u_xlat2.x * u_xlat15.x;
					    u_xlat14.x = u_xlat14.x * u_xlat2.x + 1.0;
					    u_xlat14.x = u_xlat14.x * u_xlat27;
					    u_xlat14.x = u_xlat40 * u_xlat14.x;
					    u_xlat39 = abs(u_xlat39) + u_xlat40;
					    u_xlat39 = u_xlat39 + 9.99999975e-06;
					    u_xlat0.w = 0.5 / u_xlat39;
					    u_xlat0 = u_xlat0 * vec4(0.779083729, 0.779083729, 0.779083729, 0.999999881);
					    u_xlat39 = max(u_xlat0.w, 9.99999975e-05);
					    u_xlat39 = sqrt(u_xlat39);
					    u_xlat39 = u_xlat40 * u_xlat39;
					    u_xlat14.xyz = u_xlat14.xxx * u_xlat6.xyz;
					    u_xlat15.xyz = u_xlat6.xyz * vec3(u_xlat39);
					    u_xlat39 = (-u_xlat1.x) + 1.0;
					    u_xlat1.x = u_xlat39 * u_xlat39;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat39 = u_xlat39 * u_xlat1.x;
					    u_xlat39 = u_xlat39 * 0.779083729 + 0.220916301;
					    u_xlat15.xyz = vec3(u_xlat39) * u_xlat15.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat14.xyz + u_xlat15.xyz;
					    u_xlat1.xyz = u_xlat8.xyz * vec3(0.720000029, 0.720000029, 0.720000029);
					    u_xlat39 = u_xlat2.x * -2.98023224e-08 + 0.220916301;
					    u_xlat0.xyz = u_xlat1.xyz * vec3(u_xlat39) + u_xlat0.xyz;
					    u_xlat39 = vs_TEXCOORD5 / _ProjectionParams.y;
					    u_xlat39 = (-u_xlat39) + 1.0;
					    u_xlat39 = u_xlat39 * _ProjectionParams.z;
					    u_xlat39 = max(u_xlat39, 0.0);
					    u_xlat39 = u_xlat39 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat39 = clamp(u_xlat39, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat39) * u_xlat0.xyz + unity_FogColor.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[38];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_2_5[4];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_7;
					};
					layout(std140) uniform UnityFog {
						vec4 unity_FogColor;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityReflectionProbes {
						vec4 unity_SpecCube0_BoxMax;
						vec4 unity_SpecCube0_BoxMin;
						vec4 unity_SpecCube0_ProbePosition;
						vec4 unity_SpecCube0_HDR;
						vec4 unity_SpecCube1_BoxMax;
						vec4 unity_SpecCube1_BoxMin;
						vec4 unity_SpecCube1_ProbePosition;
						vec4 unity_SpecCube1_HDR;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD5;
					in  vec4 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					vec3 u_xlat10;
					vec3 u_xlat11;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					bvec3 u_xlatb13;
					vec3 u_xlat15;
					vec3 u_xlat16;
					float u_xlat29;
					float u_xlat42;
					float u_xlat43;
					float u_xlat44;
					bool u_xlatb44;
					float u_xlat45;
					bool u_xlatb45;
					float u_xlat46;
					float u_xlat47;
					void main()
					{
					    u_xlat0.x = vs_TEXCOORD1.w;
					    u_xlat0.y = vs_TEXCOORD2.w;
					    u_xlat0.z = vs_TEXCOORD3.w;
					    u_xlat1.xyz = (-u_xlat0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat42 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat2.xyz = vec3(u_xlat42) * u_xlat1.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat43 = u_xlat3.w * _AlphaScale;
					    u_xlat43 = clamp(u_xlat43, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat44 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat44 = min(u_xlat44, 1.0);
					    u_xlat44 = (-u_xlat44) + 1.0;
					    u_xlat4.z = sqrt(u_xlat44);
					    u_xlat44 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat44 = inversesqrt(u_xlat44);
					    u_xlat4.xyz = vec3(u_xlat44) * u_xlat4.xyz;
					    u_xlatb44 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb44){
					        u_xlatb45 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb45)) ? u_xlat5.xyz : u_xlat0.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat45 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat46 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat45, u_xlat46);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat45 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat45 = clamp(u_xlat45, 0.0, 1.0);
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat4.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat4.x = inversesqrt(u_xlat4.x);
					    u_xlat4.xyz = u_xlat4.xxx * u_xlat5.xyz;
					    u_xlat5.x = dot((-u_xlat2.xyz), u_xlat4.xyz);
					    u_xlat5.x = u_xlat5.x + u_xlat5.x;
					    u_xlat5.xyz = u_xlat4.xyz * (-u_xlat5.xxx) + (-u_xlat2.xyz);
					    u_xlat6.xyz = vec3(u_xlat45) * _LightColor0.xyz;
					    if(u_xlatb44){
					        u_xlatb44 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat7.xyz = vs_TEXCOORD2.www * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat7.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.www + u_xlat7.xyz;
					        u_xlat7.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD3.www + u_xlat7.xyz;
					        u_xlat7.xyz = u_xlat7.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat7.xyz = (bool(u_xlatb44)) ? u_xlat7.xyz : u_xlat0.xyz;
					        u_xlat7.xyz = u_xlat7.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat7.yzw = u_xlat7.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat44 = u_xlat7.y * 0.25;
					        u_xlat45 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat47 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat44 = max(u_xlat44, u_xlat45);
					        u_xlat7.x = min(u_xlat47, u_xlat44);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat7.xzw);
					        u_xlat9.xyz = u_xlat7.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat9 = texture(unity_ProbeVolumeSH, u_xlat9.xyz);
					        u_xlat7.xyz = u_xlat7.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat7.xyz);
					        u_xlat4.w = 1.0;
					        u_xlat8.x = dot(u_xlat8, u_xlat4);
					        u_xlat8.y = dot(u_xlat9, u_xlat4);
					        u_xlat8.z = dot(u_xlat7, u_xlat4);
					    } else {
					        u_xlat4.w = 1.0;
					        u_xlat8.x = dot(unity_SHAr, u_xlat4);
					        u_xlat8.y = dot(unity_SHAg, u_xlat4);
					        u_xlat8.z = dot(unity_SHAb, u_xlat4);
					    }
					    u_xlat7.xyz = u_xlat8.xyz + vs_TEXCOORD4.xyz;
					    u_xlat7.xyz = max(u_xlat7.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat7.xyz = log2(u_xlat7.xyz);
					    u_xlat7.xyz = u_xlat7.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat7.xyz = exp2(u_xlat7.xyz);
					    u_xlat7.xyz = u_xlat7.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat7.xyz = max(u_xlat7.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlatb44 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb44){
					        u_xlat44 = dot(u_xlat5.xyz, u_xlat5.xyz);
					        u_xlat44 = inversesqrt(u_xlat44);
					        u_xlat8.xyz = vec3(u_xlat44) * u_xlat5.xyz;
					        u_xlat9.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					        u_xlat10.xyz = (-u_xlat0.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					        u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat9;
					            hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					            hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					            hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					            u_xlat9 = hlslcc_movcTemp;
					        }
					        u_xlat44 = min(u_xlat9.y, u_xlat9.x);
					        u_xlat44 = min(u_xlat9.z, u_xlat44);
					        u_xlat9.xyz = u_xlat0.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat8.xyz = u_xlat8.xyz * vec3(u_xlat44) + u_xlat9.xyz;
					    } else {
					        u_xlat8.xyz = u_xlat5.xyz;
					    }
					    u_xlat8 = textureLod(unity_SpecCube0, u_xlat8.xyz, 6.0);
					    u_xlat44 = u_xlat8.w + -1.0;
					    u_xlat44 = unity_SpecCube0_HDR.w * u_xlat44 + 1.0;
					    u_xlat44 = u_xlat44 * unity_SpecCube0_HDR.x;
					    u_xlat9.xyz = u_xlat8.xyz * vec3(u_xlat44);
					    u_xlatb45 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb45){
					        u_xlatb45 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb45){
					            u_xlat45 = dot(u_xlat5.xyz, u_xlat5.xyz);
					            u_xlat45 = inversesqrt(u_xlat45);
					            u_xlat10.xyz = vec3(u_xlat45) * u_xlat5.xyz;
					            u_xlat11.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat11.xyz = u_xlat11.xyz / u_xlat10.xyz;
					            u_xlat12.xyz = (-u_xlat0.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat12.xyz = u_xlat12.xyz / u_xlat10.xyz;
					            u_xlatb13.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat10.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat11;
					                hlslcc_movcTemp.x = (u_xlatb13.x) ? u_xlat11.x : u_xlat12.x;
					                hlslcc_movcTemp.y = (u_xlatb13.y) ? u_xlat11.y : u_xlat12.y;
					                hlslcc_movcTemp.z = (u_xlatb13.z) ? u_xlat11.z : u_xlat12.z;
					                u_xlat11 = hlslcc_movcTemp;
					            }
					            u_xlat45 = min(u_xlat11.y, u_xlat11.x);
					            u_xlat45 = min(u_xlat11.z, u_xlat45);
					            u_xlat0.xyz = u_xlat0.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat5.xyz = u_xlat10.xyz * vec3(u_xlat45) + u_xlat0.xyz;
					        }
					        u_xlat5 = textureLod(unity_SpecCube1, u_xlat5.xyz, 6.0);
					        u_xlat0.x = u_xlat5.w + -1.0;
					        u_xlat0.x = unity_SpecCube1_HDR.w * u_xlat0.x + 1.0;
					        u_xlat0.x = u_xlat0.x * unity_SpecCube1_HDR.x;
					        u_xlat0.xyz = u_xlat5.xyz * u_xlat0.xxx;
					        u_xlat5.xyz = vec3(u_xlat44) * u_xlat8.xyz + (-u_xlat0.xyz);
					        u_xlat9.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat0.xyz;
					    }
					    u_xlat0.xyz = vec3(u_xlat43) * u_xlat3.xyz;
					    SV_Target0.w = u_xlat43 * 0.779083729 + 0.220916271;
					    u_xlat1.xyz = u_xlat1.xyz * vec3(u_xlat42) + _WorldSpaceLightPos0.xyz;
					    u_xlat42 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat42 = max(u_xlat42, 0.00100000005);
					    u_xlat42 = inversesqrt(u_xlat42);
					    u_xlat1.xyz = vec3(u_xlat42) * u_xlat1.xyz;
					    u_xlat42 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat43 = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat43 = clamp(u_xlat43, 0.0, 1.0);
					    u_xlat1.x = dot(_WorldSpaceLightPos0.xyz, u_xlat1.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat15.x = dot(u_xlat1.xx, u_xlat1.xx);
					    u_xlat15.x = u_xlat15.x + -0.5;
					    u_xlat29 = (-u_xlat43) + 1.0;
					    u_xlat2.x = u_xlat29 * u_xlat29;
					    u_xlat2.x = u_xlat2.x * u_xlat2.x;
					    u_xlat29 = u_xlat29 * u_xlat2.x;
					    u_xlat29 = u_xlat15.x * u_xlat29 + 1.0;
					    u_xlat2.x = -abs(u_xlat42) + 1.0;
					    u_xlat16.x = u_xlat2.x * u_xlat2.x;
					    u_xlat16.x = u_xlat16.x * u_xlat16.x;
					    u_xlat2.x = u_xlat2.x * u_xlat16.x;
					    u_xlat15.x = u_xlat15.x * u_xlat2.x + 1.0;
					    u_xlat15.x = u_xlat15.x * u_xlat29;
					    u_xlat15.x = u_xlat43 * u_xlat15.x;
					    u_xlat42 = abs(u_xlat42) + u_xlat43;
					    u_xlat42 = u_xlat42 + 9.99999975e-06;
					    u_xlat0.w = 0.5 / u_xlat42;
					    u_xlat0 = u_xlat0 * vec4(0.779083729, 0.779083729, 0.779083729, 0.999999881);
					    u_xlat42 = max(u_xlat0.w, 9.99999975e-05);
					    u_xlat42 = sqrt(u_xlat42);
					    u_xlat42 = u_xlat43 * u_xlat42;
					    u_xlat15.xyz = u_xlat6.xyz * u_xlat15.xxx + u_xlat7.xyz;
					    u_xlat16.xyz = u_xlat6.xyz * vec3(u_xlat42);
					    u_xlat42 = (-u_xlat1.x) + 1.0;
					    u_xlat1.x = u_xlat42 * u_xlat42;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat42 = u_xlat42 * u_xlat1.x;
					    u_xlat42 = u_xlat42 * 0.779083729 + 0.220916301;
					    u_xlat16.xyz = vec3(u_xlat42) * u_xlat16.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat15.xyz + u_xlat16.xyz;
					    u_xlat1.xyz = u_xlat9.xyz * vec3(0.720000029, 0.720000029, 0.720000029);
					    u_xlat42 = u_xlat2.x * -2.98023224e-08 + 0.220916301;
					    u_xlat0.xyz = u_xlat1.xyz * vec3(u_xlat42) + u_xlat0.xyz;
					    u_xlat42 = vs_TEXCOORD5 / _ProjectionParams.y;
					    u_xlat42 = (-u_xlat42) + 1.0;
					    u_xlat42 = u_xlat42 * _ProjectionParams.z;
					    u_xlat42 = max(u_xlat42, 0.0);
					    u_xlat42 = u_xlat42 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat42 = clamp(u_xlat42, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat42) * u_xlat0.xyz + unity_FogColor.xyz;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDADD" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend One One, One One
			ColorMask RGB -1
			ZWrite Off
			GpuProgramID 102521
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD5.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec3 u_xlat2;
					float u_xlat9;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat2.xyz);
					    u_xlat9 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz;
					    vs_TEXCOORD1.y = u_xlat2.x;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.z = u_xlat0.y;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.z = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat0.x;
					    vs_TEXCOORD2.y = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat2.z;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD5 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD5.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD5;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat9;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0.xy = u_xlat1.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat1.xx + u_xlat0.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat1.zz + u_xlat0.xy;
					    vs_TEXCOORD5.xy = unity_WorldToLight[3].xy * u_xlat1.ww + u_xlat0.xy;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat2.xyz);
					    u_xlat9 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz;
					    vs_TEXCOORD1.y = u_xlat2.x;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.z = u_xlat0.y;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.z = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat0.x;
					    vs_TEXCOORD2.y = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat2.z;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD6;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD6 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD5.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" }
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
						vec4 unused_0_0[4];
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD6;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec3 u_xlat2;
					float u_xlat9;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD6 = u_xlat0.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat2.xyz);
					    u_xlat9 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz;
					    vs_TEXCOORD1.y = u_xlat2.x;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.z = u_xlat0.y;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.z = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat0.x;
					    vs_TEXCOORD2.y = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat2.z;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SPOT" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD6;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD6 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD5 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out float vs_TEXCOORD6;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					out vec3 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD6 = u_xlat1.z;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat13 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat1.xyz = vec3(u_xlat13) * u_xlat1.xyz;
					    u_xlat2.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat2.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat2.xyz;
					    u_xlat2.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat2.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.xyz * u_xlat2.xyz;
					    u_xlat3.xyz = u_xlat1.zxy * u_xlat2.yzx + (-u_xlat3.xyz);
					    u_xlat13 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat3.xyz = vec3(u_xlat13) * u_xlat3.xyz;
					    vs_TEXCOORD1.y = u_xlat3.x;
					    vs_TEXCOORD1.x = u_xlat2.z;
					    vs_TEXCOORD1.z = u_xlat1.y;
					    vs_TEXCOORD2.x = u_xlat2.x;
					    vs_TEXCOORD3.x = u_xlat2.y;
					    vs_TEXCOORD2.z = u_xlat1.z;
					    vs_TEXCOORD3.z = u_xlat1.x;
					    vs_TEXCOORD2.y = u_xlat3.y;
					    vs_TEXCOORD3.y = u_xlat3.z;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD5.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "FOG_LINEAR" }
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
						vec4 unused_0_0[4];
						mat4x4 unity_WorldToLight;
						float _DisplacementClouds;
						vec4 _MainTex_ST;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2;
						vec4 unity_WorldTransformParams;
						vec4 unused_1_4;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TANGENT0;
					in  vec3 in_NORMAL0;
					in  vec4 in_TEXCOORD0;
					out vec2 vs_TEXCOORD0;
					out vec2 vs_TEXCOORD5;
					out vec3 vs_TEXCOORD1;
					out float vs_TEXCOORD6;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat9;
					void main()
					{
					    u_xlat0.xyz = in_NORMAL0.xyz * vec3(vec3(_DisplacementClouds, _DisplacementClouds, _DisplacementClouds)) + in_POSITION0.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat0 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD6 = u_xlat1.z;
					    u_xlat1 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    vs_TEXCOORD4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0.xy = u_xlat1.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat1.xx + u_xlat0.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat1.zz + u_xlat0.xy;
					    vs_TEXCOORD5.xy = unity_WorldToLight[3].xy * u_xlat1.ww + u_xlat0.xy;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat1.xyz = in_TANGENT0.yyy * unity_ObjectToWorld[1].yzx;
					    u_xlat1.xyz = unity_ObjectToWorld[0].yzx * in_TANGENT0.xxx + u_xlat1.xyz;
					    u_xlat1.xyz = unity_ObjectToWorld[2].yzx * in_TANGENT0.zzz + u_xlat1.xyz;
					    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    u_xlat2.xyz = u_xlat0.zxy * u_xlat1.yzx + (-u_xlat2.xyz);
					    u_xlat9 = in_TANGENT0.w * unity_WorldTransformParams.w;
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz;
					    vs_TEXCOORD1.y = u_xlat2.x;
					    vs_TEXCOORD1.x = u_xlat1.z;
					    vs_TEXCOORD1.z = u_xlat0.y;
					    vs_TEXCOORD2.x = u_xlat1.x;
					    vs_TEXCOORD3.x = u_xlat1.y;
					    vs_TEXCOORD2.z = u_xlat0.z;
					    vs_TEXCOORD3.z = u_xlat0.x;
					    vs_TEXCOORD2.y = u_xlat2.y;
					    vs_TEXCOORD3.y = u_xlat2.z;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, vec2(u_xlat24));
					    u_xlat23 = u_xlat23 * u_xlat5.x;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					float u_xlat18;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat16 = u_xlat2.w * _AlphaScale;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat3.xy = u_xlat3.xy * vec2(_BumpScale);
					    u_xlat17 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat17 = min(u_xlat17, 1.0);
					    u_xlat17 = (-u_xlat17) + 1.0;
					    u_xlat3.z = sqrt(u_xlat17);
					    u_xlat17 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat3.xyz = vec3(u_xlat17) * u_xlat3.xyz;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat17, u_xlat18);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat4.x = dot(vs_TEXCOORD1.xyz, u_xlat3.xyz);
					    u_xlat4.y = dot(vs_TEXCOORD2.xyz, u_xlat3.xyz);
					    u_xlat4.z = dot(vs_TEXCOORD3.xyz, u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat2.xyz = vec3(u_xlat16) * u_xlat2.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat16 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat6 = u_xlat10 * u_xlat10;
					    u_xlat6 = u_xlat6 * u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat6;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat6 = -abs(u_xlat15) + 1.0;
					    u_xlat11 = u_xlat6 * u_xlat6;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat6 = u_xlat6 * u_xlat11;
					    u_xlat5.x = u_xlat5.x * u_xlat6 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = abs(u_xlat15) + u_xlat1.x;
					    u_xlat10 = u_xlat10 + 9.99999975e-06;
					    u_xlat10 = 0.5 / u_xlat10;
					    u_xlat10 = u_xlat10 * 0.999999881;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					bool u_xlatb24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5 = vs_TEXCOORD4.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD4.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD4.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlatb24 = 0.0<u_xlat5.z;
					    u_xlat24 = u_xlatb24 ? 1.0 : float(0.0);
					    u_xlat6.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat6.xy = u_xlat6.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat6.xy);
					    u_xlat24 = u_xlat24 * u_xlat6.w;
					    u_xlat25 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat25));
					    u_xlat24 = u_xlat24 * u_xlat5.x;
					    u_xlat23 = u_xlat23 * u_xlat24;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, vec2(u_xlat24));
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat24 = u_xlat5.w * u_xlat6.x;
					    u_xlat23 = u_xlat23 * u_xlat24;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat12;
					float u_xlat13;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat19 = u_xlat2.w * _AlphaScale;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat3.xy = u_xlat3.xy * vec2(_BumpScale);
					    u_xlat20 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat20 = min(u_xlat20, 1.0);
					    u_xlat20 = (-u_xlat20) + 1.0;
					    u_xlat3.z = sqrt(u_xlat20);
					    u_xlat20 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat3.xyz = vec3(u_xlat20) * u_xlat3.xyz;
					    u_xlat4.xy = vs_TEXCOORD4.yy * unity_WorldToLight[1].xy;
					    u_xlat4.xy = unity_WorldToLight[0].xy * vs_TEXCOORD4.xx + u_xlat4.xy;
					    u_xlat4.xy = unity_WorldToLight[2].xy * vs_TEXCOORD4.zz + u_xlat4.xy;
					    u_xlat4.xy = u_xlat4.xy + unity_WorldToLight[3].xy;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD4.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat20 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat21 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat20, u_xlat21);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat20 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xy);
					    u_xlat20 = u_xlat20 * u_xlat4.w;
					    u_xlat4.x = dot(vs_TEXCOORD1.xyz, u_xlat3.xyz);
					    u_xlat4.y = dot(vs_TEXCOORD2.xyz, u_xlat3.xyz);
					    u_xlat4.z = dot(vs_TEXCOORD3.xyz, u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat19 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat1.x) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat12;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat12 = u_xlat12 * u_xlat7;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat7 = -abs(u_xlat18) + 1.0;
					    u_xlat13 = u_xlat7 * u_xlat7;
					    u_xlat13 = u_xlat13 * u_xlat13;
					    u_xlat7 = u_xlat7 * u_xlat13;
					    u_xlat6.x = u_xlat6.x * u_xlat7 + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = abs(u_xlat18) + u_xlat1.x;
					    u_xlat12 = u_xlat12 + 9.99999975e-06;
					    u_xlat12 = 0.5 / u_xlat12;
					    u_xlat12 = u_xlat12 * 0.999999881;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat1.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unused_3_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, vec2(u_xlat24));
					    u_xlat23 = u_xlat23 * u_xlat5.x;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD6 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_5;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unused_3_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					float u_xlat18;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat16 = u_xlat2.w * _AlphaScale;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat3.xy = u_xlat3.xy * vec2(_BumpScale);
					    u_xlat17 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat17 = min(u_xlat17, 1.0);
					    u_xlat17 = (-u_xlat17) + 1.0;
					    u_xlat3.z = sqrt(u_xlat17);
					    u_xlat17 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat3.xyz = vec3(u_xlat17) * u_xlat3.xyz;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat17, u_xlat18);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat4.x = dot(vs_TEXCOORD1.xyz, u_xlat3.xyz);
					    u_xlat4.y = dot(vs_TEXCOORD2.xyz, u_xlat3.xyz);
					    u_xlat4.z = dot(vs_TEXCOORD3.xyz, u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat2.xyz = vec3(u_xlat16) * u_xlat2.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat16 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat6 = u_xlat10 * u_xlat10;
					    u_xlat6 = u_xlat6 * u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat6;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat6 = -abs(u_xlat15) + 1.0;
					    u_xlat11 = u_xlat6 * u_xlat6;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat6 = u_xlat6 * u_xlat11;
					    u_xlat5.x = u_xlat5.x * u_xlat6 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = abs(u_xlat15) + u_xlat1.x;
					    u_xlat10 = u_xlat10 + 9.99999975e-06;
					    u_xlat10 = 0.5 / u_xlat10;
					    u_xlat10 = u_xlat10 * 0.999999881;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat4.xyz;
					    u_xlat5.xyz = u_xlat4.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat15 = vs_TEXCOORD6 / _ProjectionParams.y;
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat15 = u_xlat15 * _ProjectionParams.z;
					    u_xlat15 = max(u_xlat15, 0.0);
					    u_xlat15 = u_xlat15 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat15);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SPOT" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unused_3_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					bool u_xlatb24;
					float u_xlat25;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5 = vs_TEXCOORD4.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD4.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD4.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlatb24 = 0.0<u_xlat5.z;
					    u_xlat24 = u_xlatb24 ? 1.0 : float(0.0);
					    u_xlat6.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat6.xy = u_xlat6.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat6.xy);
					    u_xlat24 = u_xlat24 * u_xlat6.w;
					    u_xlat25 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat25));
					    u_xlat24 = u_xlat24 * u_xlat5.x;
					    u_xlat23 = u_xlat23 * u_xlat24;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD6 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unused_3_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  float vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					float u_xlat14;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					bool u_xlatb23;
					float u_xlat24;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat2.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat3 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat4 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat22 = u_xlat3.w * _AlphaScale;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat4.x = u_xlat4.w * u_xlat4.x;
					    u_xlat4.xy = u_xlat4.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat4.xy = u_xlat4.xy * vec2(_BumpScale);
					    u_xlat23 = dot(u_xlat4.xy, u_xlat4.xy);
					    u_xlat23 = min(u_xlat23, 1.0);
					    u_xlat23 = (-u_xlat23) + 1.0;
					    u_xlat4.z = sqrt(u_xlat23);
					    u_xlat23 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat4.xyz = vec3(u_xlat23) * u_xlat4.xyz;
					    u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb23 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb23){
					        u_xlatb23 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat6.xyz = (bool(u_xlatb23)) ? u_xlat6.xyz : vs_TEXCOORD4.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat6.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat23 = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat23, u_xlat24);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat23 = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, vec2(u_xlat24));
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat24 = u_xlat5.w * u_xlat6.x;
					    u_xlat23 = u_xlat23 * u_xlat24;
					    u_xlat5.x = dot(vs_TEXCOORD1.xyz, u_xlat4.xyz);
					    u_xlat5.y = dot(vs_TEXCOORD2.xyz, u_xlat4.xyz);
					    u_xlat5.z = dot(vs_TEXCOORD3.xyz, u_xlat4.xyz);
					    u_xlat24 = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat24 = inversesqrt(u_xlat24);
					    u_xlat4.xyz = vec3(u_xlat24) * u_xlat5.xyz;
					    u_xlat5.xyz = vec3(u_xlat23) * _LightColor0.xyz;
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat3.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat22 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat2.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat2.xyz);
					    u_xlat22 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat22) + 1.0;
					    u_xlat1.x = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat1.x;
					    u_xlat14 = u_xlat7.x * u_xlat14 + 1.0;
					    u_xlat1.x = -abs(u_xlat21) + 1.0;
					    u_xlat8 = u_xlat1.x * u_xlat1.x;
					    u_xlat8 = u_xlat8 * u_xlat8;
					    u_xlat1.x = u_xlat1.x * u_xlat8;
					    u_xlat7.x = u_xlat7.x * u_xlat1.x + 1.0;
					    u_xlat7.x = u_xlat7.x * u_xlat14;
					    u_xlat14 = abs(u_xlat21) + u_xlat22;
					    u_xlat14 = u_xlat14 + 9.99999975e-06;
					    u_xlat14 = 0.5 / u_xlat14;
					    u_xlat14 = u_xlat14 * 0.999999881;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat22) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat5.xyz;
					    u_xlat7.xyz = u_xlat5.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD6 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "FOG_LINEAR" }
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
						vec4 _LightColor0;
						vec4 unused_0_2;
						mat4x4 unity_WorldToLight;
						float _BumpScale;
						float _AlphaScale;
						vec4 unused_0_6;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[45];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_3;
					};
					layout(std140) uniform UnityFog {
						vec4 unused_3_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _BumpMap;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec2 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  float vs_TEXCOORD6;
					in  vec3 vs_TEXCOORD2;
					in  vec3 vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat12;
					float u_xlat13;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD4.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2 = texture(_MainTex, vs_TEXCOORD0.xy);
					    u_xlat3 = texture(_BumpMap, vs_TEXCOORD0.xy);
					    u_xlat19 = u_xlat2.w * _AlphaScale;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat3.x = u_xlat3.w * u_xlat3.x;
					    u_xlat3.xy = u_xlat3.xy * vec2(2.0, 2.0) + vec2(-1.0, -1.0);
					    u_xlat3.xy = u_xlat3.xy * vec2(_BumpScale);
					    u_xlat20 = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat20 = min(u_xlat20, 1.0);
					    u_xlat20 = (-u_xlat20) + 1.0;
					    u_xlat3.z = sqrt(u_xlat20);
					    u_xlat20 = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat3.xyz = vec3(u_xlat20) * u_xlat3.xyz;
					    u_xlat4.xy = vs_TEXCOORD4.yy * unity_WorldToLight[1].xy;
					    u_xlat4.xy = unity_WorldToLight[0].xy * vs_TEXCOORD4.xx + u_xlat4.xy;
					    u_xlat4.xy = unity_WorldToLight[2].xy * vs_TEXCOORD4.zz + u_xlat4.xy;
					    u_xlat4.xy = u_xlat4.xy + unity_WorldToLight[3].xy;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD4.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD4.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD4.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD4.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat5.yzw = u_xlat5.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat20 = u_xlat5.y * 0.25 + 0.75;
					        u_xlat21 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat5.x = max(u_xlat20, u_xlat21);
					        u_xlat5 = texture(unity_ProbeVolumeSH, u_xlat5.xzw);
					    } else {
					        u_xlat5.x = float(1.0);
					        u_xlat5.y = float(1.0);
					        u_xlat5.z = float(1.0);
					        u_xlat5.w = float(1.0);
					    }
					    u_xlat20 = dot(u_xlat5, unity_OcclusionMaskSelector);
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xy);
					    u_xlat20 = u_xlat20 * u_xlat4.w;
					    u_xlat4.x = dot(vs_TEXCOORD1.xyz, u_xlat3.xyz);
					    u_xlat4.y = dot(vs_TEXCOORD2.xyz, u_xlat3.xyz);
					    u_xlat4.z = dot(vs_TEXCOORD3.xyz, u_xlat3.xyz);
					    u_xlat3.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat3.x = inversesqrt(u_xlat3.x);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat4.xyz;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target0.w = u_xlat19 * 0.779083729 + 0.220916271;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat3.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat3.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = dot(u_xlat0.xx, u_xlat0.xx);
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat1.x) + 1.0;
					    u_xlat7 = u_xlat12 * u_xlat12;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat12 = u_xlat12 * u_xlat7;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat7 = -abs(u_xlat18) + 1.0;
					    u_xlat13 = u_xlat7 * u_xlat7;
					    u_xlat13 = u_xlat13 * u_xlat13;
					    u_xlat7 = u_xlat7 * u_xlat13;
					    u_xlat6.x = u_xlat6.x * u_xlat7 + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = abs(u_xlat18) + u_xlat1.x;
					    u_xlat12 = u_xlat12 + 9.99999975e-06;
					    u_xlat12 = 0.5 / u_xlat12;
					    u_xlat12 = u_xlat12 * 0.999999881;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat1.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat18 = vs_TEXCOORD6 / _ProjectionParams.y;
					    u_xlat18 = (-u_xlat18) + 1.0;
					    u_xlat18 = u_xlat18 * _ProjectionParams.z;
					    u_xlat18 = max(u_xlat18, 0.0);
					    u_xlat18 = u_xlat18 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat18 = clamp(u_xlat18, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat18);
					    return;
					}"
				}
			}
		}
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" "SHADOWSUPPORT" = "true" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ColorMask RGB -1
			ZWrite Off
			GpuProgramID 217952
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "SHADOWS_SCREEN" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "SHADOWS_SCREEN" "VERTEXLIGHT_ON" }
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
						vec4 unused_0_0[8];
						float _DisplacementAtmosphere;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0.x = dot(in_NORMAL0.xyz, in_NORMAL0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.xyz = u_xlat0.xxx * in_NORMAL0.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_DisplacementAtmosphere);
					    u_xlat0.w = 0.0;
					    u_xlat0 = u_xlat0 + in_POSITION0;
					    u_xlat1 = u_xlat0.yyyy * unity_ObjectToWorld[1];
					    u_xlat1 = unity_ObjectToWorld[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_ObjectToWorld[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0 = u_xlat1 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					float u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat10 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * _WorldSpaceLightPos0.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4 = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    SV_Target0.w = u_xlat0.x * u_xlat1.x;
					    u_xlat0.x = max(u_xlat4, 0.0);
					    u_xlat1.xyz = u_xlat0.xxx * _LightColor0.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" }
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
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					float u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat10 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * _WorldSpaceLightPos0.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4 = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    SV_Target0.w = u_xlat0.x * u_xlat1.x;
					    u_xlat0.x = max(u_xlat4, 0.0);
					    u_xlat1.xyz = u_xlat0.xxx * _LightColor0.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
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
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					float u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat10 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * _WorldSpaceLightPos0.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4 = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    SV_Target0.w = u_xlat0.x * u_xlat1.x;
					    u_xlat0.x = max(u_xlat4, 0.0);
					    u_xlat1.xyz = u_xlat0.xxx * _LightColor0.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "LIGHTPROBE_SH" "SHADOWS_SCREEN" }
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
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					float u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat10 = dot(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * _WorldSpaceLightPos0.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4 = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    SV_Target0.w = u_xlat0.x * u_xlat1.x;
					    u_xlat0.x = max(u_xlat4, 0.0);
					    u_xlat1.xyz = u_xlat0.xxx * _LightColor0.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "FORWARD_DELTA"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDADD" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend One One, One One
			ColorMask RGB -1
			ZWrite Off
			Offset -1, -1
			GpuProgramID 301065
			Program "vp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
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
						vec4 unused_0_0[2];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat10;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0 = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_0_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_1_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_1_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD1.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
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
						vec4 unused_0_0[2];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat10;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0 = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
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
						vec4 unused_0_0[2];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat10;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0 = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
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
						vec4 unused_0_0[2];
						mat4x4 unity_WorldToLight;
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat10;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    vs_TEXCOORD0 = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    u_xlat1.xy = u_xlat0.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat0.xx + u_xlat1.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat0.zz + u_xlat0.xy;
					    vs_TEXCOORD2.xy = unity_WorldToLight[3].xy * u_xlat0.ww + u_xlat0.xy;
					    return;
					}"
				}
			}
			Program "fp" {
				SubProgram "d3d11 " {
					Keywords { "POINT" }
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
						vec4 unused_0_0[6];
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					uniform  sampler2D _LightTexture0;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat2.xyz = _WorldSpaceLightPos0.www * (-vs_TEXCOORD0.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat10 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * u_xlat2.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4.x = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat1.x = max(u_xlat4.x, 0.0);
					    u_xlat4.x = dot(vs_TEXCOORD2.xyz, vs_TEXCOORD2.xyz);
					    u_xlat2 = texture(_LightTexture0, u_xlat4.xx);
					    u_xlat4.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat4.xyz * u_xlat1.xxx;
					    u_xlat3.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    SV_Target0.w = 0.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" }
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
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					float u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat2.xyz = _WorldSpaceLightPos0.www * (-vs_TEXCOORD0.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat10 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * u_xlat2.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4 = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat1.x = max(u_xlat4, 0.0);
					    u_xlat1.xyz = u_xlat1.xxx * _LightColor0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    SV_Target0.w = 0.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SPOT" }
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
						vec4 unused_0_0[6];
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					bool u_xlatb0;
					vec4 u_xlat1;
					vec3 u_xlat2;
					float u_xlat3;
					float u_xlat4;
					float u_xlat7;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xy = vs_TEXCOORD2.xy / vs_TEXCOORD2.ww;
					    u_xlat0.xy = u_xlat0.xy + vec2(0.5, 0.5);
					    u_xlat0 = texture(_LightTexture0, u_xlat0.xy);
					    u_xlatb0 = 0.0<vs_TEXCOORD2.z;
					    u_xlat0.x = u_xlatb0 ? 1.0 : float(0.0);
					    u_xlat0.x = u_xlat0.w * u_xlat0.x;
					    u_xlat3 = dot(vs_TEXCOORD2.xyz, vs_TEXCOORD2.xyz);
					    u_xlat1 = texture(_LightTextureB0, vec2(u_xlat3));
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat0.xyz = u_xlat0.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = _WorldSpaceLightPos0.www * (-vs_TEXCOORD0.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat9 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * u_xlat1.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat2.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat9 = dot(u_xlat2.xyz, u_xlat1.xyz);
					    u_xlat1.x = u_xlat9 * 0.875 + 0.125;
					    u_xlat9 = u_xlat9 + u_xlat9;
					    u_xlat9 = clamp(u_xlat9, 0.0, 1.0);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xxx;
					    u_xlat1.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    u_xlat1.x = dot(u_xlat2.xyz, u_xlat1.xyz);
					    u_xlat1.x = max(u_xlat1.x, 0.0);
					    u_xlat1.x = (-u_xlat1.x) + 1.0;
					    u_xlat4 = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat4 = max(u_xlat4, 0.0);
					    u_xlat7 = (-u_xlat4) + u_xlat1.x;
					    u_xlat1.x = log2(u_xlat1.x);
					    u_xlat1.x = u_xlat1.x * 7.0;
					    u_xlat1.x = exp2(u_xlat1.x);
					    u_xlat10 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat10 = min(u_xlat10, 1.0);
					    u_xlat4 = (-u_xlat4) + u_xlat10;
					    u_xlat4 = float(1.0) / u_xlat4;
					    u_xlat4 = u_xlat4 * u_xlat7;
					    u_xlat4 = clamp(u_xlat4, 0.0, 1.0);
					    u_xlat7 = u_xlat4 * -2.0 + 3.0;
					    u_xlat4 = u_xlat4 * u_xlat4;
					    u_xlat4 = u_xlat4 * u_xlat7;
					    u_xlat7 = float(1.0) / _Outeratmospherelimit;
					    u_xlat1.x = u_xlat7 * u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat7 = u_xlat1.x * -2.0 + 3.0;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat10 = u_xlat1.x * u_xlat7;
					    u_xlat1.x = (-u_xlat7) * u_xlat1.x + 1.0;
					    u_xlat1.x = u_xlat1.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat1.x = u_xlat4 * u_xlat1.x + _Inneratmopsheredensity;
					    u_xlat9 = u_xlat9 * u_xlat1.x;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat1.xzw = vec3(u_xlat10) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat1.xzw = u_xlat1.xzw + (-_Inneratmosphere.xyz);
					    u_xlat1.xyz = vec3(u_xlat4) * u_xlat1.xzw + _Inneratmosphere.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.xyz;
					    SV_Target0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    SV_Target0.w = 0.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" }
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
						vec4 unused_0_0[6];
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					vec3 u_xlat5;
					float u_xlat8;
					float u_xlat12;
					float u_xlat13;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat12 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat0.xyz = vec3(u_xlat12) * u_xlat0.xyz;
					    u_xlat12 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat12 = inversesqrt(u_xlat12);
					    u_xlat1.xyz = vec3(u_xlat12) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat4.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat4.x = max(u_xlat4.x, 0.0);
					    u_xlat8 = (-u_xlat4.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat12 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat12 = min(u_xlat12, 1.0);
					    u_xlat4.x = (-u_xlat4.x) + u_xlat12;
					    u_xlat4.x = float(1.0) / u_xlat4.x;
					    u_xlat4.x = u_xlat4.x * u_xlat8;
					    u_xlat4.x = clamp(u_xlat4.x, 0.0, 1.0);
					    u_xlat8 = u_xlat4.x * -2.0 + 3.0;
					    u_xlat4.x = u_xlat4.x * u_xlat4.x;
					    u_xlat4.x = u_xlat4.x * u_xlat8;
					    u_xlat8 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat8 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat8 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12 = u_xlat0.x * u_xlat8;
					    u_xlat0.x = (-u_xlat8) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat4.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat12) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat4.xyz = u_xlat4.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat2.xyz = _WorldSpaceLightPos0.www * (-vs_TEXCOORD0.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat13 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat13 = inversesqrt(u_xlat13);
					    u_xlat2.xyz = vec3(u_xlat13) * u_xlat2.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat5.x = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat1.x = max(u_xlat5.x, 0.0);
					    u_xlat5.x = dot(vs_TEXCOORD2.xyz, vs_TEXCOORD2.xyz);
					    u_xlat2 = texture(_LightTextureB0, u_xlat5.xx);
					    u_xlat3 = texture(_LightTexture0, vs_TEXCOORD2.xyz);
					    u_xlat5.x = u_xlat2.x * u_xlat3.w;
					    u_xlat5.xyz = u_xlat5.xxx * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat5.xyz * u_xlat1.xxx;
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat0.xxx * u_xlat4.xyz;
					    SV_Target0.w = 0.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" }
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
						vec4 unused_0_0[6];
						vec4 _LightColor0;
						vec4 _Highatmospherecolor;
						vec4 _Lowatmospherecolor;
						float _Outeratmopsheredensity;
						float _Inneratmopsheredensity;
						float _Innerouterlimit;
						float _Inneroutersmoothness;
						float _Outeratmospherelimit;
						vec4 _Inneratmosphere;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 unused_2_1[47];
					};
					uniform  sampler2D _LightTexture0;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec2 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec4 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					float u_xlat6;
					float u_xlat9;
					float u_xlat10;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat9 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat0.xyz = vec3(u_xlat9) * u_xlat0.xyz;
					    u_xlat9 = dot(vs_TEXCOORD1.xyz, vs_TEXCOORD1.xyz);
					    u_xlat9 = inversesqrt(u_xlat9);
					    u_xlat1.xyz = vec3(u_xlat9) * vs_TEXCOORD1.xyz;
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = max(u_xlat0.x, 0.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat3.x = (-_Inneroutersmoothness) + _Innerouterlimit;
					    u_xlat3.x = max(u_xlat3.x, 0.0);
					    u_xlat6 = (-u_xlat3.x) + u_xlat0.x;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 7.0;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat9 = _Inneroutersmoothness + _Innerouterlimit;
					    u_xlat9 = min(u_xlat9, 1.0);
					    u_xlat3.x = (-u_xlat3.x) + u_xlat9;
					    u_xlat3.x = float(1.0) / u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat6 = u_xlat3.x * -2.0 + 3.0;
					    u_xlat3.x = u_xlat3.x * u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * u_xlat6;
					    u_xlat6 = float(1.0) / _Outeratmospherelimit;
					    u_xlat0.x = u_xlat6 * u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6 = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9 = u_xlat0.x * u_xlat6;
					    u_xlat0.x = (-u_xlat6) * u_xlat0.x + 1.0;
					    u_xlat0.x = u_xlat0.x * _Outeratmopsheredensity + (-_Inneratmopsheredensity);
					    u_xlat0.x = u_xlat3.x * u_xlat0.x + _Inneratmopsheredensity;
					    u_xlat2.xyz = _Highatmospherecolor.xyz + (-_Lowatmospherecolor.xyz);
					    u_xlat2.xyz = vec3(u_xlat9) * u_xlat2.xyz + _Lowatmospherecolor.xyz;
					    u_xlat2.xyz = u_xlat2.xyz + (-_Inneratmosphere.xyz);
					    u_xlat3.xyz = u_xlat3.xxx * u_xlat2.xyz + _Inneratmosphere.xyz;
					    u_xlat2.xyz = _WorldSpaceLightPos0.www * (-vs_TEXCOORD0.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat10 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat2.xyz = vec3(u_xlat10) * u_xlat2.xyz;
					    u_xlat1.x = dot(u_xlat1.xyz, u_xlat2.xyz);
					    u_xlat4.x = u_xlat1.x * 0.875 + 0.125;
					    u_xlat1.x = u_xlat1.x + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat1.x = max(u_xlat4.x, 0.0);
					    u_xlat2 = texture(_LightTexture0, vs_TEXCOORD2.xy);
					    u_xlat4.xyz = u_xlat2.www * _LightColor0.xyz;
					    u_xlat1.xyz = u_xlat4.xyz * u_xlat1.xxx;
					    u_xlat3.xyz = u_xlat3.xyz * u_xlat1.xyz;
					    SV_Target0.xyz = u_xlat0.xxx * u_xlat3.xyz;
					    SV_Target0.w = 0.0;
					    return;
					}"
				}
			}
		}
	}
	Fallback "Diffuse"
}