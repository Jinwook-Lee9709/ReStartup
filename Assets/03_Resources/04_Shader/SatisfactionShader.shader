Shader "Custom/SpriteMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 스프라이트 텍스처
        _FillAmount ("Fill Amount", Range(0, 1)) = 1.0 // 마스크 기준 (1=풀, 0=완전 마스킹)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" } // 투명 객체 처리
        Blend SrcAlpha OneMinusSrcAlpha // 알파 블렌딩

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float _FillAmount;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 스프라이트 텍스처에서 색상 가져오기
                fixed4 color = tex2D(_MainTex, i.uv);

                // 위에서 아래로 잘라내는 마스킹 처리 (_FillAmount 기준)
                if (i.uv.y > _FillAmount)
                    discard;

                return color;
            }
            ENDCG
        }
    }
}