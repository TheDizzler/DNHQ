Shader "Tutorial/GlobalDiffuseShader"
{
	SubShader
	{
		Pass
		{
			Tags {
			"Queue" = "Transparent"
			"LightMode" = "ForwardBase" }

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma target 3.0
			#pragma vertex vertexShader
			#pragma fragment fragmentShader

			float4 _LightColor0;

			struct vsIn
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
			};

			struct vsOut
			{
				float4 position : SV_POSITION;
				float3 normal : NORMAL;
			};

			vsOut vertexShader(vsIn v)
			{
				vsOut o;
				o.position = UnityObjectToClipPos(v.position);
				if (o.position.x > 0)
				{
					o.position.x += _Time / (o.position.y);

				}
				/*else if (o.position.x < 0)
				{
					o.position.x -= (o.position.y );
				}*/
				o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);

				return o;
			}

			float4 fragmentShader(vsOut psIn) : SV_Target
			{
				float4 ambientLight = UNITY_LIGHTMODEL_AMBIENT;
				float4 lightDirection = normalize(_WorldSpaceLightPos0);
				float4 diffuseTerm = saturate(dot(lightDirection, psIn.normal));
				float4 diffuseLight = diffuseTerm * _LightColor0;
				float4 dealpha = float4(1, 1, 1, .14f);
				return (ambientLight + diffuseLight) * dealpha;
			}

			ENDCG
		}
	}
}
