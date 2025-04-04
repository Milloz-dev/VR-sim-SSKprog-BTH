Shader "Custom/ShadowManBlurSemiTransparent"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 0.75) // Mostly visible, slightly transparent
        _BlurSize ("Blur Size", Range(0, 0.01)) = 0.002
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Cull Off
        ZWrite Off
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
                float alpha = _Color.a;

                // Fake soft blur by layering the color with reduced opacity
                float4 baseColor = float4(_Color.rgb, alpha * 0.5);
                float4 blurColor = baseColor;

                blurColor += float4(_Color.rgb, alpha * 0.3);
                blurColor += float4(_Color.rgb, alpha * 0.2);
                blurColor += float4(_Color.rgb, alpha * 0.15);
                blurColor += float4(_Color.rgb, alpha * 0.1);
                blurColor += float4(_Color.rgb, alpha * 0.05);
                blurColor += float4(_Color.rgb, alpha * 0.4);
                blurColor += float4(_Color.rgb, alpha * 0.25);
                blurColor += float4(_Color.rgb, alpha * 0.35);
                blurColor += float4(_Color.rgb, alpha * 0.17);
                blurColor += float4(_Color.rgb, alpha * 0.12);

                return blurColor / 10;
            }
            ENDCG
        }
    }
}