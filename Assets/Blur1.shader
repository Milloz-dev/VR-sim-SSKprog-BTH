Shader "Custom/ShadowManBlur"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _BlurSize ("Blur Size", Range(0, 0.01)) = 0.002
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha

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

            float4 _Color;
            float _BlurSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = _Color * 0.2;
                col += _Color * 0.2;
                col += _Color * 0.2;
                col += _Color * 0.2;
                col += _Color * 0.2;
                return col;
            }
            ENDCG
        }
    }
}