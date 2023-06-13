Shader "Custom/Hyperdrive" {
	Properties {
		_MainTex ("Texture1", 2D) = "white" {}
		_Brightness ("Brightness Shift", Range(0, 1)) = 0.007
		_BrightnessFade ("Brightness Fade", Range(0.5, 10)) = 3
		_RedShift ("Red Shift", Range(0, 0.333)) = 0.1
		_ColorShift ("Color Shift", Range(0, 0.01)) = 0.0007
		_Speed ("Speed", Range(0.1, 1)) = 0.6
		_YScale ("Y Scale", Range(0.01, 1)) = 0.271
		_XYScale ("XY Scale", Range(1, 1000)) = 196
		_StarCount ("Star Count", Range(0.001, 0.1)) = 0.04
	}
	SubShader {
		Tags { "FORCENOSHADOWCASTING" = "true" }
		Pass {
			Tags { "FORCENOSHADOWCASTING" = "true" }
			GpuProgramID 35193
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
						vec4 unused_0_2[2];
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
						float _BrightnessFade;
						float _Brightness;
						float _RedShift;
						float _ColorShift;
						float _Speed;
						float _YScale;
						float _XYScale;
						float _StarCount;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[8];
					};
					in  vec2 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					bool u_xlatb0;
					vec4 u_xlat1;
					vec2 u_xlat2;
					float u_xlat3;
					vec2 u_xlat6;
					vec2 u_xlat7;
					float u_xlat9;
					bool u_xlatb9;
					void main()
					{
					    u_xlat0.y = 1.0;
					    u_xlat6.x = (-_Time.x) * _Speed + (-_ColorShift);
					    u_xlat6.y = u_xlat6.x + (-_ColorShift);
					    u_xlat1.xz = u_xlat6.xy * vec2(300.0, 300.0);
					    u_xlat6.x = u_xlat6.y + (-_ColorShift);
					    u_xlat0.x = u_xlat6.x * 300.0;
					    u_xlat6.x = log2(vs_TEXCOORD0.y);
					    u_xlat6.x = u_xlat6.x * _YScale;
					    u_xlat2.x = exp2(u_xlat6.x);
					    u_xlat2.y = vs_TEXCOORD0.x;
					    u_xlat0.xy = u_xlat2.xy * vec2(vec2(_XYScale, _XYScale)) + (-u_xlat0.xy);
					    u_xlat6.xy = fract(u_xlat0.xy);
					    u_xlat0.xy = (-u_xlat6.xy) + u_xlat0.xy;
					    u_xlat6.x = dot(u_xlat6.xy, u_xlat6.xy);
					    u_xlat6.x = sqrt(u_xlat6.x);
					    u_xlat6.x = (-u_xlat6.x) + _BrightnessFade;
					    u_xlat0.x = u_xlat0.y * 100.0 + u_xlat0.x;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 1000.0;
					    u_xlat0.x = fract(u_xlat0.x);
					    u_xlatb0 = _StarCount>=u_xlat0.x;
					    u_xlat0.x = u_xlatb0 ? 1.0 : float(0.0);
					    u_xlat0.x = dot(u_xlat6.xx, u_xlat0.xx);
					    u_xlat0.x = u_xlat0.x * vs_TEXCOORD0.y;
					    u_xlat3 = vs_TEXCOORD0.y + -0.400000006;
					    u_xlat3 = u_xlat3 * 1.66666663;
					    u_xlat3 = clamp(u_xlat3, 0.0, 1.0);
					    u_xlat6.x = u_xlat3 * -2.0 + 3.0;
					    u_xlat3 = u_xlat3 * u_xlat3;
					    u_xlat3 = u_xlat3 * u_xlat6.x;
					    u_xlat0.x = u_xlat3 * u_xlat0.x;
					    u_xlat0.x = u_xlat0.x * _Brightness;
					    u_xlat6.x = (-_RedShift) * 3.0 + 1.0;
					    u_xlat9 = u_xlat6.x + _RedShift;
					    u_xlat6.x = _RedShift * 2.0 + u_xlat6.x;
					    SV_Target0.z = u_xlat9 * u_xlat0.x;
					    u_xlat1.y = float(1.0);
					    u_xlat1.w = float(1.0);
					    u_xlat0.xw = u_xlat2.xy * vec2(vec2(_XYScale, _XYScale)) + (-u_xlat1.xy);
					    u_xlat1.xy = u_xlat2.xy * vec2(vec2(_XYScale, _XYScale)) + (-u_xlat1.zw);
					    u_xlat7.xy = fract(u_xlat0.xw);
					    u_xlat0.xw = u_xlat0.xw + (-u_xlat7.xy);
					    u_xlat7.x = dot(u_xlat7.xy, u_xlat7.xy);
					    u_xlat7.x = sqrt(u_xlat7.x);
					    u_xlat7.x = (-u_xlat7.x) + _BrightnessFade;
					    u_xlat0.x = u_xlat0.w * 100.0 + u_xlat0.x;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 1000.0;
					    u_xlat0.x = fract(u_xlat0.x);
					    u_xlatb0 = _StarCount>=u_xlat0.x;
					    u_xlat0.x = u_xlatb0 ? 1.0 : float(0.0);
					    u_xlat0.x = dot(u_xlat7.xx, u_xlat0.xx);
					    u_xlat0.x = u_xlat0.x * vs_TEXCOORD0.y;
					    u_xlat0.x = u_xlat3 * u_xlat0.x;
					    SV_Target0.x = u_xlat0.x * _Brightness;
					    u_xlat0.xw = fract(u_xlat1.xy);
					    u_xlat1.xy = (-u_xlat0.xw) + u_xlat1.xy;
					    u_xlat0.x = dot(u_xlat0.xw, u_xlat0.xw);
					    u_xlat0.x = sqrt(u_xlat0.x);
					    u_xlat0.x = (-u_xlat0.x) + _BrightnessFade;
					    u_xlat9 = u_xlat1.y * 100.0 + u_xlat1.x;
					    u_xlat9 = sin(u_xlat9);
					    u_xlat9 = u_xlat9 * 1000.0;
					    u_xlat9 = fract(u_xlat9);
					    u_xlatb9 = _StarCount>=u_xlat9;
					    u_xlat9 = u_xlatb9 ? 1.0 : float(0.0);
					    u_xlat0.x = dot(u_xlat0.xx, vec2(u_xlat9));
					    u_xlat0.x = u_xlat0.x * vs_TEXCOORD0.y;
					    u_xlat0.x = u_xlat3 * u_xlat0.x;
					    u_xlat0.x = u_xlat0.x * _Brightness;
					    SV_Target0.y = u_xlat6.x * u_xlat0.x;
					    SV_Target0.w = 1.0;
					    return;
					}"
				}
			}
		}
	}
	Fallback "VertexLit"
}