Shader "Custom/ShadowManShader"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 0.2) // Default black color with 50% transparency
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }

            // Enable alpha blending for transparency
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha   // Proper alpha blending
            Cull Front                       // Cull front faces for shadow effect

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : POSITION;
            };

            float4 _Color; // Color with alpha

            // Vertex shader
            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.position); // Transform object space to clip space
                return o;
            }

            // Fragment shader
            half4 frag(Varyings i) : SV_Target
            {
                return _Color;  // Return black with transparency based on _Color
            }

            ENDHLSL
        }
    }

    Fallback "Unlit/Color"
}