Shader "TextMeshPro/URP_Text_Light&Shadows"
{
    Properties
    {
        [MainTexture] _MainTex ("Font Atlas (TMP)", 2D) = "white" {}
        [MainColor] _BaseColor ("Tint Color", Color) = (1, 1, 1, 1)
        _WeightThreshold ("Thickness Threshold (SDF)", Range(0.1, 0.9)) = 0.5
        _EdgeSmoothing ("Edge Smoothing", Range(0.001, 0.2)) = 0.05
        _GradientScale ("TMP Gradient Scale", Float) = 5.0
        
        [Toggle] _UseLighting ("Use Lighting (Lit)", Float) = 1.0
        [Toggle] _ReceiveShadows ("Receive Shadows", Float) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "TransparentCutout" 
            "Queue" = "AlphaTest" 
        }

        Pass
        {
            Name "LitText"
            Tags { "LightMode" = "UniversalForward" }

            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 vertexColor : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float fogFactor : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _BaseColor;
                float _WeightThreshold;
                float _EdgeSmoothing;
                float _GradientScale;
            CBUFFER_END

            // MUST BE OUTSIDE CBUFFER FOR MATERIALPROPERTYBLOCK TO WORK:
            float _UseLighting;
            float _ReceiveShadows;

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normInputs.normalWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.vertexColor * _BaseColor;
                output.fogFactor = ComputeFogFactor(posInputs.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half sdfDistance = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
                clip(sdfDistance - (_WeightThreshold - _EdgeSmoothing));

                half alpha = smoothstep(_WeightThreshold - _EdgeSmoothing, _WeightThreshold + _EdgeSmoothing, sdfDistance);
                alpha *= input.color.a;

                // UNLIT MODE
                if (_UseLighting < 0.5)
                {
                    half3 unlitColor = MixFog(input.color.rgb, input.fogFactor);
                    return half4(unlitColor, alpha);
                }

                // LIT MODE
                float3 normalWS = normalize(input.normalWS);
                float3 totalLight = SampleSH(normalWS);

                float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                Light mainLight = GetMainLight(shadowCoord);
                
                half mainShadow = (_ReceiveShadows > 0.5) ? mainLight.shadowAttenuation : 1.0;
                float mainNdotL = saturate(dot(normalWS, mainLight.direction));
                totalLight += mainLight.color * (mainNdotL * mainLight.distanceAttenuation * mainShadow);

                #ifdef _ADDITIONAL_LIGHTS
                uint lightCount = GetAdditionalLightsCount();
                for (uint i = 0u; i < lightCount; ++i)
                {
                    Light addLight = GetAdditionalLight(i, input.positionWS, half4(1, 1, 1, 1));
                    half addShadow = (_ReceiveShadows > 0.5) ? addLight.shadowAttenuation : 1.0;
                    float addNdotL = saturate(dot(normalWS, addLight.direction));
                    totalLight += addLight.color * (addNdotL * addLight.distanceAttenuation * addShadow);
                }
                #endif

                half3 finalColor = input.color.rgb * totalLight;
                finalColor = MixFog(finalColor, input.fogFactor);

                return half4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}