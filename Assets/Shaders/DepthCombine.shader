Shader "Unlit/DepthCombine"
{
    Properties
    {
        _Backface  ("Backface",  2D) = "black" {}
        _Frontface ("Frontface", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed2 uv : TEXCOORD0;
            };

            sampler2D _Backface;
            sampler2D _Frontface;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed backDistance  = tex2D(_Backface,  i.uv).r;
                fixed frontDistance = tex2D(_Frontface, i.uv).r;
                return fixed4(backDistance, frontDistance, 0, 1);
            }
            ENDCG
        }
    }
}