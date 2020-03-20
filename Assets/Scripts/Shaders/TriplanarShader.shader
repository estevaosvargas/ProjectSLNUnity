// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/TestTri" {
	Properties {
		_TexPower			("Textures merging", Range(0.01, 20.0)) = 0.01
		_TopPower 			("Top Multiplier (+Y)", Range(0.0,8.0)) = 1.0
		_BotPower 			("Bottom Multiplier (-Y)", Range(0.0,8.0)) = 0.0	
		
		// Diffuse
		_MainTex			("Main Texture", 2D) = "White" {}
		_Color 				("Main color tint", Color) = (1.0, 1.0, 1.0, 1.0)
		_TopTex 			("Top Texture", 2D) = "white" {}
		_TopColor			("Top color tint", Color) = (1.0,1.0,1.0,1.0)
		_BotTex 			("Bot Texture", 2D) = "white" {}
		_BotColor			("Bot color tint", Color) = (1.0,1.0,1.0,1.0)
	}
	
	SubShader{
			Tags{"RenderType"="Opaque"}
			
			LOD 300
			CGPROGRAM		
			#pragma target 3.0	
			#pragma surface surf Lambert vertex:vert
			
			uniform half 		_TexPower;
			uniform half 		_TopPower;
			uniform half 		_BotPower;				
			
			// Diffuse
			uniform sampler2D	_MainTex;
			uniform half4		_MainTex_ST;
			uniform fixed4		_Color;
			
			uniform sampler2D	_TopTex;
			uniform half4		_TopTex_ST;
			uniform fixed4		_TopColor;
			
			uniform sampler2D	_BotTex;
			uniform half4		_BotTex_ST;
			uniform fixed4		_BotColor;
					
			struct Input{
				fixed3 normal;
				fixed3 powerWorldNorm;
				fixed3 weightedWorldNorm;
				float3 worldPos;
				fixed3 viewDir;
			};
			
			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				
				// World space normals
				o.normal = normalize(mul(unity_ObjectToWorld, fixed4(v.normal, 0.0)).xyz);				
				
				// Texture merging power
				o.powerWorldNorm = max(pow(abs(o.normal), _TexPower), 0.0001);
				o.powerWorldNorm /= dot(o.powerWorldNorm, 1.0);
				
				// +Y and -Y texture's power
				o.weightedWorldNorm = o.normal;
				o.weightedWorldNorm.y = max(0.0, o.weightedWorldNorm.y) * _TopPower + min(0.0, o.weightedWorldNorm.y) * _BotPower;				
				o.weightedWorldNorm = pow(abs(o.weightedWorldNorm), _TexPower);
				o.weightedWorldNorm = max(o.weightedWorldNorm, 0.0001);
				o.weightedWorldNorm /= dot(o.weightedWorldNorm, 1.0);
				
				fixed3 normLerp = lerp(o.powerWorldNorm, o.weightedWorldNorm, o.weightedWorldNorm.y);
				
				v.tangent.xyz = 
					cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(o.normal.x),0.0,0.0)).xyz * (normLerp.x))
				  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,0.0,sign(o.normal.y),0.0)).xyz * (normLerp.y))
				  + cross(v.normal, mul(unity_WorldToObject,fixed4(0.0,sign(o.normal.z),0.0,0.0)).xyz * (normLerp.z));
								
				v.tangent.w = dot(-o.normal, normLerp);
			}
			
			void surf(Input i, inout SurfaceOutput o)
			{
				// Linear interpolation
				fixed topLerp = smoothstep(0.0, 1.0, _TopPower);				
				fixed botLerp = smoothstep(0.0, 1.0, _BotPower);	
				fixed topBotLerp = sign(i.normal.y) * 0.5 + 0.5;
				
				// Triplanar UVs				
				float2 xUV = i.worldPos.zy * _MainTex_ST.xy + _MainTex_ST.zw;	
				float2 yUV = i.worldPos.xz * _MainTex_ST.xy + _MainTex_ST.zw;				
				float2 zUV = float2(-i.worldPos.x, i.worldPos.y) * _MainTex_ST.xy + _MainTex_ST.zw;
						
				float2 yTopUV = i.worldPos.xz * _TopTex_ST.xy + _TopTex_ST.zw;
				float2 yBotUV = i.worldPos.xz * _BotTex_ST.xy + _BotTex_ST.zw;
				
				// Diffuse				
				fixed4 texY = tex2D(_MainTex, yUV) * _Color;
				
				fixed4 texTop = lerp(texY, tex2D(_TopTex, yTopUV) * _TopColor, topLerp);
				texTop = lerp(lerp(texY, tex2D(_BotTex, yBotUV) * _BotColor, botLerp), texTop, topBotLerp);
				
				// Final texture 
				fixed4 tex = 
				    lerp(tex2D(_MainTex, xUV) * _Color * i.powerWorldNorm.x, texTop * i.weightedWorldNorm.x, i.weightedWorldNorm.y)
				  + lerp(texY * i.powerWorldNorm.y, texTop * i.weightedWorldNorm.y, i.weightedWorldNorm.y)
				  + lerp(tex2D(_MainTex, zUV) * _Color * i.powerWorldNorm.z, texTop * i.weightedWorldNorm.z, i.weightedWorldNorm.y);
				
				o.Albedo = max(fixed3(0.001,0.001,0.001), tex.rgb);
				o.Alpha = 1;
				
				o.Normal = normalize(i.weightedWorldNorm);
				
				}
		ENDCG
	}		
	FallBack "Diffuse"	
}