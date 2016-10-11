// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/SpecularVertex" {
	Properties{
		_Color ("Diffuse Material Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1)
		_Shininess ("Shininess", Range(0,100)) = 10
	}
	SubShader{
		Pass{
			Tags{
				"LightMode" = "ForwardBase"
			} // make sure that all uniforms are correctly set

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _LightColor0;	// color of light source (from "Lighting.cginc")
			
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert(vertexInput input){
				vertexOutput output;
				
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				// multiplication with unity_Scale.w is unnecessary
				// because we normalize transformed vectors
				
				//normal vector
				float4 mult = mul(float4(input.normal.x, input.normal.y, input.normal.z, 0.0), modelMatrixInverse);
				float3 normalDirection = normalize(float3(mult.x, mult.y, mult.z));
				
				//view direction
				float4 sub = float4(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.y, _WorldSpaceCameraPos.z, 1) - mul(modelMatrix, input.vertex);
				float3 viewDirection = normalize(sub);

				//diffuse color
				mult = mul(modelMatrix, input.vertex * _WorldSpaceLightPos0.w);
				float3 vertexToLightSource = _WorldSpaceLightPos0 - float3(mult.x, mult.y, mult.z);
				float one_over_distance = 1.0 / length(vertexToLightSource);
				
				float attenuation = lerp(1.0, one_over_distance, _WorldSpaceLightPos0.w);

				float3 lightDirection = vertexToLightSource * one_over_distance;
				
				float3 diffuseReflection = attenuation * float3(_LightColor0.x, _LightColor0.y, _LightColor0.z) *
					float3(_Color.x, _Color.y, _Color.z) * max(0.0, dot(normalDirection, lightDirection));

				//ambient color
				float3 ambientLighting = float3(UNITY_LIGHTMODEL_AMBIENT.x, UNITY_LIGHTMODEL_AMBIENT.y, UNITY_LIGHTMODEL_AMBIENT.z) * float3(_Color.x, _Color.y, _Color.z);
				
				//specular color
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0){
					specularReflection = float3(0, 0, 0);
				}else{
					specularReflection = attenuation * float3(_LightColor0.x, _LightColor0.y, _LightColor0.z) * float3(_SpecColor.x, _SpecColor.y, _SpecColor.z)
						* pow(max(0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}

				output.col = float4(ambientLighting + diffuseReflection + specularReflection, 1.0);
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}

			float4 frag(vertexOutput input) : COLOR{
				return input.col;
			}

			ENDCG
		}
		Pass{
			Tags{
				"LightMode" = "ForwardAdd"
			} // make sure that all uniforms are correctly set
			Blend One One	//additive blending

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _LightColor0;	// color of light source (from "Lighting.cginc")
			
			uniform float4 _Color;
			uniform float4 _SpecColor;
			uniform float _Shininess;

			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 col : COLOR;
			};

			vertexOutput vert(vertexInput input){
				vertexOutput output;
				
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;
				// multiplication with unity_Scale.w is unnecessary
				// because we normalize transformed vectors
				
				//normal vector
				float4 mult = mul(float4(input.normal.x, input.normal.y, input.normal.z, 0.0), modelMatrixInverse);
				float3 normalDirection = normalize(float3(mult.x, mult.y, mult.z));

				//view direction
				float4 sub = float4(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.y, _WorldSpaceCameraPos.z, 1) - mul(modelMatrix, input.vertex);
				float3 viewDirection = normalize(sub);

				//diffuse color
				mult = mul(modelMatrix, input.vertex * _WorldSpaceLightPos0.w);
				float3 vertexToLightSource = _WorldSpaceLightPos0 - float3(mult.x, mult.y, mult.z);
				float one_over_distance = 1.0 / length(vertexToLightSource);
				
				float attenuation = lerp(1.0, one_over_distance, _WorldSpaceLightPos0.w);

				float3 lightDirection = vertexToLightSource * one_over_distance;

				float3 diffuseReflection = attenuation * float3(_LightColor0.x, _LightColor0.y, _LightColor0.z) *
					float3(_Color.x, _Color.y, _Color.z) * max(0.0, dot(normalDirection, lightDirection));
				
				//specular color
				float3 specularReflection;
				if(dot(normalDirection, lightDirection) < 0){
					specularReflection = float3(0, 0, 0);
				}else{
					specularReflection = attenuation * float3(_LightColor0.x, _LightColor0.y, _LightColor0.z) * float3(_SpecColor.x, _SpecColor.y, _SpecColor.z)
						* pow(max(0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
				}

				output.col = float4(diffuseReflection + specularReflection, 1.0);
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}

			float4 frag(vertexOutput input) : COLOR{
				return input.col;
			}

			ENDCG
		}
	}
// The definition of a fallback shader should be commented out
// during development:
// Fallback "Diffuse"
}