// HashidaWorks HD0 - distant rain sheets: additive streak texture scrolling down, faded by the fog.
Shader "HashidaWorks/HD0/RainCurtain"
{
    Properties
    {
        _MainTex ("Streaks (additive)", 2D) = "black" {}
        _Color ("Tint", Color) = (0.75, 0.78, 0.9, 1)
        _Speed ("Fall speed (v per second)", Float) = 1.6
        _Intensity ("Intensity", Float) = 0.22
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+10" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
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
            struct v2f { float4 pos : SV_POSITION; float4 uv : TEXCOORD0; UNITY_FOG_COORDS(1) UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                float t = _Time.y * _Speed;
                o.uv.xy = v.uv + float2(0, t);
                o.uv.zw = v.uv * float2(1.37, 0.81) + float2(0.31, t * 1.27);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 c = (tex2D(_MainTex, i.uv.xy).rgb + tex2D(_MainTex, i.uv.zw).rgb * 0.7) * _Color.rgb * _Intensity;
                UNITY_APPLY_FOG_COLOR(i.fogCoord, c, half4(0, 0, 0, 0));
                return half4(c, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
