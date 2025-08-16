Shader "Projector/Multiply"
{
  Properties
  {
    _ShadowTex ("Cookie", 2D) = "gray" {}
    _FalloffTex ("FallOff", 2D) = "white" {}
  }
  SubShader
  {
    Tags
    { 
      "QUEUE" = "Transparent"
    }
    Pass // ind: 1, name: 
    {
      Tags
      { 
        "QUEUE" = "Transparent"
      }
      ZWrite Off
      Offset -1, -1
      Blend DstColor Zero
      ColorMask RGB
      // m_ProgramMask = 6
      CGPROGRAM
      //#pragma target 4.0
      
      #pragma vertex vert
      #pragma fragment frag
      
      #include "UnityCG.cginc"
      
      
      #define CODE_BLOCK_VERTEX
      //uniform float4x4 unity_ObjectToWorld;
      //uniform float4x4 unity_MatrixVP;
      uniform float4x4 unity_Projector;
      uniform float4x4 unity_ProjectorClip;
      uniform sampler2D _ShadowTex;
      uniform sampler2D _FalloffTex;
      struct appdata_t
      {
          float4 vertex :POSITION0;
      };
      
      struct OUT_Data_Vert
      {
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
          float4 vertex :SV_POSITION;
      };
      
      struct v2f
      {
          float4 texcoord :TEXCOORD0;
          float4 texcoord1 :TEXCOORD1;
      };
      
      struct OUT_Data_Frag
      {
          float4 color :SV_Target0;
      };
      
      float4 u_xlat0;
      float4 u_xlat1;
      OUT_Data_Vert vert(appdata_t in_v)
      {
          OUT_Data_Vert out_v;
          out_v.texcoord = mul(unity_Projector, in_v.vertex);
          out_v.texcoord1 = mul(unity_ProjectorClip, in_v.vertex);
          out_v.vertex = UnityObjectToClipPos(in_v.vertex);
          return out_v;
      }
      
      #define CODE_BLOCK_FRAGMENT
      float2 u_xlat0_d;
      float u_xlat10_0;
      float4 u_xlat10_1;
      float4 u_xlat16_2;
      float2 u_xlat3;
      OUT_Data_Frag frag(v2f in_f)
      {
          OUT_Data_Frag out_f;
          u_xlat0_d.xy = (in_f.texcoord1.xy / in_f.texcoord1.ww);
          u_xlat10_0 = tex2D(_FalloffTex, u_xlat0_d.xy).w;
          u_xlat3.xy = (in_f.texcoord.xy / in_f.texcoord.ww);
          u_xlat10_1 = tex2D(_ShadowTex, u_xlat3.xy);
          u_xlat16_2.xyz = (u_xlat10_1.xyz + float3(-1, (-1), (-1)));
          u_xlat16_2.w = ((-u_xlat10_1.w) + 1);
          out_f.color = ((float4(u_xlat10_0, u_xlat10_0, u_xlat10_0, u_xlat10_0) * u_xlat16_2) + float4(1, 1, 1, 0));
          return out_f;
      }
      
      
      ENDCG
      
    } // end phase
  }
  FallBack Off
}
