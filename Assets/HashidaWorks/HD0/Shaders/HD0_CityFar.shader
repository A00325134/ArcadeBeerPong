// HashidaWorks HD0 - far skyline ring: alpha-tested silhouettes with tiny lit windows, mixed into a fixed haze
// (beyond the scene fog distance, so it ignores Unity fog and uses its own constant haze amount).
Shader "HashidaWorks/HD0/CityFar"
{
    Properties
    {
        _MainTex ("Skyline (RGBA)", 2D) = "white" {}
        _Haze ("Haze colour", Color) = (0.25, 0.205, 0.225, 1)
        _HazeAmount ("Haze amount", Range(0, 1)) = 0.55
        _Cutoff ("Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half4 _Haze;
            half _HazeAmount;
            half _Cutoff;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 t = tex2D(_MainTex, i.uv);
                clip(t.a - _Cutoff);
                // haze is lighter at the bottom of the ring (thicker air near the ground)
                half h = saturate(_HazeAmount + (1 - i.uv.y) * 0.25);
                half3 hz = _Haze.rgb;
                return half4(lerp(t.rgb, hz, h), 1);
            }
            ENDCG
        }
    }
    Fallback Off
}
