Shader "Wacky612/PLS2/LogoModifier"
{
    Properties
    {
        [PerRendererData]
        _MainTex          ("Sprite Texture",     2D   ) = "white" {}

        _Color            ("Tint",               Color) = (1, 1, 1, 1)

        _StencilComp      ("Stencil Comparison", Float) = 8
        _Stencil          ("Stencil ID",         Float) = 0
        _StencilOp        ("Stencil Operation",  Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask  ("Stencil Read Mask",  Float) = 255

        _ColorMask        ("Color Mask",         Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)]
        _UseUIAlphaClip   ("Use Alpha Clip",     Float) = 0

        _GrayScaleXmin    ("GrayScale Xmin",     Range(0, 1)) = 0.35
        _GrayScaleXmax    ("GrayScale Xmax",     Range(0, 1)) = 0.35
        _InvertXmin       ("Invert Xmin",        Range(0, 1)) = 0.35
        _InvertXmax       ("Invert Xmax",        Range(0, 1)) = 0.35
        _ColorXmin        ("Color Xmin",         Range(0, 1)) = 0.35
        _ColorXmax        ("Color Xmax",         Range(0, 1)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "Queue"             = "Transparent"
            "IgnoreProjector"   = "True"
            "RenderType"        = "Transparent"
            "PreviewType"       = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref       [_Stencil]
            Comp      [_StencilComp]
            Pass      [_StencilOp]
            ReadMask  [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull      Off
        Lighting  Off
        ZWrite    Off
        ZTest     [unity_GUIZTestMode]
        Blend     SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target   2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float  _GrayScale;
            float  _GrayScaleXmin;
            float  _GrayScaleXmax;
            float  _InvertXmin;
            float  _InvertXmax;
            float  _ColorXmin;
            float  _ColorXmax;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color;
                return OUT;
            }

            float test_grayscale(float2 pos)
            {
                return pos.x > _GrayScaleXmin && pos.x < _GrayScaleXmax;
            }

            float test_invert(float2 pos)
            {
                return pos.x > _InvertXmin && pos.x < _InvertXmax;
            }

            float test_color(float2 pos)
            {
                return pos.x > _ColorXmin && pos.x < _ColorXmax;
            }

            half4 grayscale(half4 color)
            {
                half v = dot(color, half3(0.299, 0.587, 0.114));
                return half4(v, v, v, color.a);
            }

            half4 invert(half4 color)
            {
                return half4(1 - color.rgb, color.a);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                color =  test_grayscale(IN.texcoord) ? grayscale(color) : color;
                color =  test_invert(IN.texcoord)    ? invert(color)    : color;
                color *= test_color(IN.texcoord)     ? _Color           : 1;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
