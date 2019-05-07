Shader "Unlit/RematerializeUp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_RematerializeTexture("Rematerialize Texture", 2D) = "white" {}
		_RematerializeY("Current Y of the rematerialize effect", float) = 0
		_RematerializeSize("Size of the effect", float) = 0
		_StartingY("Starting point of the effect", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _RematerializeTexture;
			float _RematerializeY;
			float _RematerializeSize;
			float _StartingY;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//Added this part, calculating world position
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float transition = _RematerializeY - i.worldPos.y;
				clip((_StartingY + transition));

                fixed4 col = tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
        }
    }
}
