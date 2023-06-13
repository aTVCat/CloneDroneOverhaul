Shader "HOLO/Holo" {
	Properties {
		_MainTex ("Not used confusing", 2D) = "white" {}
		_originalDiffuse ("Original Diffuse Map", 2D) = "white" {}
		_Diffuse ("Diffuse Map", 2D) = "white" {}
		[HDR] _diff_Color ("Diffuse Color Mult", Vector) = (1,1,1,1)
		_N_map ("Noise", 2D) = "white" {}
		_M_map ("Mask", 2D) = "white" {}
		_intensity ("Intensity", Float) = 0
		_deform ("Deformation Intensity", Float) = 1
		[HDR] _Color ("Outline Color Mult", Vector) = (1,1,1,1)
		_Opacity ("Base Opacity", Range(0, 1)) = 0
		_Bias ("Bias", Range(0, 1)) = 0
		_Scale ("Scale ", Range(0, 10)) = 0
		_Power ("Power", Range(0, 3)) = 0
		_Speed ("Speed", Range(-1, 1)) = 0
		_t ("Extra Option", Range(0, 1)) = 0
		_noise_details ("G/H Noise Details Amount ", Range(1, 16)) = 0
		[Toggle] _X ("Active X Axe", Float) = 1
		[Toggle] _Y ("Active X Axe", Float) = 1
		[Toggle] _glitchColor ("Display G/H Color", Float) = 1
		[Toggle] _monochrom ("Monochromatic", Float) = 1
		[Toggle] _OriginalUVSwitch ("Switch to Orginal UVs on/off", Float) = 0
		_Distance ("Distance", Float) = 0
		_Amplitude ("Amplitude", Float) = 0
		_Speed_Up ("_Speed_Up", Float) = 0
		_Amount ("Amount", Range(0, 1)) = 0
	}
	SubShader {
		Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			GpuProgramID 31229
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
						float _X;
						float _Y;
						float _noise_details;
						vec4 unused_0_12;
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
					out vec4 vs_TEXCOORD0;
					out vec4 vs_TEXCOORD1;
					out vec3 vs_TEXCOORD2;
					out vec3 vs_TEXCOORD3;
					out vec4 vs_TEXCOORD4;
					out vec4 vs_NORMAL0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					vec3 u_xlat2;
					vec3 u_xlat3;
					vec4 u_xlat4;
					vec4 u_xlat5;
					vec4 u_xlat6;
					vec3 u_xlat7;
					vec3 u_xlat8;
					float u_xlat9;
					vec3 u_xlat13;
					float u_xlat18;
					float u_xlat27;
					int u_xlati27;
					float u_xlat28;
					int u_xlati28;
					bool u_xlatb29;
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
					    u_xlat28 = u_xlat5.x * _M_map_ST.y;
					    u_xlat27 = u_xlat27 * _M_map_ST.w;
					    u_xlat4.xy = in_TEXCOORD0.xy * vec2(u_xlat28) + vec2(u_xlat27);
					    u_xlat4 = textureLod(_M_map, u_xlat4.xy, 0.0);
					    u_xlati27 = int(_noise_details);
					    u_xlat13.xyz = _Time.xyz * vec3(0.100000001, 0.100000001, 0.100000001);
					    u_xlat5.xy = in_TEXCOORD0.xy;
					    u_xlat5.z = 0.0;
					    u_xlat6.x = float(0.0);
					    u_xlat6.y = float(0.0);
					    u_xlat6.z = float(0.0);
					    for(int u_xlati_loop_1 = u_xlati27 ; u_xlati_loop_1>0 ; u_xlati_loop_1 = u_xlati_loop_1 + int(0xFFFFFFFFu))
					    {
					        u_xlat7.xyz = floor(u_xlat5.xyz);
					        u_xlat8.xyz = u_xlat13.xyz * u_xlat7.xyz;
					        u_xlat7.xyz = u_xlat7.xxz * u_xlat7.yyy + u_xlat8.xyz;
					        u_xlat7.xyz = sin(u_xlat7.xyz);
					        u_xlat6.xyz = u_xlat6.xyz + u_xlat7.xyz;
					        u_xlat5.xyz = u_xlat5.xyz * vec3(2.5, 2.5, 2.5);
					    }
					    u_xlat13.xy = vec2(_deform, _intensity) * vec2(0.00100000005, 0.100000001);
					    u_xlat5.xyz = u_xlat13.xxx * u_xlat6.xyz;
					    u_xlat13.xyz = u_xlat13.yyy * u_xlat5.xyz;
					    u_xlat4.xyz = u_xlat4.xxx * u_xlat13.xyz;
					    u_xlat4.x = u_xlat4.x * _X;
					    u_xlat4.y = u_xlat4.y * _Y;
					    u_xlat4.xyz = u_xlat4.xyz * in_NORMAL0.xyz;
					    u_xlat4.xyz = u_xlat4.xyz * vec3(10.0, 10.0, 10.0) + in_POSITION0.xyz;
					    u_xlat5 = u_xlat4.yyyy * unity_ObjectToWorld[1];
					    u_xlat5 = unity_ObjectToWorld[0] * u_xlat4.xxxx + u_xlat5;
					    u_xlat4 = unity_ObjectToWorld[2] * u_xlat4.zzzz + u_xlat5;
					    u_xlat5 = u_xlat4 + unity_ObjectToWorld[3];
					    u_xlat6 = u_xlat5.yyyy * unity_MatrixVP[1];
					    u_xlat6 = unity_MatrixVP[0] * u_xlat5.xxxx + u_xlat6;
					    u_xlat6 = unity_MatrixVP[2] * u_xlat5.zzzz + u_xlat6;
					    gl_Position = unity_MatrixVP[3] * u_xlat5.wwww + u_xlat6;
					    u_xlat4.xyz = unity_ObjectToWorld[3].xyz * in_POSITION0.www + u_xlat4.xyz;
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
					    u_xlat0.xyz = u_xlat4.xyz + (-_WorldSpaceCameraPos.xyz);
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
					    vs_TEXCOORD0 = in_TEXCOORD0;
					    vs_TEXCOORD1 = vec4(0.0, 0.0, 0.0, 0.0);
					    vs_TEXCOORD4.yzw = vec3(0.0, 0.0, 0.0);
					    vs_NORMAL0 = vec4(0.0, 0.0, 0.0, 0.0);
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
						vec4 _N_map_ST;
						vec4 _M_map_ST;
						float _intensity;
						vec4 unused_0_7;
						float _Speed;
						float _glitchColor;
						float _monochrom;
						float _Opacity;
						float _noise_details;
						vec4 unused_0_13;
						float _OriginalUVSwitch;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[8];
					};
					uniform  sampler2D _N_map;
					uniform  sampler2D _M_map;
					uniform  sampler2D _Diffuse;
					uniform  sampler2D _originalDiffuse;
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD2;
					in  vec4 vs_TEXCOORD4;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					vec4 u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					vec2 u_xlat5;
					vec3 u_xlat6;
					bvec3 u_xlatb6;
					vec2 u_xlat7;
					float u_xlat8;
					vec2 u_xlat14;
					vec2 u_xlat15;
					bool u_xlatb15;
					float u_xlat18;
					int u_xlati18;
					int u_xlati19;
					bool u_xlatb19;
					void main()
					{
					    u_xlat0.x = unused_0_7.y * _Time.x;
					    u_xlat0.xyz = u_xlat0.xxx * vec3(30.0, 60.0, 120.0);
					    u_xlat1.x = sin(u_xlat0.y);
					    u_xlat2.x = cos(u_xlat0.y);
					    u_xlat6.x = u_xlat1.x + 1.0;
					    u_xlat7.xy = u_xlat0.xx * _N_map_ST.zw;
					    u_xlat7.xy = vs_TEXCOORD0.xy * _N_map_ST.xy + u_xlat7.xy;
					    u_xlat3 = texture(_N_map, u_xlat7.xy);
					    u_xlat18 = u_xlat2.x * _M_map_ST.y;
					    u_xlat6.x = u_xlat6.x * _M_map_ST.w;
					    u_xlat6.xz = vs_TEXCOORD0.xy * vec2(u_xlat18) + u_xlat6.xx;
					    u_xlat2 = texture(_M_map, u_xlat6.xz);
					    u_xlatb6.xz = equal(unused_0_13.yyyy, vec4(0.0, 0.0, 1.0, 1.0)).xz;
					    if(u_xlatb6.x){
					        u_xlat7.xy = vs_TEXCOORD2.xy * _Diffuse_ST.yy + _Diffuse_ST.ww;
					        u_xlat4 = texture(_Diffuse, u_xlat7.xy);
					    } else {
					        u_xlat4.x = float(0.0);
					        u_xlat4.y = float(0.0);
					        u_xlat4.z = float(0.0);
					        u_xlat4.w = float(0.0);
					    }
					    if(u_xlatb6.z){
					        u_xlat4 = texture(_originalDiffuse, vs_TEXCOORD0.xy);
					    }
					    u_xlat0.y = u_xlat2.x * u_xlat3.x;
					    u_xlat0.xz = sin(u_xlat0.xz);
					    u_xlat0.xz = u_xlat0.xz * u_xlat0.yy;
					    u_xlat18 = u_xlat0.x * vs_TEXCOORD0.x;
					    u_xlat7.xy = vec2(u_xlat18) * vec2(_intensity) + vs_TEXCOORD0.xy;
					    u_xlati18 = int(_noise_details);
					    u_xlat2.xy = _Time.xy * vec2(0.100000001, 0.100000001);
					    u_xlat14.xy = u_xlat7.xy;
					    u_xlat3.x = float(0.0);
					    u_xlat3.y = float(0.0);
					    for(int u_xlati_loop_1 = u_xlati18 ; u_xlati_loop_1>0 ; u_xlati_loop_1 = u_xlati_loop_1 + int(0xFFFFFFFFu))
					    {
					        u_xlat15.xy = floor(u_xlat14.xy);
					        u_xlat5.xy = u_xlat2.xy * u_xlat15.xy;
					        u_xlat15.xy = u_xlat15.xx * u_xlat15.yy + u_xlat5.xy;
					        u_xlat15.xy = sin(u_xlat15.xy);
					        u_xlat3.xy = u_xlat15.xy + u_xlat3.xy;
					        u_xlat14.xy = u_xlat14.xy * vec2(2.5, 2.5);
					    }
					    u_xlat1.yz = u_xlat3.xy * vec2(vec2(_glitchColor, _glitchColor));
					    u_xlat0.xyz = u_xlat0.xyz * u_xlat1.yxy;
					    u_xlat0.y = u_xlat1.z * u_xlat0.y;
					    u_xlat0.xyz = u_xlat0.xyz * vec3(_intensity);
					    u_xlat1.y = u_xlat0.y * 0.375 + _Color.y;
					    u_xlat1.xz = u_xlat0.xz * vec2(0.1875, 0.75) + _Color.xz;
					    u_xlat4.w = u_xlat4.w * _Opacity;
					    u_xlat0 = u_xlat4 * _diff_Color;
					    u_xlatb19 = _monochrom==1.0;
					    u_xlat2.x = u_xlat1.y + u_xlat1.x;
					    u_xlat2.x = u_xlat1.z + u_xlat2.x;
					    u_xlat8 = u_xlat0.y + u_xlat0.x;
					    u_xlat2.y = u_xlat4.z * _diff_Color.z + u_xlat8;
					    u_xlat2.xy = u_xlat2.xy * vec2(0.333333343, 0.333333343);
					    u_xlat0.xyz = (bool(u_xlatb19)) ? u_xlat2.yyy : u_xlat0.xyz;
					    u_xlat1.xyz = (bool(u_xlatb19)) ? u_xlat2.xxx : u_xlat1.xyz;
					    u_xlat1.xyz = (-u_xlat0.xyz) + u_xlat1.xyz;
					    u_xlat1.w = (-u_xlat0.w) + _Color.w;
					    SV_Target0 = vs_TEXCOORD4.xxxx * u_xlat1 + u_xlat0;
					    return;
					}"
				}
			}
		}
	}
	CustomEditor "Glitch_Editor_lite"
}