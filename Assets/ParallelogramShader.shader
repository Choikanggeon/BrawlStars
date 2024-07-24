Shader "Custom/ParallelogramShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _FillTex ("Fill Texture", 2D) = "white" {}
        _ParallelogramOffset ("Parallelogram Offset", Vector) = (0.5, 0.5, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _FillTex;
            float4 _MainTex_ST;
            float4 _ParallelogramOffset;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.texcoord);

                // Parallelogram transformation
                float2 offsetCoord = i.texcoord + _ParallelogramOffset.xy * (1.0 - i.texcoord.y);

                fixed4 fillColor = tex2D(_FillTex, offsetCoord);

                return mainColor * fillColor;
            }
            ENDCG
        }
    }
}
