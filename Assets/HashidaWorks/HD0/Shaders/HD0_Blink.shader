// HashidaWorks HD0 - aviation obstruction lights: slow red blink, each light on its own phase (from its position).
Shader "HashidaWorks/HD0/Blink"
{
    Properties
    {
        _Color ("Colour", Color) = (1, 0.1, 0.06, 1)
        _Intensity ("Intensity", Float) = 3
        _Period ("Period (s)", Float) = 2.2
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

            half4 _Color;
            half _Intensity;
            float _Period;

            struct appdata { float4 vertex : POSITION; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 pos : SV_POSITION; half k : TEXCOORD0; UNITY_FOG_COORDS(1) UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                float3 w = mul(unity_ObjectToWorld, v.vertex).xyz;
                float ph = frac(sin(dot(floor(w.xz / 3.0), float2(12.9898, 78.233))) * 43758.5453);
                float t = frac(_Time.y / _Period + ph);
                o.k = smoothstep(0.0, 0.08, t) * (1 - smoothstep(0.42, 0.55, t));
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half3 c = _Color.rgb * (0.06 + i.k * _Intensity);
                UNITY_APPLY_FOG(i.fogCoord, c);
                return half4(c, 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
