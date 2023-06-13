Shader "Custom/Planets_Atmosphere" {
	Properties {
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
		LOD 200
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Name "FORWARD"
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" "SHADOWSUPPORT" = "true" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 49722
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
			LOD 200
			Tags { "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDADD" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend One One, One One
			ZWrite Off
			Offset -1, -1
			GpuProgramID 115560
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