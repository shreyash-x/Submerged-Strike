Shader "Unlit/EnemyIndicator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MidPoint ("Mid Point", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        
        ZWrite off
        Cull off

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
                fixed4 color: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color: COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _MidPoint;

            float _IndicatorPositionCount = 0;
            float4 _IndicatorPositions[10];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            inline float evaluate (const float t)
            {
                // return (1 - t) * (t * _MidPoint) + t * ((1 - t) * _MidPoint + t);
                return pow(t, 6);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = tex2D(_MainTex, i.uv);
                float distance = 1;
                for (int j = 0; j < _IndicatorPositionCount; ++j)
                {
                    distance = min(distance, length(_IndicatorPositions[j].xy - i.uv));
                }
                const float value = 1 - distance;
                return fixed4(1, 1, 1, evaluate(1 - distance) * 10) * tex2D(_MainTex, i.uv) * i.color;
            }
            ENDCG
        }
    }
}
