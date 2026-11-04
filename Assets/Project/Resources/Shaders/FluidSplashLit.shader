Shader "Universal Render Pipeline/Custom/FluidSplashLit"
{
    Properties
    {
        [Header(Liquid Color and Transparency)]
        [MainColor] _BaseColor ("Base Color (Shallow)", Color) = (0.08, 0.65, 0.85, 1.0)
        _DeepColor ("Deep Color (Fresnel)", Color) = (0.02, 0.22, 0.45, 1.0)
        _Opacity ("Base Opacity (Alpha)", Range(0.0, 1.0)) = 0.55
        _FresnelOpacity ("Fresnel Opacity", Range(0.0, 1.0)) = 0.9
        _FresnelPower ("Fresnel Power", Range(0.5, 8.0)) = 3.5

        [Header(PBR Lit Properties)]
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.92
        _Metallic ("Metallic", Range(0.0, 1.0)) = 0.05
        _NormalStrength ("Wave Normal Strength", Range(0.1, 5.0)) = 1.5
        _CrestColor ("Crest Highlight Color", Color) = (0.8, 0.95, 1.0, 1.0)
        _CrestIntensity ("Crest Highlight Intensity", Range(0.0, 1.0)) = 0.25

        [Header(Splash Wave Customization)]
        _RingCount ("Number of Rings", Range(1.0, 8.0)) = 1.0
        _RingThickness ("Single Ring Width", Float) = 0.6
        _TrailingFalloff ("Trailing Rings Falloff", Range(0.0, 5.0)) = 1.5
        _WaveSpeed ("Wave Speed", Float) = 3.5
        _WaveAmplitude ("Wave Amplitude", Float) = 0.15
        _TimeDecay ("Time Decay Rate", Float) = 1.2

        [HideInInspector] _HitPosition ("Hit Position", Vector) = (0, 0, 0, 0)
        [HideInInspector] _HitTime ("Hit Time", Float) = -999.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "UniversalMaterialType" = "Lit"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
            #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS     : SV_POSITION;
                float3 positionWS     : TEXCOORD0;
                float3 viewDirWS      : TEXCOORD1;
                half   fogFactor      : TEXCOORD2;
                half3  vertexLighting : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _DeepColor;
                float4 _CrestColor;
                float _Opacity;
                float _FresnelOpacity;
                float _FresnelPower;
                float _Smoothness;
                float _Metallic;
                float _NormalStrength;
                float _CrestIntensity;

                float4 _HitPosition;
                float _HitTime;
                float _RingCount;
                float _RingThickness;
                float _TrailingFalloff;
                float _WaveSpeed;
                float _WaveAmplitude;
                float _TimeDecay;
            CBUFFER_END

            // Calculates a strictly bounded number of splash rings and their surface gradient
            float CalculateSplashWave(float2 worldXZ, out float2 waveGradient)
            {
                waveGradient = float2(0.0, 0.0);
                float elapsed = _Time.y - _HitTime;

                if (elapsed <= 0.0 || elapsed > 10.0) return 0.0;

                float2 dir = worldXZ - _HitPosition.xz;
                float dist = length(dir);
                float2 normDir = dist > 0.0001 ? dir / dist : float2(0.0, 0.0);

                // Leading edge of the expanding splash wave
                float frontRadius = elapsed * _WaveSpeed;
                float totalPacketLength = max(_RingCount * _RingThickness, 0.001);

                // Normalized position inside the wave train (0 = leading edge, 1 = end of last ring)
                float u = (frontRadius - dist) / totalPacketLength;

                // Cut off everything outside the exact number of rings
                if (u <= 0.0 || u >= 1.0) return 0.0;

                // Smooth Hann window (sin^2) so the first and last rings start and end smoothly without sharp edges
                float sinWindow = sin(u * PI);
                float window = sinWindow * sinWindow;

                // Attenuate trailing inner rings relative to the first outer ring
                float trailAtten = exp(-u * _TrailingFalloff);

                // Smooth out the exact impact center while rings are emerging
                float centerFade = smoothstep(0.0, _RingThickness * 0.5, dist);

                // Overall time decay
                float timeAtten = exp(-elapsed * _TimeDecay) * saturate(elapsed * 15.0);

                float phase = u * _RingCount * TWO_PI;
                float baseEnv = _WaveAmplitude * trailAtten * centerFade * timeAtten;

                float height = sin(phase) * window * baseEnv;

                // Analytical derivative with respect to distance for accurate PBR normal perturbation
                float dWindow_du = PI * sin(2.0 * u * PI);
                float dOsc_du = _RingCount * TWO_PI * cos(phase);
                float dHeight_dDist = -(baseEnv / totalPacketLength) * (dOsc_du * window + sin(phase) * dWindow_du);

                waveGradient = normDir * dHeight_dDist;
                return height;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                float2 grad;
                float waveHeight = CalculateSplashWave(positionWS.xz, grad);

                positionWS.y += waveHeight;

                float3 normalWS = normalize(float3(-grad.x * _NormalStrength, 1.0, -grad.y * _NormalStrength));

                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.positionWS = positionWS;
                OUT.viewDirWS = GetWorldSpaceViewDir(positionWS);
                OUT.fogFactor = ComputeFogFactor(OUT.positionCS.z);
                OUT.vertexLighting = VertexLighting(positionWS, normalWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 grad;
                float waveHeight = CalculateSplashWave(IN.positionWS.xz, grad);

                half3 normalWS = normalize(half3(-grad.x * _NormalStrength, 1.0, -grad.y * _NormalStrength));
                half3 viewDirWS = SafeNormalize(IN.viewDirWS);

                half fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                half3 waterAlbedo = lerp(_BaseColor.rgb, _DeepColor.rgb, fresnel);
                half waterAlpha = lerp(_Opacity, _FresnelOpacity, fresnel);

                half crestFactor = saturate(waveHeight / (_WaveAmplitude + 0.0001)) * _CrestIntensity;

                // 1. URP SurfaceData
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = waterAlbedo;
                surfaceData.metallic = _Metallic;
                surfaceData.specular = half3(0.0h, 0.0h, 0.0h);
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = half3(0.0h, 0.0h, 1.0h);
                surfaceData.occlusion = 1.0h;
                surfaceData.emission = _CrestColor.rgb * crestFactor;
                surfaceData.alpha = saturate(waterAlpha + crestFactor);

                // 2. URP InputData
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = viewDirWS;
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                inputData.fogCoord = IN.fogFactor;
                inputData.vertexLighting = IN.vertexLighting;
                inputData.bakedGI = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);
                inputData.shadowMask = half4(1.0h, 1.0h, 1.0h, 1.0h);

                // 3. PBR Lighting
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);

                return color;
            }
            ENDHLSL
        }
    }
}