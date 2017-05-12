//INPUT
float4x4 xWorld;
float4x4 xViewProjection;
float4x4 xInvViewProjection;
float3 xCameraPosition;
texture2D xColorMap;
texture2D xNormalMap;
texture2D xSpecularMap;
texture2D xDepthMap;
texture2D xShadowMap;
texture2D xLightMap;
texture2D xPreviousLightMap;
texture2D xReflectMap;
texture2D xPreviousReflectMap;
float2 xHalfPixel;
//LIGHT
float4 xLightColor;
float xLightIntensity;
float xLightSpecFactor;
float xLightSpecPower;
float3 xLightPosition;
float3 xLightDirection;
float xLightAngle;
float xLightDecay;
float4x4 xLightViewProjection;
float xLightRadius;
float xLightAttC;
float xLightAttL;
float xLightAttQ;
//MATERIAL
float xMaterialAmbient; //ambient factor
float4 xMaterialColor;
float xMaterialDiffuse; //diffuse factor
float xMaterialShininess; //specular power
float4 xMaterialMatte; //specular color

sampler2D ColorMapSampler = sampler_state
{
	Texture = <xColorMap>;
};
sampler2D NormalMapSampler = sampler_state
{
	Texture = <xNormalMap>;
};
sampler2D SpecularMapSampler = sampler_state
{
	Texture = <xSpecularMap>;
};
sampler2D DepthMapSampler = sampler_state
{
	Texture = <xDepthMap>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};
sampler2D ShadowMapSampler = sampler_state
{
	texture = <xShadowMap>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = CLAMP;
	AddressV = CLAMP;
};
sampler2D LightMapSampler = sampler_state
{
	Texture = <xLightMap>;
};
sampler2D PreviousLightMapSampler = sampler_state
{
	Texture = <xPreviousLightMap>;
};
sampler2D ReflectMapSampler = sampler_state
{
	Texture = <xReflectMap>;
};
sampler2D PreviousReflectMapSampler = sampler_state
{
	Texture = <xPreviousReflectMap>;
};

struct AppToVertex_Scene
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL0;
	float3 Binormal : BINORMAL0;
	float3 Tangent : TANGENT0;
};
struct AppToVertex_Normal
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
};
struct AppToVertex_Texture
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};
struct AppToVertex
{
	float4 Position : POSITION0;
};
struct VertexToPixel_Scene
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 Depth : TEXCOORD1;
	float3x3 WorldToTangentSpace : TEXCOORD2;
};
struct VertexToPixel_NoTx
{
	float4 Position : POSITION0;
	float2 Depth : TEXCOORD0;
	float3 Normal : TEXCOORD1;
};
struct VertexToPixel_Normal
{
	float4 Position : POSITION0;
	float3 Normal : TEXCOORD0;
};
struct VertexToPixel_Shadow
{
	float4 Position : POSITION;
	float2 Depth : TEXCOORD0;
};
struct VertexToPixel
{
	float4 Position		: POSITION;
	float2 TexCoord		: TEXCOORD0;
};
struct PixelToFrame_Light
{
	float4 Light : COLOR0;
	float4 Specular : COLOR1;
};
struct PixelToFrame
{
	float4 Color : COLOR0;
};
struct PixelToFrame_Scene
{
	half4 Color : COLOR0;
	half4 Normal : COLOR1;
	half4 Specular : COLOR2;
	half4 Depth : COLOR3;
};

VertexToPixel_Scene VS_Scene(AppToVertex_Scene input)
{
	VertexToPixel_Scene output;
	float4x4 worldViewProjection = mul(xWorld, xViewProjection);
	output.Position = mul(input.Position, worldViewProjection);
	output.TexCoord = input.TexCoord;
	output.WorldToTangentSpace[0] = normalize(mul(input.Tangent, xWorld));
	output.WorldToTangentSpace[1] = normalize(mul(input.Binormal, xWorld));
	output.WorldToTangentSpace[2] = normalize(mul(input.Normal, xWorld));
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;
	return output;
}
VertexToPixel_NoTx VS_NoTexture(AppToVertex_Normal input)
{
	VertexToPixel_NoTx output;
	float4x4 worldViewProjection = mul(xWorld, xViewProjection);
	output.Position = mul(input.Position, worldViewProjection);
	output.Normal = normalize(mul(input.Normal, xWorld));
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;
	return output;
}
VertexToPixel_Normal VS_Basic_NoTx(AppToVertex_Normal input)
{
	VertexToPixel_Normal output;
	float4x4 worldViewProjection = mul(xWorld, xViewProjection);
	output.Position = mul(input.Position, worldViewProjection);
	output.Normal = normalize(mul(input.Normal, xWorld));
	return output;
}
VertexToPixel VS_Texture(AppToVertex_Texture input)
{
	VertexToPixel output;
	output.Position = float4(input.Position.xyz, 1);
	output.TexCoord = input.TexCoord + xHalfPixel;
	return output;
}
VertexToPixel_Shadow VS_Shadow(AppToVertex input)
{
	VertexToPixel_Shadow output;
	float4x4 LightWorldViewProjection = mul(xWorld, xLightViewProjection);

	output.Position = mul(input.Position, LightWorldViewProjection);
	output.Depth.x = output.Position.z;
	output.Depth.y = output.Position.w;
	return output;
}
PixelToFrame_Scene PS_Scene(VertexToPixel_Scene input)
{
	PixelToFrame_Scene output;

	//Map the color data output.Color.RGB
	output.Color = xMaterialColor * tex2D(ColorMapSampler, input.TexCoord);

	//Map the normal data to output.Normal.RGB
	float3 normalMap = 2.0 *(tex2D(NormalMapSampler, input.TexCoord)) - 1.0;
	normalMap = 0.5f * (normalize(mul(normalMap, input.WorldToTangentSpace)) + 1);
	output.Normal = float4(normalMap, 1.0);

	//Map the specular data to output.Specular.RGB
	output.Specular = xMaterialMatte;

	//Map material data to open data slots
	output.Color.a = xMaterialAmbient;
	output.Normal.a = xMaterialDiffuse;
	output.Specular.a *= xMaterialShininess;

	//Map depth data
	output.Depth = input.Depth.x / input.Depth.y;
	return output;
}
PixelToFrame_Scene PS_NoTexture(VertexToPixel_NoTx input)
{
	PixelToFrame_Scene output;

	//Map the color data output.Color.RGB
	output.Color = xMaterialColor;

	//Map the normal data to output.Normal.RGB
	float3 normalMap = 0.5f * input.Normal + 1;
	output.Normal = float4(normalMap, 1.0);

	//Map the specular data to output.Specular.RGB
	output.Specular = xMaterialMatte;

	//Map material data to open data slots
	output.Color.a = xMaterialAmbient;
	output.Normal.a = xMaterialDiffuse;
	output.Specular.a *= xMaterialShininess;

	//Map depth data
	output.Depth = input.Depth.x / input.Depth.y;
	return output;
}
PixelToFrame PS_Basic_NoTx(VertexToPixel_Normal input)
{
	PixelToFrame output;
	float3 lightDir = float3(1, -1, 0);
	float3 diffuse = saturate(dot(input.Normal, -lightDir));
	output.Color = float4(xMaterialColor.rgb * (diffuse + xMaterialAmbient), xMaterialColor.a);
	return output;
}
PixelToFrame PS_Black(VertexToPixel_Normal input)
{
	PixelToFrame output;
	output.Color = 0;
	return output;
}
PixelToFrame PS_Shadow(VertexToPixel_Shadow input)
{
	PixelToFrame output;
	output.Color = input.Depth.x / input.Depth.y;
	return output;
}
PixelToFrame_Light PS_Light_Directional(VertexToPixel input)
{
	PixelToFrame_Light output;
	//Extract Normal data from Render Target
	float4 normalData = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize(normalData.xyz*2.0f - 1.0f);
	//Extract diffuse factor from normal data
	float4 diffuseFactor = normalData.a * xLightIntensity * xLightColor;
	//Extract Specular data from Render Target
	float4 specularData = tex2D(SpecularMapSampler, input.TexCoord);
	float3 specularColor = xLightSpecFactor * specularData.rgb;
	float specularPower = xLightSpecPower * specularData.a * 255;
	//Extract Depth data from Render Target
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;

	//Extrapolate 3D position from this pixel's screen position and depth
	float4 screenPosition;
	screenPosition.x = input.TexCoord.x*2.0f - 1.0f;
	screenPosition.y = -(input.TexCoord.y*2.0f - 1.0f);
	screenPosition.z = depth;
	screenPosition.w = 1.0f;
	float4 worldPosition = mul(screenPosition, xInvViewProjection);
	worldPosition /= worldPosition.w;

	//Phong calculations
	float3 viewDir = normalize(xCameraPosition - worldPosition.xyz);
	float3 reflection = normalize(reflect(xLightDirection, normal));
	float3 diffuse = diffuseFactor.rgb * saturate(dot(normal, -xLightDirection));
	float3 specular = specularColor * pow(saturate(dot(reflection, viewDir)), specularPower);

	//Extract previous maps
	float4 previousLight = tex2D(PreviousLightMapSampler, input.TexCoord);
	float4 previousReflect = tex2D(PreviousReflectMapSampler, input.TexCoord);

	//Add to previous maps
	output.Light.rgb = previousLight.rgb + diffuse.rgb;
	output.Light.a = 1.0f;
	output.Specular.rgb = previousReflect.rgb + specular.rgb;
	output.Specular.a = 1.0f;
	return output;
}
PixelToFrame_Light PS_Light_Ambient(VertexToPixel input)
{
	PixelToFrame_Light output;
	//Extract Normal data from Render Target
	float4 normalData = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize(normalData.xyz*2.0f - 1.0f);
	//Extract diffuse factor from normal data
	float4 diffuseFactor = normalData.a * xLightIntensity * xLightColor;
	//Extract Specular data from Render Target
	float4 specularData = tex2D(SpecularMapSampler, input.TexCoord);
	float3 specularColor = xLightSpecFactor * specularData.rgb;
	float specularPower = xLightSpecPower * specularData.a * 255;
	//Extract Depth data from Render Target
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;

	//Extrapolate 3D position from this pixel's screen position and depth
	float4 screenPosition;
	screenPosition.x = input.TexCoord.x*2.0f - 1.0f;
	screenPosition.y = -(input.TexCoord.y*2.0f - 1.0f);
	screenPosition.z = depth;
	screenPosition.w = 1.0f;
	float4 worldPosition = mul(screenPosition, xInvViewProjection);
	worldPosition /= worldPosition.w;

	//Phong calculations
	float3 viewDir = normalize(xCameraPosition - worldPosition.xyz);
	float3 lightDir = normalize(worldPosition - xCameraPosition);
	float3 reflection = normalize(reflect(lightDir, normal));
	float3 diffuse = diffuseFactor.rgb * saturate(dot(normal, -lightDir));
	float3 specular = specularColor * pow(saturate(dot(reflection, viewDir)), specularPower);

	//Extract previous maps
	float4 previousLight = tex2D(PreviousLightMapSampler, input.TexCoord);
	float4 previousReflect = tex2D(PreviousReflectMapSampler, input.TexCoord);

	//Add to previous maps
	output.Light.rgb = previousLight.rgb + diffuse.rgb;
	output.Light.a = 1.0f;
	output.Specular.rgb = previousReflect.rgb + specular.rgb;
	output.Specular.a = 1.0f;
	return output;
}
PixelToFrame_Light PS_Light_Point(VertexToPixel input)
{
	PixelToFrame_Light output;
	float4 color = float4(0, 0, 0, 1);
	float4 spec = float4(0, 0, 0, 1);
	//Extract Normal data from Render Target
	float4 normalData = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize(normalData.xyz*2.0f - 1.0f);
	//Extract diffuse factor from normal data
	float4 diffuseFactor = normalData.a * xLightIntensity * xLightColor;
	//Extract Specular data from Render Target
	float4 specularData = tex2D(SpecularMapSampler, input.TexCoord);
	float3 specularColor = xLightSpecFactor * specularData.rgb;
	float specularPower = xLightSpecPower * specularData.a * 255;
	//Extract Depth data from Render Target
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;

	//Extrapolate 3D position from this pixel's screen position and depth
	float4 screenPosition;
	screenPosition.x = input.TexCoord.x*2.0f - 1.0f;
	screenPosition.y = -(input.TexCoord.y*2.0f - 1.0f);
	screenPosition.z = depth;
	screenPosition.w = 1.0f;
	float4 worldPosition = mul(screenPosition, xInvViewProjection);
	worldPosition /= worldPosition.w;

	//Point light calculations
	float3 lightVector = xLightPosition - worldPosition;
	float d = length(lightVector);
	float3 lightDir = normalize(lightVector);
	float attFactors = (xLightAttC + d * xLightAttL + d * d * xLightAttQ);
	float att = saturate(1.0f - attFactors / xLightRadius);

	//Phong calculations
	float3 viewDir = normalize(xCameraPosition - worldPosition.xyz);
	float3 reflection = normalize(reflect(xLightDirection, normal));
	float3 diffuse = att * diffuseFactor.rgb * saturate(dot(normal, lightDir));
	float3 specular = att * specularColor * pow(saturate(dot(reflection, viewDir)), specularPower);

	//Add to previous maps
	color.rgb = diffuse.rgb;
	spec.rgb = specular.rgb;

	//Extract previous maps
	float4 previousLight = tex2D(PreviousLightMapSampler, input.TexCoord);
	float4 previousReflect = tex2D(PreviousReflectMapSampler, input.TexCoord);
	color.rgb += previousLight.rgb;
	spec.rgb += previousReflect.rgb;

	output.Light = color;
	output.Specular = spec;
	return output;
}
PixelToFrame_Light PS_Light_Spot(VertexToPixel input)
{
	PixelToFrame_Light output;
	float4 color = float4(0, 0, 0, 1);
	float4 spec = float4(0, 0, 0, 1);
	//Extract Normal data from Render Target
	float4 normalData = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize(normalData.xyz*2.0f - 1.0f);
	//Extract diffuse factor from normal data
	float4 diffuseFactor = normalData.a * xLightIntensity * xLightColor;
	//Extract Specular data from Render Target
	float4 specularData = tex2D(SpecularMapSampler, input.TexCoord);
	float3 specularColor = xLightSpecFactor * specularData.rgb;
	float specularPower = xLightSpecPower * specularData.a * 255;
	//Extract Depth data from Render Target
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;

	//Extrapolate 3D position from this pixel's screen position and depth
	float4 screenPosition;
	screenPosition.x = input.TexCoord.x*2.0f - 1.0f;
	screenPosition.y = -(input.TexCoord.y*2.0f - 1.0f);
	screenPosition.z = depth;
	screenPosition.w = 1.0f;
	float4 worldPosition = mul(screenPosition, xInvViewProjection);
	worldPosition /= worldPosition.w;

	//determine cone criteria
	float3 lightVector = xLightPosition - worldPosition;
	float3 lightDir = normalize(lightVector);
	float spotEffect = dot(-lightDir, normalize(xLightDirection));
	bool coneCondition = xLightAngle <= spotEffect;

	if (coneCondition)
	{
		float attFactors = (xLightAttC + spotEffect * xLightAttL + spotEffect * spotEffect * xLightAttQ);
		float att = 1 - pow(saturate(xLightAngle / attFactors), xLightDecay);

		//Phong calculations
		float3 viewDir = normalize(xCameraPosition - worldPosition.xyz);
		float3 reflection = normalize(reflect(xLightDirection, normal));
		float3 diffuse = att * diffuseFactor.rgb * saturate(dot(normal, lightDir));
		float3 specular = att * specularColor * pow(saturate(dot(reflection, viewDir)), specularPower);

		color.rgb = diffuse.rgb;
		spec.rgb = specular.rgb;
	}

	float4 previousLight = tex2D(PreviousLightMapSampler, input.TexCoord);
	float4 previousReflect = tex2D(PreviousReflectMapSampler, input.TexCoord);
	color.rgb += previousLight.rgb;
	spec.rgb += previousReflect.rgb;

	output.Light = color;
	output.Specular = spec;
	return output;
}
PixelToFrame_Light PS_Light_Spot2(VertexToPixel input)
{
	PixelToFrame_Light output;
	float4 color = float4(0, 0, 0, 1);
	float4 spec = float4(0, 0, 0, 1);
	//Extract Normal data from Render Target
	float4 normalData = tex2D(NormalMapSampler, input.TexCoord);
	float3 normal = normalize(normalData.xyz*2.0f - 1.0f);
	//Extract diffuse factor from normal data
	float4 diffuseFactor = normalData.a * xLightIntensity * xLightColor;
	//Extract Specular data from Render Target
	float4 specularData = tex2D(SpecularMapSampler, input.TexCoord);
	float3 specularColor = xLightSpecFactor * specularData.rgb;
	float specularPower = xLightSpecPower * specularData.a * 255;
	//Extract Depth data from Render Target
	float depth = tex2D(DepthMapSampler, input.TexCoord).r;

	//Extrapolate 3D position from this pixel's screen position and depth
	float4 screenPosition;
	screenPosition.x = input.TexCoord.x*2.0f - 1.0f;
	screenPosition.y = -(input.TexCoord.y*2.0f - 1.0f);
	screenPosition.z = depth;
	screenPosition.w = 1.0f;
	float4 worldPosition = mul(screenPosition, xInvViewProjection);
	worldPosition /= worldPosition.w;

	//Find the screen position of this pixel as seen by the light
	float4 lightScreenPos = mul(worldPosition, xLightViewProjection);
	lightScreenPos /= lightScreenPos.w;

	//find sample position in shadow map
	float2 lightSamplePos;
	lightSamplePos.x = lightScreenPos.x / 2.0f + 0.5f;
	lightSamplePos.y = (-lightScreenPos.y / 2.0f + 0.5f);

	//determine shadowing criteria
	float realDistance = lightScreenPos.z;
	float shadowDepth = tex2D(ShadowMapSampler, lightSamplePos).r;
	bool shadowCondition = shadowDepth <= realDistance - 0.0005f;

	//determine cone criteria
	float3 lightVector = xLightPosition - worldPosition;
	float3 lightDir = normalize(lightVector);
	float spotEffect = dot(-lightDir, normalize(xLightDirection));
	bool coneCondition = xLightAngle <= spotEffect;

	if (coneCondition && !shadowCondition)
	{
		float attFactors = (xLightAttC + spotEffect * xLightAttL + spotEffect * spotEffect * xLightAttQ);
		float att = 1 - pow(saturate(xLightAngle / attFactors), xLightDecay);

		//Phong calculations
		float3 viewDir = normalize(xCameraPosition - worldPosition.xyz);
		float3 reflection = normalize(reflect(xLightDirection, normal));
		float3 diffuse = att * diffuseFactor.rgb * saturate(dot(normal, lightDir));
		float3 specular = att * specularColor * pow(saturate(dot(reflection, viewDir)), specularPower);

		color.rgb = diffuse.rgb;
		spec.rgb = specular.rgb;

		////Shape the light with a texture
		//float lightTextureFactor = tex2D(CarLightSampler, ProjectedTexCoords).r;
		//diffuseLightingFactor *= lightTextureFactor;
	}
	
	float4 previousLight = tex2D(PreviousLightMapSampler, input.TexCoord);
	float4 previousReflect = tex2D(PreviousReflectMapSampler, input.TexCoord);
	color.rgb += previousLight.rgb;
	spec.rgb += previousReflect.rgb;

	output.Light = color;
	output.Specular = spec;
	return output;
}
PixelToFrame PS_Final(VertexToPixel input)
{
	PixelToFrame output;
	//Extract Color from Render Target
	float4 colorData = tex2D(ColorMapSampler, input.TexCoord);
	float3 color = colorData.rgb;
	float materialAmbient = colorData.a;
	//Ignore lighting if magic number 1
	if (materialAmbient == 1)
	{
		output.Color = colorData;
		return output;
	}
	//Extract Light from Render Target
	float3 light = tex2D(LightMapSampler, input.TexCoord).rgb;
	//Extract Specular from Render Target
	float3 specular = tex2D(ReflectMapSampler, input.TexCoord).rgb;
	//Build Ambient light
	float4 ambient = xLightColor * (xLightIntensity + materialAmbient);
	light.rgb += ambient.rgb;

	output.Color.rgb = color * light.rgb + specular.rgb;
	output.Color.a = xMaterialColor.a;
	return output;
}
PixelToFrame PS_Texture(VertexToPixel input)
{
	PixelToFrame output;
	float4 colorData = tex2D(ColorMapSampler, input.TexCoord);
	float3 color = colorData.rgb;
	output.Color.rgb = color;
	output.Color.a = 1.0f;
	return output;
}
PixelToFrame PS_TextureBlend(VertexToPixel input)
{
	PixelToFrame output;
	float4 colorData1 = tex2D(ColorMapSampler, input.TexCoord);
	float3 color1 = colorData1.rgb;

	float4 colorData2 = tex2D(NormalMapSampler, input.TexCoord);
	float3 color2 = colorData2.rgb;
	float alpha = colorData2.a;
	
	output.Color.rgb = color1 * (1 - alpha) + color2 * alpha;
	output.Color.a = 1.0f;
	return output;
}

technique BasicNoTexture
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Basic_NoTx();
		PixelShader = compile ps_2_0 PS_Basic_NoTx();
	}
};
technique NoTexture
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_NoTexture();
		PixelShader = compile ps_2_0 PS_NoTexture();
	}
}
technique Black
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Basic_NoTx();
		PixelShader = compile ps_2_0 PS_Black();
	}
}
technique Texture
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Texture();
	}
}
technique Scene
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Scene();
		PixelShader = compile ps_2_0 PS_Scene();
	}
}
technique Shadow
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Shadow();
		PixelShader = compile ps_2_0 PS_Shadow();
	}
}
technique Final
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Final();
	}
}
technique Spot
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Light_Spot();
	}
}
technique Spot2
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VS_Texture();
		PixelShader = compile ps_3_0 PS_Light_Spot2();
	}
}
technique Directional
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Light_Directional();
	}
}
technique Ambient
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Light_Ambient();
	}
}
technique Point
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_Light_Point();
	}
}
technique TextureBlend
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VS_Texture();
		PixelShader = compile ps_2_0 PS_TextureBlend();
	}
};
