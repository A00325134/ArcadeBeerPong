// HashidaWorks HD0 - window glass in the rain.
// Static beads + two layers of drops running down (UV = world metres along the pane, v = height), catching the light
// of the room / city; the glass itself reflects the reflection probe (the room at night, the city glow) by Fresnel.
// No grab pass (Quest): the beads are drawn as light-catching specks, not refraction.
Shader "HashidaWorks/HD0/RainGlass"
{
    Properties
    {
        _Color ("Glass tint (a = base opacity)", Color) = (0.6, 0.66, 0.75, 0.05)
        _Drops ("Beads (RGBA)", 2D) = "black" {}
        _Run ("Running drops (RGBA)", 2D) = "black" {}
        _DropTile ("Bead tile (m)", Float) = 0.8
        _RunTile ("Run tile (m, x / y)", Vector) = (0.25, 1.0, 0, 0)
        _RunSpeed ("Run speed", Float) = 0.11
        _Density ("Bead density", Range(0, 1)) = 1
        _RunAmount ("Running drops", Range(0, 1)) = 1
        _DropLight ("Light caught by drops", Color) = (0.42, 0.45, 0.52, 1)
        _Reflect ("Reflection", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            half4 _Color;
            sampler2D _Drops;
            sampler2D _Run;
            float _DropTile;
            float4 _RunTile;
            float _RunSpeed;
            half _Density;
            half _RunAmount;
            half4 _DropLight;
            half _Reflect;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; float2 uv : TEXCOORD0; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 wpos : TEXCOORD1;
                float3 wn : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.wn = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            half4 frag(v2f i, fixed facing : VFACE) : SV_Target
            {
                float t = _Time.y * _RunSpeed;
                half4 b = tex2D(_Drops, i.uv / _DropTile);
                half4 r1 = tex2D(_Run, float2(i.uv.x / _RunTile.x, i.uv.y / _RunTile.y + t));
                half4 r2 = tex2D(_Run, float2(i.uv.x / (_RunTile.x * 1.37) + 0.43, i.uv.y / (_RunTile.y * 1.21) + t * 0.63));
                half a = max(b.a * _Density, max(r1.a, r2.a * 0.8) * _RunAmount);
                half lum = max(b.r * _Density, max(r1.r, r2.r) * _RunAmount);
                half3 drop = _DropLight.rgb * (0.12 + 0.9 * lum * lum);
                float3 n = normalize(i.wn) * (facing > 0 ? 1 : -1);
                float3 vd = normalize(_WorldSpaceCameraPos - i.wpos);
                half f = 0.04 + 0.96 * pow(1 - saturate(dot(n, vd)), 5);
                half3 env = DecodeHDR(UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, reflect(-vd, n)), unity_SpecCube0_HDR);
                half3 refl = env * f * _Reflect;
                half base = _Color.a + f * 0.25;
                half3 c = _Color.rgb * base + refl + drop * a * 0.7;
                half alpha = saturate(base + a * 0.55);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, c, half4(0, 0, 0, 0));
                return half4(c, alpha);
            }
            ENDCG
        }
    }
    Fallback Off
}
