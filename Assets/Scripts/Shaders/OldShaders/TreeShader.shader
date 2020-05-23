Shader "Custom/Wind_HueVariation" {
    Properties{
        _Color("Main Color", Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _BumpMap("Normalmap", 2D) = "bump" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
        [Toggle(HUE)]_Hue("Hue Variation", int) = 0
        _HueVariation("", Color) = (1,0.25,0,0.1)
        [Toggle(POSITION_BASED)]_Positioned("Based on Position", int) = 0
        [Toggle(ACTIVATE_WIND)]_ActivateWind("Activate Wind", int) = 1
        _WindStrength("Wind Strength", Range(0,1)) = 0.25
        _WindSpeed("Wind Speed", Range(0,100)) = 0.25
        _NoiseStrength("Noise Strength", Range(0,1)) = 0.25
        _NoiseSpeed("Noise Speed", Range(0,10)) = 0.25
        _WindYShake("Wind Y Shake", Range(0,10)) = 5
            /*[HideInInspector]*/_WindVector("Wind Vector", Vector) = (1,1,1,0)
            /*[HideInInspector]*/_WindFreq("Wind Frequency", Vector) = (1,1,1,0)
    }

        SubShader{
            Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
            LOD 200
            Cull Off
            CGPROGRAM
            #pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow
            #pragma shader_feature ACTIVATE_WIND
            #pragma shader_feature POSITION_BASED
            #pragma shader_feature HUE
            sampler2D _MainTex;
            sampler2D _BumpMap;
            fixed4 _Color;
            fixed4 _HueVariation;
            half _WindStrength;
            half _NoiseStrength;
            half _WindSpeed;
            half _NoiseSpeed;
            half _WindYShake;
            fixed4 _WindVector;
            fixed4 _WindFreq;

            struct Input
            {
                float2 uv_MainTex;
                float2 uv_BumpMap;
                float3 pos;
            };

            float rand(float3 myVector)
            {
                return frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
            }

            void vert(inout appdata_full v, out Input o)
            {
                UNITY_INITIALIZE_OUTPUT(Input, o);
                #ifdef HUE
                float3 WorldPosition = mul(unity_ObjectToWorld, float4(0, 0, 0, 1));
                float3 VertPosition = mul(unity_ObjectToWorld, v.vertex);

                float r = frac(VertPosition.xyz * v.vertex.xyz);
                float g = frac(VertPosition.xyz * v.vertex.xyz);
                float b = frac(VertPosition.xyz * v.vertex.xyz);
                o.pos = _HueVariation.rgb;
                o.pos *= saturate(rand(WorldPosition));
                o.pos *= _HueVariation.a;
                //o.pos = mul(unity_ObjectToWorld, v.vertex).xyz * step(_HueVariation.r+ _HueVariation.g+ _HueVariation.b,rand(mul(unity_ObjectToWorld, v.vertex).yyy))* _HueVariation.rgb;
                #endif
                //gradient = _WindYShake - mul(unity_ObjectToWorld, v.vertex).y;
                    
                #ifdef ACTIVATE_WIND  
                    if (v.vertex.y >= 0) {
                        v.vertex.x += ((((_WindVector.x + rand(v.color.rgb)) * sin(_WindFreq.x * (_Time.y * _WindSpeed + v.vertex.y))) * (rand(v.color.rgb) * _WindStrength * 0.1)) + (sin(_Time.y * _NoiseSpeed + v.vertex.z) * (rand(v.color.rgb) * _NoiseStrength * 0.1))) * (v.vertex.y - _WindYShake);
                        v.vertex.y += ((((_WindVector.y + rand(v.color.rgb)) * sin(_WindFreq.y * (_Time.x * _WindSpeed + v.vertex.x))) * (rand(v.color.rgb) * _WindStrength * 0.1)) + (cos(_Time.y * _NoiseSpeed + v.vertex.z) * (rand(v.color.rgb) * _NoiseStrength * 0.1))) * (v.vertex.y - _WindYShake);
                        v.vertex.z += ((((_WindVector.z + rand(v.color.rgb)) * cos(_WindFreq.z * (_Time.x * _WindSpeed + v.vertex.y))) * (rand(v.color.rgb) * _WindStrength * 0.1)) + (cos(_Time.y * _NoiseSpeed + v.vertex.x) * (rand(v.color.rgb) * _NoiseStrength * 0.1))) * (v.vertex.y - _WindYShake);
                    }
                   
                #endif
            }

            void surf(Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb + (IN.pos * _HueVariation.a);
                o.Alpha = c.a;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            }
            ENDCG
        }
            FallBack "Legacy Shaders/Transparent/Cutout/Diffuse"
}