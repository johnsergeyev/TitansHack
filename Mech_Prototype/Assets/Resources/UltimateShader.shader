// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "AOM/UltimateShader"
{
	Properties
	{
		_Rim ("Rim", Float) = 1
		//_KRoughness ("Roughness", Range(0.0,1.0)) = 1
		_Bump ("Bump", Range(0.0,1.0)) = 1
		_Contrast ("Contrast", Range(0.0,1.0)) = 0
		_FX ("FX", Color) = (0,0,0,0)
		_Tint ("Tint", Color) = (1,1,1,1)
		_Emission("Emission", Color) = (0,0,0,0)
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap("Normal Map", 2D) = "bump" {}
		_SpecularMap("Specular Map", 2D) = "black" {}
		_EmissionMap("Emission Map", 2D) = "black" {}
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags {"LightMode"="ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			//#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
			#pragma target 3.0
			//#pragma only_renderers gles
			#pragma fragmentoption ARB_precision_hint_fastest

			#ifdef SHADER_API_OPENGL  
			#pragma glsl
			#endif

			#include "UnityCG.cginc"
			#include "UnityStandardConfig.cginc"

			uniform sampler2D _ShadowMapTex;
			uniform float4x4 _ShadowProjectionMatrix;
			uniform sampler2D _RimShadowMapTex;
			uniform float4x4 _RimShadowProjectionMatrix;
			uniform half _ShadowMapBias;

			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform sampler2D _SpecularMap;
			uniform sampler2D _EmissionMap;
			uniform fixed4 _Tint;
			uniform fixed4 _Emission;
			uniform half _Rim;
			uniform half _Contrast;
			uniform half3 _FX;
uniform half _Bump;
//uniform float _KRoughness;

			uniform half3 _AmbientColor;
			uniform half3 _FloorColor;
			uniform half3 _KeyLightPosition;
			uniform half3 _KeyLightDirection;
			uniform half3 _KeyLightColor;
			uniform half3 _RimLightDirection;
			uniform half3 _RimLightColor;
			uniform half3 _FxLightPosition;
			uniform half3 _FxLightColor;

			uniform fixed3 _Gain;
			uniform fixed3 _Offset;
			uniform fixed _Saturation;

			samplerCUBE _EnvironmentMap;

            struct v2f {
            	float4 pos 			: SV_POSITION;
                float3 worldPos 	: TEXCOORD0;
                half3 tspace0 		: TEXCOORD1;
                half3 tspace1 		: TEXCOORD2;
                half3 tspace2 		: TEXCOORD3;
                float2 uv 			: TEXCOORD4;
       			float4 ShMapUV	: TEXCOORD5;
				float dpth  		: TEXCOORD6;
       			//float4 ShMapRimUV	: TEXCOORD7;
				//float dpthRim  		: TEXCOORD8;
				fixed occlusion 	: TEXCOORD9;
				fixed fogFactor 	: TEXCOORD10;
                //UNITY_FOG_COORDS(11)
            };

            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0, fixed3 vcolor : COLOR)
            {
                v2f o;
                //o.pos = UnityObjectToClipPos(vertex);
                o.pos = UnityObjectToClipPos(vertex);
                float4 Pworld = mul( unity_ObjectToWorld, vertex );

                o.worldPos = Pworld.xyz;
                half3 wNormal = UnityObjectToWorldNormal(normal);
                half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
                half tangentSign = tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                o.uv = uv;

                // FOG_EXP2
                // UNITY_TRANSFER_FOG(o, o.pos);
                float unityFogFactor = unity_FogParams.x * o.pos.z;
                o.fogFactor = exp2(-unityFogFactor*unityFogFactor);

				// shadow coords
				o.ShMapUV = mul( _ShadowProjectionMatrix, Pworld );					// ShMapUV.xy - shadowmap UV
				o.dpth = ( o.ShMapUV.z / o.ShMapUV.w );								// vertex distance from light
				//o.ShMapRimUV = mul( _RimShadowProjectionMatrix, Pworld );			// ShMapUV.xy - shadowmap UV
				//o.dpthRim = ( o.ShMapRimUV.z / o.ShMapRimUV.w );					// vertex distance from light

				o.occlusion = vcolor.r;// * 0.7 + 0.3;

                return o;
            }

			inline fixed sampleShadowmap( sampler2D tex, float2 uv, float depth )
			{
				float shD = tex2D (tex, uv) + _ShadowMapBias;
				shD += saturate(shD - 0.999)*10000;
				return step(depth, shD);
			}


			float schlick_fresnel (half3 rd, half3 nrm, half _FresnelPower)
			{
			    float R0 = (1.0 - _FresnelPower) / (1.0 + _FresnelPower);
			    R0 = R0 * R0;
			    float cos_a = max(dot(rd, nrm), 0.0);
			    return R0 + (1.0 - R0) * pow((1.0 - cos_a), 5.0);
			}

			half fresnel (half3 rd, half3 nrm, half _FresnelPower)
			{
			    float cos_a = max(dot(rd, nrm), 0.0);
			    float cos_b = sqrt(1.0 - (1.0 - cos_a*cos_a)/(_FresnelPower * _FresnelPower));
			    //float p_polar = abs((cos_b - _FresnelPower * cos_a) / (cos_b + _FresnelPower * cos_a));
			    //return p_polar * p_polar;
			    float s_polar = abs((cos_a - _FresnelPower * cos_b) / (cos_a + _FresnelPower * cos_b));
			    return s_polar * s_polar;
			}

			samplerCUBE _SpecCube;

			/*
			half3 SampleEnvironment(half3 dir, half roughness)
			{
 				//half3 env = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, dir, (1 - roughness) * UNITY_SPECCUBE_LOD_STEPS);
 				//half3 env = texCUBElod ( unity_SpecCube0, half4(dir, (1 - roughness) * 6) );
 				half3 env = texCUBElod ( _EnvironmentMap, half4(dir, (1-roughness)*6 ) );	
 				//half3 env = texCUBEbias ( _SpecCube, half4(dir, (1-roughness)*8 ) );

                env = DecodeHDR(half4(env,1), unity_SpecCube0_HDR).rgb;

                env -= _Offset;
                env /= _Gain;
                env *= env;
                return env;
			}
			*/

			half3 SampleEnvironment(half3 dir, half roughness)
			{
                //env = lerp(_FloorColor, _AmbientColor, dir.y*0.5 + 0.5);	// TODO: add horizon to improve reflection

                half3 Horizon = (_AmbientColor + _FloorColor)* 0.6;
                half3 sky = lerp(Horizon, _AmbientColor, dir.y*dir.y);
                half3 ground = lerp(Horizon, _FloorColor, -dir.y);
                half mask = step(0, dir.y);
                half3 env = ground * (1-mask) + sky * mask;
                // indirect
                half indirect = saturate(-dir.y * 0.5 + 0.5);
                indirect*= indirect;
                indirect *= 1-indirect*indirect;
                env += indirect * _FloorColor * 10 * normalize(_KeyLightColor);
                return env;
            }

            fixed4 frag (v2f i) : SV_Target
            {
             	fixed4 c = 1;

            	//
                // NORMAL MAP
                //
//no normal map
half3 GeoNormal;
GeoNormal.x = i.tspace0.z;
GeoNormal.y = i.tspace1.z;
GeoNormal.z = i.tspace2.z;
GeoNormal = normalize(GeoNormal);

                half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
                half3 worldNormal;
                worldNormal.x = dot(i.tspace0, tnormal);
                worldNormal.y = dot(i.tspace1, tnormal);
                worldNormal.z = dot(i.tspace2, tnormal);
                half3 Nn = normalize(worldNormal);
//Nn = normalize(lerp(GeoNormal, Nn, _Bump));
//Nn=GeoNormal;

                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                half3 worldRefl = reflect(-worldViewDir, Nn);

                //
                // SHADOWS
                //
                fixed KeyShadow = sampleShadowmap(_ShadowMapTex, i.ShMapUV.xy, i.dpth);
				fixed RimShadow = 1;//sampleShadowmap(_RimShadowMapTex, i.ShMapRimUV.xy, i.dpthRim);


                //
                // TEXTURES
                //
                half3 baseColor = tex2D(_MainTex, i.uv).rgb * 2;
                half roughness = tex2D(_SpecularMap, i.uv).r;
                roughness = roughness*roughness;
                half emission = tex2D(_EmissionMap, i.uv).r;

                //
                // LIGHT
                //
                half3 dir = normalize( lerp(Nn, worldRefl, pow(roughness, 0.25)) );
                half p = roughness * roughness * 64 + 1;

				// environment
				half3 env = SampleEnvironment(dir, roughness);
                //env = lerp (env * i.occlusion * 1.57, env, roughness);
                env = env * i.occlusion;
                env = lerp (env*0.25, env*0.5, roughness);

                // key
                half3 toKey = _KeyLightPosition - i.worldPos;
                half KeyAttenuation = dot(toKey, toKey);
				half KeyShade = max(0, dot(dir, normalize(_KeyLightDirection.xyz)));
  				KeyShade = pow(KeyShade, p) * (roughness+1);
  				half3 KeyLight = KeyShade * _KeyLightColor * KeyShadow / KeyAttenuation;
  				//KeyLight *= max(0, dot(GeoNormal, normalize(_KeyLightDirection.xyz)));

  				// rim
				half RimShade = max(0, dot(dir, normalize(_RimLightDirection.xyz)));
  				RimShade = pow(RimShade, p) * (roughness+1);
  				half3 RimLight = RimShadow * _RimLightColor * RimShade;
  				RimLight *= max(0, dot(GeoNormal, normalize(_RimLightDirection.xyz)))*2;

                // fx light
                half3 toFxLight = _FxLightPosition - i.worldPos;
                half FxAttenuation = dot(toFxLight, toFxLight);
                half3 FxLight = saturate(dot(Nn, normalize(toFxLight))) * _FxLightColor / (dot(toFxLight, toFxLight));

                //
 				// FINAL SHADE
 				//
                c.rgb = (env + KeyLight + RimLight + FxLight) * baseColor * _Tint;
                c.rgb += 10.0 * emission * baseColor * _Emission;

                half Kf = fresnel(worldRefl, Nn, 1.2);
                //c.rgb = lerp(c.rgb, c.rgb*0.5+SampleEnvironment(worldRefl, 0.0), Kf * i.occlusion * _Rim);
                c.rgb = lerp(c.rgb, c.rgb*0.5+0.5*lerp(_FloorColor, _AmbientColor, smoothstep(-0.6, 0.6, worldRefl.y)), Kf * i.occlusion * _Rim);


				//
				// FOG
				//
               	//UNITY_APPLY_FOG(i.fogCoord, c);
              	c.rgb = lerp(unity_FogColor, c.rgb, saturate(i.fogFactor));

              	// FX
              	c.rgb += _FX * 2;

                //
                // FINAL GRADE
                //
                c = sqrt(c);

                // grade
	            c.rgb *= _Gain;
				c.rgb += _Offset;

				// Vignette
                fixed4 viewCoord = i.pos/_ScreenParams;
                //viewCoord.y = lerp(1-viewCoord.y, viewCoord.y, step(0, _ProjectionParams.x));
                viewCoord -= 0.5;
                fixed mask = viewCoord.x*viewCoord.x + viewCoord.y*viewCoord.y;
                mask *= 2.2;
                c.rgb = lerp(c.rgb, c.rgb*c.rgb*0.5, mask*mask);

                // Saturation
                fixed3 lum = 0.213 * c.r + 0.715 * c.g + 0.072 * c.b;
                fixed3 dif = c.rgb - lum;
                c.rgb = lum + dif * _Saturation;

                //c.rgb = mask;
                //mask = 1-viewCoord.y;
                //mask *= mask;
                //c.rgb += mask * _LensGlow;//half3(0.1,0.11,0.12);
                //c.rgb = baseColor * (KeyShadow * 0.4 + 0.6); // juggernaut wars

                return c;
            }

			ENDCG
		}

	}
}
