Shader "Custom/BrushShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BrushColor ("Brush Color", Color) = (1,1,1,1)
        _BrushSize ("Brush Size", Float) = 0.05
        _UVCoord ("UV Coord", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            float4 _UVCoord;
            float _BrushSize;
            float4 _BrushColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _UVCoord.xy);
                if (dist < _BrushSize)
                {
                    return _BrushColor;
                }
                else
                {
                    return half4(0, 0, 0, 0); // Transparent outside brush area
                }
            }
            ENDCG
        }
    }
}
