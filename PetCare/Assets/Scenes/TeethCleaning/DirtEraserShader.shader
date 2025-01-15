Shader "Custom/DirtEraserShader"
{
    Properties
    {
        _MainTex ("Dirt Texture", 2D) = "white" {}
        _MaskTex ("Erase Mask", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _MaskTex;

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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Wo die Maske weiﬂ ist, entferne den Dreck
                col.rgb = lerp(col.rgb, fixed3(1, 1, 1), mask.r);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
