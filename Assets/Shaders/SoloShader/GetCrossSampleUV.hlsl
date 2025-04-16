void GetCrossSampleUV_float(
    float4 uv,
    float2 texelSize,
    float OffsetMultiplier,
    out float2 UVOriginal,
    out float2 UVTopRight,
    out float2 UVBottomLeft,
    out float2 UVTopLeft,
    out float2 UVBottomRight)
{
    UVOriginal = uv;
    UVTopRight = uv.xy + float2(texelSize.x, texelSize.y) * OffsetMultiplier;
    UVBottomLeft = uv.xy - float2(texelSize.x, texelSize.y) * OffsetMultiplier;
    UVTopLeft = uv.xy + float2(-texelSize.x * OffsetMultiplier, texelSize.y * OffsetMultiplier);
    UVBottomRight = uv.xy + float2(texelSize.x * OffsetMultiplier, -texelSize.y * OffsetMultiplier);
}