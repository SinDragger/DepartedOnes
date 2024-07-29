Shader "Hidden/EdgeDetect"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _EdgeOnly ("EdgeOnly", Range(0, 1)) = 1.0
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform half4 _MainTex_TexelSize;
            fixed _EdgeOnly;
            fixed4 _EdgeColor;
            fixed4 _BackgroundColor;

            struct appdata
            {
                float4 vertex : POSITION;
                half2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[9] : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                half2 uv = v.uv;
                o.uv[0] = uv + _MainTex_TexelSize.xy * half2(-1, -1);
                o.uv[1] = uv + _MainTex_TexelSize.xy * half2(0, -1);
                o.uv[2] = uv + _MainTex_TexelSize.xy * half2(1, -1);
                o.uv[3] = uv + _MainTex_TexelSize.xy * half2(-1, 0);
                o.uv[4] = uv + _MainTex_TexelSize.xy * half2(0, 0);
                o.uv[5] = uv + _MainTex_TexelSize.xy * half2(1, 0);
                o.uv[6] = uv + _MainTex_TexelSize.xy * half2(-1, 1);
                o.uv[7] = uv + _MainTex_TexelSize.xy * half2(0, 1);
                o.uv[8] = uv + _MainTex_TexelSize.xy * half2(1, 1);
                return o;
            }

            fixed luminance(fixed4 c)
            {
                return 0.2125 * c.r + 0.7154 * c.g + 0.0721 * c.b;
            }

            
            half Edge(v2f i)
            {
                //Prewitt
                //const half Gx[9] = {-1, 0, 1, -1, 0, 1, -1, 0, 1};
                //const half Gy[9] = {-1, -1, 1, 0, 0, 0, 1, 1, 1};

                //Sobel
                const half Gx[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
                const half Gy[9] = {-1, 2, 1, 0, 0, 0, 1, 2, 1};

                half lum;
                half Ex = 0;
                half Ey = 0;
                for(int j = 0; j < 9; ++j)
                {
                    lum = luminance(tex2D(_MainTex, i.uv[j]));
                    Ex += lum * Gx[j];
                    Ey += lum * Gy[j];
                }
                half E = 1 - abs(Ex) - abs(Ey);
                return E;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                half edge = Edge(i);
                fixed4 withEdgeColor = lerp(_EdgeColor, tex2D(_MainTex, i.uv[4]), edge);
                fixed4 onlyEdgeColor = lerp(_EdgeColor, _BackgroundColor, edge);
                return lerp(withEdgeColor, onlyEdgeColor, _EdgeOnly);
            }
            ENDCG
        }
    }
}
