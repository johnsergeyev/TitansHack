Shader "ShadersLabs/Universal"
{
    Properties
    {	
	
        _MainColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _NormalMap("Normal", 2D) = "bump" {}
	_EmissionColor("Emission Color", Color) = (1,1,1,1)
        _EmissionMap("Emission Map", 2D) = "black" {}
	_MetallicMap ("Metallic Map", 2D) = "white" {}
	_Roughness("Roughness", Range(0,1)) = 0.1
        _ReflectionPower("Reflection Power", Range(0.01, 5)) = 3
        _Metallic ("Metallic", Range(0,1)) = 0.0
	_MetallicG ("Metallic Global", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
       
        CGPROGRAM
        #pragma surface surf Universal fullforwardshadows exclude_path:prepass exclude_path:deferred
        #pragma target 3.0
 
        struct Input
        {
            half2 uv_MainTex;

        };
       
        struct SurfaceOutputUniversal
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            fixed Specular;
            fixed Metallic;
            fixed Roughness;
            fixed ReflectionPower;
            fixed Alpha;
        };
       
        sampler2D _MainTex;
        sampler2D _NormalMap;
        sampler2D _SpecularMap;
        sampler2D _EmissionMap;
	sampler2D _MetallicMap;
	fixed4 _EmissionColor;
        fixed4 _MainColor;
        fixed _Roughness;
        fixed _ReflectionPower;
        fixed _Metallic;
	fixed _MetallicG;
       
        inline fixed3 CalculateCookTorrance(SurfaceOutputUniversal s, half3 n, fixed vdn, half3 viewDir, UnityLight light)
        {
            half3 h = normalize(light.dir + viewDir);
            fixed ndl = saturate(dot(n, light.dir));
            fixed ndh = saturate(dot(n, h));
            fixed vdh = saturate(dot(viewDir, h));
            fixed ndh2 = ndh * ndh;
            fixed sp2 = max(s.Roughness * s.Roughness, 0.001);
           
            fixed G = min(1.0, 2.0 * ndh * min(vdn, ndl) / vdh);
            fixed D = exp((ndh2 - 1.0)/(sp2 * ndh2)) / (4.0 * sp2 * ndh2 * ndh2);
            fixed F = 0.5 + 0.5 * pow(1.0 - vdh, s.ReflectionPower);
            fixed spec = saturate(G * D * F / (vdn * ndl));
           
            return light.color * (s.Albedo * ndl + fixed3(s.Specular, s.Specular, s.Specular) * spec);
        }
       
        inline fixed3 CalculateIndirectSpecular(SurfaceOutputUniversal s, fixed vdn, half3 indirectSpec)
        {
            fixed rim = saturate(pow(1.0 - vdn, s.ReflectionPower));
            return indirectSpec * rim * s.Metallic;
        }
       
        inline fixed4 LightingUniversal(SurfaceOutputUniversal s, half3 viewDir, UnityGI gi)
        {
            half3 n = normalize(s.Normal);
            fixed vdn = saturate(dot(viewDir, n));
            fixed4 c = fixed4(CalculateCookTorrance(s, n, vdn, viewDir, gi.light), s.Alpha);
            
            #if defined(DIRLIGHTMAP_SEPARATE)
                #ifdef LIGHTMAP_ON
                    c.rgb += CalculateCookTorrance(s, n, vdn, viewDir, gi.light2);
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    c.rgb += CalculateCookTorrance(s, n, vdn, viewDir, gi.light3);
                #endif
            #endif
           
           	#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
            	c.rgb += (s.Albedo * gi.indirect.diffuse + CalculateIndirectSpecular(s, vdn, gi.indirect.specular));
            #endif
           
            return c;
        }
       
        inline void LightingUniversal_GI(SurfaceOutputUniversal s, UnityGIInput data, inout UnityGI gi)
        {
            gi = UnityGlobalIllumination(data, 1.0 /* occlusion */, 1.0 - s.Roughness, normalize(s.Normal));
        }
       
        void surf(Input IN, inout SurfaceOutputUniversal o)
        {
            fixed4 c = _MainColor * tex2D(_MainTex, IN.uv_MainTex);
	    fixed4 e = _EmissionColor * tex2D(_EmissionMap, IN.uv_MainTex).r;
            o.Albedo = c.rgb;
            o.Normal = normalize(UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex)));
            o.Specular = e.a;
            o.Emission = e.rgb;
	    o.Metallic = _Metallic * tex2D(_MetallicMap, IN.uv_MainTex) + _MetallicG;
            o.Roughness = _Roughness;
            o.ReflectionPower = _ReflectionPower;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}