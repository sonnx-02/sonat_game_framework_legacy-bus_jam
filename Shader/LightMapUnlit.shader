Shader "Unlit/LightMapUnlit"
{
   Properties {
 
    _Color ("Main Color", Color) = (1,1,1,1)
 
    _MainTex ("Base (RGB)", 2D) = "white" {}
 
    _LightMap ("Lightmap (RGB)", 2D) = "black" {}
 
}
 
 
 
SubShader {
 
    LOD 300
 
    Tags { "RenderType" = "Opaque" }
 
CGPROGRAM
 
#pragma surface surf None
 
 
 
half4 LightingNone (SurfaceOutput s, half3 lightDir, half atten)
 
{
 
    half4 c;
 
    c.rgb = s.Albedo*atten;
 
    c.a = s.Alpha;
 
    return c;
 
}
 
     
 
struct Input {
 
  float2 uv_MainTex;
 
  float2 uv2_LightMap;
 
};
 
sampler2D _MainTex;
 
sampler2D _BumpMap;
 
sampler2D _LightMap;
 
fixed4 _Color;
 
void surf (Input IN, inout SurfaceOutput o)
 
{
 
  half4 c;
 
  o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb*_Color;
 
  half4 lm = tex2D (_LightMap, IN.uv2_LightMap);
 
  o.Albedo *= lm.rgb;
 
  o.Alpha = lm.a;
 
}
 
ENDCG
 
}
 
FallBack "Legacy Shaders/Lightmapped/Diffuse"
 

}
