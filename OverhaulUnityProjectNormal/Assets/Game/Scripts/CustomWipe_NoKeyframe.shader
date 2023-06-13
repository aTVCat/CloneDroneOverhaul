Shader "Custom/Wipe_NoKeyframe" {
	Properties {
		_MainTex ("Texture1", 2D) = "white" {}
		_MaterialTime ("Current Time", Float) = 0
		_Gradient ("Gradient", Vector) = (0,1,0,0.35)
		_BarOpacity ("Bar Opacity", Float) = 0.35
		_BarEmissionScale ("Bar Emission Scale", Float) = 2
		_BarWidth ("Gradient Width", Float) = 64
		_StreakLength ("Streak Length (inverted)", Float) = 0.035
		_GlitchAmount ("Glitch Amount", Float) = 0.02
		_PixelResolution ("Pixel Resolution", Float) = 50
	}
	SubShader {
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 35615
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
						vec4 unused_0_2[4];
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
					in  vec2 in_TEXCOORD0;
					out vec4 vs_COLOR0;
					out vec2 vs_TEXCOORD0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					void main()
					{
					    vs_COLOR0 = vec4(1.0, 1.0, 1.0, 1.0);
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    u_xlat0 = in_POSITION0.yyyy * unity_ObjectToWorld[1];
					    u_xlat0 = unity_ObjectToWorld[0] * in_POSITION0.xxxx + u_xlat0;
					    u_xlat0 = unity_ObjectToWorld[2] * in_POSITION0.zzzz + u_xlat0;
					    u_xlat0 = u_xlat0 + unity_ObjectToWorld[3];
					    u_xlat1 = u_xlat0.yyyy * unity_MatrixVP[1];
					    u_xlat1 = unity_MatrixVP[0] * u_xlat0.xxxx + u_xlat1;
					    u_xlat1 = unity_MatrixVP[2] * u_xlat0.zzzz + u_xlat1;
					    gl_Position = unity_MatrixVP[3] * u_xlat0.wwww + u_xlat1;
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
						vec4 _Gradient;
						float _MaterialTime;
						float _BarOpacity;
						float _BarEmissionScale;
						float _BarWidth;
						float _StreakLength;
						float _GlitchAmount;
						float _PixelResolution;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[8];
					};
					uniform  sampler2D _MainTex;
					in  vec2 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					bool u_xlatb0;
					vec4 u_xlat1;
					float u_xlat2;
					vec4 u_xlat3;
					vec4 u_xlat4;
					float u_xlat5;
					float u_xlat6;
					float u_xlat7;
					float u_xlat10;
					vec2 u_xlat11;
					float u_xlat15;
					float u_xlat16;
					void main()
					{
					vec4 hlslcc_FragCoord = vec4(gl_FragCoord.xyz, 1.0/gl_FragCoord.w);
					    u_xlat0.x = hlslcc_FragCoord.y * 0.5;
					    u_xlat0.x = floor(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 0.5;
					    u_xlat0.x = fract(u_xlat0.x);
					    u_xlatb0 = (-u_xlat0.x)<0.0;
					    if(((int(u_xlatb0) * int(0xffffffffu)))!=0){discard;}
					    u_xlat0.x = vs_TEXCOORD0.y * _Time.x;
					    u_xlat0.x = u_xlat0.x * 12.9898005;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 43758.5469;
					    u_xlat0.x = fract(u_xlat0.x);
					    u_xlat0.x = min(u_xlat0.x, _GlitchAmount);
					    u_xlat0.x = (-u_xlat0.x) + _GlitchAmount;
					    u_xlat0.x = u_xlat0.x + vs_TEXCOORD0.x;
					    u_xlat5 = sin(_Time.x);
					    u_xlat5 = u_xlat5 * 43758.5469;
					    u_xlat5 = fract(u_xlat5);
					    u_xlat5 = min(u_xlat5, _GlitchAmount);
					    u_xlat5 = (-u_xlat5) + _GlitchAmount;
					    u_xlat0.x = u_xlat5 + u_xlat0.x;
					    u_xlat10 = u_xlat0.x * _PixelResolution;
					    u_xlat10 = floor(u_xlat10);
					    u_xlat15 = float(1.0) / _PixelResolution;
					    u_xlat10 = u_xlat15 * u_xlat10;
					    u_xlat15 = _BarWidth * _StreakLength;
					    u_xlat1.x = _BarWidth * 0.00100000005;
					    u_xlat6 = u_xlat15 * u_xlat10 + u_xlat1.x;
					    u_xlat10 = _BarWidth * u_xlat10 + u_xlat1.x;
					    u_xlat15 = u_xlat0.x * u_xlat15;
					    u_xlat15 = u_xlat15 * 0.699999988 + u_xlat1.x;
					    u_xlat1.x = _BarWidth * u_xlat0.x + u_xlat1.x;
					    u_xlat11.xy = vec2(_MaterialTime) + vec2(5.0, 0.300000012);
					    u_xlat16 = fract(u_xlat11.y);
					    u_xlat11.x = u_xlat11.x;
					    u_xlat11.x = clamp(u_xlat11.x, 0.0, 1.0);
					    u_xlat2 = u_xlat16 * -2.0 + 3.0;
					    u_xlat16 = u_xlat16 * u_xlat16;
					    u_xlat16 = u_xlat16 * u_xlat2;
					    u_xlat2 = _BarWidth * 0.00100000005 + 1.0;
					    u_xlat2 = u_xlat2 * _BarWidth;
					    u_xlat7 = u_xlat2 * _StreakLength;
					    u_xlat6 = u_xlat7 * u_xlat16 + (-u_xlat6);
					    u_xlat7 = u_xlat16 * u_xlat7;
					    u_xlat15 = u_xlat7 * 0.699999988 + (-u_xlat15);
					    u_xlat15 = _BarWidth * 0.000899999985 + u_xlat15;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    u_xlat6 = _BarWidth * 0.000899999985 + u_xlat6;
					    u_xlat6 = clamp(u_xlat6, 0.0, 1.0);
					    u_xlat7 = (-u_xlat2) * u_xlat16 + u_xlat10;
					    u_xlat1.x = (-u_xlat2) * u_xlat16 + u_xlat1.x;
					    u_xlat1.x = _BarWidth * 0.000899999985 + u_xlat1.x;
					    u_xlat1.x = clamp(u_xlat1.x, 0.0, 1.0);
					    u_xlat15 = u_xlat15 + u_xlat1.x;
					    u_xlat3 = vec4(u_xlat15) * (-_Gradient) + _Gradient;
					    u_xlat15 = _BarWidth * 0.000899999985 + u_xlat7;
					    u_xlat15 = clamp(u_xlat15, 0.0, 1.0);
					    u_xlat15 = u_xlat6 + u_xlat15;
					    u_xlat4.x = float(0.0);
					    u_xlat4.y = float(1.0);
					    u_xlat4.z = float(0.0);
					    u_xlat4.w = _BarOpacity;
					    u_xlat4 = vec4(u_xlat15) * (-u_xlat4.zyzw) + u_xlat4;
					    u_xlat15 = u_xlat11.x * -2.0 + 3.0;
					    u_xlat1.x = u_xlat11.x * u_xlat11.x;
					    u_xlat15 = u_xlat15 * u_xlat1.x;
					    u_xlat10 = (-u_xlat2) * u_xlat15 + u_xlat10;
					    u_xlat10 = clamp(u_xlat10, 0.0, 1.0);
					    u_xlat0.y = vs_TEXCOORD0.y;
					    u_xlat1 = texture(_MainTex, u_xlat0.xy);
					    u_xlat0 = vec4(u_xlat10) * (-u_xlat1) + u_xlat1;
					    u_xlat0 = u_xlat4 * vec4(vec4(_BarEmissionScale, _BarEmissionScale, _BarEmissionScale, _BarEmissionScale)) + u_xlat0;
					    SV_Target0 = u_xlat3 + u_xlat0;
					    return;
					}"
				}
			}
		}
	}
	Fallback "VertexLit"
}