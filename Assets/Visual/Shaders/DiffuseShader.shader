Shader "z/DiffuseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Ambient ("Ambient color", Color) = (1, 1, 1, 1)
     //    MySrcMode ("SrcMode", Integer) = 0
	    // MyDstMode ("DstMode", Integer) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Blend DstColor Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            fixed4 _Ambient;
            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = _Ambient;
                color.a = 1.0;
                color.rgb = (_Ambient.rgb + tex2D(_MainTex, i.uv).rgb);
                return color;
            }
            ENDCG
        }
    }
}
