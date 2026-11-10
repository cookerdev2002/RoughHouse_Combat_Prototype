Shader "RoughHouse/FlatColor"
{
    Properties { _Color ("Base Color", Color) = (1,1,1,1) }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            ZWrite On
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            fixed4 _Color;
            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f { float4 pos : SV_POSITION; float3 worldNormal : TEXCOORD0; };
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float3 n=normalize(i.worldNormal);
                float light=0.40 + 0.60*max(0,dot(n,normalize(float3(0.55,0.85,-0.5))));
                return fixed4(_Color.rgb*light,_Color.a);
            }
            ENDCG
        }
    }
    Fallback Off
}
