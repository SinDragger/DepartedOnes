Shader "Hidden/NewImageEffectShader"
{
    Properties
    {
        _BurnMap ("BurnMap", 2D) = "white" {}
        _OffsetXY ("OffsetXY", vector) = (0, 0, 0, 0)
        _R ("_R", float) = 6
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _BurnMap;
            float4 OffsetXY;
            float _R;

            fixed4 frag (v2f i) : SV_Target
            {
                float noise = tex2D(_BurnMap, i.uv + OffsetXY.xy).a;
                float r2 = sqrt((i.vertex.x - OffsetXY.z) * (i.vertex.x - OffsetXY.z) + (i.vertex.y - OffsetXY.w) * (i.vertex.y - OffsetXY.w));

                float alpha = lerp(1, 0, (r2 / _R) * noise * 2);
                return fixed4(1, 1, 1, alpha);
            }
            ENDCG
        }
    }
}
