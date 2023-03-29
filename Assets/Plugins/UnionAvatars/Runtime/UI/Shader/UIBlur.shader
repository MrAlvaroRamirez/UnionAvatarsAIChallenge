Shader "Custom/MaskedUIBlur" {
    Properties {
        _Size ("Blur", Range(0, 30)) = 1
        [HideInInspector] _MainTex ("Masking Texture", 2D) = "white" {}
        _AdditiveColor ("Additive Tint color", Color) = (0, 0, 0, 0)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    Category {

        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }


        SubShader
        {
            // Horizontal blur
            GrabPass
            {
                "_HBlur"
            }
            /*
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha
            */

            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp] 
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }

            ColorMask [_ColorMask]
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest [unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {          
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvmain : TEXCOORD1;
                    float4 worldPosition : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _ClipRect;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.worldPosition = v.vertex;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif

                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;

                    o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
                    return o;
                }

                sampler2D _HBlur;
                float4 _HBlur_TexelSize;
                float _Size;
                float4 _AdditiveColor;
                float4 _MultiplyColor;

                half4 frag( v2f i ) : COLOR
                {   
                    half4 sum = half4(0,0,0,0);

                    #define GRABPIXEL(weight,kernelx) tex2Dproj( _HBlur, UNITY_PROJ_COORD(float4(i.uvgrab.x + _HBlur_TexelSize.x * kernelx * _Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
                    #define GRABPIXEL2(weight,kernely) tex2Dproj( _HBlur, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _HBlur_TexelSize.y * kernely * _Size, i.uvgrab.z, i.uvgrab.w))) * weight

                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);

                    half4 result = half4((sum.r + _AdditiveColor.r), 
                                        (sum.g + _AdditiveColor.g), 
                                        (sum.b + _AdditiveColor.b), 
                                        tex2D(_MainTex, i.uvmain).a);

                    #ifdef UNITY_UI_CLIP_RECT
                        result.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    #endif

                    return result;
                }
                ENDCG
            }

            // Vertical blur
            GrabPass
            {
                "_VBlur"
            }

            Pass
            {          
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #include "UnityCG.cginc"
                #include "UnityUI.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvmain : TEXCOORD1;
                    float4 worldPosition : TEXCOORD2;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _ClipRect;

                v2f vert (appdata_t v) {
                    v2f o;
                    o.worldPosition = v.vertex;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif

                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;

                    o.uvmain = TRANSFORM_TEX(v.texcoord, _MainTex);
    
                    return o;
                }

                sampler2D _VBlur;
                float4 _VBlur_TexelSize;
                float _Size;
                float4 _AdditiveColor;
                float4 _MultiplyColor;

                half4 frag( v2f i ) : COLOR
                {
                    half4 sum = half4(0,0,0,0);

                    #define GRABPIXEL(weight,kernely) tex2Dproj( _VBlur, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _VBlur_TexelSize.y * kernely * _Size, i.uvgrab.z, i.uvgrab.w))) * weight

                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);

                    half4 result = half4((sum.r + _AdditiveColor.r), 
                                        (sum.g + _AdditiveColor.g), 
                                        (sum.b + _AdditiveColor.b), 
                                        tex2D(_MainTex, i.uvmain).a);

                    #ifdef UNITY_UI_CLIP_RECT
                        result.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    #endif

                    return result;
                }
                ENDCG
            }
        }
    }
}