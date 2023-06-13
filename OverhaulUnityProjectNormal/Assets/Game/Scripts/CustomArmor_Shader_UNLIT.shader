Shader "Custom/Armor_Shader_UNLIT" {
	Properties {
		_ArmorColor ("Armor Color", Vector) = (1,0.639,0,0.7)
		_EmissionColor ("Emission Color", Vector) = (1,0.75,0,0)
		_EmissionScale ("Emission Scale", Range(0.01, 10)) = 4.5
		_AnimationSpeed ("Animation Speed", Float) = 30
		_AnimationAmount ("Animation Amount", Range(0, 1)) = 0.3
		_RippleSize ("Ripple Size", Range(1, 200)) = 200
	}
	SubShader {
		LOD 300
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			LOD 300
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 40933
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
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					out vec4 vs_TEXCOORD2;
					vec4 u_xlat0;
					vec4 u_xlat1;
					float u_xlat7;
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
					    u_xlat0 = unity_MatrixVP[3] * u_xlat1.wwww + u_xlat0;
					    gl_Position = u_xlat0;
					    u_xlat1.x = dot(in_NORMAL0.xyz, unity_WorldToObject[0].xyz);
					    u_xlat1.y = dot(in_NORMAL0.xyz, unity_WorldToObject[1].xyz);
					    u_xlat1.z = dot(in_NORMAL0.xyz, unity_WorldToObject[2].xyz);
					    u_xlat7 = dot(u_xlat1.xyz, u_xlat1.xyz);
					    u_xlat7 = inversesqrt(u_xlat7);
					    vs_TEXCOORD1.xyz = vec3(u_xlat7) * u_xlat1.xyz;
					    u_xlat0.y = u_xlat0.y * _ProjectionParams.x;
					    u_xlat1.xzw = u_xlat0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD2.zw = u_xlat0.zw;
					    vs_TEXCOORD2.xy = u_xlat1.zz + u_xlat1.xw;
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
						vec4 _ArmorColor;
						vec4 unused_0_2;
						vec4 _EmissionColor;
						float _EmissionScale;
						int _AnimationSpeed;
						float _AnimationAmount;
						float _RippleSize;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 glstate_lightmodel_ambient;
						vec4 unused_2_1[22];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					in  vec4 vs_TEXCOORD2;
					layout(location = 0) out vec4 SV_Target0;
					vec2 u_xlat0;
					vec4 u_xlat1;
					vec3 u_xlat2;
					vec4 u_xlat3;
					vec3 u_xlat4;
					vec2 u_xlat8;
					float u_xlat12;
					void main()
					{
					    u_xlat0.xy = vs_TEXCOORD2.xy / vs_TEXCOORD2.ww;
					    u_xlat8.xy = u_xlat0.xy + vec2(-1.0, -0.0);
					    u_xlat8.x = dot(u_xlat8.xy, u_xlat8.xy);
					    u_xlat8.x = sqrt(u_xlat8.x);
					    u_xlat8.x = sin(u_xlat8.x);
					    u_xlat1 = vec4(vec4(_RippleSize, _RippleSize, _RippleSize, _RippleSize)) + vec4(53.0, 37.0, 61.0, 47.0);
					    u_xlat2.xyz = _Time.xxx * vec3(345.0, 300.0, 390.0);
					    u_xlat8.x = (-u_xlat8.x) * u_xlat1.z + u_xlat2.y;
					    u_xlat8.x = sin(u_xlat8.x);
					    u_xlat8.x = u_xlat8.x * 0.300000012;
					    u_xlat12 = dot(u_xlat0.xy, u_xlat0.xy);
					    u_xlat12 = sqrt(u_xlat12);
					    u_xlat12 = sin(u_xlat12);
					    u_xlat12 = (-u_xlat12) * u_xlat1.w + u_xlat2.z;
					    u_xlat12 = sin(u_xlat12);
					    u_xlat8.x = u_xlat12 * 0.600000024 + u_xlat8.x;
					    u_xlat3 = u_xlat0.xyxy + vec4(-1.0, -1.0, -0.0, -1.0);
					    u_xlat0.x = sin(u_xlat0.y);
					    u_xlat4.x = dot(u_xlat3.zw, u_xlat3.zw);
					    u_xlat4.z = dot(u_xlat3.xy, u_xlat3.xy);
					    u_xlat4.xz = sqrt(u_xlat4.xz);
					    u_xlat12 = sin(u_xlat4.z);
					    u_xlat4.z = (-u_xlat12) * u_xlat1.x + u_xlat2.x;
					    u_xlat4.xz = sin(u_xlat4.xz);
					    u_xlat4.x = (-u_xlat4.x) * u_xlat1.y + u_xlat2.x;
					    u_xlat4.x = sin(u_xlat4.x);
					    u_xlat4.x = u_xlat4.x * 0.400000006 + u_xlat8.x;
					    u_xlat4.x = u_xlat4.z * 0.5 + u_xlat4.x;
					    u_xlat8.x = _RippleSize + 83.0;
					    u_xlat0.x = u_xlat0.x * u_xlat8.x + u_xlat2.z;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * 0.899999976 + u_xlat4.x;
					    u_xlat0.x = u_xlat0.x * 10.0;
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat4.x = u_xlat0.x * -2.0 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat0.x;
					    u_xlat0.x = u_xlat0.x * u_xlat4.x;
					    u_xlat0.x = u_xlat0.x * 0.000300000014;
					    u_xlat4.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat1.x = dot(u_xlat4.xyz, u_xlat4.xyz);
					    u_xlat1.x = inversesqrt(u_xlat1.x);
					    u_xlat4.xyz = u_xlat4.xyz * u_xlat1.xxx;
					    u_xlat4.x = dot(u_xlat4.xyz, vs_TEXCOORD1.xyz);
					    u_xlat4.x = clamp(u_xlat4.x, 0.0, 1.0);
					    u_xlat4.x = (-u_xlat4.x) + 1.0;
					    u_xlat4.x = log2(u_xlat4.x);
					    u_xlat8.x = (-_EmissionScale) * 0.300000012 + 3.0;
					    u_xlat4.x = u_xlat4.x * u_xlat8.x;
					    u_xlat4.x = exp2(u_xlat4.x);
					    u_xlat1.xyz = glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz;
					    u_xlat4.xyz = u_xlat4.xxx * _EmissionColor.xyz + u_xlat1.xyz;
					    u_xlat0.x = u_xlat0.x / u_xlat4.x;
					    u_xlat0.x = u_xlat0.x / u_xlat4.y;
					    u_xlat0.x = u_xlat0.x / u_xlat4.z;
					    u_xlat4.xyz = u_xlat4.xyz * _ArmorColor.xyz;
					    SV_Target0.xyz = u_xlat4.xyz * vec3(_EmissionScale);
					    u_xlat4.x = float(_AnimationSpeed);
					    u_xlat4.x = u_xlat4.x * _Time.x;
					    u_xlat4.x = sin(u_xlat4.x);
					    u_xlat4.x = u_xlat4.x * _AnimationAmount;
					    u_xlat4.x = -abs(u_xlat4.x) + _ArmorColor.w;
					    u_xlat4.x = clamp(u_xlat4.x, 0.0, 1.0);
					    SV_Target0.w = u_xlat0.x + u_xlat4.x;
					    return;
					}"
				}
			}
		}
	}
	SubShader {
		LOD 200
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			LOD 200
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 123626
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
						vec4 _ArmorColor;
						vec4 unused_0_2;
						vec4 _EmissionColor;
						float _EmissionScale;
						int _AnimationSpeed;
						float _AnimationAmount;
					};
					layout(std140) uniform UnityPerCamera {
						vec4 _Time;
						vec4 unused_1_1[3];
						vec3 _WorldSpaceCameraPos;
						vec4 unused_1_3[4];
					};
					layout(std140) uniform UnityPerFrame {
						vec4 glstate_lightmodel_ambient;
						vec4 unused_2_1[22];
					};
					in  vec4 vs_TEXCOORD0;
					in  vec3 vs_TEXCOORD1;
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					vec3 u_xlat1;
					float u_xlat3;
					void main()
					{
					    u_xlat0.xyz = (-vs_TEXCOORD0.xyz) + _WorldSpaceCameraPos.xyz;
					    u_xlat3 = dot(u_xlat0.xyz, u_xlat0.xyz);
					    u_xlat3 = inversesqrt(u_xlat3);
					    u_xlat0.xyz = vec3(u_xlat3) * u_xlat0.xyz;
					    u_xlat0.x = dot(u_xlat0.xyz, vs_TEXCOORD1.xyz);
					    u_xlat0.x = clamp(u_xlat0.x, 0.0, 1.0);
					    u_xlat0.x = (-u_xlat0.x) + 1.0;
					    u_xlat0.x = log2(u_xlat0.x);
					    u_xlat1.x = (-_EmissionScale) * 0.300000012 + 3.0;
					    u_xlat0.x = u_xlat0.x * u_xlat1.x;
					    u_xlat0.x = exp2(u_xlat0.x);
					    u_xlat1.xyz = glstate_lightmodel_ambient.xyz + glstate_lightmodel_ambient.xyz;
					    u_xlat0.xyz = u_xlat0.xxx * _EmissionColor.xyz + u_xlat1.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * _ArmorColor.xyz;
					    SV_Target0.xyz = u_xlat0.xyz * vec3(_EmissionScale);
					    u_xlat0.x = float(_AnimationSpeed);
					    u_xlat0.x = u_xlat0.x * _Time.x;
					    u_xlat0.x = sin(u_xlat0.x);
					    u_xlat0.x = u_xlat0.x * _AnimationAmount;
					    SV_Target0.w = -abs(u_xlat0.x) + _ArmorColor.w;
					    SV_Target0.w = clamp(SV_Target0.w, 0.0, 1.0);
					    return;
					}"
				}
			}
		}
	}
	SubShader {
		LOD 100
		Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
		Pass {
			LOD 100
			Tags { "FORCENOSHADOWCASTING" = "true" "IGNOREPROJECTOR" = "true" "LIGHTMODE" = "FORWARDBASE" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha, SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			GpuProgramID 180113
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
					out vec4 vs_TEXCOORD0;
					out vec3 vs_TEXCOORD1;
					vec4 u_xlat0;
					vec4 u_xlat1;
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
					    vs_TEXCOORD1.xyz = vec3(0.0, 0.0, 0.0);
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
						vec4 _ArmorColor;
						float _ArmorAlpha;
						vec4 _EmissionColor;
						float _EmissionScale;
					};
					layout(std140) uniform UnityPerFrame {
						vec4 glstate_lightmodel_ambient;
						vec4 unused_1_1[22];
					};
					layout(location = 0) out vec4 SV_Target0;
					vec3 u_xlat0;
					void main()
					{
					    u_xlat0.xyz = glstate_lightmodel_ambient.xyz * vec3(2.0, 2.0, 2.0) + _EmissionColor.xyz;
					    u_xlat0.xyz = u_xlat0.xyz * _ArmorColor.xyz;
					    SV_Target0.xyz = u_xlat0.xyz * vec3(_EmissionScale);
					    SV_Target0.w = _ArmorAlpha;
					    return;
					}"
				}
			}
		}
	}
	Fallback "Unlit"
}