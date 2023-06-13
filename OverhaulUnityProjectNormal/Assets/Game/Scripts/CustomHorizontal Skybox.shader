Shader "Custom/Horizontal Skybox" {
	Properties {
		_Color1 ("Top Color", Vector) = (1,1,1,0)
		_Color2 ("Horizon Color", Vector) = (1,1,1,0)
		_Color3 ("Bottom Color", Vector) = (1,1,1,0)
		_Exponent1 ("Exponent Factor for Top Half", Float) = 1
		_Exponent2 ("Exponent Factor for Bottom Half", Float) = 1
		_Intensity ("Intensity Amplifier", Float) = 1
		_Angle ("Angle", Float) = 0
	}
	SubShader {
		Tags { "QUEUE" = "Background" "RenderType" = "Background" }
		Pass {
			Tags { "QUEUE" = "Background" "RenderType" = "Background" }
			ZWrite Off
			Cull Off
			Fog {
				Mode Off
			}
			GpuProgramID 16661
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
						vec4 unused_0_1[7];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 unused_1_0[17];
						mat4x4 unity_MatrixVP;
						vec4 unused_1_2[2];
					};
					in  vec4 in_POSITION0;
					in  vec3 in_TEXCOORD0;
					out vec3 vs_TEXCOORD0;
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
					    vs_TEXCOORD0.xz = in_TEXCOORD0.yy;
					    vs_TEXCOORD0.y = 0.5;
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
						vec4 _Color1;
						vec4 _Color2;
						vec4 _Color3;
						float _Intensity;
						float _Exponent1;
						float _Exponent2;
					};
					in  vec3 vs_TEXCOORD0;
					layout(location = 0) out vec4 SV_Target0;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat2;
					float u_xlat4;
					void main()
					{
					    u_xlat0.x = dot(vs_TEXCOORD0.xyz, vs_TEXCOORD0.xyz);
					    u_xlat0.x = inversesqrt(u_xlat0.x);
					    u_xlat0.y = (-vs_TEXCOORD0.x) * u_xlat0.x + 1.0;
					    u_xlat0.x = vs_TEXCOORD0.x * u_xlat0.x + 1.0;
					    u_xlat0.xy = min(u_xlat0.xy, vec2(1.0, 1.0));
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _Exponent2;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat2 = log2(u_xlat0.y);
					    u_xlat2 = u_xlat2 * _Exponent1;
					    u_xlat0.y = exp2(u_xlat2);
					    u_xlat0.xy = (-u_xlat0.xy) + vec2(1.0, 1.0);
					    u_xlat4 = (-u_xlat0.y) + 1.0;
					    u_xlat4 = (-u_xlat0.x) + u_xlat4;
					    u_xlat1 = vec4(u_xlat4) * _Color2;
					    u_xlat1 = _Color1 * u_xlat0.yyyy + u_xlat1;
					    u_xlat0 = _Color3 * u_xlat0.xxxx + u_xlat1;
					    SV_Target0 = u_xlat0 * vec4(_Intensity);
					    return;
					}"
				}
			}
		}
	}
}