// HashidaWorks HD0 - the night city (facades with lit windows, roofs, roads, lamps, signs).
// Unlit and cheap: colour = texture * tint * glow, plus the scene fog. No lightmaps, no realtime lights.
Shader "HashidaWorks/HD0/City"
{
    Properties
    {
        _MainTex ("Facade (lit windows baked in)", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _Glow ("Glow", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Color;
            half _Glow;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; UNITY_FOG_COORDS(1) UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 c = tex2D(_MainTex, i.uv).rgb * _Color.rgb * _Glow;
                UNITY_APPLY_FOG(i.fogCoord, c);
                return half4(c, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
