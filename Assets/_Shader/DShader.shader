Shader "Unlit/DefaultShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Highlight("Highlight", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry"}
    
        Pass
        {
            Tags {"LightMode" = "ForwardBase"}
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
                SHADOW_COORDS(2) 
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Highlight;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.normal);
                float3 L = normalize(_WorldSpaceLightPos0);
                float3 Lamb = dot(N, L);
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed shadow = SHADOW_ATTENUATION(i);

                Lamb *= shadow;

                if(Lamb.x >= 0.75) {
                    return _Highlight == 1 ? fixed4(1, 1, 1, 1) :  col;
                }

                if(Lamb.x > 0.25 && Lamb.x < 0.75){
                    return col;
                }
                
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }

        Pass 
		{
			Tags { "LightMode" = "ShadowCaster" }
			
			Fog {Mode Off}
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
	
			struct v2f { 
				V2F_SHADOW_CASTER;
			};
	
			v2f vert( appdata_base v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				return o;
			}
	
			float4 frag( v2f i ) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}

        Pass {
            Tags {"LightMode" = "ForwardAdd"}
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                LIGHTING_COORDS(2, 3)
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 wPos : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o); //lighting
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.normal);
                float3 L = normalize(UnityWorldSpaceLightDir(i.wPos));
                float Lamb = clamp(dot(N, L), 0, 1);
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                 
                if(Lamb.x >= 0.75) {
                    return fixed4(1, 1, 1, 1);
                }

                if(Lamb.x > 0.25 && Lamb.x < 0.75){
                    return col;
                }
                
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }

      
    }
}
