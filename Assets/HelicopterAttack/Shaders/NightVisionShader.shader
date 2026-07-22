Shader "Custom/NightVisionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LuminanceAmount ("Luminance Amount", Range(0, 1)) = 0.0
        _Contrast ("Contrast", Range(0.5, 2.0)) = 1.1
        _Brightness ("Brightness Boost", Range(1.0, 3.0)) = 1.6
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _LuminanceAmount;
            float _Contrast;
            float _Brightness;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Convert RGB to Grayscale
                float gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

                // Lift shadows and boost dark scene brightness (Night Cam Gain)
                gray = pow(max(0.001, gray), 0.48) * _Brightness;
                gray = saturate((gray - 0.35) * _Contrast + 0.35 + 0.12);

                // Clear monochrome security camera tint
                float3 grayscaleColor = float3(gray * 0.95, gray * 0.98, gray * 1.0);

                // Smoothly blend between full color (0) and grayscale (1)
                float3 finalColor = lerp(col.rgb, grayscaleColor, _LuminanceAmount);

                return fixed4(finalColor, col.a);
            }
            ENDCG
        }
    }
}
