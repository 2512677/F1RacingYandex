Shader "MRFusion/RedBlueBlinkTransparent"
{
    Properties
    {
        _ColorRed    ("Red Color",  Color) = (1, 0, 0, 1)
        _ColorBlue   ("Blue Color", Color) = (0, 0, 1, 1)
        _Frequency   ("Blink Frequency", Range(0.1, 10)) = 1.0

        _MainTex     ("Texture (необязательно)", 2D) = "white" {}
        _UseTexture  ("Use Texture", Float) = 0
    }

    SubShader
    {
        // **ВАЖНО**: указываем, что это прозрачный шейдер
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // Отменяем запись в Z-буфер, чтобы блендинг по альфа-каналу работал как надо
        ZWrite Off
        // Обычный альфа-блендинг: srcAlpha, 1-srcAlpha
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
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv     : TEXCOORD0;
                float4 pos    : SV_POSITION;
            };

            // Исходные параметры из Properties
            fixed4 _ColorRed;
            fixed4 _ColorBlue;
            float  _Frequency;
            float  _UseTexture;
            sampler2D _MainTex;
            float4    _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Получаем значение синуса от времени (в Play Mode _Time.y растёт)
                float raw = sin (_Time.y * _Frequency);

                // Резкое переключение: синус >0 → синий, иначе красный.
                fixed4 blinkColor = raw > 0.0 ? _ColorBlue : _ColorRed;

                if (_UseTexture > 0.5)
                {
                    // Берём цвет из текстуры (с альфой)
                    fixed4 texCol = tex2D(_MainTex, i.uv);

                    // Итоговый RGB — это RGB текстуры умноженный на RGB blinkColor
                    // Итоговая альфа — альфа текстуры умноженная на альфу blinkColor (обычно blinkColor.a=1)
                    fixed3 finalRGB   = texCol.rgb * blinkColor.rgb;
                    fixed  finalAlpha = texCol.a * blinkColor.a;

                    return fixed4(finalRGB, finalAlpha);
                }
                else
                {
                    // Если текстуры нет, просто рисуем чистый blinkColor (с opaque альфой = 1)
                    return blinkColor;
                }
            }
            ENDCG
        }
    }

    // В случае ошибки падаем на Diffuse (но он не будет прозрачным)
    FallBack "Diffuse"
}
