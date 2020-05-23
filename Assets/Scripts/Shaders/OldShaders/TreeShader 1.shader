Shader "Transparent/Cutout/Diffuse2" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
        _HueVariation("", Color) = (1,0.25,0,0.1)
    }

        SubShader{
            Tags {"Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout"}
            LOD 200
            Cull Off

        CGPROGRAM
        #pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow

        sampler2D _MainTex;
        fixed4 _Color;
        fixed4 _HueVariation;

        struct Input {
            float2 uv_MainTex;
            float3 pos;
        };
        float rand(float3 myVector)
        {
            return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
        }
        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);

            float3 WorldPosition = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
            float3 VertPosition = mul(unity_ObjectToWorld, v.vertex);

            float r = frac(VertPosition.xyz * v.vertex.xyz);
            float g = frac(VertPosition.xyz * v.vertex.xyz);
            float b = frac(VertPosition.xyz * v.vertex.xyz);
            o.pos = _HueVariation.rgb;

            o.pos *= saturate(float3(r, g, b) * rand(WorldPosition + VertPosition));
            o.pos *= _HueVariation.a;
        }

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb + (IN.pos * _HueVariation.a);
            o.Alpha = c.a;
        }
        ENDCG
        }

            Fallback "Transparent/Cutout/VertexLit"
}