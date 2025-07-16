Shader "Custom/DiagonalFade_RightBottomToLeftTop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeProgress ("Fade Progress", Range(0,1)) = 0
        _FadeSharpness ("Fade Sharpness", Range(0.1, 10)) = 2
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _FadeProgress;
            float _FadeSharpness;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // 오른쪽 아래(1,0) → 왼쪽 위(0,1)
                float diagProgress = ((1.0 - uv.x) + uv.y) / 2.0;

                float range = 0.03 / _FadeSharpness;
                float alpha = smoothstep(_FadeProgress - range, _FadeProgress + range, diagProgress);

                float4 col = tex2D(_MainTex, uv);

                if (alpha >= 1.0)
                    return float4(0, 0, 0, 0);
                else
                    col.a *= 1.0 - alpha;

                return col * i.color;
            }
            ENDCG
        }
    }
}
