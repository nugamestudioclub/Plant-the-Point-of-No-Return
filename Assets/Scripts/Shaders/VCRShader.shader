Shader "Custom/VCRUIShader"
{
    //Developed from this tutorial and modifed to work as a UI shader: https://gamedev.center/how-to-make-a-retro-vhs-effect-shader-in-unity/
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0,4)) = 0.5
        _NoiseIntensity ("Noise Intensity", Range(0,2)) = 0.2
        _ColorBleed ("Color Bleed", Range(0,10)) = 0.02
        _Distortion ("Distortion", Range(0,10)) = 0.05
        _WobbleFrequency ("Wobble Frequency", Range(0,110)) = 40
        _WobbleSpeed ("Wobble Speed", Range(0,20)) = 1
        _TimeParam ("Time Override", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8

        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
       
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }


        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"



            #pragma shader_feature_local UNITY_UI_ALPHACLIP


            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv       : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv       : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 mask : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _UIMaskSoftnessX;
            
            float _ScanlineIntensity;
            float _NoiseIntensity;
            float _ColorBleed;
            float _Distortion;
            float _WobbleFrequency;
            float _WobbleSpeed;
            float _TimeParam;

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.mask = float4(
                    v.vertex.xy,
                    0,
                    0
                );
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                
                float time = (_TimeParam > 0.0) ? _TimeParam : _Time.y;
                
                float2 uv = i.uv;

               
                float wobble = sin(uv.y * _WobbleFrequency + time * _WobbleSpeed) * _Distortion * 0.02;
                float2 distortedUV = uv + float2(wobble, 0);
                
               
                distortedUV = clamp(distortedUV, 0.0, 1.0);

                
                fixed4 col = (tex2D(_MainTex, uv) + _TextureSampleAdd);
                fixed4 colDistorted = (tex2D(_MainTex, distortedUV) + _TextureSampleAdd);
                
                col.rgb = lerp(col.rgb, colDistorted.rgb, _Distortion);

               
                float scanline = sin(uv.y * _ScreenParams.y * 3.14159);
                float scanlineMask = lerp(1.0, 0.7, (scanline * 0.5 + 0.5) * _ScanlineIntensity);
                col.rgb *= scanlineMask;

                
                float gray = dot(col.rgb, float3(0.3, 0.59, 0.11));
                col.rgb = lerp(col.rgb, float3(gray, gray, gray), 0.15);
                col.rgb = lerp(col.rgb, float3(0.9, 0.95, 1.1), 0.08);

                
                float brightness = dot(col.rgb, float3(0.3, 0.59, 0.11));
                float blendAmount = _ColorBleed * smoothstep(0.05, 0.2, brightness);
                float2 offset = float2(_ColorBleed / _ScreenParams.x, 0);
                
                float r = (tex2D(_MainTex, uv + offset) + _TextureSampleAdd).r;
                float g = col.g;
                float b = (tex2D(_MainTex, uv - offset) + _TextureSampleAdd).b;
                
                float3 aberrated = float3(r, g, b);
                col.rgb = lerp(col.rgb, aberrated, blendAmount);

               
                float noise = (rand(uv * time * 0.5 + time) - 0.5) * _NoiseIntensity;
                col.rgb += noise;
                
                
                col *= i.color;

                
               #ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
               #endif

                return col;
            }
            ENDCG
        }
    }
}
