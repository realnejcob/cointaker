Shader "Custom/UI/ShadowCaster" {
    Properties{
        _Color("Shadow Color", Color) = (1,1,1,1)
        _ShadowInt("Shadow Intensity", Range(0,1)) = 1.0
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
        SubShader{
            Tags
            {
                "Queue" = "AlphaTest"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
            }
            LOD 200
            ZWrite Off
            Blend zero SrcColor
        CGPROGRAM
        //        #pragma surface surf ShadowCaster alphatest:_Cutoff noambient approxview halfasview novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd nolppv noshadowmask
                #pragma surface surf ShadowCaster alphatest:_Cutoff NoLighting noambient
                fixed4 _Color;
                float _ShadowInt;
                struct Input {
                    float2 uv_MainTex;
                };
                inline fixed4 LightingShadowCaster(SurfaceOutput s, fixed3 lightDir, fixed atten)
                {
                    fixed4 c;
                    c.rgb = lerp(s.Albedo, float3(1.0,1.0,1.0), atten);
                    c.a = 1.0 - atten;
                    return c;
                }
                void surf(Input IN, inout SurfaceOutput o) {
                    o.Albedo = lerp(float3(1.0,1.0,1.0), _Color.rgb, _ShadowInt);
                    o.Alpha = 1.0;
                }
                ENDCG
    }
        Fallback "Transparent/Cutout/VertexLit"
}