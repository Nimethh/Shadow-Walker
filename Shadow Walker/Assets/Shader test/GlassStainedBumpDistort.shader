// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "CookbookShaders/Ch06/StainedGlass"
{
	Properties
	{
		_MainTex("Base (RGBA)", 2D) = "white" {}
		_BumpMap("Noise Texture", 2D) = "bump" {}
		_Magnitude("Magnitude", Range(0,1)) = 0.05
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Opaque"
			}

			ZWrite On Lighting Off Cull Off Fog{ Mode Off } Blend One Zero

			GrabPass 
			{
			}

			Pass
			{
				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				sampler2D _MainTex;
				sampler2D _BumpMap;
				float _Magnitude;

				sampler2D _GrabTexture;

				struct vertInput
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct vertOutput
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					float4 uvgrab : TEXCOORD1;
				};

				vertOutput vert(vertInput v)
				{
					vertOutput o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = v.texcoord;
					o.uvgrab = ComputeGrabScreenPos(o.vertex);	// grab screen texture
					return o;
				}

				half4 frag(vertOutput i) : COLOR
				{
					half4 mainColor = tex2D(_MainTex, i.texcoord);

					half4 bump = tex2D(_BumpMap, i.texcoord);
					half2 distortion = UnpackNormal(bump).rg;

					i.uvgrab.xy += distortion * _Magnitude;	// distortion: offset the UV data of the grab texture

					fixed4 col = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.uvgrab));	// apply the screen texture in the correct position
					return col * mainColor;
				}

				ENDCG
			}
		}
}