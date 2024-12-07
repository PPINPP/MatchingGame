Shader "UI/TransparentMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:blend

        sampler2D _MainTex;
        sampler2D _MaskTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mainColor = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 maskColor = tex2D(_MaskTex, IN.uv_MainTex);

            // Make masked area transparent
            mainColor.a = mainColor.a * (1 - maskColor.r);
            o.Albedo = mainColor.rgb;
            o.Alpha = mainColor.a;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/VertexLit"
}
