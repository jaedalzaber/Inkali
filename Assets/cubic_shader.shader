Shader "Unlit/cubic_shader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_inColor ("Inside Color", Color) = (1,0,0,1)
		_outColor ("outside Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Transparent+1" }
		LOD 100
		Cull off
			ZWrite off
			Blend SrcAlpha OneMinusSrcAlpha
			//ColorMask RGBA
			//ZTest LEqual
			Stencil {
				Ref 1
				Comp Equal
				Pass Zero
				Fail Zero
				ZFail Zero
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				/*if (i.uv.w == 0)
					discard;*/
				if (i.uv.w == 1) {
					float3 px = ddx(i.uv.xyz);
					float3 py = ddy(i.uv.xyz);

					// Chain rule
					float fx = (3 * i.uv.x *i.uv.x)*px.x - (i.uv.z)*px.y - (i.uv.y)*px.z;
					float fy = (3 * i.uv.x *i.uv.x)*py.x - (i.uv.z)*py.y - (i.uv.y)*py.z;
					// Signed distance
					float sd = (i.uv.x * i.uv.x * i.uv.x - i.uv.y * i.uv.z) / sqrt(fx*fx + fy * fy);
					float alpha = .2 - sd;


					if (alpha > 1)       // Inside
						alpha = 1;
					else if (alpha < 0)  // Outside
						discard;
					else
						// Near boundary
						alpha = alpha;
					return fixed4(_inColor.xyz, alpha * _inColor.w);
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
						discard;
					else
						// Near boundary
						alpha = alpha;
					return fixed4(_inColor.xyz, alpha * _inColor.w);
				}
				else if (i.uv.w == 3) {
					/*if ((i.uv.x * i.uv.x + i.uv.y * i.uv.y) > 1) {
						discard;
					}*/
					/*float r = 0.0, delta = 0.0, alpha = 1.0;
					fixed2 cxy = 2.0 * i.uv.xy - 1.0;
					r = dot(cxy, cxy);
					delta = fwidth(r);
					alpha = 1.0 - smoothstep(1.0 - delta, 1.0 + delta, r);*/
					//fixed2 dist = i.uv.xy - 0.5;
					float alpha = 0;
					

					const float pi = 3.14159;
					
					if (i.uv.z > 180 && i.uv.z <= 360) {
						float a = pi - (i.uv.z* 0.01745329252) / 2.0;
						float t = pi / 2 - a;
						float h = sin(t);
						float hx = h * cos(a);
						float hy = -h * sin(a);
						float Dsqrt = (hx - i.uv.x) * (hx - i.uv.x) + (hy - i.uv.y) * (hy - i.uv.y);
						float Rsqrt = i.uv.x * i.uv.x + i.uv.y * i.uv.y;

						float p = (i.uv.z* 0.01745329252)-pi;
						float d = (atan2(i.uv.y, i.uv.x));
						//float d = abs(atan(i.uv.y / i.uv.x));
						float c = pi + d + t - p;
						float r = h / sin(c);

						float dist = distance(i.uv.xy, fixed2(0, 0));
						float delta = fwidth(dist);
						alpha = smoothstep(r-delta, r, dist);
						if((2*pi+d) < (i.uv.z* 0.01745329252) || d > 0){
							float dist = distance(i.uv.xy, fixed2(0, 0));
							float delta = fwidth(dist);
							alpha = smoothstep(1 - delta, 1, dist);
						}
					}

					else if (i.uv.z > 0 && i.uv.z <= 180) {
						float a = i.uv.z* 0.01745329252 / 2.0;
						float t = (pi - i.uv.z* 0.01745329252) / 2.0;
						//float h = sin(t);
						//float hx = h * cos(a);
						//float hy = h * sin(a);
						//float Dsqrt = (hx - i.uv.x) * (hx - i.uv.x) + (hy - i.uv.y) * (hy - i.uv.y);
						//float Rsqrt = i.uv.x * i.uv.x + i.uv.y * i.uv.y;
						//if (Rsqrt - Dsqrt < .2) {
						//	float dist = distance(i.uv.xy, fixed2(0, 0));
						//	float delta = fwidth(dist);
						//	alpha = 1;
						//}

						float h = sin(t);

						float p = (i.uv.z* 0.01745329252);
						float d = (atan2(i.uv.y, i.uv.x));
						//float d = abs(atan(i.uv.y / i.uv.x));
						float c = .5 * ( pi - 2*d + p);
						float r = h / sin(c);

						float dist = distance(i.uv.xy, fixed2(0, 0));
						float delta = fwidth(dist);
							alpha = smoothstep(r+delta, r, dist);
						if (dist >= 1-delta) {
						alpha = smoothstep(1 - delta, 1, dist);
							//return fixed4(0,1,0, ( alpha) * _inColor.w);
						}
					}
					return fixed4(_inColor.xyz, (1-alpha) * _inColor.w);
				}
				return fixed4(_inColor.xyz, 1 * _inColor.w);
			}
			ENDCG
		}
	}
}
