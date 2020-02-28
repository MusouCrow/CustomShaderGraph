#ifndef CUSTOM_OUTLINE_PASS_INCLUDED
#define CUSTOM_OUTLINE_PASS_INCLUDED

PackedVaryings vert(Attributes input)
{
    Varyings output = (Varyings)0;

    float3 positionWS = TransformObjectToWorld(input.positionOS);
    float3 positionVS = TransformWorldToView(positionWS);
    float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, input.normalOS);
    normal.z = -4.0;
    positionVS += normalize(normal) * 0.01;
    output.positionCS = TransformWViewToHClip(positionVS);

    PackedVaryings packedOutput = PackVaryings(output);

    return packedOutput;
}

half4 frag(PackedVaryings packedInput) : SV_TARGET 
{    
    Varyings unpacked = UnpackVaryings(packedInput);
    UNITY_SETUP_INSTANCE_ID(unpacked);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(unpacked);

    SurfaceDescriptionInputs surfaceDescriptionInputs = BuildSurfaceDescriptionInputs(unpacked);
    SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

#ifdef _ALPHATEST_ON
    clip(surfaceDescription.Color.a - _AlphaClip);
#endif

#ifdef _SURFACE_TYPE_TRANSPARENT
    return half4(0.0, 0.0, 0.0, 0.0);
#endif
    
    return surfaceDescription.Color * 0.3;
}

#endif