Shader "Unlit/Backface"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest off
        Cull front

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.vertex.z;
            }
            ENDCG
        }
    }
}