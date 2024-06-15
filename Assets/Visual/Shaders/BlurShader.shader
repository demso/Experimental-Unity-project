

Shader "z/BlurShader"
{
    Properties
    {
        _Dir ("Direction", Vector) = (0, 1, 1, 1)
        _MainTex ("Texture", 2D) = "blue" {}
        _FBO_W ("FBO_W", Float) = 480
        _FBO_H ("FBO_W", Float) = 270
    }
    SubShader
    {
        
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Blend SrcAlpha Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float2 v_tc0    : TEXCOORD1;
                float2 v_tc1    : TEXCOORD2;
                float2 v_tc2    : TEXCOORD3;
                float2 v_tc3    : TEXCOORD4;
                float2 v_tc4    : TEXCOORD5;
            };

            vector _Dir;
            sampler2D _MainTex;
            float _FBO_W;
            float _FBO_H;
 
            v2f vert (appdata v)
            {
                float2 futher = float2(3.2307692308 / _FBO_W, 3.2307692308 / _FBO_H);
                float2 close = float2(1.3846153846 / _FBO_W, 1.3846153846 / _FBO_H);
                
                float2 f = futher * _Dir.xy;
                float2 c = close  * _Dir.xy;
                
                v2f o;
                
                o.v_tc0 = v.uv - f;
                o.v_tc1 = v.uv - c;
                o.v_tc2 = v.uv;
                o.v_tc3 = v.uv + c;
                o.v_tc4 = v.uv + f;
                
                o.vertex = v.vertex;
                o.uv = v.uv; 
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float center = 0.2270270270;
                float closeFrag = 0.3162162162;
                float far    = 0.0702702703;
                
                fixed4 color = fixed4(0, 0, 0, 1);
                
                color.rgb =  far         * tex2D( _MainTex, i.v_tc0);
                color.rgb += closeFrag   * tex2D( _MainTex, i.v_tc1);
                color.rgb += center      * tex2D( _MainTex, i.v_tc2);
                color.rgb += closeFrag   * tex2D( _MainTex, i.v_tc3);
                color.rgb += far         * tex2D( _MainTex, i.v_tc4);
                
                return color;
            }
            ENDCG
        }
    }
}