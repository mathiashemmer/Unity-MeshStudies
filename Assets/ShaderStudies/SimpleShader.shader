Shader "Unlit/SimpleShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragg

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv0 : TEXCOORD0;
            };
            
            struct VertexOutput {
                float4 vertex : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 position_in_world_space : TEXCOORD2;
            };

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;

                float3 worldVertexPos = mul(unity_ObjectToWorld, v.vertex);
                float3 displacedWorldVertexPos = worldVertexPos + sin(_Time) * v.normal;
                float3 newLocalPosition = mul(unity_WorldToObject, displacedWorldVertexPos);
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.position_in_world_space = float4(worldVertexPos.xyz,0);
                o.uv0 = v.uv0;
                o.normal = v.normal;
                return o;
            }

            float4 fragg(VertexOutput o) : SV_Target
            {

                return float4(o.position_in_world_space.xyz,0);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightCol = _LightColor0.rgb;

                float lightFalloff = max(0, dot(lightDir, o.normal));
                float difuse = lightCol * lightFalloff;

                float3 ambient = (.1, .1, .1);
                return float4(ambient + difuse, 0);
            }
            ENDCG
        }
    }
}
