Shader "Unlit/Shield"
{
    Properties
    {
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _Angle ("Angle", Range(0, 180)) = 30
        _MainTex ("Texture", 2D) = "white" {}
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
            fixed4 _Color;
            fixed _Angle;

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
                const fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
                const fixed2 uv = normalize(i.uv - float2(0.5, 0.5));
                const fixed angle = abs(degrees(acos(dot(fixed2(0, 1), uv))));
                const fixed alpha = clamp(floor(angle / _Angle), 0, 1);
                return col * fixed4(1, 1, 1, 1 - alpha);
            }
            ENDCG
        }
    }
}
