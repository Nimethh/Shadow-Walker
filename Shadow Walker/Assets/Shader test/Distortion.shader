Shader "Custom/Distort"
{
	Properties
	{
		_Noise("Noise", 2D) = "white" {}
		_StrengthFilter("Strength Filter", 2D) = "white" {}
		_Strength("Distort Strength", float) = 1.0
		_Speed("Distort Speed", float) = 1.0
		/*_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpAmt("Distortion", range(0,128)) = 10*/
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"DisableBatching" = "True"
				//"LightMode" = "Always"
			}

			GrabPass
			{
				//Name "BASE"
				"_BackgroundTexture"
				//"LightMode" = "Always"
			}

			Pass
			{
				// Draw on top of everything.
				//ZTest Always
				/*Name "BASE"
				"LightMode" = "Always"*/

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				//#pragma multi_compile_fog
				#include "UnityCG.cginc"

				sampler2D _Noise;
				sampler2D _StrengthFilter;
				sampler2D _BackgroundTexture;
				float     _Strength;
				float     _Speed;

				/*float _BumpAmt;
				float4 _BumpMap_ST;*/

				struct vertexInput
				{
					float4 vertex : POSITION;
					float3 texCoord : TEXCOORD0;
				};

				struct vertexOutput
				{
					float4 pos : SV_POSITION;
					float4 grabPos : TEXCOORD0;
				};

				vertexOutput vert(vertexInput input)
				{
					vertexOutput output;
					float4 pos = input.vertex;
					pos = mul(UNITY_MATRIX_P,
						  mul(UNITY_MATRIX_MV, float4(0, 0, 0, 0.55))
						  + float4(pos.x, pos.z, 0, 0));
						  output.pos = pos;

					output.grabPos = ComputeGrabScreenPos(output.pos);


					float noise = tex2Dlod(_Noise, float4(input.texCoord, 0)).rgb;
					float3 filt = tex2Dlod(_StrengthFilter, float4(input.texCoord, 0)).rgb;
					output.grabPos.x += cos(noise*_Time.x*_Speed) * filt * _Strength;
					output.grabPos.y += sin(noise*_Time.x*_Speed) * filt * _Strength;

					return output;
				}

				float4 frag(vertexOutput input) : COLOR
				{
					return tex2Dproj(_BackgroundTexture, input.grabPos);
				}

			ENDCG
			}

		}
			Fallback "Sprites/Default"
}