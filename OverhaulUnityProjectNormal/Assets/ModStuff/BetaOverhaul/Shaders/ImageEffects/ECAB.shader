Shader "Custom/Chromatic aberration2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
 
        [Header(Red)]
        _RedX ("Offset X", Range(-0.5, 0.5)) = 0.0
        _RedY ("Offset Y", Range(-0.5, 0.5)) = 0.0
 
        [Header(Green)]
        _GreenX ("Offset X", Range(-0.5, 0.5)) = 0.0
        _GreenY ("Offset Y", Range(-0.5, 0.5)) = 0.0
 
        [Header(Blue)]
        _BlueX ("Offset X", Range(-0.5, 0.5)) = 0.0
        _BlueY ("Offset Y", Range(-0.5, 0.5)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
           
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float _RedX;
            float _RedY;
            float _GreenX;
            float _GreenY;
            float _BlueX;
            float _BlueY;
           
            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = fixed4(1, 1, 1, 1);
 
                float2 red_uv = i.uv + float2(_RedX, _RedY);
                float2 green_uv = i.uv + float2(_GreenX, _GreenY);
                float2 blue_uv = i.uv + float2(_BlueX, _BlueY);
 
                int lower = -(9 / 2);
                int upper = -lower;
                
                for (int y = lower; y <= upper; ++y)
                {
                col.r += tex2D(_MainTex, red_uv).r * y;
                col.g += tex2D(_MainTex, green_uv).g * y;
                col.b += tex2D(_MainTex, blue_uv).b * y;
                }
 
                return col;
            }
            ENDCG
        }
    }
}