Shader "Custom/Crt pixel"
{
	Properties
	{
		_MainTex("Texture Pixel", 2D) = "white" {}
		_ScansColor("Color Scans", COLOR) = (1,1,1,1)
		_Tiles("Tiles value", Range(1, 10000)) = 500
		_VertsColor("Verts fill color", Range(0, 1)) = 0
		_VertsColor2("Verts fill color 2", Range(0, 1)) = 0
		_Contrast("Contrast", Range(-20, 20)) = 0
		_Br("Brightness", Range(-200, 200)) = 0
		_Density("Density", Range(0, 30)) = 0

		_Strength("Strenght curve", Range(-0.035, 30)) = 0.0
		_DisplacementTex("Displace Text", 2D) = "white" {}
		_Distort("Distortion", Range(-0.1, 0.1)) = 0.0
		_OnOff("OnOff", Range(0.0, 1000.0)) = 1.0
		_ScanDensity("Scan density", Range(0.0, 1.0)) = 1.0
		_ScanThikness("Scan thick", Range(0.0, 500.0)) = 100.0
		_ScanPoint("Scan point", Range(-200, 2000)) = 0.0
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

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
					float4 scr_pos : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				float _Tiles;
				sampler2D _MainTex;
				float _VertsColor;
				float _VertsColor2;
				float _Contrast;
				float _Br;
				float4 _ScansColor;
				float _Density;
				float _Strength;
				sampler2D _DisplacementTex;
				float _Distort;
				float _OnOff;
				float _ScanDensity;
				float _ScanThikness;
				float _ScanPoint;

				v2f vert(appdata v)
				{
					v2f o;

					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.scr_pos = ComputeScreenPos(o.vertex);
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					///Curvature
					half2 n = tex2D(_DisplacementTex, i.uv);
					half2 d = n * 2 - 1;
					i.uv += d * _Strength;
					i.uv = saturate(i.uv);
					///

					//Distort image on y axis
					i.uv.y += _Distort;

					float4 color = tex2D(_MainTex, i.uv);

					//Vertical lines
					float2 ps = i.scr_pos.xy * _ScreenParams.xy / i.scr_pos.w;
					int pp = (int)ps.x % _Density;
					float4 outcolor = float4(0, 0, 0, 1);
					float4 muls = float4(0, 0, 0, 1);
					if (pp < _Density / 3) {
						outcolor.r = color.r;
						outcolor.g = color.g*_VertsColor;
						outcolor.b = color.b*_VertsColor2;
					}
					else if (pp < (2 * _Density) / 3) {
						outcolor.g = color.g;
						outcolor.r = color.r*_VertsColor;
						outcolor.b = color.b*_VertsColor2;
					}
					else {
						outcolor.b = color.b;
						outcolor.r = color.r*_VertsColor;
						outcolor.g = color.g*_VertsColor2;
					}

					//Horizontal lines
					if ((int)ps.y % _Density == 0) outcolor *= float4(_ScansColor.r, _ScansColor.g, _ScansColor.b, 1);

					//Color correciton
					outcolor += (_Br / 255);
					outcolor = outcolor - _Contrast * (outcolor - 1.0) * outcolor *(outcolor - 0.5);

					//Scan lines
					if ((i.scr_pos.y *_ScreenParams.y) >= _ScanPoint && (i.scr_pos.y *_ScreenParams.y) < _ScanPoint + _ScanThikness)
					{
						outcolor *= _ScanDensity;
					}

					return outcolor;
				}
				ENDCG
			}
		}
}