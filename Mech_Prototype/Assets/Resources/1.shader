Shader "Custom/1" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_BumpMap("Normal Map", 2D) = "bump" {}
		_SpecularMap("Specular Map", 2D) = "black" {}
		_EmissionColor("Emission", Color) = (0,0,0,0)
		_EmissionMap("Emission Map", 2D) = "black" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform sampler2D _SpecularMap;
			uniform sampler2D _EmissionMap;

		struct Input {
		float2 uv_MainTex;
		float2 uv_EmissionMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		
		struct v2f {
		float4 pos 		: SV_POSITION;
                float3 worldPos 	: TEXCOORD0;
                half3 tspace0 		: TEXCOORD1;
                half3 tspace1 		: TEXCOORD2;
                half3 tspace2 		: TEXCOORD3;
                float2 uv 		: TEXCOORD4;
       		float4 ShMapUV	        : TEXCOORD5;
		float dpth  		: TEXCOORD6;
       		float4 ShMapRimUV	: TEXCOORD7;
		float dpthRim  		: TEXCOORD8;
		fixed occlusion 	: TEXCOORD9;
		fixed fogFactor 	: TEXCOORD10;
                UNITY_FOG_COORDS(11)
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
				o.dpth = ( o.ShMapUV.z / o.ShMapUV.w );								// vertex distance from light
				//o.ShMapRimUV = mul( _RimShadowProjectionMatrix, Pworld );			// ShMapUV.xy - shadowmap UV
				//o.dpthRim = ( o.ShMapRimUV.z / o.ShMapRimUV.w );					// vertex distance from light

				o.occlusion = vcolor.r;// * 0.7 + 0.3;

                return o;
            }
				UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;			
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
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
                // TEXTURES
                //
                half3 baseColor = tex2D(_MainTex, i.uv).rgb * 2;
                half roughness = tex2D(_SpecularMap, i.uv).r;
                roughness = roughness*roughness;
                half emission = tex2D(_EmissionMap, i.uv).r;
		//
 				                                return c;
            }

		ENDCG
	}
	FallBack "Diffuse"
}
