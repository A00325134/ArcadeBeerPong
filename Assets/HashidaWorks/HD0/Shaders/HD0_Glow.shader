// HashidaWorks HD0 - additive particles (rain streaks, drips, splashes); vertex colour carries the fade.
Shader "HashidaWorks/HD0/Glow"
{
    Properties
    {
        _MainTex ("Sprite (additive, black = clear)", 2D) = "white" {}
        _Intensity ("Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent+10" "RenderType" = "Transparent" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
        Blend One One
        ZWrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Intensity;

            struct appdata { float4 vertex : POSITION; half4 color : COLOR; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f { float4 pos : SV_POSITION; half4 color : COLOR; float2 uv : TEXCOORD0; UNITY_VERTEX_OUTPUT_STEREO };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 c = tex2D(_MainTex, i.uv) * i.color;
                return half4(c.rgb * c.a * _Intensity, 0);
            }
            ENDCG
        }
    }
}
