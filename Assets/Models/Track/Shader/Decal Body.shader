Shader "MRFusion/Decal Body" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color ("Main Color", Color) = (1,1,1,1)
		_ColOut ("Outer Color", Color) = (1,1,1,1)
		_GlossOut ("Outer Gloss", Range(0,1)) = 0.97
		_ColIn ("Inner Color", Color) = (1,1,1,1)
		_GlossIn ("Inner Gloss", Range (0,1)) = 0.95
		_Highlight ("Highlight", Range(0,0.5)) = 0.1
		_ScratchTex ("Scratch Texture", 2D) = "black" {}
		_ScratchIntensity ("Scratch Intensity", Range(0,1)) = 0.5
		_DecalTex ("Decal Texture", 2D) = "black" {}
		_DecalBlend ("Decal Blend", Range(0,1)) = 0.8
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex;
			float2 uv_ScratchTex;
			float2 uv_DecalTex;
		};

		float4 _Color;
		float4 _ColOut;
		float _GlossOut;
		float4 _ColIn;
		float _GlossIn;
		float _Highlight;
		float _ScratchIntensity;
		float _DecalBlend;
		sampler2D _MainTex;
		sampler2D _ScratchTex;
		sampler2D _DecalTex;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo and main texture
			fixed4 mainTexColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			// Outer and inner colors influence
			float highlightFactor = saturate(_Highlight);
			fixed4 blendedColor = lerp(_ColIn, _ColOut, highlightFactor);
			o.Albedo = mainTexColor.rgb * blendedColor.rgb;

			// Outer and inner gloss
			o.Smoothness = lerp(_GlossIn, _GlossOut, highlightFactor);

			// Add scratches
			fixed4 scratch = tex2D(_ScratchTex, IN.uv_ScratchTex);
			o.Albedo = lerp(o.Albedo, scratch.rgb, _ScratchIntensity * scratch.a);

			// Add decals
			fixed4 decal = tex2D(_DecalTex, IN.uv_DecalTex);
			o.Albedo = lerp(o.Albedo, decal.rgb, _DecalBlend * decal.a);
		}
		ENDCG
	}
	Fallback "Diffuse"
}
