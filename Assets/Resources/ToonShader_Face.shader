// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/ToonShader"
{
    Properties
    {
		_Color ("Default Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Range(0, 1)) = 1
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_BumpScale("Bump Scale", Float) = 1.0
		//_SpecularMask("Specular Mask", 2D) = "White" {}
		//_SpecularScale("Specular Mask Scale", Float) = 1.0
		//_Specular("Specular", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8.0 , 256)) = 20
		_RampTex("ShadowRamp" , 2D) = "black" {} //It must be "Clamp" in Texture Setting, otherwise
			// screen will get a error result.
		_Tint("ShadowColor" , 2D) = "black" {}
		_TintScale("Scale" , Range(0, 1)) = 1
	}
		SubShader
		{
			Tags { "Queue" = "Transparent + 100 " "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 100

			//Extra pass that renders to depth buffer only
			Pass
			{
				ZWrite On
				ColorMask 0
			}

			Pass  // fwdbase Front
			{
				Tags { "LightMode" = "ForwardBase" }

				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				
				CGPROGRAM
				#pragma multi_compile_fwdbase
				
				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _AlphaScale;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				float _BumpScale;
				//sampler2D _SpecularMask;
				//float _SpecularScale;
				//fixed4 _Specular;
				float _Gloss;
				sampler2D _RampTex;
				sampler2D _Tint;
				float _TintScale;

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f
            {
				float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 TtoW0 : TEXCOORD1;
				float4 TtoW1 : TEXCOORD2;
				float4 TtoW2 : TEXCOORD3;
				SHADOW_COORDS(4)
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
				//compute the binormal
				TANGENT_SPACE_ROTATION;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);

                //UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//get the texel in the normal map
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump),
					dot(i.TtoW2.xyz, bump)));

				//fixed3 tangentNormal;
				//If the texture is not marked as "Normal Map"
			//	tangentNormal.xy = (packedNormal.xy * 2 - 1) * _BumpScale;
			//	tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				//Or mark the texture as "Normal Map", and use built-in function
				//tangentNormal = UnpackNormal(packedNormal);
				//tangentNormal.xy *= _BumpScale;
				//tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed3 ShadowColor = tex2D(_Tint, i.uv.xy).rgb * _TintScale;
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ramp = tex2D(_RampTex, float2((max(0, dot(bump,
					lightDir))), 0)).rgb;//HalfLambert
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * ramp; //Toon Diffuse
				//Realistic Diffuse ： fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));

				fixed3 halfDir = normalize(lightDir + viewDir);
				//fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
				//fixed3 rampSpec = tex2D(_RampTex, float2((max(0, dot(bump,
				//	halfDir))), 0)).rgb;
				//fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(rampSpec.r, _Gloss) * specularMask; //Toon Specular
				//Realistic Specular ： fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, 
				//		halfDir)), _Gloss) * specularMask;
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);
                // sample the texture
                fixed4 col = fixed4(ambient + (diffuse) * atten + ShadowColor,
					texColor.a * _AlphaScale);
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

			Pass  // fwdadd
			{
				Tags { "LightMode" = "ForwardAdd" }

				ZWrite Off
				Blend One One

				CGPROGRAM
				#pragma multi_compile_fwdadd

				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				//#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _AlphaScale;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				float _BumpScale;
				//sampler2D _SpecularMask;
				//float _SpecularScale;
				//fixed4 _Specular;
				float _Gloss;
				sampler2D _RampTex;
				sampler2D _Tint;
				float _TintScale;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 TtoW0 : TEXCOORD1;
				float4 TtoW1 : TEXCOORD2;
				float4 TtoW2 : TEXCOORD3;
				SHADOW_COORDS(4)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
				//compute the binormal
				

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);

				//UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//get the texel in the normal map
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump),
					dot(i.TtoW2.xyz, bump)));

				//fixed3 tangentNormal;
				//If the texture is not marked as "Normal Map"
			//	tangentNormal.xy = (packedNormal.xy * 2 - 1) * _BumpScale;
			//	tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				//Or mark the texture as "Normal Map", and use built-in function
				//tangentNormal = UnpackNormal(packedNormal);
				//tangentNormal.xy *= _BumpScale;
				//tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed3 ShadowColor = tex2D(_Tint, i.uv.xy).rgb * _TintScale;
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ramp = tex2D(_RampTex, float2((max(0, dot(bump,
					lightDir))), 0)).rgb;//HalfLambert
				fixed3 diffuse = _LightColor0.rgb * albedo * ramp; //Toon Diffuse
				//Realistic Diffuse ： fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));

				fixed3 halfDir = normalize(lightDir + viewDir);
				//fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
				//fixed3 rampSpec = tex2D(_RampTex, float2((max(0, dot(bump,
					halfDir))), 0)).rgb;
				//fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(rampSpec.r, _Gloss) * specularMask; //Toon Specular
				//Realistic Specular ： fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, 
				//		halfDir)), _Gloss) * specularMask;
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);
				// sample the texture
				fixed4 col = fixed4((diffuse) * atten + ShadowColor, texColor.a * _AlphaScale);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
			
			Pass  // fwdbase back
			{
				Tags { "LightMode" = "ForwardBase" }

				Cull Front
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma multi_compile_fwdbase

				#pragma vertex vert
				#pragma fragment frag
			// make fog work
			//#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _AlphaScale;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				float _BumpScale;
				//sampler2D _SpecularMask;
				//float _SpecularScale;
				//fixed4 _Specular;
				float _Gloss;
				sampler2D _RampTex;
				sampler2D _Tint;
				float _TintScale;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 TtoW0 : TEXCOORD1;
				float4 TtoW1 : TEXCOORD2;
				float4 TtoW2 : TEXCOORD3;
				SHADOW_COORDS(4)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
				//compute the binormal
				TANGENT_SPACE_ROTATION;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;

				o.TtoW0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TtoW1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TtoW2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				TRANSFER_SHADOW(o);

				//UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);

				fixed3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));
				fixed3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				//get the texel in the normal map
				fixed3 bump = UnpackNormal(tex2D(_BumpMap, i.uv.zw));
				bump = normalize(half3(dot(i.TtoW0.xyz, bump), dot(i.TtoW1.xyz, bump),
					dot(i.TtoW2.xyz, bump)));

				//fixed3 tangentNormal;
				//If the texture is not marked as "Normal Map"
			//	tangentNormal.xy = (packedNormal.xy * 2 - 1) * _BumpScale;
			//	tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
				//Or mark the texture as "Normal Map", and use built-in function
				//tangentNormal = UnpackNormal(packedNormal);
				//tangentNormal.xy *= _BumpScale;
				//tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

				fixed4 texColor = tex2D(_MainTex, i.uv.xy);
				fixed3 ShadowColor = tex2D(_Tint, i.uv.xy) * _TintScale;
				fixed3 albedo = texColor.rgb * _Color.rgb;
				fixed3 ramp = tex2D(_RampTex, float2((max(0, dot(bump,
					lightDir))), 0)).rgb;//HalfLambert
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
				fixed3 diffuse = _LightColor0.rgb * albedo * ramp; //Toon Diffuse
				//Realistic Diffuse ： fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));

				fixed3 halfDir = normalize(lightDir + viewDir);
				//fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
				//fixed3 rampSpec = tex2D(_RampTex, float2((max(0, dot(bump,
					halfDir))), 0)).rgb;
				//fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(rampSpec.r, _Gloss) * specularMask; //Toon Specular
				//Realistic Specular ： fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, 
				//		halfDir)), _Gloss) * specularMask;
				UNITY_LIGHT_ATTENUATION(atten, i, worldPos);
				// sample the texture
				fixed4 col = fixed4(ambient + (diffuse) * atten + ShadowColor, texColor.a * _AlphaScale);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}

			/*Pass  // Shade front face
			{
				Tags { "LightMode" = "ForwardBase" }

				Cull Back
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				// make fog work
				//#pragma multi_compile_fog

				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				fixed4 _Color;
				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed _AlphaScale;
				sampler2D _BumpMap;
				float4 _BumpMap_ST;
				float _BumpScale;
				sampler2D _SpecularMask;
				float _SpecularScale;
				fixed4 _Specular;
				float _Gloss;
				sampler2D _RampTex;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float4 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;
					//UNITY_FOG_COORDS(1)
					float3 lightDir: TEXCOORD1;
					float3 viewDir : TEXCOORD2;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					o.uv.zw = v.texcoord.xy * _BumpMap_ST.xy + _BumpMap_ST.zw;
					//compute the binormal
					TANGENT_SPACE_ROTATION;
					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex)).xyz;

					//UNITY_TRANSFER_FOG(o,o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed3 tangentLightDir = normalize(i.lightDir);
					fixed3 tangentViewDir = normalize(i.viewDir);
					//get the texel in the normal map
					fixed4 packedNormal = tex2D(_BumpMap, i.uv.zw);
					fixed3 tangentNormal;
					//If the texture is not marked as "Normal Map"
				//	tangentNormal.xy = (packedNormal.xy * 2 - 1) * _BumpScale;
				//	tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));
					//Or mark the texture as "Normal Map", and use built-in function
					tangentNormal = UnpackNormal(packedNormal);
					tangentNormal.xy *= _BumpScale;
					tangentNormal.z = sqrt(1.0 - saturate(dot(tangentNormal.xy, tangentNormal.xy)));

					fixed4 texColor = tex2D(_MainTex, i.uv);
					fixed3 albedo = texColor.rgb * _Color.rgb;
					fixed3 ramp = tex2D(_RampTex, float2((max(0, dot(tangentNormal,
						tangentLightDir))) * 0.5 + 0.5, 0)).rgb;//HalfLambert
					fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;
					fixed3 diffuse = _LightColor0.rgb * albedo * ramp; //Toon Diffuse
					//Realistic Diffuse ： fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(tangentNormal, tangentLightDir));

					fixed3 halfDir = normalize(tangentLightDir + tangentViewDir);
					fixed specularMask = tex2D(_SpecularMask, i.uv).r * _SpecularScale;
					fixed3 rampSpec = tex2D(_RampTex, float2((max(0, dot(tangentNormal,
						halfDir))) * 0.5 + 0.5, 0)).rgb;
					fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(rampSpec.r, _Gloss) * specularMask; //Toon Specular
					//Realistic Specular ： fixed3 specular = _LightColor0.rgb * _Specular.rgb * pow(max(0, dot(tangentNormal, 
					//		halfDir)), _Gloss) * specularMask;
					// sample the texture
					fixed4 col = fixed4(ambient + diffuse + specular, texColor.a * _AlphaScale);
					// apply fog
					//UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}*/
    }
	Fallback "Specular"
}
