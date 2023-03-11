Shader "UI/GlowVisualizer"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _TileColor ("Tiling Color", Color) = (1, 1, 1, .25)
        _FresnelPower ("Fresnel Power", Range (0.0, 5.0)) = 2.5
        _GlowStrength ("Glow Strength", Range (0.0, 1.0)) = 1.0
        _BaseGlow ("Base Glow", Range (0.0, 0.5)) = .1
        _FogDistance ("Fog Distance", Float) = 100.0
        _CheckerTileSizes ("Tile Size", Vector) = (1, 1, 1, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldSpace : TEXCOORD0;
                float3 uv : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _TileColor;
            float _FresnelPower;
            float _GlowStrength;
            float _BaseGlow;
            float _FogDistance;
            float4 _CheckerTileSizes;

            v2f vert (appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldSpace = mul(unity_ObjectToWorld, v.vertex);
                o.uv = normalize(mul(unity_ObjectToWorld, v.uv));

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 delta = _WorldSpaceCameraPos - i.worldSpace;
                float3 cameraViewNormal = normalize(delta);
                float dotProductFactor = 1. - dot(i.uv, cameraViewNormal);
                float fresnelFactor = _BaseGlow + pow(dotProductFactor, _FresnelPower);

                float4 col = float4(_BaseColor.rgb, fresnelFactor);
                col += fresnelFactor * _GlowStrength;
                col.a *= _BaseColor.a;

                int3 integerPosition = int3(i.worldSpace / _CheckerTileSizes.xyz + 100);
                if ((integerPosition.x + integerPosition.y + integerPosition.z) & 1u == 1u)
                    col = float4(lerp(col.rgb, _TileColor.rgb, _TileColor.a / (_TileColor.a + col.a)), _TileColor.a + col.a);

                col.a *= exp(-length(delta) / _FogDistance);
                return col;
            }
            ENDCG
        }
    }
}
