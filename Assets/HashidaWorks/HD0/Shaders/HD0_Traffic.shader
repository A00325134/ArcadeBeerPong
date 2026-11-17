// HashidaWorks HD0 - additive light strips: car head / tail lights scrolling along the road (u), street light
// pools on the wet asphalt (speed 0).  Fog fades them to black (additive).
Shader "HashidaWorks/HD0/Traffic"
{
    Properties
    {
        _MainTex ("Lights (black = nothing)", 2D) = "black" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _Speed ("Scroll speed (u per second)", Float) = 0.04
        _Intensity ("Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+20" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Blend One One
        ZWrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _Color;
            float _Speed;
            half _Intensity;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; UNITY_FOG_COORDS(1) UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv + float2(frac(_Time.y * _Speed), 0);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 c = tex2D(_MainTex, i.uv).rgb * _Color.rgb * _Intensity;
                UNITY_APPLY_FOG_COLOR(i.fogCoord, c, half4(0, 0, 0, 0));
                return half4(c, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
