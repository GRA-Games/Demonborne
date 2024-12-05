Shader "Custom/UnlitWhiteShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)  // Default is white
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            // No lighting applied, making it "unlit"
            Cull Off
            ZWrite On
            ZTest LEqual

            // Set a white color
            Color [_Color]

            // Vertex and fragment shaders (simplified)
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1.0, 1.0, 1.0, 1.0);  // Solid white color
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
