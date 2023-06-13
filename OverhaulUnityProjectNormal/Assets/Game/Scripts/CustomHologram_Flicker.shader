Shader "Custom/Hologram_Flicker" {
	Properties {
		_MainTex ("Texture1", 2D) = "white" {}
		_SecondTex ("Texture2", 2D) = "white" {}
		_RotationSpeed ("Rotation Speed", Float) = 1
		_ReverseSpeed ("Reverse Speed", Float) = 1
		_GlitchAmount ("Glitch Amount", Float) = 0.02
		_FlickerAmount ("Flicker Amount", Float) = 0.5
	}
	SubShader {
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 11836
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
						vec4 unused_0_0[2];
						vec4 _MainTex_ST;
						vec4 unused_0_2;
					};
					layout(std140) uniform UnityPerDraw {
						mat4x4 unity_ObjectToWorld;
						vec4 unused_1_1[7];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_2_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_2_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec4 in_TEXCOORD0;
					out vec4 vs_COLOR0;
					out vec2 vs_TEXCOORD0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
					    vs_COLOR0 = vec4(1.0, 1.0, 1.0, 1.0);
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
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
						vec4 unused_0_0[3];
						float _RotationSpeed;
						float _ReverseSpeed;
						float _GlitchAmount;
						float _FlickerAmount;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[8];
					};
					uniform  sampler2D _MainTex;
					uniform  sampler2D _SecondTex;
					in  vec2 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    u_xlat0.x = vs_TEXCOORD0.x * _Time.x;
					    u_xlat0.x = u_xlat0.x * 12.9898005;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 43758.5469;
					    u_xlat0.x = fract(u_xlat0.x);
					    u_xlat0.x = min(u_xlat0.x, _GlitchAmount);
					    u_xlat0.x = (-u_xlat0.x) + _GlitchAmount;
					    u_xlat0.x = u_xlat0.x + vs_TEXCOORD0.x;
					    u_xlat0.y = _Time.x * _RotationSpeed + u_xlat0.x;
					    u_xlat0.x = (-_Time.x) * _ReverseSpeed + u_xlat0.x;
					    u_xlat1.xz = fract(u_xlat0.yx);
					    u_xlat1.yw = vs_TEXCOORD0.yy;
					    u_xlat0 = texture(_MainTex, u_xlat1.xy);
					    u_xlat1 = texture(_SecondTex, u_xlat1.zw);
					    u_xlat0 = u_xlat0 + u_xlat1;
					    u_xlat1.x = sin(_Time.x);
					    u_xlat1.x = u_xlat1.x * 43758.5469;
					    u_xlat1.x = fract(u_xlat1.x);
					    u_xlat1.x = min(u_xlat1.x, _FlickerAmount);
					    SV_Target0.w = u_xlat0.w + (-u_xlat1.x);
					    SV_Target0.w = clamp(SV_Target0.w, 0.0, 1.0);
					    SV_Target0.xyz = u_xlat0.xyz;
					    SV_Target0.xyz = clamp(SV_Target0.xyz, 0.0, 1.0);
					    return;
					}"
				}
			}
		}
	}
	Fallback "VertexLit"
}