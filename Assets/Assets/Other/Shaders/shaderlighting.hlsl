half3 LightingHalfLambert(half3 lightColor, half3 lightDir, half3 normal)
{
    half NdotL = saturate(dot(normal, lightDir)) * 0.5 + 0.5;
    return lightColor * NdotL;
}

void AllLights_float(float3 WorldPos, float3 WorldNormal, out float3 Color)
{
#if SHADERGRAPH_PREVIEW
   Color = WorldPos.xxx * 2;
#else

#if SHADOWS_SCREEN
   half4 clipPos = TransformWorldToHClip(WorldPos);
   half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    
    WorldNormal = normalize(WorldNormal);
    
    // Do main light
    Light light = GetMainLight(shadowCoord);
    float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    float3 diffuseColor = LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        light = GetAdditionalLight(i, shadowCoord);
        attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    }
    Color = diffuseColor;
#endif
}

void AllLightsWithSpecular_float(float3 WorldPos, float3 WorldNormal, float3 WorldViewDirection, float3 SpecularSurfaceColor, float1 Smoothness, out float3 DiffuseColor, out float3 SpecularColor)
{
#if SHADERGRAPH_PREVIEW
   DiffuseColor = WorldPos.xxx * 2;
   SpecularColor = SpecularSurfaceColor * WorldPos.yyy * 2;
#else

#if SHADOWS_SCREEN
   half4 clipPos = TransformWorldToHClip(WorldPos);
   half4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
    
    WorldNormal = normalize(WorldNormal);
    
    Smoothness = exp2(10 * Smoothness + 1);
    WorldViewDirection = SafeNormalize(WorldViewDirection);
    
    // Do main light
    Light light = GetMainLight(shadowCoord);
    float3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
    float3 diffuseColor = LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    float3 specularColor = LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldViewDirection, float4(SpecularSurfaceColor, 0), Smoothness);
    
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; i++)
    {
        light = GetAdditionalLight(i, shadowCoord);
        attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
        specularColor += LightingSpecular(attenuatedLightColor, light.direction, WorldNormal, WorldViewDirection, float4(SpecularSurfaceColor, 0), Smoothness);
    }
    DiffuseColor = diffuseColor;
    SpecularColor = specularColor;
#endif
}