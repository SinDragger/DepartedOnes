Shader "Custom/ScreenCircle"
{
    Properties
    {
        _MainTex ("MainTexture", 2D) = "white" {}
        _BackGroundColor ("BackGroundColor", Color) = (0,0,0,0.5)
        _CircleColor ("CircleColor", Color) = (1,1,1,0)
        _CircleRadius("CircleRadius", float) = 3
        _CircleX("CircleX", float) = 0
        _CircleY("CircleY", float) = 0
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

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
            

            fixed4 _BackGroundColor;
            fixed4 _CircleColor;
            float _CircleRadius;
            float _CircleX;
            float _CircleY;

            fixed4 frag (v2f i) : SV_Target
            {
                float radius = (i.vertex.x - _CircleX) * (i.vertex.x - _CircleX) + (i.vertex.y - _CircleY) * (i.vertex.y - _CircleY);
                if(radius <= _CircleRadius * _CircleRadius)
                    return _CircleColor;
                return _BackGroundColor;
            }
            ENDCG
        }
    }
}
