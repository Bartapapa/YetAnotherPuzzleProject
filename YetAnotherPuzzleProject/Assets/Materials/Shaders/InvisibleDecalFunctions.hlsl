#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

#pragma multi_compile _ _LIGHT_LAYERS

void RevealUsingLayer_float(float3 WorldPosition, float UltravioletLayerIndex, out float Out)
{
	half4 Shadowmask = half4(1, 1, 1, 1);
	float totalAtten = 0;
#ifndef SHADERGRAPH_PREVIEW
	uint pixelLightCount = GetAdditionalLightsCount();
	uint ultravioletLayerMask = 1 << int(UltravioletLayerIndex);

	InputData inputData = (InputData) 0;
	float4 screenPos = ComputeScreenPos(TransformWorldToHClip(WorldPosition));
	inputData.normalizedScreenSpaceUV = screenPos.xy / screenPos.w;
	inputData.positionWS = WorldPosition;

	LIGHT_LOOP_BEGIN(pixelLightCount);
		Light light = GetAdditionalLight(lightIndex, WorldPosition, Shadowmask);
    #ifdef _LIGHT_LAYERS
        if (IsMatchingLightLayer(light.layerMask, ultravioletLayerMask))
    #endif
        {
		float intensity = length(light.color.rgb);
		float atten = intensity * light.distanceAttenuation * light.shadowAttenuation;
		totalAtten += atten;
	}
	LIGHT_LOOP_END
#endif
    Out = totalAtten;
}

#endif