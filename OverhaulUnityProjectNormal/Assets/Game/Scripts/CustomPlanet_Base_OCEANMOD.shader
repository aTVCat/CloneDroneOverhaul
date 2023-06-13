Shader "Custom/Planet_Base_OCEANMOD" {
	Properties {
		_Oceancolor ("Ocean Color", Vector) = (0.5,0.5,0.5,0)
		_OceanGlossiness ("Ocean Glossiness", Vector) = (0.5,0.5,0.5,0)
		_OceanAO ("Ocean AO", Vector) = (0.5,0.5,0.5,0)
		_OceanEmission ("Ocean Emission", Vector) = (0.5,0.5,0.5,0)
		_AOalbedo ("AO in albedy", Range(0, 2)) = 0
		_SmoothnessShift ("Smoothness shift", Range(-1, 1)) = 0
		_AOsmoothness ("AO in smoothness", Range(-1, 1)) = 0
		_AOintensity ("AO intensity", Range(0.2, 5)) = 1
		_EmissionScale ("Emission Scale", Float) = 0
	}
	SubShader {
		LOD 300
		Tags { "RenderType" = "Opaque" }
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			GpuProgramID 65173
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_5[2];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    u_xlat0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD0.xyz = u_xlat0.xyz;
					    u_xlat6 = u_xlat0.y * u_xlat0.y;
					    u_xlat6 = u_xlat0.x * u_xlat0.x + (-u_xlat6);
					    u_xlat1 = u_xlat0.yzzx * u_xlat0.xyzz;
					    u_xlat0.x = dot(unity_SHBr, u_xlat1);
					    u_xlat0.y = dot(unity_SHBg, u_xlat1);
					    u_xlat0.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD2.xyz = unity_SHC.xyz * vec3(u_xlat6) + u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
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
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD0.xyz = u_xlat1.xyz;
					    u_xlat10 = u_xlat1.y * u_xlat1.y;
					    u_xlat10 = u_xlat1.x * u_xlat1.x + (-u_xlat10);
					    u_xlat2 = u_xlat1.yzzx * u_xlat1.xyzz;
					    u_xlat1.x = dot(unity_SHBr, u_xlat2);
					    u_xlat1.y = dot(unity_SHBg, u_xlat2);
					    u_xlat1.z = dot(unity_SHBb, u_xlat2);
					    vs_TEXCOORD2.xyz = unity_SHC.xyz * vec3(u_xlat10) + u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_0_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_11[2];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					float u_xlat15;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat15 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat1.xyz;
					    vs_TEXCOORD0.xyz = u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = u_xlat0.xyz;
					    u_xlat2 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat3 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat4 = u_xlat1.yyyy * u_xlat3;
					    u_xlat3 = u_xlat3 * u_xlat3;
					    u_xlat3 = u_xlat2 * u_xlat2 + u_xlat3;
					    u_xlat2 = u_xlat2 * u_xlat1.xxxx + u_xlat4;
					    u_xlat2 = u_xlat0 * u_xlat1.zzzz + u_xlat2;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat3;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat3 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat2 = u_xlat2 * u_xlat3;
					    u_xlat2 = max(u_xlat2, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat2;
					    u_xlat2.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat2.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat2.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat2.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat1.y * u_xlat1.y;
					    u_xlat15 = u_xlat1.x * u_xlat1.x + (-u_xlat15);
					    u_xlat1 = u_xlat1.yzzx * u_xlat1.xyzz;
					    u_xlat3.x = dot(unity_SHBr, u_xlat1);
					    u_xlat3.y = dot(unity_SHBg, u_xlat1);
					    u_xlat3.z = dot(unity_SHBb, u_xlat1);
					    u_xlat1.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD2.xyz = u_xlat0.xyz * u_xlat2.xyz + u_xlat1.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					float u_xlat18;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat18 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat2.xyz = vec3(u_xlat18) * u_xlat2.xyz;
					    vs_TEXCOORD0.xyz = u_xlat2.xyz;
					    vs_TEXCOORD1.xyz = u_xlat0.xyz;
					    u_xlat3 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat4 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat5 = u_xlat2.yyyy * u_xlat4;
					    u_xlat4 = u_xlat4 * u_xlat4;
					    u_xlat4 = u_xlat3 * u_xlat3 + u_xlat4;
					    u_xlat3 = u_xlat3 * u_xlat2.xxxx + u_xlat5;
					    u_xlat3 = u_xlat0 * u_xlat2.zzzz + u_xlat3;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat4;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat4 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat3 = max(u_xlat3, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat3;
					    u_xlat3.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat3.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat3.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat3.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat3.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat3.xyz = u_xlat0.xyz * u_xlat3.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat18 = u_xlat2.y * u_xlat2.y;
					    u_xlat18 = u_xlat2.x * u_xlat2.x + (-u_xlat18);
					    u_xlat2 = u_xlat2.yzzx * u_xlat2.xyzz;
					    u_xlat4.x = dot(unity_SHBr, u_xlat2);
					    u_xlat4.y = dot(unity_SHBg, u_xlat2);
					    u_xlat4.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat18) + u_xlat4.xyz;
					    vs_TEXCOORD2.xyz = u_xlat0.xyz * u_xlat3.xyz + u_xlat2.xyz;
					    u_xlat0.x = u_xlat1.y * _ProjectionParams.x;
					    u_xlat0.w = u_xlat0.x * 0.5;
					    u_xlat0.xz = u_xlat1.xw * vec2(0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat1.zw;
					    vs_TEXCOORD4.xy = u_xlat0.zz + u_xlat0.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_5[2];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    u_xlat0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD0.xyz = u_xlat0.xyz;
					    u_xlat6 = u_xlat0.y * u_xlat0.y;
					    u_xlat6 = u_xlat0.x * u_xlat0.x + (-u_xlat6);
					    u_xlat1 = u_xlat0.yzzx * u_xlat0.xyzz;
					    u_xlat0.x = dot(unity_SHBr, u_xlat1);
					    u_xlat0.y = dot(unity_SHBg, u_xlat1);
					    u_xlat0.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD2.xyz = unity_SHC.xyz * vec3(u_xlat6) + u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" "SHADOWS_SCREEN" }
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
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
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    u_xlat1.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD0.xyz = u_xlat1.xyz;
					    u_xlat10 = u_xlat1.y * u_xlat1.y;
					    u_xlat10 = u_xlat1.x * u_xlat1.x + (-u_xlat10);
					    u_xlat2 = u_xlat1.yzzx * u_xlat1.xyzz;
					    u_xlat1.x = dot(unity_SHBr, u_xlat2);
					    u_xlat1.y = dot(unity_SHBg, u_xlat2);
					    u_xlat1.z = dot(unity_SHBb, u_xlat2);
					    vs_TEXCOORD2.xyz = unity_SHC.xyz * vec3(u_xlat10) + u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "VERTEXLIGHT_ON" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[3];
						vec4 unity_4LightPosX0;
						vec4 unity_4LightPosY0;
						vec4 unity_4LightPosZ0;
						vec4 unity_4LightAtten0;
						vec4 unity_LightColor[8];
						vec4 unused_0_6[34];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_11[2];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					float u_xlat15;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD3 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat15 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat1.xyz;
					    vs_TEXCOORD0.xyz = u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = u_xlat0.xyz;
					    u_xlat2 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat3 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat4 = u_xlat1.yyyy * u_xlat3;
					    u_xlat3 = u_xlat3 * u_xlat3;
					    u_xlat3 = u_xlat2 * u_xlat2 + u_xlat3;
					    u_xlat2 = u_xlat2 * u_xlat1.xxxx + u_xlat4;
					    u_xlat2 = u_xlat0 * u_xlat1.zzzz + u_xlat2;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat3;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat3 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat2 = u_xlat2 * u_xlat3;
					    u_xlat2 = max(u_xlat2, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat2;
					    u_xlat2.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat2.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat2.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat2.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat2.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat2.xyz = u_xlat0.xyz * u_xlat2.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat15 = u_xlat1.y * u_xlat1.y;
					    u_xlat15 = u_xlat1.x * u_xlat1.x + (-u_xlat15);
					    u_xlat1 = u_xlat1.yzzx * u_xlat1.xyzz;
					    u_xlat3.x = dot(unity_SHBr, u_xlat1);
					    u_xlat3.y = dot(unity_SHBg, u_xlat1);
					    u_xlat3.z = dot(unity_SHBb, u_xlat1);
					    u_xlat1.xyz = unity_SHC.xyz * vec3(u_xlat15) + u_xlat3.xyz;
					    vs_TEXCOORD2.xyz = u_xlat0.xyz * u_xlat2.xyz + u_xlat1.xyz;
					    vs_TEXCOORD4 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "SHADOWS_SCREEN" "VERTEXLIGHT_ON" }
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    vs_TEXCOORD3 = u_xlat0.z;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat0.zw;
					    vs_TEXCOORD4.xy = u_xlat1.zz + u_xlat1.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" "SHADOWS_SCREEN" "VERTEXLIGHT_ON" }
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD3;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_TEXCOORD5;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					float u_xlat18;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat0.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD3 = u_xlat1.z;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat18 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat2.xyz = vec3(u_xlat18) * u_xlat2.xyz;
					    vs_TEXCOORD0.xyz = u_xlat2.xyz;
					    vs_TEXCOORD1.xyz = u_xlat0.xyz;
					    u_xlat3 = (-u_xlat0.xxxx) + unity_4LightPosX0;
					    u_xlat4 = (-u_xlat0.yyyy) + unity_4LightPosY0;
					    u_xlat0 = (-u_xlat0.zzzz) + unity_4LightPosZ0;
					    u_xlat5 = u_xlat2.yyyy * u_xlat4;
					    u_xlat4 = u_xlat4 * u_xlat4;
					    u_xlat4 = u_xlat3 * u_xlat3 + u_xlat4;
					    u_xlat3 = u_xlat3 * u_xlat2.xxxx + u_xlat5;
					    u_xlat3 = u_xlat0 * u_xlat2.zzzz + u_xlat3;
					    u_xlat0 = u_xlat0 * u_xlat0 + u_xlat4;
					    u_xlat0 = max(u_xlat0, vec4(9.99999997e-07, 9.99999997e-07, 9.99999997e-07, 9.99999997e-07));
					    u_xlat4 = inversesqrt(u_xlat0);
					    u_xlat0 = u_xlat0 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    u_xlat0 = vec4(1.0, 1.0, 1.0, 1.0) / u_xlat0;
					    u_xlat3 = u_xlat3 * u_xlat4;
					    u_xlat3 = max(u_xlat3, vec4(0.0, 0.0, 0.0, 0.0));
					    u_xlat0 = u_xlat0 * u_xlat3;
					    u_xlat3.xyz = u_xlat0.yyy * unity_LightColor[1].xyz;
					    u_xlat3.xyz = unity_LightColor[0].xyz * u_xlat0.xxx + u_xlat3.xyz;
					    u_xlat0.xyz = unity_LightColor[2].xyz * u_xlat0.zzz + u_xlat3.xyz;
					    u_xlat0.xyz = unity_LightColor[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    u_xlat3.xyz = u_xlat0.xyz * vec3(0.305306017, 0.305306017, 0.305306017) + vec3(0.682171106, 0.682171106, 0.682171106);
					    u_xlat3.xyz = u_xlat0.xyz * u_xlat3.xyz + vec3(0.0125228781, 0.0125228781, 0.0125228781);
					    u_xlat18 = u_xlat2.y * u_xlat2.y;
					    u_xlat18 = u_xlat2.x * u_xlat2.x + (-u_xlat18);
					    u_xlat2 = u_xlat2.yzzx * u_xlat2.xyzz;
					    u_xlat4.x = dot(unity_SHBr, u_xlat2);
					    u_xlat4.y = dot(unity_SHBg, u_xlat2);
					    u_xlat4.z = dot(unity_SHBb, u_xlat2);
					    u_xlat2.xyz = unity_SHC.xyz * vec3(u_xlat18) + u_xlat4.xyz;
					    vs_TEXCOORD2.xyz = u_xlat0.xyz * u_xlat3.xyz + u_xlat2.xyz;
					    u_xlat0.x = u_xlat1.y * _ProjectionParams.x;
					    u_xlat0.w = u_xlat0.x * 0.5;
					    u_xlat0.xz = u_xlat1.xw * vec2(0.5, 0.5);
					    vs_TEXCOORD4.zw = u_xlat1.zw;
					    vs_TEXCOORD4.xy = u_xlat0.zz + u_xlat0.xw;
					    vs_TEXCOORD5 = vec4(0.0, 0.0, 0.0, 0.0);
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec3 u_xlat7;
					vec3 u_xlat8;
					bvec3 u_xlatb8;
					vec3 u_xlat9;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					vec3 u_xlat14;
					vec2 u_xlat22;
					float u_xlat23;
					float u_xlat25;
					float u_xlat33;
					float u_xlat34;
					float u_xlat37;
					bool u_xlatb37;
					float u_xlat38;
					float u_xlat39;
					bool u_xlatb39;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat2.x = (-u_xlat34) + 1.0;
					    u_xlat13.x = u_xlat2.x * _AOalbedo;
					    u_xlat13.x = u_xlat13.x;
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlat13.xyz = u_xlat13.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat14.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat14.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat14.xyz;
					        u_xlat14.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat14.xyz;
					        u_xlat14.xyz = u_xlat14.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb3)) ? u_xlat14.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat14.x = u_xlat3.y * 0.25 + 0.75;
					        u_xlat4.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat14.x, u_xlat4.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat3.x = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat14.x = (-u_xlat2.x) + 1.0;
					    u_xlat25 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat25 = u_xlat25 + u_xlat25;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat25)) + (-u_xlat1.xyz);
					    u_xlat3.xzw = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlatb37 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb37){
					        u_xlat37 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat37 = inversesqrt(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat37) * u_xlat4.xyz;
					        u_xlat6.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat5.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat5.xyz;
					        u_xlatb8.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat5.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat6;
					            hlslcc_movcTemp.x = (u_xlatb8.x) ? u_xlat6.x : u_xlat7.x;
					            hlslcc_movcTemp.y = (u_xlatb8.y) ? u_xlat6.y : u_xlat7.y;
					            hlslcc_movcTemp.z = (u_xlatb8.z) ? u_xlat6.z : u_xlat7.z;
					            u_xlat6 = hlslcc_movcTemp;
					        }
					        u_xlat37 = min(u_xlat6.y, u_xlat6.x);
					        u_xlat37 = min(u_xlat6.z, u_xlat37);
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat5.xyz = u_xlat5.xyz * vec3(u_xlat37) + u_xlat6.xyz;
					    } else {
					        u_xlat5.xyz = u_xlat4.xyz;
					    }
					    u_xlat37 = (-u_xlat14.x) * 0.699999988 + 1.70000005;
					    u_xlat37 = u_xlat14.x * u_xlat37;
					    u_xlat37 = u_xlat37 * 6.0;
					    u_xlat5 = textureLod(unity_SpecCube0, u_xlat5.xyz, u_xlat37);
					    u_xlat38 = u_xlat5.w + -1.0;
					    u_xlat38 = unity_SpecCube0_HDR.w * u_xlat38 + 1.0;
					    u_xlat38 = u_xlat38 * unity_SpecCube0_HDR.x;
					    u_xlat6.xyz = u_xlat5.xyz * vec3(u_xlat38);
					    u_xlatb39 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb39){
					        u_xlatb39 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb39){
					            u_xlat39 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat39 = inversesqrt(u_xlat39);
					            u_xlat7.xyz = u_xlat4.xyz * vec3(u_xlat39);
					            u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					            u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat8;
					                hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					                hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					                hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					                u_xlat8 = hlslcc_movcTemp;
					            }
					            u_xlat39 = min(u_xlat8.y, u_xlat8.x);
					            u_xlat39 = min(u_xlat8.z, u_xlat39);
					            u_xlat8.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat7.xyz * vec3(u_xlat39) + u_xlat8.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat37);
					        u_xlat37 = u_xlat4.w + -1.0;
					        u_xlat37 = unity_SpecCube1_HDR.w * u_xlat37 + 1.0;
					        u_xlat37 = u_xlat37 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat38) * u_xlat5.xyz + (-u_xlat4.xyz);
					        u_xlat6.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat34) * u_xlat6.xyz;
					    u_xlat34 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat5.xyz = vec3(u_xlat34) * vs_TEXCOORD0.xyz;
					    u_xlat13.xyz = u_xlat13.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat12 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat12 = clamp(u_xlat12, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, u_xlat14.xx);
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22.x = (-u_xlat1.x) + 1.0;
					    u_xlat23 = u_xlat22.x * u_xlat22.x;
					    u_xlat23 = u_xlat23 * u_xlat23;
					    u_xlat22.x = u_xlat22.x * u_xlat23;
					    u_xlat22.x = u_xlat11.x * u_xlat22.x + 1.0;
					    u_xlat23 = -abs(u_xlat33) + 1.0;
					    u_xlat34 = u_xlat23 * u_xlat23;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat23 = u_xlat23 * u_xlat34;
					    u_xlat11.x = u_xlat11.x * u_xlat23 + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22.x;
					    u_xlat22.x = u_xlat14.x * u_xlat14.x;
					    u_xlat22.x = max(u_xlat22.x, 0.00200000009);
					    u_xlat34 = (-u_xlat22.x) + 1.0;
					    u_xlat37 = abs(u_xlat33) * u_xlat34 + u_xlat22.x;
					    u_xlat34 = u_xlat1.x * u_xlat34 + u_xlat22.x;
					    u_xlat33 = abs(u_xlat33) * u_xlat34;
					    u_xlat33 = u_xlat1.x * u_xlat37 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat34 = u_xlat22.x * u_xlat22.x;
					    u_xlat37 = u_xlat12 * u_xlat34 + (-u_xlat12);
					    u_xlat12 = u_xlat37 * u_xlat12 + 1.0;
					    u_xlat34 = u_xlat34 * 0.318309873;
					    u_xlat12 = u_xlat12 * u_xlat12 + 1.00000001e-07;
					    u_xlat12 = u_xlat34 / u_xlat12;
					    u_xlat22.y = u_xlat33 * u_xlat12;
					    u_xlat22.xy = u_xlat22.xy * vec2(0.280000001, 3.14159274);
					    u_xlat33 = max(u_xlat22.y, 9.99999975e-05);
					    u_xlat11.z = sqrt(u_xlat33);
					    u_xlat11.xz = u_xlat1.xx * u_xlat11.xz;
					    u_xlat22.x = (-u_xlat22.x) * u_xlat14.x + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat5.xyz = u_xlat11.xxx * u_xlat3.xzw;
					    u_xlat3.xyz = u_xlat3.xzw * u_xlat11.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = u_xlat11.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat0.xyw = u_xlat13.xyz * u_xlat5.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat22.xxx;
					    u_xlat22.x = u_xlat1.x + -0.220916301;
					    u_xlat22.x = u_xlat23 * u_xlat22.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat22.xxx + u_xlat0.xyw;
					    SV_Target0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					bvec3 u_xlatb9;
					vec3 u_xlat10;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					float u_xlat13;
					vec3 u_xlat14;
					vec3 u_xlat15;
					bool u_xlatb15;
					vec2 u_xlat24;
					float u_xlat25;
					float u_xlat27;
					float u_xlat36;
					float u_xlat37;
					float u_xlat39;
					float u_xlat40;
					bool u_xlatb40;
					float u_xlat41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat1.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat37 = log2(_OceanAO.x);
					    u_xlat37 = u_xlat37 * _AOintensity;
					    u_xlat37 = exp2(u_xlat37);
					    u_xlat2.x = (-u_xlat37) + 1.0;
					    u_xlat14.x = u_xlat2.x * _AOalbedo;
					    u_xlat14.x = u_xlat14.x;
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat14.xyz = u_xlat14.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb15 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat15.xyz = (bool(u_xlatb15)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat15.xyz = u_xlat15.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat15.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat15.x = u_xlat4.y * 0.25 + 0.75;
					        u_xlat27 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat27, u_xlat15.x);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat15.x = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat15.x = clamp(u_xlat15.x, 0.0, 1.0);
					    u_xlat27 = (-u_xlat2.x) + 1.0;
					    u_xlat39 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat39 = u_xlat39 + u_xlat39;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat39)) + (-u_xlat1.xyz);
					    u_xlat5.xyz = u_xlat15.xxx * _LightColor0.xyz;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyw = (bool(u_xlatb3)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyw = u_xlat3.xyw + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat3.xyw * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat3.x = u_xlat6.y * 0.25;
					        u_xlat15.x = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat39 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat3.x = max(u_xlat15.x, u_xlat3.x);
					        u_xlat6.x = min(u_xlat39, u_xlat3.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat9.xyz = vs_TEXCOORD0.xyz;
					        u_xlat9.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat9);
					        u_xlat7.y = dot(u_xlat8, u_xlat9);
					        u_xlat7.z = dot(u_xlat6, u_xlat9);
					    } else {
					        u_xlat6.xyz = vs_TEXCOORD0.xyz;
					        u_xlat6.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat6);
					        u_xlat7.y = dot(unity_SHAg, u_xlat6);
					        u_xlat7.z = dot(unity_SHAb, u_xlat6);
					    }
					    u_xlat3.xyw = u_xlat7.xyz + vs_TEXCOORD2.xyz;
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlat3.xyw = log2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat3.xyw = exp2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlatb40 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb40){
					        u_xlat40 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat40 = inversesqrt(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat40) * u_xlat4.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat6.xyz;
					        u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat6.xyz;
					        u_xlatb9.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat6.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat7;
					            hlslcc_movcTemp.x = (u_xlatb9.x) ? u_xlat7.x : u_xlat8.x;
					            hlslcc_movcTemp.y = (u_xlatb9.y) ? u_xlat7.y : u_xlat8.y;
					            hlslcc_movcTemp.z = (u_xlatb9.z) ? u_xlat7.z : u_xlat8.z;
					            u_xlat7 = hlslcc_movcTemp;
					        }
					        u_xlat40 = min(u_xlat7.y, u_xlat7.x);
					        u_xlat40 = min(u_xlat7.z, u_xlat40);
					        u_xlat7.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat6.xyz = u_xlat6.xyz * vec3(u_xlat40) + u_xlat7.xyz;
					    } else {
					        u_xlat6.xyz = u_xlat4.xyz;
					    }
					    u_xlat40 = (-u_xlat27) * 0.699999988 + 1.70000005;
					    u_xlat40 = u_xlat27 * u_xlat40;
					    u_xlat40 = u_xlat40 * 6.0;
					    u_xlat6 = textureLod(unity_SpecCube0, u_xlat6.xyz, u_xlat40);
					    u_xlat41 = u_xlat6.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat7.xyz = u_xlat6.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat8.xyz = u_xlat4.xyz * vec3(u_xlat42);
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					            u_xlat10.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					            u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					            {
					                vec4 hlslcc_movcTemp = u_xlat9;
					                hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					                hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					                hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					                u_xlat9 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat9.y, u_xlat9.x);
					            u_xlat42 = min(u_xlat9.z, u_xlat42);
					            u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat8.xyz * vec3(u_xlat42) + u_xlat9.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat40);
					        u_xlat40 = u_xlat4.w + -1.0;
					        u_xlat40 = unity_SpecCube1_HDR.w * u_xlat40 + 1.0;
					        u_xlat40 = u_xlat40 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat41) * u_xlat6.xyz + (-u_xlat4.xyz);
					        u_xlat7.xyz = unity_SpecCube0_BoxMin.www * u_xlat6.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat37) * u_xlat7.xyz;
					    u_xlat40 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat40 = inversesqrt(u_xlat40);
					    u_xlat6.xyz = vec3(u_xlat40) * vs_TEXCOORD0.xyz;
					    u_xlat14.xyz = u_xlat14.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat36) + _WorldSpaceLightPos0.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = max(u_xlat36, 0.00100000005);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat0.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat36 = dot(u_xlat6.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat6.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat13 = dot(u_xlat6.xyz, u_xlat0.xyz);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = dot(u_xlat12.xx, vec2(u_xlat27));
					    u_xlat12.x = u_xlat12.x + -0.5;
					    u_xlat24.x = (-u_xlat1.x) + 1.0;
					    u_xlat25 = u_xlat24.x * u_xlat24.x;
					    u_xlat25 = u_xlat25 * u_xlat25;
					    u_xlat24.x = u_xlat24.x * u_xlat25;
					    u_xlat24.x = u_xlat12.x * u_xlat24.x + 1.0;
					    u_xlat25 = -abs(u_xlat36) + 1.0;
					    u_xlat40 = u_xlat25 * u_xlat25;
					    u_xlat40 = u_xlat40 * u_xlat40;
					    u_xlat25 = u_xlat25 * u_xlat40;
					    u_xlat12.x = u_xlat12.x * u_xlat25 + 1.0;
					    u_xlat12.x = u_xlat12.x * u_xlat24.x;
					    u_xlat24.x = u_xlat27 * u_xlat27;
					    u_xlat24.x = max(u_xlat24.x, 0.00200000009);
					    u_xlat40 = (-u_xlat24.x) + 1.0;
					    u_xlat41 = abs(u_xlat36) * u_xlat40 + u_xlat24.x;
					    u_xlat40 = u_xlat1.x * u_xlat40 + u_xlat24.x;
					    u_xlat36 = abs(u_xlat36) * u_xlat40;
					    u_xlat36 = u_xlat1.x * u_xlat41 + u_xlat36;
					    u_xlat36 = u_xlat36 + 9.99999975e-06;
					    u_xlat36 = 0.5 / u_xlat36;
					    u_xlat40 = u_xlat24.x * u_xlat24.x;
					    u_xlat41 = u_xlat13 * u_xlat40 + (-u_xlat13);
					    u_xlat13 = u_xlat41 * u_xlat13 + 1.0;
					    u_xlat40 = u_xlat40 * 0.318309873;
					    u_xlat13 = u_xlat13 * u_xlat13 + 1.00000001e-07;
					    u_xlat13 = u_xlat40 / u_xlat13;
					    u_xlat24.y = u_xlat36 * u_xlat13;
					    u_xlat24.xy = u_xlat24.xy * vec2(0.280000001, 3.14159274);
					    u_xlat36 = max(u_xlat24.y, 9.99999975e-05);
					    u_xlat12.z = sqrt(u_xlat36);
					    u_xlat12.xz = u_xlat1.xx * u_xlat12.xz;
					    u_xlat24.x = (-u_xlat24.x) * u_xlat27 + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat6.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = u_xlat3.xyw * vec3(u_xlat37) + u_xlat6.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat12.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = u_xlat12.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyw = u_xlat14.xyz * u_xlat3.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat24.xxx;
					    u_xlat24.x = u_xlat1.x + -0.220916301;
					    u_xlat24.x = u_xlat25 * u_xlat24.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat24.xxx + u_xlat0.xyw;
					    SV_Target0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 unused_0_2;
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
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
					uniform  sampler2D _ShadowMapTexture;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec3 u_xlat7;
					vec3 u_xlat8;
					bvec3 u_xlatb8;
					vec3 u_xlat9;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					vec3 u_xlat14;
					bool u_xlatb14;
					vec2 u_xlat22;
					float u_xlat23;
					vec2 u_xlat25;
					float u_xlat33;
					float u_xlat34;
					float u_xlat37;
					bool u_xlatb37;
					float u_xlat38;
					float u_xlat39;
					bool u_xlatb39;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat2.x = (-u_xlat34) + 1.0;
					    u_xlat13.x = u_xlat2.x * _AOalbedo;
					    u_xlat13.x = u_xlat13.x;
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlat13.xyz = u_xlat13.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat3.x = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat14.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat14.x = dot(u_xlat14.xyz, u_xlat14.xyz);
					    u_xlat14.x = sqrt(u_xlat14.x);
					    u_xlat14.x = (-u_xlat3.x) + u_xlat14.x;
					    u_xlat3.x = unity_ShadowFadeCenterAndType.w * u_xlat14.x + u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlatb14 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb14){
					        u_xlatb14 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat14.xyz = (bool(u_xlatb14)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat14.xyz = u_xlat14.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat14.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat14.x = u_xlat4.y * 0.25 + 0.75;
					        u_xlat25.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat25.x, u_xlat14.x);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat14.x = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat25.xy = vs_TEXCOORD4.xy / vs_TEXCOORD4.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat25.xy);
					    u_xlat14.x = u_xlat14.x + (-u_xlat4.x);
					    u_xlat3.x = u_xlat3.x * u_xlat14.x + u_xlat4.x;
					    u_xlat14.x = (-u_xlat2.x) + 1.0;
					    u_xlat25.x = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat25.x = u_xlat25.x + u_xlat25.x;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-u_xlat25.xxx) + (-u_xlat1.xyz);
					    u_xlat3.xzw = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlatb37 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb37){
					        u_xlat37 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat37 = inversesqrt(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat37) * u_xlat4.xyz;
					        u_xlat6.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat5.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat5.xyz;
					        u_xlatb8.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat5.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat6;
					            hlslcc_movcTemp.x = (u_xlatb8.x) ? u_xlat6.x : u_xlat7.x;
					            hlslcc_movcTemp.y = (u_xlatb8.y) ? u_xlat6.y : u_xlat7.y;
					            hlslcc_movcTemp.z = (u_xlatb8.z) ? u_xlat6.z : u_xlat7.z;
					            u_xlat6 = hlslcc_movcTemp;
					        }
					        u_xlat37 = min(u_xlat6.y, u_xlat6.x);
					        u_xlat37 = min(u_xlat6.z, u_xlat37);
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat5.xyz = u_xlat5.xyz * vec3(u_xlat37) + u_xlat6.xyz;
					    } else {
					        u_xlat5.xyz = u_xlat4.xyz;
					    }
					    u_xlat37 = (-u_xlat14.x) * 0.699999988 + 1.70000005;
					    u_xlat37 = u_xlat14.x * u_xlat37;
					    u_xlat37 = u_xlat37 * 6.0;
					    u_xlat5 = textureLod(unity_SpecCube0, u_xlat5.xyz, u_xlat37);
					    u_xlat38 = u_xlat5.w + -1.0;
					    u_xlat38 = unity_SpecCube0_HDR.w * u_xlat38 + 1.0;
					    u_xlat38 = u_xlat38 * unity_SpecCube0_HDR.x;
					    u_xlat6.xyz = u_xlat5.xyz * vec3(u_xlat38);
					    u_xlatb39 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb39){
					        u_xlatb39 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb39){
					            u_xlat39 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat39 = inversesqrt(u_xlat39);
					            u_xlat7.xyz = u_xlat4.xyz * vec3(u_xlat39);
					            u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					            u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat8;
					                hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					                hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					                hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					                u_xlat8 = hlslcc_movcTemp;
					            }
					            u_xlat39 = min(u_xlat8.y, u_xlat8.x);
					            u_xlat39 = min(u_xlat8.z, u_xlat39);
					            u_xlat8.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat7.xyz * vec3(u_xlat39) + u_xlat8.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat37);
					        u_xlat37 = u_xlat4.w + -1.0;
					        u_xlat37 = unity_SpecCube1_HDR.w * u_xlat37 + 1.0;
					        u_xlat37 = u_xlat37 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat38) * u_xlat5.xyz + (-u_xlat4.xyz);
					        u_xlat6.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat34) * u_xlat6.xyz;
					    u_xlat34 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat5.xyz = vec3(u_xlat34) * vs_TEXCOORD0.xyz;
					    u_xlat13.xyz = u_xlat13.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat12 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat12 = clamp(u_xlat12, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, u_xlat14.xx);
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22.x = (-u_xlat1.x) + 1.0;
					    u_xlat23 = u_xlat22.x * u_xlat22.x;
					    u_xlat23 = u_xlat23 * u_xlat23;
					    u_xlat22.x = u_xlat22.x * u_xlat23;
					    u_xlat22.x = u_xlat11.x * u_xlat22.x + 1.0;
					    u_xlat23 = -abs(u_xlat33) + 1.0;
					    u_xlat34 = u_xlat23 * u_xlat23;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat23 = u_xlat23 * u_xlat34;
					    u_xlat11.x = u_xlat11.x * u_xlat23 + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22.x;
					    u_xlat22.x = u_xlat14.x * u_xlat14.x;
					    u_xlat22.x = max(u_xlat22.x, 0.00200000009);
					    u_xlat34 = (-u_xlat22.x) + 1.0;
					    u_xlat37 = abs(u_xlat33) * u_xlat34 + u_xlat22.x;
					    u_xlat34 = u_xlat1.x * u_xlat34 + u_xlat22.x;
					    u_xlat33 = abs(u_xlat33) * u_xlat34;
					    u_xlat33 = u_xlat1.x * u_xlat37 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat34 = u_xlat22.x * u_xlat22.x;
					    u_xlat37 = u_xlat12 * u_xlat34 + (-u_xlat12);
					    u_xlat12 = u_xlat37 * u_xlat12 + 1.0;
					    u_xlat34 = u_xlat34 * 0.318309873;
					    u_xlat12 = u_xlat12 * u_xlat12 + 1.00000001e-07;
					    u_xlat12 = u_xlat34 / u_xlat12;
					    u_xlat22.y = u_xlat33 * u_xlat12;
					    u_xlat22.xy = u_xlat22.xy * vec2(0.280000001, 3.14159274);
					    u_xlat33 = max(u_xlat22.y, 9.99999975e-05);
					    u_xlat11.z = sqrt(u_xlat33);
					    u_xlat11.xz = u_xlat1.xx * u_xlat11.xz;
					    u_xlat22.x = (-u_xlat22.x) * u_xlat14.x + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat5.xyz = u_xlat11.xxx * u_xlat3.xzw;
					    u_xlat3.xyz = u_xlat3.xzw * u_xlat11.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = u_xlat11.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat0.xyw = u_xlat13.xyz * u_xlat5.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat22.xxx;
					    u_xlat22.x = u_xlat1.x + -0.220916301;
					    u_xlat22.x = u_xlat23 * u_xlat22.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat22.xxx + u_xlat0.xyw;
					    SV_Target0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 unused_0_2;
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
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
					uniform  sampler2D _ShadowMapTexture;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					bvec3 u_xlatb9;
					vec3 u_xlat10;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					float u_xlat13;
					vec3 u_xlat14;
					vec3 u_xlat15;
					bool u_xlatb15;
					vec2 u_xlat24;
					float u_xlat25;
					float u_xlat27;
					bool u_xlatb27;
					float u_xlat36;
					float u_xlat37;
					float u_xlat39;
					float u_xlat40;
					bool u_xlatb40;
					float u_xlat41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat1.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat37 = log2(_OceanAO.x);
					    u_xlat37 = u_xlat37 * _AOintensity;
					    u_xlat37 = exp2(u_xlat37);
					    u_xlat2.x = (-u_xlat37) + 1.0;
					    u_xlat14.x = u_xlat2.x * _AOalbedo;
					    u_xlat14.x = u_xlat14.x;
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat14.xyz = u_xlat14.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat3.x = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat15.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat15.x = dot(u_xlat15.xyz, u_xlat15.xyz);
					    u_xlat15.x = sqrt(u_xlat15.x);
					    u_xlat15.x = (-u_xlat3.x) + u_xlat15.x;
					    u_xlat3.x = unity_ShadowFadeCenterAndType.w * u_xlat15.x + u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlatb15 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb15){
					        u_xlatb27 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb27)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat27 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat39 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat39, u_xlat27);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat27 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat27 = clamp(u_xlat27, 0.0, 1.0);
					    u_xlat4.xy = vs_TEXCOORD4.xy / vs_TEXCOORD4.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat4.xy);
					    u_xlat27 = u_xlat27 + (-u_xlat4.x);
					    u_xlat3.x = u_xlat3.x * u_xlat27 + u_xlat4.x;
					    u_xlat27 = (-u_xlat2.x) + 1.0;
					    u_xlat39 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat39 = u_xlat39 + u_xlat39;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat39)) + (-u_xlat1.xyz);
					    u_xlat5.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    if(u_xlatb15){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyw = (bool(u_xlatb3)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyw = u_xlat3.xyw + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat3.xyw * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat3.x = u_xlat6.y * 0.25;
					        u_xlat15.x = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat39 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat3.x = max(u_xlat15.x, u_xlat3.x);
					        u_xlat6.x = min(u_xlat39, u_xlat3.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat9.xyz = vs_TEXCOORD0.xyz;
					        u_xlat9.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat9);
					        u_xlat7.y = dot(u_xlat8, u_xlat9);
					        u_xlat7.z = dot(u_xlat6, u_xlat9);
					    } else {
					        u_xlat6.xyz = vs_TEXCOORD0.xyz;
					        u_xlat6.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat6);
					        u_xlat7.y = dot(unity_SHAg, u_xlat6);
					        u_xlat7.z = dot(unity_SHAb, u_xlat6);
					    }
					    u_xlat3.xyw = u_xlat7.xyz + vs_TEXCOORD2.xyz;
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlat3.xyw = log2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat3.xyw = exp2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlatb40 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb40){
					        u_xlat40 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat40 = inversesqrt(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat40) * u_xlat4.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat6.xyz;
					        u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat6.xyz;
					        u_xlatb9.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat6.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat7;
					            hlslcc_movcTemp.x = (u_xlatb9.x) ? u_xlat7.x : u_xlat8.x;
					            hlslcc_movcTemp.y = (u_xlatb9.y) ? u_xlat7.y : u_xlat8.y;
					            hlslcc_movcTemp.z = (u_xlatb9.z) ? u_xlat7.z : u_xlat8.z;
					            u_xlat7 = hlslcc_movcTemp;
					        }
					        u_xlat40 = min(u_xlat7.y, u_xlat7.x);
					        u_xlat40 = min(u_xlat7.z, u_xlat40);
					        u_xlat7.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat6.xyz = u_xlat6.xyz * vec3(u_xlat40) + u_xlat7.xyz;
					    } else {
					        u_xlat6.xyz = u_xlat4.xyz;
					    }
					    u_xlat40 = (-u_xlat27) * 0.699999988 + 1.70000005;
					    u_xlat40 = u_xlat27 * u_xlat40;
					    u_xlat40 = u_xlat40 * 6.0;
					    u_xlat6 = textureLod(unity_SpecCube0, u_xlat6.xyz, u_xlat40);
					    u_xlat41 = u_xlat6.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat7.xyz = u_xlat6.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat8.xyz = u_xlat4.xyz * vec3(u_xlat42);
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					            u_xlat10.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					            u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					            {
					                vec4 hlslcc_movcTemp = u_xlat9;
					                hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					                hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					                hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					                u_xlat9 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat9.y, u_xlat9.x);
					            u_xlat42 = min(u_xlat9.z, u_xlat42);
					            u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat8.xyz * vec3(u_xlat42) + u_xlat9.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat40);
					        u_xlat40 = u_xlat4.w + -1.0;
					        u_xlat40 = unity_SpecCube1_HDR.w * u_xlat40 + 1.0;
					        u_xlat40 = u_xlat40 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat41) * u_xlat6.xyz + (-u_xlat4.xyz);
					        u_xlat7.xyz = unity_SpecCube0_BoxMin.www * u_xlat6.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat37) * u_xlat7.xyz;
					    u_xlat40 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat40 = inversesqrt(u_xlat40);
					    u_xlat6.xyz = vec3(u_xlat40) * vs_TEXCOORD0.xyz;
					    u_xlat14.xyz = u_xlat14.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat36) + _WorldSpaceLightPos0.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = max(u_xlat36, 0.00100000005);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat0.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat36 = dot(u_xlat6.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat6.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat13 = dot(u_xlat6.xyz, u_xlat0.xyz);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = dot(u_xlat12.xx, vec2(u_xlat27));
					    u_xlat12.x = u_xlat12.x + -0.5;
					    u_xlat24.x = (-u_xlat1.x) + 1.0;
					    u_xlat25 = u_xlat24.x * u_xlat24.x;
					    u_xlat25 = u_xlat25 * u_xlat25;
					    u_xlat24.x = u_xlat24.x * u_xlat25;
					    u_xlat24.x = u_xlat12.x * u_xlat24.x + 1.0;
					    u_xlat25 = -abs(u_xlat36) + 1.0;
					    u_xlat40 = u_xlat25 * u_xlat25;
					    u_xlat40 = u_xlat40 * u_xlat40;
					    u_xlat25 = u_xlat25 * u_xlat40;
					    u_xlat12.x = u_xlat12.x * u_xlat25 + 1.0;
					    u_xlat12.x = u_xlat12.x * u_xlat24.x;
					    u_xlat24.x = u_xlat27 * u_xlat27;
					    u_xlat24.x = max(u_xlat24.x, 0.00200000009);
					    u_xlat40 = (-u_xlat24.x) + 1.0;
					    u_xlat41 = abs(u_xlat36) * u_xlat40 + u_xlat24.x;
					    u_xlat40 = u_xlat1.x * u_xlat40 + u_xlat24.x;
					    u_xlat36 = abs(u_xlat36) * u_xlat40;
					    u_xlat36 = u_xlat1.x * u_xlat41 + u_xlat36;
					    u_xlat36 = u_xlat36 + 9.99999975e-06;
					    u_xlat36 = 0.5 / u_xlat36;
					    u_xlat40 = u_xlat24.x * u_xlat24.x;
					    u_xlat41 = u_xlat13 * u_xlat40 + (-u_xlat13);
					    u_xlat13 = u_xlat41 * u_xlat13 + 1.0;
					    u_xlat40 = u_xlat40 * 0.318309873;
					    u_xlat13 = u_xlat13 * u_xlat13 + 1.00000001e-07;
					    u_xlat13 = u_xlat40 / u_xlat13;
					    u_xlat24.y = u_xlat36 * u_xlat13;
					    u_xlat24.xy = u_xlat24.xy * vec2(0.280000001, 3.14159274);
					    u_xlat36 = max(u_xlat24.y, 9.99999975e-05);
					    u_xlat12.z = sqrt(u_xlat36);
					    u_xlat12.xz = u_xlat1.xx * u_xlat12.xz;
					    u_xlat24.x = (-u_xlat24.x) * u_xlat27 + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat6.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = u_xlat3.xyw * vec3(u_xlat37) + u_xlat6.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat12.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = u_xlat12.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyw = u_xlat14.xyz * u_xlat3.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat24.xxx;
					    u_xlat24.x = u_xlat1.x + -0.220916301;
					    u_xlat24.x = u_xlat25 * u_xlat24.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat24.xxx + u_xlat0.xyw;
					    SV_Target0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec3 u_xlat7;
					vec3 u_xlat8;
					bvec3 u_xlatb8;
					vec3 u_xlat9;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					vec3 u_xlat14;
					vec2 u_xlat22;
					float u_xlat23;
					float u_xlat25;
					float u_xlat33;
					float u_xlat34;
					float u_xlat37;
					bool u_xlatb37;
					float u_xlat38;
					float u_xlat39;
					bool u_xlatb39;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat2.x = (-u_xlat34) + 1.0;
					    u_xlat13.x = u_xlat2.x * _AOalbedo;
					    u_xlat13.x = u_xlat13.x;
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlat13.xyz = u_xlat13.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat14.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat14.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat14.xyz;
					        u_xlat14.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat14.xyz;
					        u_xlat14.xyz = u_xlat14.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb3)) ? u_xlat14.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat14.x = u_xlat3.y * 0.25 + 0.75;
					        u_xlat4.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat14.x, u_xlat4.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat3.x = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat14.x = (-u_xlat2.x) + 1.0;
					    u_xlat25 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat25 = u_xlat25 + u_xlat25;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat25)) + (-u_xlat1.xyz);
					    u_xlat3.xzw = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlatb37 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb37){
					        u_xlat37 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat37 = inversesqrt(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat37) * u_xlat4.xyz;
					        u_xlat6.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat5.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat5.xyz;
					        u_xlatb8.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat5.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat6;
					            hlslcc_movcTemp.x = (u_xlatb8.x) ? u_xlat6.x : u_xlat7.x;
					            hlslcc_movcTemp.y = (u_xlatb8.y) ? u_xlat6.y : u_xlat7.y;
					            hlslcc_movcTemp.z = (u_xlatb8.z) ? u_xlat6.z : u_xlat7.z;
					            u_xlat6 = hlslcc_movcTemp;
					        }
					        u_xlat37 = min(u_xlat6.y, u_xlat6.x);
					        u_xlat37 = min(u_xlat6.z, u_xlat37);
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat5.xyz = u_xlat5.xyz * vec3(u_xlat37) + u_xlat6.xyz;
					    } else {
					        u_xlat5.xyz = u_xlat4.xyz;
					    }
					    u_xlat37 = (-u_xlat14.x) * 0.699999988 + 1.70000005;
					    u_xlat37 = u_xlat14.x * u_xlat37;
					    u_xlat37 = u_xlat37 * 6.0;
					    u_xlat5 = textureLod(unity_SpecCube0, u_xlat5.xyz, u_xlat37);
					    u_xlat38 = u_xlat5.w + -1.0;
					    u_xlat38 = unity_SpecCube0_HDR.w * u_xlat38 + 1.0;
					    u_xlat38 = u_xlat38 * unity_SpecCube0_HDR.x;
					    u_xlat6.xyz = u_xlat5.xyz * vec3(u_xlat38);
					    u_xlatb39 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb39){
					        u_xlatb39 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb39){
					            u_xlat39 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat39 = inversesqrt(u_xlat39);
					            u_xlat7.xyz = u_xlat4.xyz * vec3(u_xlat39);
					            u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					            u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat8;
					                hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					                hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					                hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					                u_xlat8 = hlslcc_movcTemp;
					            }
					            u_xlat39 = min(u_xlat8.y, u_xlat8.x);
					            u_xlat39 = min(u_xlat8.z, u_xlat39);
					            u_xlat8.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat7.xyz * vec3(u_xlat39) + u_xlat8.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat37);
					        u_xlat37 = u_xlat4.w + -1.0;
					        u_xlat37 = unity_SpecCube1_HDR.w * u_xlat37 + 1.0;
					        u_xlat37 = u_xlat37 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat38) * u_xlat5.xyz + (-u_xlat4.xyz);
					        u_xlat6.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat34) * u_xlat6.xyz;
					    u_xlat34 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat5.xyz = vec3(u_xlat34) * vs_TEXCOORD0.xyz;
					    u_xlat13.xyz = u_xlat13.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat12 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat12 = clamp(u_xlat12, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, u_xlat14.xx);
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22.x = (-u_xlat1.x) + 1.0;
					    u_xlat23 = u_xlat22.x * u_xlat22.x;
					    u_xlat23 = u_xlat23 * u_xlat23;
					    u_xlat22.x = u_xlat22.x * u_xlat23;
					    u_xlat22.x = u_xlat11.x * u_xlat22.x + 1.0;
					    u_xlat23 = -abs(u_xlat33) + 1.0;
					    u_xlat34 = u_xlat23 * u_xlat23;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat23 = u_xlat23 * u_xlat34;
					    u_xlat11.x = u_xlat11.x * u_xlat23 + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22.x;
					    u_xlat22.x = u_xlat14.x * u_xlat14.x;
					    u_xlat22.x = max(u_xlat22.x, 0.00200000009);
					    u_xlat34 = (-u_xlat22.x) + 1.0;
					    u_xlat37 = abs(u_xlat33) * u_xlat34 + u_xlat22.x;
					    u_xlat34 = u_xlat1.x * u_xlat34 + u_xlat22.x;
					    u_xlat33 = abs(u_xlat33) * u_xlat34;
					    u_xlat33 = u_xlat1.x * u_xlat37 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat34 = u_xlat22.x * u_xlat22.x;
					    u_xlat37 = u_xlat12 * u_xlat34 + (-u_xlat12);
					    u_xlat12 = u_xlat37 * u_xlat12 + 1.0;
					    u_xlat34 = u_xlat34 * 0.318309873;
					    u_xlat12 = u_xlat12 * u_xlat12 + 1.00000001e-07;
					    u_xlat12 = u_xlat34 / u_xlat12;
					    u_xlat22.y = u_xlat33 * u_xlat12;
					    u_xlat22.xy = u_xlat22.xy * vec2(0.280000001, 3.14159274);
					    u_xlat33 = max(u_xlat22.y, 9.99999975e-05);
					    u_xlat11.z = sqrt(u_xlat33);
					    u_xlat11.xz = u_xlat1.xx * u_xlat11.xz;
					    u_xlat22.x = (-u_xlat22.x) * u_xlat14.x + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat5.xyz = u_xlat11.xxx * u_xlat3.xzw;
					    u_xlat3.xyz = u_xlat3.xzw * u_xlat11.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = u_xlat11.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat0.xyw = u_xlat13.xyz * u_xlat5.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat22.xxx;
					    u_xlat22.x = u_xlat1.x + -0.220916301;
					    u_xlat22.x = u_xlat23 * u_xlat22.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat22.xxx + u_xlat0.xyw;
					    u_xlat0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    u_xlat33 = vs_TEXCOORD3 / _ProjectionParams.y;
					    u_xlat33 = (-u_xlat33) + 1.0;
					    u_xlat33 = u_xlat33 * _ProjectionParams.z;
					    u_xlat33 = max(u_xlat33, 0.0);
					    u_xlat33 = u_xlat33 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat33 = clamp(u_xlat33, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat33) * u_xlat0.xyz + unity_FogColor.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					bvec3 u_xlatb9;
					vec3 u_xlat10;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					float u_xlat13;
					vec3 u_xlat14;
					vec3 u_xlat15;
					bool u_xlatb15;
					vec2 u_xlat24;
					float u_xlat25;
					float u_xlat27;
					float u_xlat36;
					float u_xlat37;
					float u_xlat39;
					float u_xlat40;
					bool u_xlatb40;
					float u_xlat41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat1.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat37 = log2(_OceanAO.x);
					    u_xlat37 = u_xlat37 * _AOintensity;
					    u_xlat37 = exp2(u_xlat37);
					    u_xlat2.x = (-u_xlat37) + 1.0;
					    u_xlat14.x = u_xlat2.x * _AOalbedo;
					    u_xlat14.x = u_xlat14.x;
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat14.xyz = u_xlat14.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb15 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat15.xyz = (bool(u_xlatb15)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat15.xyz = u_xlat15.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat15.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat15.x = u_xlat4.y * 0.25 + 0.75;
					        u_xlat27 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat27, u_xlat15.x);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat15.x = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat15.x = clamp(u_xlat15.x, 0.0, 1.0);
					    u_xlat27 = (-u_xlat2.x) + 1.0;
					    u_xlat39 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat39 = u_xlat39 + u_xlat39;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat39)) + (-u_xlat1.xyz);
					    u_xlat5.xyz = u_xlat15.xxx * _LightColor0.xyz;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyw = (bool(u_xlatb3)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyw = u_xlat3.xyw + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat3.xyw * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat3.x = u_xlat6.y * 0.25;
					        u_xlat15.x = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat39 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat3.x = max(u_xlat15.x, u_xlat3.x);
					        u_xlat6.x = min(u_xlat39, u_xlat3.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat9.xyz = vs_TEXCOORD0.xyz;
					        u_xlat9.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat9);
					        u_xlat7.y = dot(u_xlat8, u_xlat9);
					        u_xlat7.z = dot(u_xlat6, u_xlat9);
					    } else {
					        u_xlat6.xyz = vs_TEXCOORD0.xyz;
					        u_xlat6.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat6);
					        u_xlat7.y = dot(unity_SHAg, u_xlat6);
					        u_xlat7.z = dot(unity_SHAb, u_xlat6);
					    }
					    u_xlat3.xyw = u_xlat7.xyz + vs_TEXCOORD2.xyz;
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlat3.xyw = log2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat3.xyw = exp2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlatb40 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb40){
					        u_xlat40 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat40 = inversesqrt(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat40) * u_xlat4.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat6.xyz;
					        u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat6.xyz;
					        u_xlatb9.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat6.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat7;
					            hlslcc_movcTemp.x = (u_xlatb9.x) ? u_xlat7.x : u_xlat8.x;
					            hlslcc_movcTemp.y = (u_xlatb9.y) ? u_xlat7.y : u_xlat8.y;
					            hlslcc_movcTemp.z = (u_xlatb9.z) ? u_xlat7.z : u_xlat8.z;
					            u_xlat7 = hlslcc_movcTemp;
					        }
					        u_xlat40 = min(u_xlat7.y, u_xlat7.x);
					        u_xlat40 = min(u_xlat7.z, u_xlat40);
					        u_xlat7.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat6.xyz = u_xlat6.xyz * vec3(u_xlat40) + u_xlat7.xyz;
					    } else {
					        u_xlat6.xyz = u_xlat4.xyz;
					    }
					    u_xlat40 = (-u_xlat27) * 0.699999988 + 1.70000005;
					    u_xlat40 = u_xlat27 * u_xlat40;
					    u_xlat40 = u_xlat40 * 6.0;
					    u_xlat6 = textureLod(unity_SpecCube0, u_xlat6.xyz, u_xlat40);
					    u_xlat41 = u_xlat6.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat7.xyz = u_xlat6.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat8.xyz = u_xlat4.xyz * vec3(u_xlat42);
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					            u_xlat10.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					            u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					            {
					                vec4 hlslcc_movcTemp = u_xlat9;
					                hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					                hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					                hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					                u_xlat9 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat9.y, u_xlat9.x);
					            u_xlat42 = min(u_xlat9.z, u_xlat42);
					            u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat8.xyz * vec3(u_xlat42) + u_xlat9.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat40);
					        u_xlat40 = u_xlat4.w + -1.0;
					        u_xlat40 = unity_SpecCube1_HDR.w * u_xlat40 + 1.0;
					        u_xlat40 = u_xlat40 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat41) * u_xlat6.xyz + (-u_xlat4.xyz);
					        u_xlat7.xyz = unity_SpecCube0_BoxMin.www * u_xlat6.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat37) * u_xlat7.xyz;
					    u_xlat40 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat40 = inversesqrt(u_xlat40);
					    u_xlat6.xyz = vec3(u_xlat40) * vs_TEXCOORD0.xyz;
					    u_xlat14.xyz = u_xlat14.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat36) + _WorldSpaceLightPos0.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = max(u_xlat36, 0.00100000005);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat0.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat36 = dot(u_xlat6.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat6.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat13 = dot(u_xlat6.xyz, u_xlat0.xyz);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = dot(u_xlat12.xx, vec2(u_xlat27));
					    u_xlat12.x = u_xlat12.x + -0.5;
					    u_xlat24.x = (-u_xlat1.x) + 1.0;
					    u_xlat25 = u_xlat24.x * u_xlat24.x;
					    u_xlat25 = u_xlat25 * u_xlat25;
					    u_xlat24.x = u_xlat24.x * u_xlat25;
					    u_xlat24.x = u_xlat12.x * u_xlat24.x + 1.0;
					    u_xlat25 = -abs(u_xlat36) + 1.0;
					    u_xlat40 = u_xlat25 * u_xlat25;
					    u_xlat40 = u_xlat40 * u_xlat40;
					    u_xlat25 = u_xlat25 * u_xlat40;
					    u_xlat12.x = u_xlat12.x * u_xlat25 + 1.0;
					    u_xlat12.x = u_xlat12.x * u_xlat24.x;
					    u_xlat24.x = u_xlat27 * u_xlat27;
					    u_xlat24.x = max(u_xlat24.x, 0.00200000009);
					    u_xlat40 = (-u_xlat24.x) + 1.0;
					    u_xlat41 = abs(u_xlat36) * u_xlat40 + u_xlat24.x;
					    u_xlat40 = u_xlat1.x * u_xlat40 + u_xlat24.x;
					    u_xlat36 = abs(u_xlat36) * u_xlat40;
					    u_xlat36 = u_xlat1.x * u_xlat41 + u_xlat36;
					    u_xlat36 = u_xlat36 + 9.99999975e-06;
					    u_xlat36 = 0.5 / u_xlat36;
					    u_xlat40 = u_xlat24.x * u_xlat24.x;
					    u_xlat41 = u_xlat13 * u_xlat40 + (-u_xlat13);
					    u_xlat13 = u_xlat41 * u_xlat13 + 1.0;
					    u_xlat40 = u_xlat40 * 0.318309873;
					    u_xlat13 = u_xlat13 * u_xlat13 + 1.00000001e-07;
					    u_xlat13 = u_xlat40 / u_xlat13;
					    u_xlat24.y = u_xlat36 * u_xlat13;
					    u_xlat24.xy = u_xlat24.xy * vec2(0.280000001, 3.14159274);
					    u_xlat36 = max(u_xlat24.y, 9.99999975e-05);
					    u_xlat12.z = sqrt(u_xlat36);
					    u_xlat12.xz = u_xlat1.xx * u_xlat12.xz;
					    u_xlat24.x = (-u_xlat24.x) * u_xlat27 + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat6.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = u_xlat3.xyw * vec3(u_xlat37) + u_xlat6.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat12.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = u_xlat12.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyw = u_xlat14.xyz * u_xlat3.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat24.xxx;
					    u_xlat24.x = u_xlat1.x + -0.220916301;
					    u_xlat24.x = u_xlat25 * u_xlat24.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat24.xxx + u_xlat0.xyw;
					    u_xlat0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    u_xlat36 = vs_TEXCOORD3 / _ProjectionParams.y;
					    u_xlat36 = (-u_xlat36) + 1.0;
					    u_xlat36 = u_xlat36 * _ProjectionParams.z;
					    u_xlat36 = max(u_xlat36, 0.0);
					    u_xlat36 = u_xlat36 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat36 = clamp(u_xlat36, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat36) * u_xlat0.xyz + unity_FogColor.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
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
					uniform  sampler2D _ShadowMapTexture;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					vec3 u_xlat7;
					vec3 u_xlat8;
					bvec3 u_xlatb8;
					vec3 u_xlat9;
					bvec3 u_xlatb10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					vec3 u_xlat14;
					bool u_xlatb14;
					vec2 u_xlat22;
					float u_xlat23;
					vec2 u_xlat25;
					float u_xlat33;
					float u_xlat34;
					float u_xlat37;
					bool u_xlatb37;
					float u_xlat38;
					float u_xlat39;
					bool u_xlatb39;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat2.x = (-u_xlat34) + 1.0;
					    u_xlat13.x = u_xlat2.x * _AOalbedo;
					    u_xlat13.x = u_xlat13.x;
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlat13.xyz = u_xlat13.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat3.x = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat14.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat14.x = dot(u_xlat14.xyz, u_xlat14.xyz);
					    u_xlat14.x = sqrt(u_xlat14.x);
					    u_xlat14.x = (-u_xlat3.x) + u_xlat14.x;
					    u_xlat3.x = unity_ShadowFadeCenterAndType.w * u_xlat14.x + u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlatb14 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb14){
					        u_xlatb14 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat14.xyz = (bool(u_xlatb14)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat14.xyz = u_xlat14.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat14.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat14.x = u_xlat4.y * 0.25 + 0.75;
					        u_xlat25.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat25.x, u_xlat14.x);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat14.x = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat25.xy = vs_TEXCOORD4.xy / vs_TEXCOORD4.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat25.xy);
					    u_xlat14.x = u_xlat14.x + (-u_xlat4.x);
					    u_xlat3.x = u_xlat3.x * u_xlat14.x + u_xlat4.x;
					    u_xlat14.x = (-u_xlat2.x) + 1.0;
					    u_xlat25.x = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat25.x = u_xlat25.x + u_xlat25.x;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-u_xlat25.xxx) + (-u_xlat1.xyz);
					    u_xlat3.xzw = u_xlat3.xxx * _LightColor0.xyz;
					    u_xlatb37 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb37){
					        u_xlat37 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat37 = inversesqrt(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat37) * u_xlat4.xyz;
					        u_xlat6.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat5.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat5.xyz;
					        u_xlatb8.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat5.xyzx).xyz;
					        {
					            vec3 hlslcc_movcTemp = u_xlat6;
					            hlslcc_movcTemp.x = (u_xlatb8.x) ? u_xlat6.x : u_xlat7.x;
					            hlslcc_movcTemp.y = (u_xlatb8.y) ? u_xlat6.y : u_xlat7.y;
					            hlslcc_movcTemp.z = (u_xlatb8.z) ? u_xlat6.z : u_xlat7.z;
					            u_xlat6 = hlslcc_movcTemp;
					        }
					        u_xlat37 = min(u_xlat6.y, u_xlat6.x);
					        u_xlat37 = min(u_xlat6.z, u_xlat37);
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat5.xyz = u_xlat5.xyz * vec3(u_xlat37) + u_xlat6.xyz;
					    } else {
					        u_xlat5.xyz = u_xlat4.xyz;
					    }
					    u_xlat37 = (-u_xlat14.x) * 0.699999988 + 1.70000005;
					    u_xlat37 = u_xlat14.x * u_xlat37;
					    u_xlat37 = u_xlat37 * 6.0;
					    u_xlat5 = textureLod(unity_SpecCube0, u_xlat5.xyz, u_xlat37);
					    u_xlat38 = u_xlat5.w + -1.0;
					    u_xlat38 = unity_SpecCube0_HDR.w * u_xlat38 + 1.0;
					    u_xlat38 = u_xlat38 * unity_SpecCube0_HDR.x;
					    u_xlat6.xyz = u_xlat5.xyz * vec3(u_xlat38);
					    u_xlatb39 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb39){
					        u_xlatb39 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb39){
					            u_xlat39 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat39 = inversesqrt(u_xlat39);
					            u_xlat7.xyz = u_xlat4.xyz * vec3(u_xlat39);
					            u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat8.xyz = u_xlat8.xyz / u_xlat7.xyz;
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat7.xyz;
					            u_xlatb10.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat7.xyzx).xyz;
					            {
					                vec3 hlslcc_movcTemp = u_xlat8;
					                hlslcc_movcTemp.x = (u_xlatb10.x) ? u_xlat8.x : u_xlat9.x;
					                hlslcc_movcTemp.y = (u_xlatb10.y) ? u_xlat8.y : u_xlat9.y;
					                hlslcc_movcTemp.z = (u_xlatb10.z) ? u_xlat8.z : u_xlat9.z;
					                u_xlat8 = hlslcc_movcTemp;
					            }
					            u_xlat39 = min(u_xlat8.y, u_xlat8.x);
					            u_xlat39 = min(u_xlat8.z, u_xlat39);
					            u_xlat8.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat7.xyz * vec3(u_xlat39) + u_xlat8.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat37);
					        u_xlat37 = u_xlat4.w + -1.0;
					        u_xlat37 = unity_SpecCube1_HDR.w * u_xlat37 + 1.0;
					        u_xlat37 = u_xlat37 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat37);
					        u_xlat5.xyz = vec3(u_xlat38) * u_xlat5.xyz + (-u_xlat4.xyz);
					        u_xlat6.xyz = unity_SpecCube0_BoxMin.www * u_xlat5.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat34) * u_xlat6.xyz;
					    u_xlat34 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat5.xyz = vec3(u_xlat34) * vs_TEXCOORD0.xyz;
					    u_xlat13.xyz = u_xlat13.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat5.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat12 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat12 = clamp(u_xlat12, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, u_xlat14.xx);
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22.x = (-u_xlat1.x) + 1.0;
					    u_xlat23 = u_xlat22.x * u_xlat22.x;
					    u_xlat23 = u_xlat23 * u_xlat23;
					    u_xlat22.x = u_xlat22.x * u_xlat23;
					    u_xlat22.x = u_xlat11.x * u_xlat22.x + 1.0;
					    u_xlat23 = -abs(u_xlat33) + 1.0;
					    u_xlat34 = u_xlat23 * u_xlat23;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat23 = u_xlat23 * u_xlat34;
					    u_xlat11.x = u_xlat11.x * u_xlat23 + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22.x;
					    u_xlat22.x = u_xlat14.x * u_xlat14.x;
					    u_xlat22.x = max(u_xlat22.x, 0.00200000009);
					    u_xlat34 = (-u_xlat22.x) + 1.0;
					    u_xlat37 = abs(u_xlat33) * u_xlat34 + u_xlat22.x;
					    u_xlat34 = u_xlat1.x * u_xlat34 + u_xlat22.x;
					    u_xlat33 = abs(u_xlat33) * u_xlat34;
					    u_xlat33 = u_xlat1.x * u_xlat37 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat34 = u_xlat22.x * u_xlat22.x;
					    u_xlat37 = u_xlat12 * u_xlat34 + (-u_xlat12);
					    u_xlat12 = u_xlat37 * u_xlat12 + 1.0;
					    u_xlat34 = u_xlat34 * 0.318309873;
					    u_xlat12 = u_xlat12 * u_xlat12 + 1.00000001e-07;
					    u_xlat12 = u_xlat34 / u_xlat12;
					    u_xlat22.y = u_xlat33 * u_xlat12;
					    u_xlat22.xy = u_xlat22.xy * vec2(0.280000001, 3.14159274);
					    u_xlat33 = max(u_xlat22.y, 9.99999975e-05);
					    u_xlat11.z = sqrt(u_xlat33);
					    u_xlat11.xz = u_xlat1.xx * u_xlat11.xz;
					    u_xlat22.x = (-u_xlat22.x) * u_xlat14.x + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat5.xyz = u_xlat11.xxx * u_xlat3.xzw;
					    u_xlat3.xyz = u_xlat3.xzw * u_xlat11.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = u_xlat11.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * u_xlat11.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat3.xyz;
					    u_xlat0.xyw = u_xlat13.xyz * u_xlat5.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat22.xxx;
					    u_xlat22.x = u_xlat1.x + -0.220916301;
					    u_xlat22.x = u_xlat23 * u_xlat22.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat22.xxx + u_xlat0.xyw;
					    u_xlat0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    u_xlat33 = vs_TEXCOORD3 / _ProjectionParams.y;
					    u_xlat33 = (-u_xlat33) + 1.0;
					    u_xlat33 = u_xlat33 * _ProjectionParams.z;
					    u_xlat33 = max(u_xlat33, 0.0);
					    u_xlat33 = u_xlat33 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat33 = clamp(u_xlat33, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat33) * u_xlat0.xyz + unity_FogColor.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "LIGHTPROBE_SH" "SHADOWS_SCREEN" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
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
					uniform  sampler2D _ShadowMapTexture;
					uniform  samplerCube unity_SpecCube0;
					uniform  samplerCube unity_SpecCube1;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD3;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					bvec3 u_xlatb9;
					vec3 u_xlat10;
					bvec3 u_xlatb11;
					vec3 u_xlat12;
					float u_xlat13;
					vec3 u_xlat14;
					vec3 u_xlat15;
					bool u_xlatb15;
					vec2 u_xlat24;
					float u_xlat25;
					float u_xlat27;
					bool u_xlatb27;
					float u_xlat36;
					float u_xlat37;
					float u_xlat39;
					float u_xlat40;
					bool u_xlatb40;
					float u_xlat41;
					float u_xlat42;
					bool u_xlatb42;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat1.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat37 = log2(_OceanAO.x);
					    u_xlat37 = u_xlat37 * _AOintensity;
					    u_xlat37 = exp2(u_xlat37);
					    u_xlat2.x = (-u_xlat37) + 1.0;
					    u_xlat14.x = u_xlat2.x * _AOalbedo;
					    u_xlat14.x = u_xlat14.x;
					    u_xlat14.x = clamp(u_xlat14.x, 0.0, 1.0);
					    u_xlat14.xyz = u_xlat14.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat2.x = u_xlat2.x * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat2.x = u_xlat2.x + _SmoothnessShift;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat3.x = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat15.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat15.x = dot(u_xlat15.xyz, u_xlat15.xyz);
					    u_xlat15.x = sqrt(u_xlat15.x);
					    u_xlat15.x = (-u_xlat3.x) + u_xlat15.x;
					    u_xlat3.x = unity_ShadowFadeCenterAndType.w * u_xlat15.x + u_xlat3.x;
					    u_xlat3.x = u_xlat3.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlatb15 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb15){
					        u_xlatb27 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb27)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat27 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat39 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat39, u_xlat27);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat27 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat27 = clamp(u_xlat27, 0.0, 1.0);
					    u_xlat4.xy = vs_TEXCOORD4.xy / vs_TEXCOORD4.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat4.xy);
					    u_xlat27 = u_xlat27 + (-u_xlat4.x);
					    u_xlat3.x = u_xlat3.x * u_xlat27 + u_xlat4.x;
					    u_xlat27 = (-u_xlat2.x) + 1.0;
					    u_xlat39 = dot((-u_xlat1.xyz), vs_TEXCOORD0.xyz);
					    u_xlat39 = u_xlat39 + u_xlat39;
					    u_xlat4.xyz = vs_TEXCOORD0.xyz * (-vec3(u_xlat39)) + (-u_xlat1.xyz);
					    u_xlat5.xyz = u_xlat3.xxx * _LightColor0.xyz;
					    if(u_xlatb15){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyw = (bool(u_xlatb3)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyw = u_xlat3.xyw + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat3.xyw * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat3.x = u_xlat6.y * 0.25;
					        u_xlat15.x = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat39 = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat3.x = max(u_xlat15.x, u_xlat3.x);
					        u_xlat6.x = min(u_xlat39, u_xlat3.x);
					        u_xlat7 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat8 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat3.xyw = u_xlat6.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat3.xyw);
					        u_xlat9.xyz = vs_TEXCOORD0.xyz;
					        u_xlat9.w = 1.0;
					        u_xlat7.x = dot(u_xlat7, u_xlat9);
					        u_xlat7.y = dot(u_xlat8, u_xlat9);
					        u_xlat7.z = dot(u_xlat6, u_xlat9);
					    } else {
					        u_xlat6.xyz = vs_TEXCOORD0.xyz;
					        u_xlat6.w = 1.0;
					        u_xlat7.x = dot(unity_SHAr, u_xlat6);
					        u_xlat7.y = dot(unity_SHAg, u_xlat6);
					        u_xlat7.z = dot(unity_SHAb, u_xlat6);
					    }
					    u_xlat3.xyw = u_xlat7.xyz + vs_TEXCOORD2.xyz;
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlat3.xyw = log2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat3.xyw = exp2(u_xlat3.xyw);
					    u_xlat3.xyw = u_xlat3.xyw * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat3.xyw = max(u_xlat3.xyw, vec3(0.0, 0.0, 0.0));
					    u_xlatb40 = 0.0<unity_SpecCube0_ProbePosition.w;
					    if(u_xlatb40){
					        u_xlat40 = dot(u_xlat4.xyz, u_xlat4.xyz);
					        u_xlat40 = inversesqrt(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat40) * u_xlat4.xyz;
					        u_xlat7.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMax.xyz;
					        u_xlat7.xyz = u_xlat7.xyz / u_xlat6.xyz;
					        u_xlat8.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube0_BoxMin.xyz;
					        u_xlat8.xyz = u_xlat8.xyz / u_xlat6.xyz;
					        u_xlatb9.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat6.xyzx).xyz;
					        {
					            vec4 hlslcc_movcTemp = u_xlat7;
					            hlslcc_movcTemp.x = (u_xlatb9.x) ? u_xlat7.x : u_xlat8.x;
					            hlslcc_movcTemp.y = (u_xlatb9.y) ? u_xlat7.y : u_xlat8.y;
					            hlslcc_movcTemp.z = (u_xlatb9.z) ? u_xlat7.z : u_xlat8.z;
					            u_xlat7 = hlslcc_movcTemp;
					        }
					        u_xlat40 = min(u_xlat7.y, u_xlat7.x);
					        u_xlat40 = min(u_xlat7.z, u_xlat40);
					        u_xlat7.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube0_ProbePosition.xyz);
					        u_xlat6.xyz = u_xlat6.xyz * vec3(u_xlat40) + u_xlat7.xyz;
					    } else {
					        u_xlat6.xyz = u_xlat4.xyz;
					    }
					    u_xlat40 = (-u_xlat27) * 0.699999988 + 1.70000005;
					    u_xlat40 = u_xlat27 * u_xlat40;
					    u_xlat40 = u_xlat40 * 6.0;
					    u_xlat6 = textureLod(unity_SpecCube0, u_xlat6.xyz, u_xlat40);
					    u_xlat41 = u_xlat6.w + -1.0;
					    u_xlat41 = unity_SpecCube0_HDR.w * u_xlat41 + 1.0;
					    u_xlat41 = u_xlat41 * unity_SpecCube0_HDR.x;
					    u_xlat7.xyz = u_xlat6.xyz * vec3(u_xlat41);
					    u_xlatb42 = unity_SpecCube0_BoxMin.w<0.999989986;
					    if(u_xlatb42){
					        u_xlatb42 = 0.0<unity_SpecCube1_ProbePosition.w;
					        if(u_xlatb42){
					            u_xlat42 = dot(u_xlat4.xyz, u_xlat4.xyz);
					            u_xlat42 = inversesqrt(u_xlat42);
					            u_xlat8.xyz = u_xlat4.xyz * vec3(u_xlat42);
					            u_xlat9.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMax.xyz;
					            u_xlat9.xyz = u_xlat9.xyz / u_xlat8.xyz;
					            u_xlat10.xyz = (-vs_TEXCOORD1.xyz) + unity_SpecCube1_BoxMin.xyz;
					            u_xlat10.xyz = u_xlat10.xyz / u_xlat8.xyz;
					            u_xlatb11.xyz = lessThan(vec4(0.0, 0.0, 0.0, 0.0), u_xlat8.xyzx).xyz;
					            {
					                vec4 hlslcc_movcTemp = u_xlat9;
					                hlslcc_movcTemp.x = (u_xlatb11.x) ? u_xlat9.x : u_xlat10.x;
					                hlslcc_movcTemp.y = (u_xlatb11.y) ? u_xlat9.y : u_xlat10.y;
					                hlslcc_movcTemp.z = (u_xlatb11.z) ? u_xlat9.z : u_xlat10.z;
					                u_xlat9 = hlslcc_movcTemp;
					            }
					            u_xlat42 = min(u_xlat9.y, u_xlat9.x);
					            u_xlat42 = min(u_xlat9.z, u_xlat42);
					            u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_SpecCube1_ProbePosition.xyz);
					            u_xlat4.xyz = u_xlat8.xyz * vec3(u_xlat42) + u_xlat9.xyz;
					        }
					        u_xlat4 = textureLod(unity_SpecCube1, u_xlat4.xyz, u_xlat40);
					        u_xlat40 = u_xlat4.w + -1.0;
					        u_xlat40 = unity_SpecCube1_HDR.w * u_xlat40 + 1.0;
					        u_xlat40 = u_xlat40 * unity_SpecCube1_HDR.x;
					        u_xlat4.xyz = u_xlat4.xyz * vec3(u_xlat40);
					        u_xlat6.xyz = vec3(u_xlat41) * u_xlat6.xyz + (-u_xlat4.xyz);
					        u_xlat7.xyz = unity_SpecCube0_BoxMin.www * u_xlat6.xyz + u_xlat4.xyz;
					    }
					    u_xlat4.xyz = vec3(u_xlat37) * u_xlat7.xyz;
					    u_xlat40 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat40 = inversesqrt(u_xlat40);
					    u_xlat6.xyz = vec3(u_xlat40) * vs_TEXCOORD0.xyz;
					    u_xlat14.xyz = u_xlat14.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat36) + _WorldSpaceLightPos0.xyz;
					    u_xlat36 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat36 = max(u_xlat36, 0.00100000005);
					    u_xlat36 = inversesqrt(u_xlat36);
					    u_xlat0.xyz = vec3(u_xlat36) * u_xlat0.xyz;
					    u_xlat36 = dot(u_xlat6.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat6.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat13 = dot(u_xlat6.xyz, u_xlat0.xyz);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = dot(u_xlat12.xx, vec2(u_xlat27));
					    u_xlat12.x = u_xlat12.x + -0.5;
					    u_xlat24.x = (-u_xlat1.x) + 1.0;
					    u_xlat25 = u_xlat24.x * u_xlat24.x;
					    u_xlat25 = u_xlat25 * u_xlat25;
					    u_xlat24.x = u_xlat24.x * u_xlat25;
					    u_xlat24.x = u_xlat12.x * u_xlat24.x + 1.0;
					    u_xlat25 = -abs(u_xlat36) + 1.0;
					    u_xlat40 = u_xlat25 * u_xlat25;
					    u_xlat40 = u_xlat40 * u_xlat40;
					    u_xlat25 = u_xlat25 * u_xlat40;
					    u_xlat12.x = u_xlat12.x * u_xlat25 + 1.0;
					    u_xlat12.x = u_xlat12.x * u_xlat24.x;
					    u_xlat24.x = u_xlat27 * u_xlat27;
					    u_xlat24.x = max(u_xlat24.x, 0.00200000009);
					    u_xlat40 = (-u_xlat24.x) + 1.0;
					    u_xlat41 = abs(u_xlat36) * u_xlat40 + u_xlat24.x;
					    u_xlat40 = u_xlat1.x * u_xlat40 + u_xlat24.x;
					    u_xlat36 = abs(u_xlat36) * u_xlat40;
					    u_xlat36 = u_xlat1.x * u_xlat41 + u_xlat36;
					    u_xlat36 = u_xlat36 + 9.99999975e-06;
					    u_xlat36 = 0.5 / u_xlat36;
					    u_xlat40 = u_xlat24.x * u_xlat24.x;
					    u_xlat41 = u_xlat13 * u_xlat40 + (-u_xlat13);
					    u_xlat13 = u_xlat41 * u_xlat13 + 1.0;
					    u_xlat40 = u_xlat40 * 0.318309873;
					    u_xlat13 = u_xlat13 * u_xlat13 + 1.00000001e-07;
					    u_xlat13 = u_xlat40 / u_xlat13;
					    u_xlat24.y = u_xlat36 * u_xlat13;
					    u_xlat24.xy = u_xlat24.xy * vec2(0.280000001, 3.14159274);
					    u_xlat36 = max(u_xlat24.y, 9.99999975e-05);
					    u_xlat12.z = sqrt(u_xlat36);
					    u_xlat12.xz = u_xlat1.xx * u_xlat12.xz;
					    u_xlat24.x = (-u_xlat24.x) * u_xlat27 + 1.0;
					    u_xlat1.x = u_xlat2.x + 0.220916271;
					    u_xlat1.x = min(u_xlat1.x, 1.0);
					    u_xlat6.xyz = u_xlat12.xxx * u_xlat5.xyz;
					    u_xlat3.xyz = u_xlat3.xyw * vec3(u_xlat37) + u_xlat6.xyz;
					    u_xlat5.xyz = u_xlat5.xyz * u_xlat12.zzz;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat12.x = u_xlat0.x * u_xlat0.x;
					    u_xlat12.x = u_xlat12.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * u_xlat12.x;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyw = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyw = u_xlat14.xyz * u_xlat3.xyz + u_xlat0.xyw;
					    u_xlat2.xyz = u_xlat4.xyz * u_xlat24.xxx;
					    u_xlat24.x = u_xlat1.x + -0.220916301;
					    u_xlat24.x = u_xlat25 * u_xlat24.x + 0.220916301;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat24.xxx + u_xlat0.xyw;
					    u_xlat0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat0.xyz;
					    u_xlat36 = vs_TEXCOORD3 / _ProjectionParams.y;
					    u_xlat36 = (-u_xlat36) + 1.0;
					    u_xlat36 = u_xlat36 * _ProjectionParams.z;
					    u_xlat36 = max(u_xlat36, 0.0);
					    u_xlat36 = u_xlat36 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat36 = clamp(u_xlat36, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xyz + (-unity_FogColor.xyz);
					    SV_Target0.xyz = vec3(u_xlat36) * u_xlat0.xyz + unity_FogColor.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "FORWARD"
			LOD 300
			Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
			Blend One One, One One
			ZWrite Off
			GpuProgramID 100783
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xy = u_xlat0.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat0.xx + u_xlat1.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat0.zz + u_xlat0.xy;
					    vs_TEXCOORD2.xy = unity_WorldToLight[3].xy * u_xlat0.ww + u_xlat0.xy;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" "SPOT" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" "SHADOWS_SOFT" "SPOT" }
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
						vec4 unused_0_0[9];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD3.zw = u_xlat0.zw;
					    vs_TEXCOORD3.xy = u_xlat1.zz + u_xlat1.xw;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
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
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[5];
						vec4 _ProjectionParams;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat11;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat11 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat11 = inversesqrt(u_xlat11);
					    vs_TEXCOORD0.xyz = vec3(u_xlat11) * u_xlat2.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2.xy = u_xlat0.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat0.xx + u_xlat2.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat0.zz + u_xlat0.xy;
					    vs_TEXCOORD2.xy = unity_WorldToLight[3].xy * u_xlat0.ww + u_xlat0.xy;
					    u_xlat0.x = u_xlat1.y * _ProjectionParams.x;
					    u_xlat0.w = u_xlat0.x * 0.5;
					    u_xlat0.xz = u_xlat1.xw * vec2(0.5, 0.5);
					    vs_TEXCOORD3.zw = u_xlat1.zw;
					    vs_TEXCOORD3.xy = u_xlat0.zz + u_xlat0.xw;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT" "SHADOWS_CUBE" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    vs_TEXCOORD4 = u_xlat0.z;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD2;
					out float vs_TEXCOORD4;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xy = u_xlat0.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat0.xx + u_xlat1.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat0.zz + u_xlat0.xy;
					    vs_TEXCOORD2.xy = unity_WorldToLight[3].xy * u_xlat0.ww + u_xlat0.xy;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SHADOWS_DEPTH" "SPOT" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SPOT" }
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
						vec4 unused_0_0[9];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1 = u_xlat0.yyyy * unity_WorldToLight[1];
					    u_xlat1 = unity_WorldToLight[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_WorldToLight[2] * u_xlat0.zzzz + u_xlat1;
					    vs_TEXCOORD2 = unity_WorldToLight[3] * u_xlat0.wwww + u_xlat1;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
					layout(std140) uniform UnityPerCamera {
						vec4 unused_0_0[5];
						vec4 _ProjectionParams;
						vec4 unused_0_2[3];
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD0.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    vs_TEXCOORD4 = u_xlat0.z;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD3.zw = u_xlat0.zw;
					    vs_TEXCOORD3.xy = u_xlat1.zz + u_xlat1.xw;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
						vec4 unused_0_2[6];
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[5];
						vec4 _ProjectionParams;
						vec4 unused_1_2[3];
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						mat4x4 unity_WorldToObject;
						vec4 unused_2_2[3];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_3_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_3_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_NORMAL0;
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec2 vs_TEXCOORD2;
					out float vs_TEXCOORD4;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					float u_xlat11;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    u_xlat2.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat2.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat2.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat11 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat11 = inversesqrt(u_xlat11);
					    vs_TEXCOORD0.xyz = vec3(u_xlat11) * u_xlat2.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat2.xy = u_xlat0.yy * unity_WorldToLight[1].xy;
					    u_xlat0.xy = unity_WorldToLight[0].xy * u_xlat0.xx + u_xlat2.xy;
					    u_xlat0.xy = unity_WorldToLight[2].xy * u_xlat0.zz + u_xlat0.xy;
					    vs_TEXCOORD2.xy = unity_WorldToLight[3].xy * u_xlat0.ww + u_xlat0.xy;
					    vs_TEXCOORD4 = u_xlat1.z;
					    vs_TEXCOORD3.zw = u_xlat1.zw;
					    u_xlat0.x = u_xlat1.y * _ProjectionParams.x;
					    u_xlat1.xz = u_xlat1.xw * vec2(0.5, 0.5);
					    u_xlat1.w = u_xlat0.x * 0.5;
					    vs_TEXCOORD3.xy = u_xlat1.zz + u_xlat1.xw;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" "SHADOWS_CUBE" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" "SHADOWS_CUBE" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
					out vec3 vs_TEXCOORD0;
					out float vs_TEXCOORD4;
					out vec3 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec4 vs_TEXCOORD3;
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
					    u_xlat2 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat2 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat2;
					    u_xlat2 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat2;
					    u_xlat1 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat2;
					    gl_Position = u_xlat1;
					    vs_TEXCOORD4 = u_xlat1.z;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat10 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat10 = inversesqrt(u_xlat10);
					    vs_TEXCOORD0.xyz = vec3(u_xlat10) * u_xlat1.xyz;
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = unity_ObjectToWorld[3] * in_POSITION0.wwww + u_xlat0;
					    u_xlat1.xyz = u_xlat0.yyy * unity_WorldToLight[1].xyz;
					    u_xlat1.xyz = unity_WorldToLight[0].xyz * u_xlat0.xxx + u_xlat1.xyz;
					    u_xlat0.xyz = unity_WorldToLight[2].xyz * u_xlat0.zzz + u_xlat1.xyz;
					    vs_TEXCOORD2.xyz = unity_WorldToLight[3].xyz * u_xlat0.www + u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat4 = texture(_LightTexture0, vec2(u_xlat21));
					    u_xlat20 = u_xlat20 * u_xlat4.x;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_6;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat8;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat3.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat3.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat3.xyz;
					        u_xlat3.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat3.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb17)) ? u_xlat3.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat3.y * 0.25 + 0.75;
					        u_xlat8 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat17, u_xlat8);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					bool u_xlatb21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat4 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat4;
					    u_xlat4 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat4;
					    u_xlat4 = u_xlat4 + unity_WorldToLight[3];
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlatb21 = 0.0<u_xlat4.z;
					    u_xlat21 = u_xlatb21 ? 1.0 : float(0.0);
					    u_xlat5.xy = u_xlat4.xy / u_xlat4.ww;
					    u_xlat5.xy = u_xlat5.xy + vec2(0.5, 0.5);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xy);
					    u_xlat21 = u_xlat21 * u_xlat5.w;
					    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat4 = texture(_LightTextureB0, u_xlat4.xx);
					    u_xlat21 = u_xlat21 * u_xlat4.x;
					    u_xlat20 = u_xlat20 * u_xlat21;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat21));
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xyz);
					    u_xlat21 = u_xlat4.w * u_xlat5.x;
					    u_xlat20 = u_xlat20 * u_xlat21;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    SV_Target0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat13;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.xy = vs_TEXCOORD1.yy * unity_WorldToLight[1].xy;
					    u_xlat3.xy = unity_WorldToLight[0].xy * vs_TEXCOORD1.xx + u_xlat3.xy;
					    u_xlat3.xy = unity_WorldToLight[2].xy * vs_TEXCOORD1.zz + u_xlat3.xy;
					    u_xlat3.xy = u_xlat3.xy + unity_WorldToLight[3].xy;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat13 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat17, u_xlat13);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat3 = texture(_LightTexture0, u_xlat3.xy);
					    u_xlat17 = u_xlat17 * u_xlat3.w;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" "SPOT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[8];
						mat4x4 unity_WorldToShadow[4];
						vec4 unused_3_2[12];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					vec2 u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16.x, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6 = vs_TEXCOORD1.yyyy * unity_WorldToShadow[1 / 4][1 % 4];
					    u_xlat6 = unity_WorldToShadow[0 / 4][0 % 4] * vs_TEXCOORD1.xxxx + u_xlat6;
					    u_xlat6 = unity_WorldToShadow[2 / 4][2 % 4] * vs_TEXCOORD1.zzzz + u_xlat6;
					    u_xlat6 = u_xlat6 + unity_WorldToShadow[3 / 4][3 % 4];
					    u_xlat6.xyz = u_xlat6.xyz / u_xlat6.www;
					    vec3 txVec0 = vec3(u_xlat6.xy,u_xlat6.z);
					    u_xlat16.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16.x = u_xlat16.x * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16.x) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16.x;
					    u_xlatb9 = 0.0<u_xlat5.z;
					    u_xlat9.x = u_xlatb9 ? 1.0 : float(0.0);
					    u_xlat16.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat16.xy = u_xlat16.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat16.xy);
					    u_xlat9.x = u_xlat9.x * u_xlat6.w;
					    u_xlat16.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat16.xx);
					    u_xlat9.x = u_xlat9.x * u_xlat5.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "SHADOWS_DEPTH" "SHADOWS_SOFT" "SPOT" }
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
						vec4 unused_0_2[5];
						vec4 _ShadowMapTexture_TexelSize;
						mat4x4 unity_WorldToLight;
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_8;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[8];
						mat4x4 unity_WorldToShadow[4];
						vec4 unused_3_2[12];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					vec4 u_xlat10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					bool u_xlatb13;
					float u_xlat22;
					vec2 u_xlat24;
					bool u_xlatb24;
					vec2 u_xlat31;
					float u_xlat33;
					float u_xlat34;
					float u_xlat35;
					float u_xlat36;
					float u_xlat37;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat34 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat3.xyz = vec3(u_xlat34) * u_xlat2.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat34 = (-u_xlat34) + 1.0;
					    u_xlat35 = u_xlat34 * _AOalbedo;
					    u_xlat35 = u_xlat35;
					    u_xlat35 = clamp(u_xlat35, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat35) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat34 = u_xlat34 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat34 = u_xlat34 + _SmoothnessShift;
					    u_xlat34 = clamp(u_xlat34, 0.0, 1.0);
					    u_xlat5 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat13.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat13.x = dot(u_xlat13.xyz, u_xlat13.xyz);
					    u_xlat13.x = sqrt(u_xlat13.x);
					    u_xlat13.x = (-u_xlat2.x) + u_xlat13.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat13.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb13 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb13){
					        u_xlatb13 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat13.xyz = (bool(u_xlatb13)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat13.xyz = u_xlat13.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat13.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat13.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat24.x, u_xlat13.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat13.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlatb24 = u_xlat2.x<0.99000001;
					    if(u_xlatb24){
					        u_xlat6 = vs_TEXCOORD1.yyyy * unity_WorldToShadow[1 / 4][1 % 4];
					        u_xlat6 = unity_WorldToShadow[0 / 4][0 % 4] * vs_TEXCOORD1.xxxx + u_xlat6;
					        u_xlat6 = unity_WorldToShadow[2 / 4][2 % 4] * vs_TEXCOORD1.zzzz + u_xlat6;
					        u_xlat6 = u_xlat6 + unity_WorldToShadow[3 / 4][3 % 4];
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat6.www;
					        u_xlat24.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.zw + vec2(0.5, 0.5);
					        u_xlat24.xy = floor(u_xlat24.xy);
					        u_xlat6.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.zw + (-u_xlat24.xy);
					        u_xlat7 = u_xlat6.xxyy + vec4(0.5, 1.0, 0.5, 1.0);
					        u_xlat8.xw = u_xlat7.xz * u_xlat7.xz;
					        u_xlat7.xz = u_xlat8.xw * vec2(0.5, 0.5) + (-u_xlat6.xy);
					        u_xlat9.xy = (-u_xlat6.xy) + vec2(1.0, 1.0);
					        u_xlat31.xy = min(u_xlat6.xy, vec2(0.0, 0.0));
					        u_xlat9.xy = (-u_xlat31.xy) * u_xlat31.xy + u_xlat9.xy;
					        u_xlat6.xy = max(u_xlat6.xy, vec2(0.0, 0.0));
					        u_xlat6.xy = (-u_xlat6.xy) * u_xlat6.xy + u_xlat7.yw;
					        u_xlat10.x = u_xlat7.x;
					        u_xlat10.y = u_xlat9.x;
					        u_xlat10.z = u_xlat6.x;
					        u_xlat10.w = u_xlat8.x;
					        u_xlat10 = u_xlat10 * vec4(0.444440007, 0.444440007, 0.444440007, 0.222220004);
					        u_xlat8.x = u_xlat7.z;
					        u_xlat8.y = u_xlat9.y;
					        u_xlat8.z = u_xlat6.y;
					        u_xlat7 = u_xlat8 * vec4(0.444440007, 0.444440007, 0.444440007, 0.222220004);
					        u_xlat8 = u_xlat10.ywyw + u_xlat10.xzxz;
					        u_xlat9 = u_xlat7.yyww + u_xlat7.xxzz;
					        u_xlat6.xy = u_xlat10.yw / u_xlat8.zw;
					        u_xlat6.xy = u_xlat6.xy + vec2(-1.5, 0.5);
					        u_xlat7.xy = u_xlat7.yw / u_xlat9.yw;
					        u_xlat7.xy = u_xlat7.xy + vec2(-1.5, 0.5);
					        u_xlat10.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.xx;
					        u_xlat10.zw = u_xlat7.xy * _ShadowMapTexture_TexelSize.yy;
					        u_xlat7 = u_xlat8 * u_xlat9;
					        u_xlat8 = u_xlat24.xyxy * _ShadowMapTexture_TexelSize.xyxy + u_xlat10.xzyz;
					        vec3 txVec0 = vec3(u_xlat8.xy,u_xlat6.z);
					        u_xlat36 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        vec3 txVec1 = vec3(u_xlat8.zw,u_xlat6.z);
					        u_xlat37 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat37 = u_xlat37 * u_xlat7.y;
					        u_xlat36 = u_xlat7.x * u_xlat36 + u_xlat37;
					        u_xlat8 = u_xlat24.xyxy * _ShadowMapTexture_TexelSize.xyxy + u_xlat10.xwyw;
					        vec3 txVec2 = vec3(u_xlat8.xy,u_xlat6.z);
					        u_xlat24.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat24.x = u_xlat7.z * u_xlat24.x + u_xlat36;
					        vec3 txVec3 = vec3(u_xlat8.zw,u_xlat6.z);
					        u_xlat35 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat24.x = u_xlat7.w * u_xlat35 + u_xlat24.x;
					        u_xlat35 = (-_LightShadowData.x) + 1.0;
					        u_xlat24.x = u_xlat24.x * u_xlat35 + _LightShadowData.x;
					    } else {
					        u_xlat24.x = 1.0;
					    }
					    u_xlat13.x = (-u_xlat24.x) + u_xlat13.x;
					    u_xlat2.x = u_xlat2.x * u_xlat13.x + u_xlat24.x;
					    u_xlatb13 = 0.0<u_xlat5.z;
					    u_xlat13.x = u_xlatb13 ? 1.0 : float(0.0);
					    u_xlat24.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat24.xy = u_xlat24.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat24.xy);
					    u_xlat13.x = u_xlat13.x * u_xlat6.w;
					    u_xlat24.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat24.xx);
					    u_xlat13.x = u_xlat13.x * u_xlat5.x;
					    u_xlat2.x = u_xlat2.x * u_xlat13.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat35 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat35 = inversesqrt(u_xlat35);
					    u_xlat5.xyz = vec3(u_xlat35) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat34 = (-u_xlat34) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + u_xlat3.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat35 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat35 = clamp(u_xlat35, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, vec2(u_xlat34));
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22 = (-u_xlat35) + 1.0;
					    u_xlat1.x = u_xlat22 * u_xlat22;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat22 = u_xlat22 * u_xlat1.x;
					    u_xlat22 = u_xlat11.x * u_xlat22 + 1.0;
					    u_xlat1.x = -abs(u_xlat33) + 1.0;
					    u_xlat12 = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat12;
					    u_xlat11.x = u_xlat11.x * u_xlat1.x + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22;
					    u_xlat22 = u_xlat34 * u_xlat34;
					    u_xlat22 = max(u_xlat22, 0.00200000009);
					    u_xlat1.x = (-u_xlat22) + 1.0;
					    u_xlat12 = abs(u_xlat33) * u_xlat1.x + u_xlat22;
					    u_xlat1.x = u_xlat35 * u_xlat1.x + u_xlat22;
					    u_xlat33 = abs(u_xlat33) * u_xlat1.x;
					    u_xlat33 = u_xlat35 * u_xlat12 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat1.x = u_xlat3.x * u_xlat22 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat22 = u_xlat22 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat22 = u_xlat22 / u_xlat1.x;
					    u_xlat22 = u_xlat22 * u_xlat33;
					    u_xlat22 = u_xlat22 * 3.14159274;
					    u_xlat22 = max(u_xlat22, 9.99999975e-05);
					    u_xlat11.y = sqrt(u_xlat22);
					    u_xlat11.xy = vec2(u_xlat35) * u_xlat11.xy;
					    u_xlat1.xyz = u_xlat11.xxx * u_xlat2.xyz;
					    u_xlat11.xyz = u_xlat2.xyz * u_xlat11.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat34 = u_xlat0.x * u_xlat0.x;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat0.x = u_xlat0.x * u_xlat34;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat11.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 unused_0_2;
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_6;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					vec3 u_xlat8;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat17 = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat3.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat3.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat3.x = sqrt(u_xlat3.x);
					    u_xlat3.x = (-u_xlat17) + u_xlat3.x;
					    u_xlat17 = unity_ShadowFadeCenterAndType.w * u_xlat3.x + u_xlat17;
					    u_xlat17 = u_xlat17 * _LightShadowData.z + _LightShadowData.w;
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat8.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat8.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat8.xyz;
					        u_xlat8.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat8.xyz;
					        u_xlat8.xyz = u_xlat8.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb3)) ? u_xlat8.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat8.x = u_xlat3.y * 0.25 + 0.75;
					        u_xlat4.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat8.x, u_xlat4.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat3.x = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat8.xy = vs_TEXCOORD3.xy / vs_TEXCOORD3.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat8.xy);
					    u_xlat3.x = u_xlat3.x + (-u_xlat4.x);
					    u_xlat17 = u_xlat17 * u_xlat3.x + u_xlat4.x;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat13;
					bool u_xlatb13;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					float u_xlat18;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.xy = vs_TEXCOORD1.yy * unity_WorldToLight[1].xy;
					    u_xlat3.xy = unity_WorldToLight[0].xy * vs_TEXCOORD1.xx + u_xlat3.xy;
					    u_xlat3.xy = unity_WorldToLight[2].xy * vs_TEXCOORD1.zz + u_xlat3.xy;
					    u_xlat3.xy = u_xlat3.xy + unity_WorldToLight[3].xy;
					    u_xlat4.x = unity_MatrixV[0].z;
					    u_xlat4.y = unity_MatrixV[1].z;
					    u_xlat4.z = unity_MatrixV[2].z;
					    u_xlat17 = dot(u_xlat0.xyz, u_xlat4.xyz);
					    u_xlat4.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat13 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat13 = sqrt(u_xlat13);
					    u_xlat13 = (-u_xlat17) + u_xlat13;
					    u_xlat17 = unity_ShadowFadeCenterAndType.w * u_xlat13 + u_xlat17;
					    u_xlat17 = u_xlat17 * _LightShadowData.z + _LightShadowData.w;
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlatb13 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb13){
					        u_xlatb13 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb13)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat13 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat18, u_xlat13);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat13 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat4.xy = vs_TEXCOORD3.xy / vs_TEXCOORD3.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat4.xy);
					    u_xlat13 = u_xlat13 + (-u_xlat4.x);
					    u_xlat17 = u_xlat17 * u_xlat13 + u_xlat4.x;
					    u_xlat3 = texture(_LightTexture0, u_xlat3.xy);
					    u_xlat17 = u_xlat17 * u_xlat3.w;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    SV_Target0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT" "SHADOWS_CUBE" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					    u_xlat16 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					    u_xlat16 = max(abs(u_xlat6.z), u_xlat16);
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.z);
					    u_xlat16 = max(u_xlat16, 9.99999975e-06);
					    u_xlat16 = u_xlat16 * _LightProjectionParams.w;
					    u_xlat16 = _LightProjectionParams.y / u_xlat16;
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.x);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    vec4 txVec0 = vec4(u_xlat6.xyz,u_xlat16);
					    u_xlat16 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16;
					    u_xlat9.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, u_xlat9.xx);
					    u_xlat2.x = u_xlat2.x * u_xlat5.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					vec3 u_xlat11;
					bool u_xlatb11;
					float u_xlat18;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat28 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat28 = inversesqrt(u_xlat28);
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat2.xyz;
					    u_xlat28 = log2(_OceanAO.x);
					    u_xlat28 = u_xlat28 * _AOintensity;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat29 = u_xlat28 * _AOalbedo;
					    u_xlat29 = u_xlat29;
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat29) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat28 = u_xlat28 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat28 = u_xlat28 + _SmoothnessShift;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat11.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat11.x = dot(u_xlat11.xyz, u_xlat11.xyz);
					    u_xlat11.x = sqrt(u_xlat11.x);
					    u_xlat11.x = (-u_xlat2.x) + u_xlat11.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat11.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb11 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb11){
					        u_xlatb11 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat11.xyz = (bool(u_xlatb11)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat11.xyz = u_xlat11.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat11.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat11.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat20 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat20, u_xlat11.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat11.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat11.x = clamp(u_xlat11.x, 0.0, 1.0);
					    u_xlatb20 = u_xlat2.x<0.99000001;
					    if(u_xlatb20){
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					        u_xlat20 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					        u_xlat20 = max(abs(u_xlat6.z), u_xlat20);
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.z);
					        u_xlat20 = max(u_xlat20, 9.99999975e-06);
					        u_xlat20 = u_xlat20 * _LightProjectionParams.w;
					        u_xlat20 = _LightProjectionParams.y / u_xlat20;
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.x);
					        u_xlat20 = (-u_xlat20) + 1.0;
					        u_xlat7.xyz = u_xlat6.xyz + vec3(0.0078125, 0.0078125, 0.0078125);
					        vec4 txVec0 = vec4(u_xlat7.xyz,u_xlat20);
					        u_xlat7.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, -0.0078125, 0.0078125);
					        vec4 txVec1 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.y = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, 0.0078125, -0.0078125);
					        vec4 txVec2 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.z = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat6.xyz = u_xlat6.xyz + vec3(0.0078125, -0.0078125, -0.0078125);
					        vec4 txVec3 = vec4(u_xlat6.xyz,u_xlat20);
					        u_xlat7.w = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat20 = dot(u_xlat7, vec4(0.25, 0.25, 0.25, 0.25));
					        u_xlat29 = (-_LightShadowData.x) + 1.0;
					        u_xlat20 = u_xlat20 * u_xlat29 + _LightShadowData.x;
					    } else {
					        u_xlat20 = 1.0;
					    }
					    u_xlat11.x = (-u_xlat20) + u_xlat11.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x + u_xlat20;
					    u_xlat11.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, u_xlat11.xx);
					    u_xlat2.x = u_xlat2.x * u_xlat5.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat29 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat29 = inversesqrt(u_xlat29);
					    u_xlat5.xyz = vec3(u_xlat29) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat27) + u_xlat3.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = max(u_xlat27, 0.00100000005);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat29 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9.x = dot(u_xlat9.xx, vec2(u_xlat28));
					    u_xlat9.x = u_xlat9.x + -0.5;
					    u_xlat18 = (-u_xlat29) + 1.0;
					    u_xlat1.x = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat1.x;
					    u_xlat18 = u_xlat9.x * u_xlat18 + 1.0;
					    u_xlat1.x = -abs(u_xlat27) + 1.0;
					    u_xlat10 = u_xlat1.x * u_xlat1.x;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat9.x = u_xlat9.x * u_xlat1.x + 1.0;
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat18 = u_xlat28 * u_xlat28;
					    u_xlat18 = max(u_xlat18, 0.00200000009);
					    u_xlat1.x = (-u_xlat18) + 1.0;
					    u_xlat10 = abs(u_xlat27) * u_xlat1.x + u_xlat18;
					    u_xlat1.x = u_xlat29 * u_xlat1.x + u_xlat18;
					    u_xlat27 = abs(u_xlat27) * u_xlat1.x;
					    u_xlat27 = u_xlat29 * u_xlat10 + u_xlat27;
					    u_xlat27 = u_xlat27 + 9.99999975e-06;
					    u_xlat27 = 0.5 / u_xlat27;
					    u_xlat18 = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat3.x * u_xlat18 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat18 = u_xlat18 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat18 = u_xlat18 / u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat27;
					    u_xlat18 = u_xlat18 * 3.14159274;
					    u_xlat18 = max(u_xlat18, 9.99999975e-05);
					    u_xlat9.y = sqrt(u_xlat18);
					    u_xlat9.xy = vec2(u_xlat29) * u_xlat9.xy;
					    u_xlat1.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat9.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat28 = u_xlat0.x * u_xlat0.x;
					    u_xlat28 = u_xlat28 * u_xlat28;
					    u_xlat0.x = u_xlat0.x * u_xlat28;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat9.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					    u_xlat16 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					    u_xlat16 = max(abs(u_xlat6.z), u_xlat16);
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.z);
					    u_xlat16 = max(u_xlat16, 9.99999975e-06);
					    u_xlat16 = u_xlat16 * _LightProjectionParams.w;
					    u_xlat16 = _LightProjectionParams.y / u_xlat16;
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.x);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    vec4 txVec0 = vec4(u_xlat6.xyz,u_xlat16);
					    u_xlat16 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16;
					    u_xlat9.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, u_xlat9.xx);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat9.x = u_xlat5.w * u_xlat6.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_2[4];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					vec3 u_xlat11;
					bool u_xlatb11;
					float u_xlat18;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat28 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat28 = inversesqrt(u_xlat28);
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat2.xyz;
					    u_xlat28 = log2(_OceanAO.x);
					    u_xlat28 = u_xlat28 * _AOintensity;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat29 = u_xlat28 * _AOalbedo;
					    u_xlat29 = u_xlat29;
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat29) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat28 = u_xlat28 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat28 = u_xlat28 + _SmoothnessShift;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat11.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat11.x = dot(u_xlat11.xyz, u_xlat11.xyz);
					    u_xlat11.x = sqrt(u_xlat11.x);
					    u_xlat11.x = (-u_xlat2.x) + u_xlat11.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat11.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb11 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb11){
					        u_xlatb11 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat11.xyz = (bool(u_xlatb11)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat11.xyz = u_xlat11.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat11.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat11.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat20 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat20, u_xlat11.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat11.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat11.x = clamp(u_xlat11.x, 0.0, 1.0);
					    u_xlatb20 = u_xlat2.x<0.99000001;
					    if(u_xlatb20){
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					        u_xlat20 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					        u_xlat20 = max(abs(u_xlat6.z), u_xlat20);
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.z);
					        u_xlat20 = max(u_xlat20, 9.99999975e-06);
					        u_xlat20 = u_xlat20 * _LightProjectionParams.w;
					        u_xlat20 = _LightProjectionParams.y / u_xlat20;
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.x);
					        u_xlat20 = (-u_xlat20) + 1.0;
					        u_xlat7.xyz = u_xlat6.xyz + vec3(0.0078125, 0.0078125, 0.0078125);
					        vec4 txVec0 = vec4(u_xlat7.xyz,u_xlat20);
					        u_xlat7.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, -0.0078125, 0.0078125);
					        vec4 txVec1 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.y = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, 0.0078125, -0.0078125);
					        vec4 txVec2 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.z = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat6.xyz = u_xlat6.xyz + vec3(0.0078125, -0.0078125, -0.0078125);
					        vec4 txVec3 = vec4(u_xlat6.xyz,u_xlat20);
					        u_xlat7.w = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat20 = dot(u_xlat7, vec4(0.25, 0.25, 0.25, 0.25));
					        u_xlat29 = (-_LightShadowData.x) + 1.0;
					        u_xlat20 = u_xlat20 * u_xlat29 + _LightShadowData.x;
					    } else {
					        u_xlat20 = 1.0;
					    }
					    u_xlat11.x = (-u_xlat20) + u_xlat11.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x + u_xlat20;
					    u_xlat11.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, u_xlat11.xx);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat11.x = u_xlat5.w * u_xlat6.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat29 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat29 = inversesqrt(u_xlat29);
					    u_xlat5.xyz = vec3(u_xlat29) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat27) + u_xlat3.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = max(u_xlat27, 0.00100000005);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat29 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9.x = dot(u_xlat9.xx, vec2(u_xlat28));
					    u_xlat9.x = u_xlat9.x + -0.5;
					    u_xlat18 = (-u_xlat29) + 1.0;
					    u_xlat1.x = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat1.x;
					    u_xlat18 = u_xlat9.x * u_xlat18 + 1.0;
					    u_xlat1.x = -abs(u_xlat27) + 1.0;
					    u_xlat10 = u_xlat1.x * u_xlat1.x;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat9.x = u_xlat9.x * u_xlat1.x + 1.0;
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat18 = u_xlat28 * u_xlat28;
					    u_xlat18 = max(u_xlat18, 0.00200000009);
					    u_xlat1.x = (-u_xlat18) + 1.0;
					    u_xlat10 = abs(u_xlat27) * u_xlat1.x + u_xlat18;
					    u_xlat1.x = u_xlat29 * u_xlat1.x + u_xlat18;
					    u_xlat27 = abs(u_xlat27) * u_xlat1.x;
					    u_xlat27 = u_xlat29 * u_xlat10 + u_xlat27;
					    u_xlat27 = u_xlat27 + 9.99999975e-06;
					    u_xlat27 = 0.5 / u_xlat27;
					    u_xlat18 = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat3.x * u_xlat18 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat18 = u_xlat18 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat18 = u_xlat18 / u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat27;
					    u_xlat18 = u_xlat18 * 3.14159274;
					    u_xlat18 = max(u_xlat18, 9.99999975e-05);
					    u_xlat9.y = sqrt(u_xlat18);
					    u_xlat9.xy = vec2(u_xlat29) * u_xlat9.xy;
					    u_xlat1.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat9.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat28 = u_xlat0.x * u_xlat0.x;
					    u_xlat28 = u_xlat28 * u_xlat28;
					    u_xlat0.x = u_xlat0.x * u_xlat28;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat9.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat4 = texture(_LightTexture0, vec2(u_xlat21));
					    u_xlat20 = u_xlat20 * u_xlat4.x;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat18 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat18 = (-u_xlat18) + 1.0;
					    u_xlat18 = u_xlat18 * _ProjectionParams.z;
					    u_xlat18 = max(u_xlat18, 0.0);
					    u_xlat18 = u_xlat18 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat18 = clamp(u_xlat18, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat18);
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_6;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat8;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat3.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat3.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat3.xyz;
					        u_xlat3.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat3.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb17)) ? u_xlat3.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat3.y * 0.25 + 0.75;
					        u_xlat8 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat17, u_xlat8);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat15 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat15 = u_xlat15 * _ProjectionParams.z;
					    u_xlat15 = max(u_xlat15, 0.0);
					    u_xlat15 = u_xlat15 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat15);
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					bool u_xlatb21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat4 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat4;
					    u_xlat4 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat4;
					    u_xlat4 = u_xlat4 + unity_WorldToLight[3];
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlatb21 = 0.0<u_xlat4.z;
					    u_xlat21 = u_xlatb21 ? 1.0 : float(0.0);
					    u_xlat5.xy = u_xlat4.xy / u_xlat4.ww;
					    u_xlat5.xy = u_xlat5.xy + vec2(0.5, 0.5);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xy);
					    u_xlat21 = u_xlat21 * u_xlat5.w;
					    u_xlat4.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat4 = texture(_LightTextureB0, u_xlat4.xx);
					    u_xlat21 = u_xlat21 * u_xlat4.x;
					    u_xlat20 = u_xlat20 * u_xlat21;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat18 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat18 = (-u_xlat18) + 1.0;
					    u_xlat18 = u_xlat18 * _ProjectionParams.z;
					    u_xlat18 = max(u_xlat18, 0.0);
					    u_xlat18 = u_xlat18 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat18 = clamp(u_xlat18, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat18);
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat6;
					float u_xlat7;
					float u_xlat8;
					float u_xlat12;
					float u_xlat18;
					float u_xlat19;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat21;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat1.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat19 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat19 = inversesqrt(u_xlat19);
					    u_xlat2.xyz = vec3(u_xlat19) * u_xlat2.xyz;
					    u_xlat19 = log2(_OceanAO.x);
					    u_xlat19 = u_xlat19 * _AOintensity;
					    u_xlat19 = exp2(u_xlat19);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat20 = u_xlat19 * _AOalbedo;
					    u_xlat20 = u_xlat20;
					    u_xlat20 = clamp(u_xlat20, 0.0, 1.0);
					    u_xlat3.xyz = vec3(u_xlat20) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat19 = u_xlat19 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat19 = u_xlat19 + _SmoothnessShift;
					    u_xlat19 = clamp(u_xlat19, 0.0, 1.0);
					    u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat4.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					    u_xlat4.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					    u_xlat4.xyz = u_xlat4.xyz + unity_WorldToLight[3].xyz;
					    u_xlatb20 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb20){
					        u_xlatb20 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					        u_xlat5.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					        u_xlat5.xyz = u_xlat5.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat5.xyz = (bool(u_xlatb20)) ? u_xlat5.xyz : vs_TEXCOORD1.xyz;
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
					    u_xlat21 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat5 = texture(_LightTextureB0, vec2(u_xlat21));
					    u_xlat4 = texture(_LightTexture0, u_xlat4.xyz);
					    u_xlat21 = u_xlat4.w * u_xlat5.x;
					    u_xlat20 = u_xlat20 * u_xlat21;
					    u_xlat4.xyz = vec3(u_xlat20) * _LightColor0.xyz;
					    u_xlat20 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat20 = inversesqrt(u_xlat20);
					    u_xlat5.xyz = vec3(u_xlat20) * vs_TEXCOORD0.xyz;
					    u_xlat3.xyz = u_xlat3.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat19 = (-u_xlat19) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat18) + u_xlat2.xyz;
					    u_xlat18 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat18 = max(u_xlat18, 0.00100000005);
					    u_xlat18 = inversesqrt(u_xlat18);
					    u_xlat0.xyz = vec3(u_xlat18) * u_xlat0.xyz;
					    u_xlat18 = dot(u_xlat5.xyz, u_xlat2.xyz);
					    u_xlat2.x = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat8 = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat8 = clamp(u_xlat8, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat6.x = u_xlat0.x * u_xlat0.x;
					    u_xlat6.x = dot(u_xlat6.xx, vec2(u_xlat19));
					    u_xlat6.x = u_xlat6.x + -0.5;
					    u_xlat12 = (-u_xlat2.x) + 1.0;
					    u_xlat1.x = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat1.x;
					    u_xlat12 = u_xlat6.x * u_xlat12 + 1.0;
					    u_xlat1.x = -abs(u_xlat18) + 1.0;
					    u_xlat7 = u_xlat1.x * u_xlat1.x;
					    u_xlat7 = u_xlat7 * u_xlat7;
					    u_xlat1.x = u_xlat1.x * u_xlat7;
					    u_xlat6.x = u_xlat6.x * u_xlat1.x + 1.0;
					    u_xlat6.x = u_xlat6.x * u_xlat12;
					    u_xlat12 = u_xlat19 * u_xlat19;
					    u_xlat12 = max(u_xlat12, 0.00200000009);
					    u_xlat1.x = (-u_xlat12) + 1.0;
					    u_xlat7 = abs(u_xlat18) * u_xlat1.x + u_xlat12;
					    u_xlat1.x = u_xlat2.x * u_xlat1.x + u_xlat12;
					    u_xlat18 = abs(u_xlat18) * u_xlat1.x;
					    u_xlat18 = u_xlat2.x * u_xlat7 + u_xlat18;
					    u_xlat18 = u_xlat18 + 9.99999975e-06;
					    u_xlat18 = 0.5 / u_xlat18;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat8 * u_xlat12 + (-u_xlat8);
					    u_xlat1.x = u_xlat1.x * u_xlat8 + 1.0;
					    u_xlat12 = u_xlat12 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat12 = u_xlat12 / u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat18;
					    u_xlat12 = u_xlat12 * 3.14159274;
					    u_xlat12 = max(u_xlat12, 9.99999975e-05);
					    u_xlat6.y = sqrt(u_xlat12);
					    u_xlat6.xy = u_xlat2.xx * u_xlat6.xy;
					    u_xlat1.xyz = u_xlat6.xxx * u_xlat4.xyz;
					    u_xlat6.xyz = u_xlat4.xyz * u_xlat6.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat19 = u_xlat0.x * u_xlat0.x;
					    u_xlat19 = u_xlat19 * u_xlat19;
					    u_xlat0.x = u_xlat0.x * u_xlat19;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat6.xyz;
					    u_xlat0.xyz = u_xlat3.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat18 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat18 = (-u_xlat18) + 1.0;
					    u_xlat18 = u_xlat18 * _ProjectionParams.z;
					    u_xlat18 = max(u_xlat18, 0.0);
					    u_xlat18 = u_xlat18 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat18 = clamp(u_xlat18, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat18);
					    SV_Target0.w = 1.0;
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  float vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat13;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					bool u_xlatb17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.xy = vs_TEXCOORD1.yy * unity_WorldToLight[1].xy;
					    u_xlat3.xy = unity_WorldToLight[0].xy * vs_TEXCOORD1.xx + u_xlat3.xy;
					    u_xlat3.xy = unity_WorldToLight[2].xy * vs_TEXCOORD1.zz + u_xlat3.xy;
					    u_xlat3.xy = u_xlat3.xy + unity_WorldToLight[3].xy;
					    u_xlatb17 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb17){
					        u_xlatb17 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb17)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat17 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat13 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat17, u_xlat13);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat17 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlat3 = texture(_LightTexture0, u_xlat3.xy);
					    u_xlat17 = u_xlat17 * u_xlat3.w;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat15 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat15 = u_xlat15 * _ProjectionParams.z;
					    u_xlat15 = max(u_xlat15, 0.0);
					    u_xlat15 = u_xlat15 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat15);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SHADOWS_DEPTH" "SPOT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[8];
						mat4x4 unity_WorldToShadow[4];
						vec4 unused_3_2[12];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					vec2 u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16.x, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6 = vs_TEXCOORD1.yyyy * unity_WorldToShadow[1 / 4][1 % 4];
					    u_xlat6 = unity_WorldToShadow[0 / 4][0 % 4] * vs_TEXCOORD1.xxxx + u_xlat6;
					    u_xlat6 = unity_WorldToShadow[2 / 4][2 % 4] * vs_TEXCOORD1.zzzz + u_xlat6;
					    u_xlat6 = u_xlat6 + unity_WorldToShadow[3 / 4][3 % 4];
					    u_xlat6.xyz = u_xlat6.xyz / u_xlat6.www;
					    vec3 txVec0 = vec3(u_xlat6.xy,u_xlat6.z);
					    u_xlat16.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16.x = u_xlat16.x * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16.x) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16.x;
					    u_xlatb9 = 0.0<u_xlat5.z;
					    u_xlat9.x = u_xlatb9 ? 1.0 : float(0.0);
					    u_xlat16.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat16.xy = u_xlat16.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat16.xy);
					    u_xlat9.x = u_xlat9.x * u_xlat6.w;
					    u_xlat16.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat16.xx);
					    u_xlat9.x = u_xlat9.x * u_xlat5.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SPOT" }
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
						vec4 unused_0_2[5];
						vec4 _ShadowMapTexture_TexelSize;
						mat4x4 unity_WorldToLight;
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_8;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[8];
						mat4x4 unity_WorldToShadow[4];
						vec4 unused_3_2[12];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler2D _LightTextureB0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec4 u_xlat8;
					vec4 u_xlat9;
					vec4 u_xlat10;
					vec3 u_xlat11;
					float u_xlat12;
					vec3 u_xlat13;
					bool u_xlatb13;
					float u_xlat22;
					vec2 u_xlat24;
					bool u_xlatb24;
					vec2 u_xlat31;
					float u_xlat33;
					float u_xlat34;
					float u_xlat35;
					float u_xlat36;
					float u_xlat37;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat1.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat34 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat34 = inversesqrt(u_xlat34);
					    u_xlat3.xyz = vec3(u_xlat34) * u_xlat2.xyz;
					    u_xlat34 = log2(_OceanAO.x);
					    u_xlat34 = u_xlat34 * _AOintensity;
					    u_xlat34 = exp2(u_xlat34);
					    u_xlat34 = (-u_xlat34) + 1.0;
					    u_xlat35 = u_xlat34 * _AOalbedo;
					    u_xlat35 = u_xlat35;
					    u_xlat35 = clamp(u_xlat35, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat35) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat34 = u_xlat34 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat34 = u_xlat34 + _SmoothnessShift;
					    u_xlat34 = clamp(u_xlat34, 0.0, 1.0);
					    u_xlat5 = vs_TEXCOORD1.yyyy * unity_WorldToLight[1];
					    u_xlat5 = unity_WorldToLight[0] * vs_TEXCOORD1.xxxx + u_xlat5;
					    u_xlat5 = unity_WorldToLight[2] * vs_TEXCOORD1.zzzz + u_xlat5;
					    u_xlat5 = u_xlat5 + unity_WorldToLight[3];
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat13.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat13.x = dot(u_xlat13.xyz, u_xlat13.xyz);
					    u_xlat13.x = sqrt(u_xlat13.x);
					    u_xlat13.x = (-u_xlat2.x) + u_xlat13.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat13.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb13 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb13){
					        u_xlatb13 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat13.xyz = (bool(u_xlatb13)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat13.xyz = u_xlat13.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat13.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat13.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat24.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat24.x, u_xlat13.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat13.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat13.x = clamp(u_xlat13.x, 0.0, 1.0);
					    u_xlatb24 = u_xlat2.x<0.99000001;
					    if(u_xlatb24){
					        u_xlat6 = vs_TEXCOORD1.yyyy * unity_WorldToShadow[1 / 4][1 % 4];
					        u_xlat6 = unity_WorldToShadow[0 / 4][0 % 4] * vs_TEXCOORD1.xxxx + u_xlat6;
					        u_xlat6 = unity_WorldToShadow[2 / 4][2 % 4] * vs_TEXCOORD1.zzzz + u_xlat6;
					        u_xlat6 = u_xlat6 + unity_WorldToShadow[3 / 4][3 % 4];
					        u_xlat6.xyz = u_xlat6.xyz / u_xlat6.www;
					        u_xlat24.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.zw + vec2(0.5, 0.5);
					        u_xlat24.xy = floor(u_xlat24.xy);
					        u_xlat6.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.zw + (-u_xlat24.xy);
					        u_xlat7 = u_xlat6.xxyy + vec4(0.5, 1.0, 0.5, 1.0);
					        u_xlat8.xw = u_xlat7.xz * u_xlat7.xz;
					        u_xlat7.xz = u_xlat8.xw * vec2(0.5, 0.5) + (-u_xlat6.xy);
					        u_xlat9.xy = (-u_xlat6.xy) + vec2(1.0, 1.0);
					        u_xlat31.xy = min(u_xlat6.xy, vec2(0.0, 0.0));
					        u_xlat9.xy = (-u_xlat31.xy) * u_xlat31.xy + u_xlat9.xy;
					        u_xlat6.xy = max(u_xlat6.xy, vec2(0.0, 0.0));
					        u_xlat6.xy = (-u_xlat6.xy) * u_xlat6.xy + u_xlat7.yw;
					        u_xlat10.x = u_xlat7.x;
					        u_xlat10.y = u_xlat9.x;
					        u_xlat10.z = u_xlat6.x;
					        u_xlat10.w = u_xlat8.x;
					        u_xlat10 = u_xlat10 * vec4(0.444440007, 0.444440007, 0.444440007, 0.222220004);
					        u_xlat8.x = u_xlat7.z;
					        u_xlat8.y = u_xlat9.y;
					        u_xlat8.z = u_xlat6.y;
					        u_xlat7 = u_xlat8 * vec4(0.444440007, 0.444440007, 0.444440007, 0.222220004);
					        u_xlat8 = u_xlat10.ywyw + u_xlat10.xzxz;
					        u_xlat9 = u_xlat7.yyww + u_xlat7.xxzz;
					        u_xlat6.xy = u_xlat10.yw / u_xlat8.zw;
					        u_xlat6.xy = u_xlat6.xy + vec2(-1.5, 0.5);
					        u_xlat7.xy = u_xlat7.yw / u_xlat9.yw;
					        u_xlat7.xy = u_xlat7.xy + vec2(-1.5, 0.5);
					        u_xlat10.xy = u_xlat6.xy * _ShadowMapTexture_TexelSize.xx;
					        u_xlat10.zw = u_xlat7.xy * _ShadowMapTexture_TexelSize.yy;
					        u_xlat7 = u_xlat8 * u_xlat9;
					        u_xlat8 = u_xlat24.xyxy * _ShadowMapTexture_TexelSize.xyxy + u_xlat10.xzyz;
					        vec3 txVec0 = vec3(u_xlat8.xy,u_xlat6.z);
					        u_xlat36 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        vec3 txVec1 = vec3(u_xlat8.zw,u_xlat6.z);
					        u_xlat37 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat37 = u_xlat37 * u_xlat7.y;
					        u_xlat36 = u_xlat7.x * u_xlat36 + u_xlat37;
					        u_xlat8 = u_xlat24.xyxy * _ShadowMapTexture_TexelSize.xyxy + u_xlat10.xwyw;
					        vec3 txVec2 = vec3(u_xlat8.xy,u_xlat6.z);
					        u_xlat24.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat24.x = u_xlat7.z * u_xlat24.x + u_xlat36;
					        vec3 txVec3 = vec3(u_xlat8.zw,u_xlat6.z);
					        u_xlat35 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat24.x = u_xlat7.w * u_xlat35 + u_xlat24.x;
					        u_xlat35 = (-_LightShadowData.x) + 1.0;
					        u_xlat24.x = u_xlat24.x * u_xlat35 + _LightShadowData.x;
					    } else {
					        u_xlat24.x = 1.0;
					    }
					    u_xlat13.x = (-u_xlat24.x) + u_xlat13.x;
					    u_xlat2.x = u_xlat2.x * u_xlat13.x + u_xlat24.x;
					    u_xlatb13 = 0.0<u_xlat5.z;
					    u_xlat13.x = u_xlatb13 ? 1.0 : float(0.0);
					    u_xlat24.xy = u_xlat5.xy / u_xlat5.ww;
					    u_xlat24.xy = u_xlat24.xy + vec2(0.5, 0.5);
					    u_xlat6 = texture(_LightTexture0, u_xlat24.xy);
					    u_xlat13.x = u_xlat13.x * u_xlat6.w;
					    u_xlat24.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTextureB0, u_xlat24.xx);
					    u_xlat13.x = u_xlat13.x * u_xlat5.x;
					    u_xlat2.x = u_xlat2.x * u_xlat13.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat35 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat35 = inversesqrt(u_xlat35);
					    u_xlat5.xyz = vec3(u_xlat35) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat34 = (-u_xlat34) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat33) + u_xlat3.xyz;
					    u_xlat33 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat33 = max(u_xlat33, 0.00100000005);
					    u_xlat33 = inversesqrt(u_xlat33);
					    u_xlat0.xyz = vec3(u_xlat33) * u_xlat0.xyz;
					    u_xlat33 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat35 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat35 = clamp(u_xlat35, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat11.x = u_xlat0.x * u_xlat0.x;
					    u_xlat11.x = dot(u_xlat11.xx, vec2(u_xlat34));
					    u_xlat11.x = u_xlat11.x + -0.5;
					    u_xlat22 = (-u_xlat35) + 1.0;
					    u_xlat1.x = u_xlat22 * u_xlat22;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat22 = u_xlat22 * u_xlat1.x;
					    u_xlat22 = u_xlat11.x * u_xlat22 + 1.0;
					    u_xlat1.x = -abs(u_xlat33) + 1.0;
					    u_xlat12 = u_xlat1.x * u_xlat1.x;
					    u_xlat12 = u_xlat12 * u_xlat12;
					    u_xlat1.x = u_xlat1.x * u_xlat12;
					    u_xlat11.x = u_xlat11.x * u_xlat1.x + 1.0;
					    u_xlat11.x = u_xlat11.x * u_xlat22;
					    u_xlat22 = u_xlat34 * u_xlat34;
					    u_xlat22 = max(u_xlat22, 0.00200000009);
					    u_xlat1.x = (-u_xlat22) + 1.0;
					    u_xlat12 = abs(u_xlat33) * u_xlat1.x + u_xlat22;
					    u_xlat1.x = u_xlat35 * u_xlat1.x + u_xlat22;
					    u_xlat33 = abs(u_xlat33) * u_xlat1.x;
					    u_xlat33 = u_xlat35 * u_xlat12 + u_xlat33;
					    u_xlat33 = u_xlat33 + 9.99999975e-06;
					    u_xlat33 = 0.5 / u_xlat33;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat1.x = u_xlat3.x * u_xlat22 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat22 = u_xlat22 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat22 = u_xlat22 / u_xlat1.x;
					    u_xlat22 = u_xlat22 * u_xlat33;
					    u_xlat22 = u_xlat22 * 3.14159274;
					    u_xlat22 = max(u_xlat22, 9.99999975e-05);
					    u_xlat11.y = sqrt(u_xlat22);
					    u_xlat11.xy = vec2(u_xlat35) * u_xlat11.xy;
					    u_xlat1.xyz = u_xlat11.xxx * u_xlat2.xyz;
					    u_xlat11.xyz = u_xlat2.xyz * u_xlat11.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat34 = u_xlat0.x * u_xlat0.x;
					    u_xlat34 = u_xlat34 * u_xlat34;
					    u_xlat0.x = u_xlat0.x * u_xlat34;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat11.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat33 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat33 = (-u_xlat33) + 1.0;
					    u_xlat33 = u_xlat33 * _ProjectionParams.z;
					    u_xlat33 = max(u_xlat33, 0.0);
					    u_xlat33 = u_xlat33 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat33 = clamp(u_xlat33, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat33);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_6;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					bool u_xlatb3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					vec3 u_xlat8;
					float u_xlat10;
					float u_xlat11;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.x = unity_MatrixV[0].z;
					    u_xlat3.y = unity_MatrixV[1].z;
					    u_xlat3.z = unity_MatrixV[2].z;
					    u_xlat17 = dot(u_xlat0.xyz, u_xlat3.xyz);
					    u_xlat3.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat3.x = dot(u_xlat3.xyz, u_xlat3.xyz);
					    u_xlat3.x = sqrt(u_xlat3.x);
					    u_xlat3.x = (-u_xlat17) + u_xlat3.x;
					    u_xlat17 = unity_ShadowFadeCenterAndType.w * u_xlat3.x + u_xlat17;
					    u_xlat17 = u_xlat17 * _LightShadowData.z + _LightShadowData.w;
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlatb3 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb3){
					        u_xlatb3 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat8.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat8.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat8.xyz;
					        u_xlat8.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat8.xyz;
					        u_xlat8.xyz = u_xlat8.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat3.xyz = (bool(u_xlatb3)) ? u_xlat8.xyz : vs_TEXCOORD1.xyz;
					        u_xlat3.xyz = u_xlat3.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat3.yzw = u_xlat3.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat8.x = u_xlat3.y * 0.25 + 0.75;
					        u_xlat4.x = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat3.x = max(u_xlat8.x, u_xlat4.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat3.xzw);
					    } else {
					        u_xlat3.x = float(1.0);
					        u_xlat3.y = float(1.0);
					        u_xlat3.z = float(1.0);
					        u_xlat3.w = float(1.0);
					    }
					    u_xlat3.x = dot(u_xlat3, unity_OcclusionMaskSelector);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat8.xy = vs_TEXCOORD3.xy / vs_TEXCOORD3.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat8.xy);
					    u_xlat3.x = u_xlat3.x + (-u_xlat4.x);
					    u_xlat17 = u_xlat17 * u_xlat3.x + u_xlat4.x;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat15 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat15 = u_xlat15 * _ProjectionParams.z;
					    u_xlat15 = max(u_xlat15, 0.0);
					    u_xlat15 = u_xlat15 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat15);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "DIRECTIONAL_COOKIE" "FOG_LINEAR" "SHADOWS_SCREEN" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
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
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _ShadowMapTexture;
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  float vs_TEXCOORD4;
					in  vec4 vs_TEXCOORD3;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec3 u_xlat5;
					float u_xlat6;
					float u_xlat10;
					float u_xlat11;
					float u_xlat13;
					bool u_xlatb13;
					float u_xlat15;
					float u_xlat16;
					float u_xlat17;
					float u_xlat18;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat1.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat16 = log2(_OceanAO.x);
					    u_xlat16 = u_xlat16 * _AOintensity;
					    u_xlat16 = exp2(u_xlat16);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat2.x = u_xlat16 * _AOalbedo;
					    u_xlat2.x = u_xlat2.x;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlat2.xyz = u_xlat2.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat16 = u_xlat16 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat16 = u_xlat16 + _SmoothnessShift;
					    u_xlat16 = clamp(u_xlat16, 0.0, 1.0);
					    u_xlat3.xy = vs_TEXCOORD1.yy * unity_WorldToLight[1].xy;
					    u_xlat3.xy = unity_WorldToLight[0].xy * vs_TEXCOORD1.xx + u_xlat3.xy;
					    u_xlat3.xy = unity_WorldToLight[2].xy * vs_TEXCOORD1.zz + u_xlat3.xy;
					    u_xlat3.xy = u_xlat3.xy + unity_WorldToLight[3].xy;
					    u_xlat4.x = unity_MatrixV[0].z;
					    u_xlat4.y = unity_MatrixV[1].z;
					    u_xlat4.z = unity_MatrixV[2].z;
					    u_xlat17 = dot(u_xlat0.xyz, u_xlat4.xyz);
					    u_xlat4.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat13 = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat13 = sqrt(u_xlat13);
					    u_xlat13 = (-u_xlat17) + u_xlat13;
					    u_xlat17 = unity_ShadowFadeCenterAndType.w * u_xlat13 + u_xlat17;
					    u_xlat17 = u_xlat17 * _LightShadowData.z + _LightShadowData.w;
					    u_xlat17 = clamp(u_xlat17, 0.0, 1.0);
					    u_xlatb13 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb13){
					        u_xlatb13 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat4.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat4.xyz;
					        u_xlat4.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat4.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat4.xyz = (bool(u_xlatb13)) ? u_xlat4.xyz : vs_TEXCOORD1.xyz;
					        u_xlat4.xyz = u_xlat4.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat4.yzw = u_xlat4.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat13 = u_xlat4.y * 0.25 + 0.75;
					        u_xlat18 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat4.x = max(u_xlat18, u_xlat13);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xzw);
					    } else {
					        u_xlat4.x = float(1.0);
					        u_xlat4.y = float(1.0);
					        u_xlat4.z = float(1.0);
					        u_xlat4.w = float(1.0);
					    }
					    u_xlat13 = dot(u_xlat4, unity_OcclusionMaskSelector);
					    u_xlat13 = clamp(u_xlat13, 0.0, 1.0);
					    u_xlat4.xy = vs_TEXCOORD3.xy / vs_TEXCOORD3.ww;
					    u_xlat4 = texture(_ShadowMapTexture, u_xlat4.xy);
					    u_xlat13 = u_xlat13 + (-u_xlat4.x);
					    u_xlat17 = u_xlat17 * u_xlat13 + u_xlat4.x;
					    u_xlat3 = texture(_LightTexture0, u_xlat3.xy);
					    u_xlat17 = u_xlat17 * u_xlat3.w;
					    u_xlat3.xyz = vec3(u_xlat17) * _LightColor0.xyz;
					    u_xlat17 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat17 = inversesqrt(u_xlat17);
					    u_xlat4.xyz = vec3(u_xlat17) * vs_TEXCOORD0.xyz;
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat15) + _WorldSpaceLightPos0.xyz;
					    u_xlat15 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat15 = max(u_xlat15, 0.00100000005);
					    u_xlat15 = inversesqrt(u_xlat15);
					    u_xlat0.xyz = vec3(u_xlat15) * u_xlat0.xyz;
					    u_xlat15 = dot(u_xlat4.xyz, u_xlat1.xyz);
					    u_xlat1.x = dot(u_xlat4.xyz, _WorldSpaceLightPos0.xyz);
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat6 = dot(u_xlat4.xyz, u_xlat0.xyz);
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat0.x = dot(_WorldSpaceLightPos0.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat5.x = u_xlat0.x * u_xlat0.x;
					    u_xlat5.x = dot(u_xlat5.xx, vec2(u_xlat16));
					    u_xlat5.x = u_xlat5.x + -0.5;
					    u_xlat10 = (-u_xlat1.x) + 1.0;
					    u_xlat11 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat11 * u_xlat11;
					    u_xlat10 = u_xlat10 * u_xlat11;
					    u_xlat10 = u_xlat5.x * u_xlat10 + 1.0;
					    u_xlat11 = -abs(u_xlat15) + 1.0;
					    u_xlat17 = u_xlat11 * u_xlat11;
					    u_xlat17 = u_xlat17 * u_xlat17;
					    u_xlat11 = u_xlat11 * u_xlat17;
					    u_xlat5.x = u_xlat5.x * u_xlat11 + 1.0;
					    u_xlat5.x = u_xlat5.x * u_xlat10;
					    u_xlat10 = u_xlat16 * u_xlat16;
					    u_xlat10 = max(u_xlat10, 0.00200000009);
					    u_xlat11 = (-u_xlat10) + 1.0;
					    u_xlat16 = abs(u_xlat15) * u_xlat11 + u_xlat10;
					    u_xlat11 = u_xlat1.x * u_xlat11 + u_xlat10;
					    u_xlat15 = abs(u_xlat15) * u_xlat11;
					    u_xlat15 = u_xlat1.x * u_xlat16 + u_xlat15;
					    u_xlat15 = u_xlat15 + 9.99999975e-06;
					    u_xlat15 = 0.5 / u_xlat15;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat11 = u_xlat6 * u_xlat10 + (-u_xlat6);
					    u_xlat6 = u_xlat11 * u_xlat6 + 1.0;
					    u_xlat10 = u_xlat10 * 0.318309873;
					    u_xlat6 = u_xlat6 * u_xlat6 + 1.00000001e-07;
					    u_xlat10 = u_xlat10 / u_xlat6;
					    u_xlat10 = u_xlat10 * u_xlat15;
					    u_xlat10 = u_xlat10 * 3.14159274;
					    u_xlat10 = max(u_xlat10, 9.99999975e-05);
					    u_xlat5.y = sqrt(u_xlat10);
					    u_xlat5.xy = u_xlat1.xx * u_xlat5.xy;
					    u_xlat1.xyz = u_xlat5.xxx * u_xlat3.xyz;
					    u_xlat5.xyz = u_xlat3.xyz * u_xlat5.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat16 = u_xlat0.x * u_xlat0.x;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat0.x = u_xlat0.x * u_xlat16;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat5.xyz;
					    u_xlat0.xyz = u_xlat2.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat15 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat15 = (-u_xlat15) + 1.0;
					    u_xlat15 = u_xlat15 * _ProjectionParams.z;
					    u_xlat15 = max(u_xlat15, 0.0);
					    u_xlat15 = u_xlat15 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat15);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" "SHADOWS_CUBE" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					    u_xlat16 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					    u_xlat16 = max(abs(u_xlat6.z), u_xlat16);
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.z);
					    u_xlat16 = max(u_xlat16, 9.99999975e-06);
					    u_xlat16 = u_xlat16 * _LightProjectionParams.w;
					    u_xlat16 = _LightProjectionParams.y / u_xlat16;
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.x);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    vec4 txVec0 = vec4(u_xlat6.xyz,u_xlat16);
					    u_xlat16 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16;
					    u_xlat9.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, u_xlat9.xx);
					    u_xlat2.x = u_xlat2.x * u_xlat5.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					vec3 u_xlat11;
					bool u_xlatb11;
					float u_xlat18;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat28 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat28 = inversesqrt(u_xlat28);
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat2.xyz;
					    u_xlat28 = log2(_OceanAO.x);
					    u_xlat28 = u_xlat28 * _AOintensity;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat29 = u_xlat28 * _AOalbedo;
					    u_xlat29 = u_xlat29;
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat29) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat28 = u_xlat28 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat28 = u_xlat28 + _SmoothnessShift;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat11.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat11.x = dot(u_xlat11.xyz, u_xlat11.xyz);
					    u_xlat11.x = sqrt(u_xlat11.x);
					    u_xlat11.x = (-u_xlat2.x) + u_xlat11.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat11.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb11 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb11){
					        u_xlatb11 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat11.xyz = (bool(u_xlatb11)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat11.xyz = u_xlat11.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat11.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat11.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat20 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat20, u_xlat11.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat11.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat11.x = clamp(u_xlat11.x, 0.0, 1.0);
					    u_xlatb20 = u_xlat2.x<0.99000001;
					    if(u_xlatb20){
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					        u_xlat20 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					        u_xlat20 = max(abs(u_xlat6.z), u_xlat20);
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.z);
					        u_xlat20 = max(u_xlat20, 9.99999975e-06);
					        u_xlat20 = u_xlat20 * _LightProjectionParams.w;
					        u_xlat20 = _LightProjectionParams.y / u_xlat20;
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.x);
					        u_xlat20 = (-u_xlat20) + 1.0;
					        u_xlat7.xyz = u_xlat6.xyz + vec3(0.0078125, 0.0078125, 0.0078125);
					        vec4 txVec0 = vec4(u_xlat7.xyz,u_xlat20);
					        u_xlat7.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, -0.0078125, 0.0078125);
					        vec4 txVec1 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.y = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, 0.0078125, -0.0078125);
					        vec4 txVec2 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.z = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat6.xyz = u_xlat6.xyz + vec3(0.0078125, -0.0078125, -0.0078125);
					        vec4 txVec3 = vec4(u_xlat6.xyz,u_xlat20);
					        u_xlat7.w = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat20 = dot(u_xlat7, vec4(0.25, 0.25, 0.25, 0.25));
					        u_xlat29 = (-_LightShadowData.x) + 1.0;
					        u_xlat20 = u_xlat20 * u_xlat29 + _LightShadowData.x;
					    } else {
					        u_xlat20 = 1.0;
					    }
					    u_xlat11.x = (-u_xlat20) + u_xlat11.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x + u_xlat20;
					    u_xlat11.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat5 = texture(_LightTexture0, u_xlat11.xx);
					    u_xlat2.x = u_xlat2.x * u_xlat5.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat29 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat29 = inversesqrt(u_xlat29);
					    u_xlat5.xyz = vec3(u_xlat29) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat27) + u_xlat3.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = max(u_xlat27, 0.00100000005);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat29 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9.x = dot(u_xlat9.xx, vec2(u_xlat28));
					    u_xlat9.x = u_xlat9.x + -0.5;
					    u_xlat18 = (-u_xlat29) + 1.0;
					    u_xlat1.x = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat1.x;
					    u_xlat18 = u_xlat9.x * u_xlat18 + 1.0;
					    u_xlat1.x = -abs(u_xlat27) + 1.0;
					    u_xlat10 = u_xlat1.x * u_xlat1.x;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat9.x = u_xlat9.x * u_xlat1.x + 1.0;
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat18 = u_xlat28 * u_xlat28;
					    u_xlat18 = max(u_xlat18, 0.00200000009);
					    u_xlat1.x = (-u_xlat18) + 1.0;
					    u_xlat10 = abs(u_xlat27) * u_xlat1.x + u_xlat18;
					    u_xlat1.x = u_xlat29 * u_xlat1.x + u_xlat18;
					    u_xlat27 = abs(u_xlat27) * u_xlat1.x;
					    u_xlat27 = u_xlat29 * u_xlat10 + u_xlat27;
					    u_xlat27 = u_xlat27 + 9.99999975e-06;
					    u_xlat27 = 0.5 / u_xlat27;
					    u_xlat18 = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat3.x * u_xlat18 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat18 = u_xlat18 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat18 = u_xlat18 / u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat27;
					    u_xlat18 = u_xlat18 * 3.14159274;
					    u_xlat18 = max(u_xlat18, 9.99999975e-05);
					    u_xlat9.y = sqrt(u_xlat18);
					    u_xlat9.xy = vec2(u_xlat29) * u_xlat9.xy;
					    u_xlat1.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat9.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat28 = u_xlat0.x * u_xlat0.x;
					    u_xlat28 = u_xlat28 * u_xlat28;
					    u_xlat0.x = u_xlat0.x * u_xlat28;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat9.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat27 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat27 = (-u_xlat27) + 1.0;
					    u_xlat27 = u_xlat27 * _ProjectionParams.z;
					    u_xlat27 = max(u_xlat27, 0.0);
					    u_xlat27 = u_xlat27 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat27 = clamp(u_xlat27, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat27);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" "SHADOWS_CUBE" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					float u_xlat8;
					vec3 u_xlat9;
					bool u_xlatb9;
					float u_xlat14;
					float u_xlat16;
					float u_xlat21;
					float u_xlat22;
					float u_xlat23;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat1.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat22 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat22 = inversesqrt(u_xlat22);
					    u_xlat3.xyz = vec3(u_xlat22) * u_xlat2.xyz;
					    u_xlat22 = log2(_OceanAO.x);
					    u_xlat22 = u_xlat22 * _AOintensity;
					    u_xlat22 = exp2(u_xlat22);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat23 = u_xlat22 * _AOalbedo;
					    u_xlat23 = u_xlat23;
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat23) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat22 = u_xlat22 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat22 = u_xlat22 + _SmoothnessShift;
					    u_xlat22 = clamp(u_xlat22, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat9.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat9.x = dot(u_xlat9.xyz, u_xlat9.xyz);
					    u_xlat9.x = sqrt(u_xlat9.x);
					    u_xlat9.x = (-u_xlat2.x) + u_xlat9.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat9.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb9 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb9){
					        u_xlatb9 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat9.xyz = (bool(u_xlatb9)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat9.xyz = u_xlat9.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat9.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat9.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat16 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat16, u_xlat9.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat9.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat9.x = clamp(u_xlat9.x, 0.0, 1.0);
					    u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					    u_xlat16 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					    u_xlat16 = max(abs(u_xlat6.z), u_xlat16);
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.z);
					    u_xlat16 = max(u_xlat16, 9.99999975e-06);
					    u_xlat16 = u_xlat16 * _LightProjectionParams.w;
					    u_xlat16 = _LightProjectionParams.y / u_xlat16;
					    u_xlat16 = u_xlat16 + (-_LightProjectionParams.x);
					    u_xlat16 = (-u_xlat16) + 1.0;
					    vec4 txVec0 = vec4(u_xlat6.xyz,u_xlat16);
					    u_xlat16 = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					    u_xlat23 = (-_LightShadowData.x) + 1.0;
					    u_xlat16 = u_xlat16 * u_xlat23 + _LightShadowData.x;
					    u_xlat9.x = (-u_xlat16) + u_xlat9.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x + u_xlat16;
					    u_xlat9.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, u_xlat9.xx);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat9.x = u_xlat5.w * u_xlat6.x;
					    u_xlat2.x = u_xlat2.x * u_xlat9.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat23 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat23 = inversesqrt(u_xlat23);
					    u_xlat5.xyz = vec3(u_xlat23) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat22 = (-u_xlat22) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat21) + u_xlat3.xyz;
					    u_xlat21 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat21 = max(u_xlat21, 0.00100000005);
					    u_xlat21 = inversesqrt(u_xlat21);
					    u_xlat0.xyz = vec3(u_xlat21) * u_xlat0.xyz;
					    u_xlat21 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat23 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat23 = clamp(u_xlat23, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat7.x = u_xlat0.x * u_xlat0.x;
					    u_xlat7.x = dot(u_xlat7.xx, vec2(u_xlat22));
					    u_xlat7.x = u_xlat7.x + -0.5;
					    u_xlat14 = (-u_xlat23) + 1.0;
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
					    u_xlat14 = u_xlat22 * u_xlat22;
					    u_xlat14 = max(u_xlat14, 0.00200000009);
					    u_xlat1.x = (-u_xlat14) + 1.0;
					    u_xlat8 = abs(u_xlat21) * u_xlat1.x + u_xlat14;
					    u_xlat1.x = u_xlat23 * u_xlat1.x + u_xlat14;
					    u_xlat21 = abs(u_xlat21) * u_xlat1.x;
					    u_xlat21 = u_xlat23 * u_xlat8 + u_xlat21;
					    u_xlat21 = u_xlat21 + 9.99999975e-06;
					    u_xlat21 = 0.5 / u_xlat21;
					    u_xlat14 = u_xlat14 * u_xlat14;
					    u_xlat1.x = u_xlat3.x * u_xlat14 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat14 = u_xlat14 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat14 = u_xlat14 / u_xlat1.x;
					    u_xlat14 = u_xlat14 * u_xlat21;
					    u_xlat14 = u_xlat14 * 3.14159274;
					    u_xlat14 = max(u_xlat14, 9.99999975e-05);
					    u_xlat7.y = sqrt(u_xlat14);
					    u_xlat7.xy = vec2(u_xlat23) * u_xlat7.xy;
					    u_xlat1.xyz = u_xlat7.xxx * u_xlat2.xyz;
					    u_xlat7.xyz = u_xlat2.xyz * u_xlat7.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat22 = u_xlat0.x * u_xlat0.x;
					    u_xlat22 = u_xlat22 * u_xlat22;
					    u_xlat0.x = u_xlat0.x * u_xlat22;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat7.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat21 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat21 = (-u_xlat21) + 1.0;
					    u_xlat21 = u_xlat21 * _ProjectionParams.z;
					    u_xlat21 = max(u_xlat21, 0.0);
					    u_xlat21 = u_xlat21 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat21 = clamp(u_xlat21, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat21);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "FOG_LINEAR" "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
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
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 unused_0_7;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _AOintensity;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 unused_1_0[4];
						vec3 _WorldSpaceCameraPos;
						vec4 _ProjectionParams;
						vec4 unused_1_3[3];
					};
					layout(std140) uniform UnityLighting {
						vec4 _WorldSpaceLightPos0;
						vec4 _LightPositionRange;
						vec4 _LightProjectionParams;
						vec4 unused_2_3[43];
						vec4 unity_OcclusionMaskSelector;
						vec4 unused_2_5;
					};
					layout(std140) uniform UnityShadows {
						vec4 unused_3_0[24];
						vec4 _LightShadowData;
						vec4 unity_ShadowFadeCenterAndType;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_4_0[9];
						mat4x4 unity_MatrixV;
						vec4 unused_4_2[10];
					};
					layout(std140) uniform UnityFog {
						vec4 unused_5_0;
						vec4 unity_FogParams;
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler2D _LightTextureB0;
					uniform  samplerCube _LightTexture0;
					uniform  sampler3D unity_ProbeVolumeSH;
					uniform  samplerCube _ShadowMapTexture;
					uniform  samplerCubeShadow hlslcc_zcmp_ShadowMapTexture;
					in  vec3 vs_TEXCOORD0;
					in  float vs_TEXCOORD4;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec3 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec4 u_xlat7;
					vec3 u_xlat8;
					vec3 u_xlat9;
					float u_xlat10;
					vec3 u_xlat11;
					bool u_xlatb11;
					float u_xlat18;
					float u_xlat20;
					bool u_xlatb20;
					float u_xlat27;
					float u_xlat28;
					float u_xlat29;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat1.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat28 = dot(u_xlat2.xyz, u_xlat2.xyz);
					    u_xlat28 = inversesqrt(u_xlat28);
					    u_xlat3.xyz = vec3(u_xlat28) * u_xlat2.xyz;
					    u_xlat28 = log2(_OceanAO.x);
					    u_xlat28 = u_xlat28 * _AOintensity;
					    u_xlat28 = exp2(u_xlat28);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat29 = u_xlat28 * _AOalbedo;
					    u_xlat29 = u_xlat29;
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat4.xyz = vec3(u_xlat29) * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat28 = u_xlat28 * _AOsmoothness + _OceanGlossiness.x;
					    u_xlat28 = u_xlat28 + _SmoothnessShift;
					    u_xlat28 = clamp(u_xlat28, 0.0, 1.0);
					    u_xlat5.xyz = vs_TEXCOORD1.yyy * unity_WorldToLight[1].xyz;
					    u_xlat5.xyz = unity_WorldToLight[0].xyz * vs_TEXCOORD1.xxx + u_xlat5.xyz;
					    u_xlat5.xyz = unity_WorldToLight[2].xyz * vs_TEXCOORD1.zzz + u_xlat5.xyz;
					    u_xlat5.xyz = u_xlat5.xyz + unity_WorldToLight[3].xyz;
					    u_xlat6.x = unity_MatrixV[0].z;
					    u_xlat6.y = unity_MatrixV[1].z;
					    u_xlat6.z = unity_MatrixV[2].z;
					    u_xlat2.x = dot(u_xlat2.xyz, u_xlat6.xyz);
					    u_xlat11.xyz = vs_TEXCOORD1.xyz + (-unity_ShadowFadeCenterAndType.xyz);
					    u_xlat11.x = dot(u_xlat11.xyz, u_xlat11.xyz);
					    u_xlat11.x = sqrt(u_xlat11.x);
					    u_xlat11.x = (-u_xlat2.x) + u_xlat11.x;
					    u_xlat2.x = unity_ShadowFadeCenterAndType.w * u_xlat11.x + u_xlat2.x;
					    u_xlat2.x = u_xlat2.x * _LightShadowData.z + _LightShadowData.w;
					    u_xlat2.x = clamp(u_xlat2.x, 0.0, 1.0);
					    u_xlatb11 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb11){
					        u_xlatb11 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat6.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat6.xyz;
					        u_xlat6.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat6.xyz;
					        u_xlat6.xyz = u_xlat6.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat11.xyz = (bool(u_xlatb11)) ? u_xlat6.xyz : vs_TEXCOORD1.xyz;
					        u_xlat11.xyz = u_xlat11.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat6.yzw = u_xlat11.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat11.x = u_xlat6.y * 0.25 + 0.75;
					        u_xlat20 = unity_ProbeVolumeParams.z * 0.5 + 0.75;
					        u_xlat6.x = max(u_xlat20, u_xlat11.x);
					        u_xlat6 = texture(unity_ProbeVolumeSH, u_xlat6.xzw);
					    } else {
					        u_xlat6.x = float(1.0);
					        u_xlat6.y = float(1.0);
					        u_xlat6.z = float(1.0);
					        u_xlat6.w = float(1.0);
					    }
					    u_xlat11.x = dot(u_xlat6, unity_OcclusionMaskSelector);
					    u_xlat11.x = clamp(u_xlat11.x, 0.0, 1.0);
					    u_xlatb20 = u_xlat2.x<0.99000001;
					    if(u_xlatb20){
					        u_xlat6.xyz = vs_TEXCOORD1.xyz + (-_LightPositionRange.xyz);
					        u_xlat20 = max(abs(u_xlat6.y), abs(u_xlat6.x));
					        u_xlat20 = max(abs(u_xlat6.z), u_xlat20);
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.z);
					        u_xlat20 = max(u_xlat20, 9.99999975e-06);
					        u_xlat20 = u_xlat20 * _LightProjectionParams.w;
					        u_xlat20 = _LightProjectionParams.y / u_xlat20;
					        u_xlat20 = u_xlat20 + (-_LightProjectionParams.x);
					        u_xlat20 = (-u_xlat20) + 1.0;
					        u_xlat7.xyz = u_xlat6.xyz + vec3(0.0078125, 0.0078125, 0.0078125);
					        vec4 txVec0 = vec4(u_xlat7.xyz,u_xlat20);
					        u_xlat7.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec0, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, -0.0078125, 0.0078125);
					        vec4 txVec1 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.y = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec1, 0.0);
					        u_xlat8.xyz = u_xlat6.xyz + vec3(-0.0078125, 0.0078125, -0.0078125);
					        vec4 txVec2 = vec4(u_xlat8.xyz,u_xlat20);
					        u_xlat7.z = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec2, 0.0);
					        u_xlat6.xyz = u_xlat6.xyz + vec3(0.0078125, -0.0078125, -0.0078125);
					        vec4 txVec3 = vec4(u_xlat6.xyz,u_xlat20);
					        u_xlat7.w = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec3, 0.0);
					        u_xlat20 = dot(u_xlat7, vec4(0.25, 0.25, 0.25, 0.25));
					        u_xlat29 = (-_LightShadowData.x) + 1.0;
					        u_xlat20 = u_xlat20 * u_xlat29 + _LightShadowData.x;
					    } else {
					        u_xlat20 = 1.0;
					    }
					    u_xlat11.x = (-u_xlat20) + u_xlat11.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x + u_xlat20;
					    u_xlat11.x = dot(u_xlat5.xyz, u_xlat5.xyz);
					    u_xlat6 = texture(_LightTextureB0, u_xlat11.xx);
					    u_xlat5 = texture(_LightTexture0, u_xlat5.xyz);
					    u_xlat11.x = u_xlat5.w * u_xlat6.x;
					    u_xlat2.x = u_xlat2.x * u_xlat11.x;
					    u_xlat2.xyz = u_xlat2.xxx * _LightColor0.xyz;
					    u_xlat29 = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat29 = inversesqrt(u_xlat29);
					    u_xlat5.xyz = vec3(u_xlat29) * vs_TEXCOORD0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat28 = (-u_xlat28) + 1.0;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(u_xlat27) + u_xlat3.xyz;
					    u_xlat27 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat27 = max(u_xlat27, 0.00100000005);
					    u_xlat27 = inversesqrt(u_xlat27);
					    u_xlat0.xyz = vec3(u_xlat27) * u_xlat0.xyz;
					    u_xlat27 = dot(u_xlat5.xyz, u_xlat3.xyz);
					    u_xlat29 = dot(u_xlat5.xyz, u_xlat1.xyz);
					    u_xlat29 = clamp(u_xlat29, 0.0, 1.0);
					    u_xlat3.x = dot(u_xlat5.xyz, u_xlat0.xyz);
					    u_xlat3.x = clamp(u_xlat3.x, 0.0, 1.0);
					    u_xlat0.x = dot(u_xlat1.xyz, u_xlat0.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat9.x = u_xlat0.x * u_xlat0.x;
					    u_xlat9.x = dot(u_xlat9.xx, vec2(u_xlat28));
					    u_xlat9.x = u_xlat9.x + -0.5;
					    u_xlat18 = (-u_xlat29) + 1.0;
					    u_xlat1.x = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat1.x;
					    u_xlat18 = u_xlat9.x * u_xlat18 + 1.0;
					    u_xlat1.x = -abs(u_xlat27) + 1.0;
					    u_xlat10 = u_xlat1.x * u_xlat1.x;
					    u_xlat10 = u_xlat10 * u_xlat10;
					    u_xlat1.x = u_xlat1.x * u_xlat10;
					    u_xlat9.x = u_xlat9.x * u_xlat1.x + 1.0;
					    u_xlat9.x = u_xlat9.x * u_xlat18;
					    u_xlat18 = u_xlat28 * u_xlat28;
					    u_xlat18 = max(u_xlat18, 0.00200000009);
					    u_xlat1.x = (-u_xlat18) + 1.0;
					    u_xlat10 = abs(u_xlat27) * u_xlat1.x + u_xlat18;
					    u_xlat1.x = u_xlat29 * u_xlat1.x + u_xlat18;
					    u_xlat27 = abs(u_xlat27) * u_xlat1.x;
					    u_xlat27 = u_xlat29 * u_xlat10 + u_xlat27;
					    u_xlat27 = u_xlat27 + 9.99999975e-06;
					    u_xlat27 = 0.5 / u_xlat27;
					    u_xlat18 = u_xlat18 * u_xlat18;
					    u_xlat1.x = u_xlat3.x * u_xlat18 + (-u_xlat3.x);
					    u_xlat1.x = u_xlat1.x * u_xlat3.x + 1.0;
					    u_xlat18 = u_xlat18 * 0.318309873;
					    u_xlat1.x = u_xlat1.x * u_xlat1.x + 1.00000001e-07;
					    u_xlat18 = u_xlat18 / u_xlat1.x;
					    u_xlat18 = u_xlat18 * u_xlat27;
					    u_xlat18 = u_xlat18 * 3.14159274;
					    u_xlat18 = max(u_xlat18, 9.99999975e-05);
					    u_xlat9.y = sqrt(u_xlat18);
					    u_xlat9.xy = vec2(u_xlat29) * u_xlat9.xy;
					    u_xlat1.xyz = u_xlat9.xxx * u_xlat2.xyz;
					    u_xlat9.xyz = u_xlat2.xyz * u_xlat9.yyy;
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat28 = u_xlat0.x * u_xlat0.x;
					    u_xlat28 = u_xlat28 * u_xlat28;
					    u_xlat0.x = u_xlat0.x * u_xlat28;
					    u_xlat0.x = u_xlat0.x * 0.779083729 + 0.220916301;
					    u_xlat0.xyz = u_xlat0.xxx * u_xlat9.xyz;
					    u_xlat0.xyz = u_xlat4.xyz * u_xlat1.xyz + u_xlat0.xyz;
					    u_xlat27 = vs_TEXCOORD4 / _ProjectionParams.y;
					    u_xlat27 = (-u_xlat27) + 1.0;
					    u_xlat27 = u_xlat27 * _ProjectionParams.z;
					    u_xlat27 = max(u_xlat27, 0.0);
					    u_xlat27 = u_xlat27 * unity_FogParams.z + unity_FogParams.w;
					    u_xlat27 = clamp(u_xlat27, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz * vec3(u_xlat27);
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
			}
		}
		Pass {
			Name "DEFERRED"
			LOD 300
			Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
			GpuProgramID 160899
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "LIGHTPROBE_SH" }
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_5[2];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    u_xlat0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD0.xyz = u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    u_xlat6 = u_xlat0.y * u_xlat0.y;
					    u_xlat6 = u_xlat0.x * u_xlat0.x + (-u_xlat6);
					    u_xlat1 = u_xlat0.yzzx * u_xlat0.xyzz;
					    u_xlat0.x = dot(unity_SHBr, u_xlat1);
					    u_xlat0.y = dot(unity_SHBg, u_xlat1);
					    u_xlat0.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD4.xyz = unity_SHC.xyz * vec3(u_xlat6) + u_xlat0.xyz;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "UNITY_HDR_ON" }
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    vs_TEXCOORD0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "LIGHTPROBE_SH" "UNITY_HDR_ON" }
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
					layout(std140) uniform UnityLighting {
						vec4 unused_0_0[42];
						vec4 unity_SHBr;
						vec4 unity_SHBg;
						vec4 unity_SHBb;
						vec4 unity_SHC;
						vec4 unused_0_5[2];
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
					out vec3 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD3;
					out vec3 vs_TEXCOORD4;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat6;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat1 = u_xlat0 + unity_ObjectToWorld[3];
					    vs_TEXCOORD1.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat0.xyz;
					    u_xlat0 = u_xlat1.yyyy * unity_MatrixVP[1];
					    u_xlat0 = unity_MatrixVP[0] * u_xlat1.xxxx + u_xlat0;
					    u_xlat0 = unity_MatrixVP[2] * u_xlat1.zzzz + u_xlat0;
					    gl_Position = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    u_xlat0.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat0.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat0.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat6 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat6 = inversesqrt(u_xlat6);
					    u_xlat0.xyz = vec3(u_xlat6) * u_xlat0.xyz;
					    vs_TEXCOORD0.xyz = u_xlat0.xyz;
					    vs_TEXCOORD3 = vec4(0.0, 0.0, 0.0, 0.0);
					    u_xlat6 = u_xlat0.y * u_xlat0.y;
					    u_xlat6 = u_xlat0.x * u_xlat0.x + (-u_xlat6);
					    u_xlat1 = u_xlat0.yzzx * u_xlat0.xyzz;
					    u_xlat0.x = dot(unity_SHBr, u_xlat1);
					    u_xlat0.y = dot(unity_SHBg, u_xlat1);
					    u_xlat0.z = dot(unity_SHBb, u_xlat1);
					    vs_TEXCOORD4.xyz = unity_SHC.xyz * vec3(u_xlat6) + u_xlat0.xyz;
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
						vec4 unused_0_0[4];
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
						vec4 unused_0_10;
					};
					in  vec3 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					layout(location = 1) out vec4 SV_Target1;
					layout(location = 2) out vec4 SV_Target2;
					layout(location = 3) out vec4 SV_Target3;
					vec3 u_xlat0;
					float u_xlat1;
					void main()
					{
					    u_xlat0.x = log2(_OceanAO.x);
					    u_xlat0.x = u_xlat0.x * _AOintensity;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat1 = (-u_xlat0.x) + 1.0;
					    SV_Target0.w = u_xlat0.x;
					    u_xlat0.x = u_xlat1 * _AOalbedo;
					    u_xlat1 = u_xlat1 * _AOsmoothness + _OceanGlossiness.x;
					    SV_Target1.w = u_xlat1 + _SmoothnessShift;
					    SV_Target1.w = clamp(SV_Target1.w, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    SV_Target0.xyz = u_xlat0.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target1.xyz = vec3(0.220916301, 0.220916301, 0.220916301);
					    SV_Target2.xyz = vs_TEXCOORD0.xyz * vec3(0.5, 0.5, 0.5) + vec3(0.5, 0.5, 0.5);
					    SV_Target2.w = 1.0;
					    u_xlat0.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale));
					    SV_Target3.xyz = exp2((-u_xlat0.xyz));
					    SV_Target3.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "LIGHTPROBE_SH" }
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
						vec4 unused_0_0[4];
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[39];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_1_4[6];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					layout(location = 1) out vec4 SV_Target1;
					layout(location = 2) out vec4 SV_Target2;
					layout(location = 3) out vec4 SV_Target3;
					vec4 u_xlat0;
					vec3 u_xlat1;
					bool u_xlatb1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat7;
					float u_xlat8;
					void main()
					{
					    u_xlat0.x = log2(_OceanAO.x);
					    u_xlat0.x = u_xlat0.x * _AOintensity;
					    u_xlat0.w = exp2(u_xlat0.x);
					    u_xlat1.x = (-u_xlat0.w) + 1.0;
					    u_xlat7.x = u_xlat1.x * _AOalbedo;
					    u_xlat7.x = u_xlat7.x;
					    u_xlat7.x = clamp(u_xlat7.x, 0.0, 1.0);
					    u_xlat7.xyz = u_xlat7.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat1.x = u_xlat1.x * _AOsmoothness + _OceanGlossiness.x;
					    SV_Target1.w = u_xlat1.x + _SmoothnessShift;
					    SV_Target1.w = clamp(SV_Target1.w, 0.0, 1.0);
					    u_xlatb1 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb1){
					        u_xlatb1 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat2.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat2.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat2.xyz;
					        u_xlat2.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat2.xyz;
					        u_xlat2.xyz = u_xlat2.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat2.xyz = (bool(u_xlatb1)) ? u_xlat2.xyz : vs_TEXCOORD1.xyz;
					        u_xlat2.xyz = u_xlat2.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat2.yzw = u_xlat2.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat1.x = u_xlat2.y * 0.25;
					        u_xlat8 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat3.x = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat1.x = max(u_xlat1.x, u_xlat8);
					        u_xlat2.x = min(u_xlat3.x, u_xlat1.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat2.xzw);
					        u_xlat4.xyz = u_xlat2.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xyz);
					        u_xlat2.xyz = u_xlat2.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat2 = texture(unity_ProbeVolumeSH, u_xlat2.xyz);
					        u_xlat5.xyz = vs_TEXCOORD0.xyz;
					        u_xlat5.w = 1.0;
					        u_xlat3.x = dot(u_xlat3, u_xlat5);
					        u_xlat3.y = dot(u_xlat4, u_xlat5);
					        u_xlat3.z = dot(u_xlat2, u_xlat5);
					    } else {
					        u_xlat2.xyz = vs_TEXCOORD0.xyz;
					        u_xlat2.w = 1.0;
					        u_xlat3.x = dot(unity_SHAr, u_xlat2);
					        u_xlat3.y = dot(unity_SHAg, u_xlat2);
					        u_xlat3.z = dot(unity_SHAb, u_xlat2);
					    }
					    u_xlat2.xyz = u_xlat3.xyz + vs_TEXCOORD4.xyz;
					    u_xlat2.xyz = max(u_xlat2.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat2.xyz = log2(u_xlat2.xyz);
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat2.xyz = exp2(u_xlat2.xyz);
					    u_xlat2.xyz = u_xlat2.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat2.xyz = max(u_xlat2.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat2.xyz = u_xlat0.www * u_xlat2.xyz;
					    u_xlat0.xyz = u_xlat7.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat1.xyz = u_xlat2.xyz * u_xlat0.xyz;
					    u_xlat1.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat1.xyz;
					    SV_Target3.xyz = exp2((-u_xlat1.xyz));
					    SV_Target0 = u_xlat0;
					    SV_Target1.xyz = vec3(0.220916301, 0.220916301, 0.220916301);
					    SV_Target2.xyz = vs_TEXCOORD0.xyz * vec3(0.5, 0.5, 0.5) + vec3(0.5, 0.5, 0.5);
					    SV_Target2.w = 1.0;
					    SV_Target3.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "UNITY_HDR_ON" }
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
						vec4 unused_0_0[4];
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
						vec4 unused_0_10;
					};
					in  vec3 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					layout(location = 1) out vec4 SV_Target1;
					layout(location = 2) out vec4 SV_Target2;
					layout(location = 3) out vec4 SV_Target3;
					vec3 u_xlat0;
					float u_xlat1;
					void main()
					{
					    u_xlat0.x = log2(_OceanAO.x);
					    u_xlat0.x = u_xlat0.x * _AOintensity;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat1 = (-u_xlat0.x) + 1.0;
					    SV_Target0.w = u_xlat0.x;
					    u_xlat0.x = u_xlat1 * _AOalbedo;
					    u_xlat1 = u_xlat1 * _AOsmoothness + _OceanGlossiness.x;
					    SV_Target1.w = u_xlat1 + _SmoothnessShift;
					    SV_Target1.w = clamp(SV_Target1.w, 0.0, 1.0);
					    u_xlat0.x = u_xlat0.x;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat0.xyz = u_xlat0.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    SV_Target0.xyz = u_xlat0.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    SV_Target1.xyz = vec3(0.220916301, 0.220916301, 0.220916301);
					    SV_Target2.xyz = vs_TEXCOORD0.xyz * vec3(0.5, 0.5, 0.5) + vec3(0.5, 0.5, 0.5);
					    SV_Target2.w = 1.0;
					    SV_Target3.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale));
					    SV_Target3.w = 1.0;
					    return;
					}"
				}
				SubProgram "d3d11 " {
					Keywords { "LIGHTPROBE_SH" "UNITY_HDR_ON" }
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
						vec4 unused_0_0[4];
						vec4 _Oceancolor;
						vec4 _OceanGlossiness;
						vec4 _OceanAO;
						vec4 _OceanEmission;
						float _AOalbedo;
						float _AOsmoothness;
						float _SmoothnessShift;
						float _EmissionScale;
						float _AOintensity;
						vec4 unused_0_10;
					};
					layout(std140) uniform UnityLighting {
						vec4 unused_1_0[39];
						vec4 unity_SHAr;
						vec4 unity_SHAg;
						vec4 unity_SHAb;
						vec4 unused_1_4[6];
					};
					layout(std140) uniform UnityProbeVolume {
						vec4 unity_ProbeVolumeParams;
						mat4x4 unity_ProbeVolumeWorldToObject;
						vec3 unity_ProbeVolumeSizeInv;
						vec3 unity_ProbeVolumeMin;
					};
					uniform  sampler3D unity_ProbeVolumeSH;
					in  vec3 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec3 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					layout(location = 1) out vec4 SV_Target1;
					layout(location = 2) out vec4 SV_Target2;
					layout(location = 3) out vec4 SV_Target3;
					vec4 u_xlat0;
					vec3 u_xlat1;
					bool u_xlatb1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec3 u_xlat7;
					float u_xlat8;
					void main()
					{
					    u_xlat0.x = log2(_OceanAO.x);
					    u_xlat0.x = u_xlat0.x * _AOintensity;
					    u_xlat0.w = exp2(u_xlat0.x);
					    u_xlat1.x = (-u_xlat0.w) + 1.0;
					    u_xlat7.x = u_xlat1.x * _AOalbedo;
					    u_xlat7.x = u_xlat7.x;
					    u_xlat7.x = clamp(u_xlat7.x, 0.0, 1.0);
					    u_xlat7.xyz = u_xlat7.xxx * (-_Oceancolor.xyz) + _Oceancolor.xyz;
					    u_xlat1.x = u_xlat1.x * _AOsmoothness + _OceanGlossiness.x;
					    SV_Target1.w = u_xlat1.x + _SmoothnessShift;
					    SV_Target1.w = clamp(SV_Target1.w, 0.0, 1.0);
					    u_xlatb1 = unity_ProbeVolumeParams.x==1.0;
					    if(u_xlatb1){
					        u_xlatb1 = unity_ProbeVolumeParams.y==1.0;
					        u_xlat2.xyz = vs_TEXCOORD1.yyy * unity_ProbeVolumeWorldToObject[1].xyz;
					        u_xlat2.xyz = unity_ProbeVolumeWorldToObject[0].xyz * vs_TEXCOORD1.xxx + u_xlat2.xyz;
					        u_xlat2.xyz = unity_ProbeVolumeWorldToObject[2].xyz * vs_TEXCOORD1.zzz + u_xlat2.xyz;
					        u_xlat2.xyz = u_xlat2.xyz + unity_ProbeVolumeWorldToObject[3].xyz;
					        u_xlat2.xyz = (bool(u_xlatb1)) ? u_xlat2.xyz : vs_TEXCOORD1.xyz;
					        u_xlat2.xyz = u_xlat2.xyz + (-unity_ProbeVolumeMin.xyz);
					        u_xlat2.yzw = u_xlat2.xyz * unity_ProbeVolumeSizeInv.xyz;
					        u_xlat1.x = u_xlat2.y * 0.25;
					        u_xlat8 = unity_ProbeVolumeParams.z * 0.5;
					        u_xlat3.x = (-unity_ProbeVolumeParams.z) * 0.5 + 0.25;
					        u_xlat1.x = max(u_xlat1.x, u_xlat8);
					        u_xlat2.x = min(u_xlat3.x, u_xlat1.x);
					        u_xlat3 = texture(unity_ProbeVolumeSH, u_xlat2.xzw);
					        u_xlat4.xyz = u_xlat2.xzw + vec3(0.25, 0.0, 0.0);
					        u_xlat4 = texture(unity_ProbeVolumeSH, u_xlat4.xyz);
					        u_xlat2.xyz = u_xlat2.xzw + vec3(0.5, 0.0, 0.0);
					        u_xlat2 = texture(unity_ProbeVolumeSH, u_xlat2.xyz);
					        u_xlat5.xyz = vs_TEXCOORD0.xyz;
					        u_xlat5.w = 1.0;
					        u_xlat3.x = dot(u_xlat3, u_xlat5);
					        u_xlat3.y = dot(u_xlat4, u_xlat5);
					        u_xlat3.z = dot(u_xlat2, u_xlat5);
					    } else {
					        u_xlat2.xyz = vs_TEXCOORD0.xyz;
					        u_xlat2.w = 1.0;
					        u_xlat3.x = dot(unity_SHAr, u_xlat2);
					        u_xlat3.y = dot(unity_SHAg, u_xlat2);
					        u_xlat3.z = dot(unity_SHAb, u_xlat2);
					    }
					    u_xlat2.xyz = u_xlat3.xyz + vs_TEXCOORD4.xyz;
					    u_xlat2.xyz = max(u_xlat2.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat2.xyz = log2(u_xlat2.xyz);
					    u_xlat2.xyz = u_xlat2.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    u_xlat2.xyz = exp2(u_xlat2.xyz);
					    u_xlat2.xyz = u_xlat2.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    u_xlat2.xyz = max(u_xlat2.xyz, vec3(0.0, 0.0, 0.0));
					    u_xlat2.xyz = u_xlat0.www * u_xlat2.xyz;
					    u_xlat0.xyz = u_xlat7.xyz * vec3(0.779083729, 0.779083729, 0.779083729);
					    u_xlat1.xyz = u_xlat2.xyz * u_xlat0.xyz;
					    SV_Target3.xyz = _OceanEmission.xyz * vec3(vec3(_EmissionScale, _EmissionScale, _EmissionScale)) + u_xlat1.xyz;
					    SV_Target0 = u_xlat0;
					    SV_Target1.xyz = vec3(0.220916301, 0.220916301, 0.220916301);
					    SV_Target2.xyz = vs_TEXCOORD0.xyz * vec3(0.5, 0.5, 0.5) + vec3(0.5, 0.5, 0.5);
					    SV_Target2.w = 1.0;
					    SV_Target3.w = 1.0;
					    return;
					}"
				}
			}
		}
	}
	Fallback "Diffuse"
}