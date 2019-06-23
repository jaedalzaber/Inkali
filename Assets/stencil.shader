Shader "Unlit/stencil"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_inColor("Inside Color", Color) = (1,1,0,1)
		_outColor("outside Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent" }
			Cull off
			ZWrite off
			ColorMask 0
			Stencil {
				Ref 0
				Comp Always
				Pass Invert
				Fail Invert
				ZFail Invert
				WriteMask 1
			}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _inColor;
			fixed4 _outColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{   
				if (i.uv.w == 1) {
					float3 px = ddx(i.uv.xyz);
					float3 py = ddy(i.uv.xyz);

					// Chain rule
					float fx = (3 * i.uv.x *i.uv.x)*px.x - (i.uv.z)*px.y - (i.uv.y)*px.z;
					float fy = (3 * i.uv.x *i.uv.x)*py.x - (i.uv.z)*py.y - (i.uv.y)*py.z;
					// Signed distance
					float sd = (i.uv.x * i.uv.x * i.uv.x - i.uv.y * i.uv.z) / sqrt(fx*fx + fy * fy);
					float alpha = 0.2 - sd;
					fixed4 col = fixed4(_inColor.xyz, 1);
					if (alpha < 0)  // Outside
						discard;
					return col;
				 }

				 else if (i.uv.w == 2) {
					 // sample the texture
					 float2 px = ddx(i.uv.xy);
					 float2 py = ddy(i.uv.xy);
					 // Chain rule
					 float fx = (2 * i.uv.x)*px.x - px.y;
					 float fy = (2 * i.uv.x)*py.x - py.y;
					 // Signed distance
					 float sd = (i.uv.x*i.uv.x - i.uv.y) / sqrt(fx*fx + fy * fy);

					 float alpha = 0.5 - sd;
					 if (alpha < 0)  // Outside
						 discard;
					 return _inColor;
				 }

				else if (i.uv.w == 3) {
					if ((i.uv.x * i.uv.x + i.uv.y * i.uv.y) > 1) {
						discard;
					}
					const float pi = 3.14159;
					if (i.uv.x == 0) {
						if (i.uv.z < 90 || i.uv.z < 270)
							discard;
					}
					else {
						if (i.uv.z > 180 && i.uv.z <= 360) {
							float a = pi - (i.uv.z* 0.01745329252) / 2.0;
							float t = pi / 2 - a;
							float h = sin(t);
							float hx = h * cos(a);
							float hy = - h * sin(a);
							float Dsqrt = (hx - i.uv.x) * (hx - i.uv.x) + (hy - i.uv.y) * (hy - i.uv.y);
							float Rsqrt = i.uv.x * i.uv.x + i.uv.y * i.uv.y;
							if (Rsqrt - Dsqrt > h * h)
								discard;
						}
						else if (i.uv.z > 0 && i.uv.z <= 180) {
							float a = i.uv.z* 0.01745329252 / 2.0;
							float t = (pi - i.uv.z* 0.01745329252) / 2.0;
							float h = sin(t);
							float hx = h * cos(a);
							float hy = h * sin(a);
							float Dsqrt = (hx - i.uv.x) * (hx - i.uv.x) + (hy - i.uv.y) * (hy - i.uv.y);
							float Rsqrt = i.uv.x * i.uv.x + i.uv.y * i.uv.y;
							if (Rsqrt - Dsqrt < h * h)
								discard;

						}
					}

					return _inColor;
				}
				 return fixed4(_inColor.xyz, 1);
			}
			ENDCG
		}
	}
}
