
Shader "Custom/URP/UI/RoundConorNew"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _RoundedRadius ("Rounded Radius", Range(0, 256)) = 16
        _Width ("View Width", Float) = 200
        _Height ("View Height", Float) = 200
        _BorderWidth ("Border Width", Float) = 0
        _BorderColor ("Border Color", Color) = (1,0,0,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip ("Use UI Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "RoundedImage"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 localPos : TEXCOORD1;
                float4 positionOS : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _BorderColor;
                float _RoundedRadius;
                float _Width;
                float _Height;
                float _BorderWidth;
                float4 _ClipRect;
                float4 _TextureSampleAdd;
            CBUFFER_END

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;

                OUT.positionOS = IN.positionOS;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color * _Color;
                OUT.localPos = IN.positionOS.xy;

                return OUT;
            }

            float RoundedBoxDistance(
                float2 p,
                float2 halfSize,
                float radius)
            {
                float2 q = abs(p) - halfSize + radius;

                return length(max(q, 0.0))
                     + min(max(q.x, q.y), 0.0)
                     - radius;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                half4 color =
                    (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv)
                    + _TextureSampleAdd) * IN.color;

                float width = max(_Width, 0.001);
                float height = max(_Height, 0.001);

                // 将 UV 映射到矩形局部坐标
                float2 p = (IN.uv - 0.5) * float2(width, height);
                float2 halfSize = float2(width, height) * 0.5;

                float radius = clamp(
                    _RoundedRadius,
                    0.0,
                    min(width, height) * 0.5
                );

                float borderWidth = clamp(
                    _BorderWidth,
                    0.0,
                    min(width, height) * 0.5
                );

                float distance = RoundedBoxDistance(
                    p, halfSize, radius
                );

                float aa = max(fwidth(distance), 0.001);
                float outerMask = 1.0 - smoothstep(
                    -aa, aa, distance
                );

                color.a *= outerMask;

                if (borderWidth > 0.0)
                {
                    float innerRadius = max(radius - borderWidth, 0.0);
                    float2 innerHalfSize = max(
                        halfSize - borderWidth, 0.0
                    );

                    float innerDistance = RoundedBoxDistance(
                        p, innerHalfSize, innerRadius
                    );

                    float innerMask = 1.0 - smoothstep(
                        -aa, aa, innerDistance
                    );

                    float borderMask = outerMask * (1.0 - innerMask);

                    color.rgb = lerp(
                        color.rgb,
                        _BorderColor.rgb,
                        borderMask * _BorderColor.a
                    );
                }

                #ifdef UNITY_UI_CLIP_RECT
                float2 clipPosition = IN.positionOS.xy;
                float2 clipFactor = saturate(
                    (_ClipRect.zw - _ClipRect.xy - abs(
                        clipPosition * 2.0 - _ClipRect.xy - _ClipRect.zw
                    )) * 0.5
                );
                color.a *= clipFactor.x * clipFactor.y;
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }

            ENDHLSL
        }
    }
}
