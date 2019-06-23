Shader "Unlit/quad"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_inColor ("Inside Color", Color) = (1,0,0,1)
		_outColor ("outside Color", Color) = (0,1,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
			Cull off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _inColor;
			fixed4 _outColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				  float2 px = ddx(i.uv.xy);
				  float2 py = ddy(i.uv.xy);
				  // Chain rule
				   float fx = (2 * i.uv.x)*px.x - px.y;
				  float fy = (2 * i.uv.x)*py.x - py.y;
				  // Signed distance
				   float sd = (i.uv.x*i.uv.x - i.uv.y) / sqrt(fx*fx + fy * fy);

				fixed4 col = _inColor;
				//float alpha = 1;
				///*if (sd > -0.01)
				//	alpha = .5;*/
				//if(sd > 0.0 )
				//	alpha = 0;
				float alpha = 0.5 - sd;
				if (alpha > 1)       // Inside
					alpha = 1;
				else if (alpha < 0)  // Outside
					alpha = 0.0;
				else
					// Near boundary
					alpha = alpha;
				return fixed4(_inColor.xyz, alpha);
			}
			ENDCG
		}
	}
}
