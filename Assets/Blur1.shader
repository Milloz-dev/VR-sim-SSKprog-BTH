Shader "Custom/ShadowManBlurSemiTransparent_DepthAPI_URP"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 0.75)
        _BlurSize ("Blur Size", Range(0, 0.01)) = 0.002
        _EnvironmentDepthBias("Environment Depth Bias", Float) = 0.06
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ShadowManURP"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION

            // Core URP and Meta Depth API includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.meta.xr.sdk.core/Shaders/EnvironmentDepth/URP/EnvironmentOcclusionURP.hlsl"

            float4 _Color;
            float _BlurSize;
            float _EnvironmentDepthBias;

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;

                META_DEPTH_VERTEX_OUTPUT(0)     // Uses TEXCOORD0
                float3 worldPos : TEXCOORD1;    // Uses TEXCOORD1 — no conflict

                UNITY_VERTEX_OUTPUT_STEREO
            };
            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.worldPos = worldPos;

                OUT.positionHCS = TransformWorldToHClip(worldPos);
                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(OUT, IN.positionOS);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                // Ghost color blend
                float alpha = _Color.a;
                half4 fragmentShaderResult = float4(_Color.rgb, alpha * 0.5);

                fragmentShaderResult += float4(_Color.rgb, alpha * 0.3);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.2);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.15);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.1);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.05);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.4);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.25);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.35);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.17);
                fragmentShaderResult += float4(_Color.rgb, alpha * 0.12);
                fragmentShaderResult /= 10;

                // Final occlusion check (world position variant)
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY_WORLDPOS(IN.worldPos, fragmentShaderResult, _EnvironmentDepthBias);

                return fragmentShaderResult;
            }

            ENDHLSL
        }
    }
}