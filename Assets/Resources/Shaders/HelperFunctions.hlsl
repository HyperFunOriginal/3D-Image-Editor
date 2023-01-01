struct KeyColorPair
{
    float value;
    float4 colorVal;
};
struct KeyValuePair
{
    float input;
    float output;
    float derivative;
};

uint3 D2tD3(uint2 coords, uint sliceLength, uint sqrtSliceLength)
{
    uint2 xy = coords % uint2(sliceLength, sliceLength);
    uint2 z = coords / uint2(sliceLength, sliceLength);
    return uint3(xy, z.y + z.x * sqrtSliceLength);
}

uint2 D3tD2(uint3 coords, uint sliceLength, uint sqrtSliceLength)
{
    uint2 z = uint2(coords.z / sqrtSliceLength, coords.z % sqrtSliceLength);
    return coords.xy + z * sliceLength;
}

float3 D3tUV(uint3 coords, uint sliceLength)
{
    return float3(coords) / float(sliceLength) * 2.0 - float3(1, 1, 1);
}

uint2 UVtD2(float3 coords, uint sliceLength, uint sqrtSliceLength)
{
    coords = (coords + 1.0) * 0.5;
    if (coords.x < 0 || coords.y < 0 || coords.x > 1.0 || coords.y > 1.0)
        return uint2(0x7fffffffu, 0x7fffffffu);
    return D3tD2(uint3(coords * float(sliceLength)), sliceLength, sqrtSliceLength);
}

float4 ReadFromTexture(float3 uvs, uint sliceLength, uint sqrtSliceLength, Texture2D<float4> image)
{
    uint2 coords = UVtD2(uvs, sliceLength, sqrtSliceLength);
    if (coords.x == 0x7fffffffu)
        return 0;
    return image[coords];
}
float4 ReadFromTexture(float3 uvs, uint sliceLength, uint sqrtSliceLength, RWTexture2D<float4> image)
{
    uint2 coords = UVtD2(uvs, sliceLength, sqrtSliceLength);
    if (coords.x == 0x7fffffffu)
        return 0;
    return image[coords];
}
void WriteToTexture(float3 uvs, float4 data, uint sliceLength, uint sqrtSliceLength, RWTexture2D<float4> image)
{
    uint2 coords = UVtD2(uvs, sliceLength, sqrtSliceLength);
    if (coords.x == 0x7fffffffu)
        return;
    image[coords] = data;
}

int4 RNG(int4 seed)
{
    int4 a = seed.xzwy << 7;
    seed = seed * 195457815 + 1013904223;
    seed += seed.yxzy * seed.xwxz;
    seed ^= a + (seed << 17);
    seed += seed.zwyx * seed.yxwz;
    return seed ^ a;
}

float4 RNGVec(int4 seed)
{
    return float4(RNG(seed)) / float(0x7fffffffu);
}

float3 RNGVec3D(int4 seed)
{
    return RNGVec(seed).xyz;
}

void RNGRotation(int4 seed, out float3 x, out float3 y, out float3 z)
{
    x = normalize(RNGVec3D(seed));
    seed += 14891;
    y = normalize(RNGVec3D(seed));
    z = normalize(cross(x, y));
    y = normalize(cross(x, z));
}

float2 RNGVec2D(int4 seed)
{
    return RNGVec(seed).xz;
}

float RNGFloat(int4 seed)
{
    return RNGVec(seed).z;
}

float3 PerlinValue3D(float3 samplePos, int4 seed)
{
    samplePos += float3(100, 100, 100);
    int3 roundPos = int3(samplePos);
    float3 lerpD = frac(samplePos);
    
    float3 o    = normalize(RNGVec3D(seed + int4(roundPos + int3(0,0,0), 0)));
    float3 X    = normalize(RNGVec3D(seed + int4(roundPos + int3(1,0,0), 0)));
    float3 Y    = normalize(RNGVec3D(seed + int4(roundPos + int3(0,1,0), 0)));
    float3 Z    = normalize(RNGVec3D(seed + int4(roundPos + int3(0,0,1), 0)));
    float3 XY   = normalize(RNGVec3D(seed + int4(roundPos + int3(1,1,0), 0)));
    float3 XZ   = normalize(RNGVec3D(seed + int4(roundPos + int3(1,0,1), 0)));
    float3 YZ   = normalize(RNGVec3D(seed + int4(roundPos + int3(0,1,1), 0)));
    float3 XYZ  = normalize(RNGVec3D(seed + int4(roundPos + int3(1,1,1), 0)));
    
    o = lerp(o, Z, lerpD.z);
    X = lerp(X, XZ, lerpD.z);
    Y = lerp(Y, YZ, lerpD.z);
    XY = lerp(XY, XYZ, lerpD.z);
    
    o = lerp(o, Y, lerpD.y);
    X = lerp(X, XY, lerpD.y);
    
    return lerp(o, X, lerpD.x);
}

float PerlinValue(float3 samplePos, int4 seed)
{
    return dot(PerlinValue3D(samplePos, seed), float3(0.8489069620259946, -0.4284845180604056, 0.3094478754270879));
}

float3 FractalPerlinValue3D(float3 samplePos, int4 seed, float lacunarity, float persistence, int octaves)
{
    float3 x, y, z;
    RNGRotation(seed, x, y, z);
    
    float3 value = 0.0;
    float amplitude = 1.0;
    float freq = 1.0;
    for (int i = 0; i < octaves; i++)
    {
        samplePos = samplePos.x * x + samplePos.y * y + samplePos.z * z;
        value += PerlinValue3D(samplePos * freq, seed) * amplitude;
        freq *= lacunarity;
        amplitude *= persistence;
        seed = RNG(seed);
    }
    return value;
}

float FractalPerlinCorrectionFactor(float persistence, int octaves)
{
    float value = 0.0;
    float amplitude = 0.8;
    for (int i = 0; i < octaves; i++)
    {
        value += amplitude;
        amplitude *= persistence;
    }
    return value;
}

float FractalPerlinValue(float3 samplePos, int4 seed, float lacunarity, float persistence, int octaves)
{
    float3 x, y, z;
    RNGRotation(seed, x, y, z);
    
    float value = 0.0;
    float amplitude = 1.0;
    float freq = 1.0;
    for (int i = 0; i < octaves; i++)
    {
        samplePos = samplePos.x * x + samplePos.y * y + samplePos.z * z;
        value += PerlinValue(samplePos * freq, seed) * amplitude;
        freq *= lacunarity;
        amplitude *= persistence;
        seed = RNG(seed);
    }
    return value;
}

float InverseLerp(float t, float a, float b)
{
    return (t - a) / (b - a);
}

float SDFSphere(float3 samplePos, float3 origin, float radius)
{
    return distance(samplePos, origin) - radius;
}

float SDFCube(float3 samplePos, float3 origin, float3 dimensions)
{
    samplePos -= origin;
    float3 absC = abs(samplePos) - dimensions;
    float maxMagCoord = max(max(absC.x, absC.y), absC.z);
    float coordBoxDist = length(samplePos - clamp(samplePos, -dimensions, dimensions));
    
    if (coordBoxDist > 0.001)
        return coordBoxDist;
    return maxMagCoord;
}

float Bezier(float t, float4 params)
{
    params *= float4((1 - t) * (1 - t) * (1 - t), 3.0 * (1 - t) * (1 - t) * t, 3.0 * (1 - t) * t * t, t * t * t);
    return params.x + params.y + params.z + params.w;
}

float Levels(StructuredBuffer<KeyValuePair> keys, uint count, float t)
{
    float smallVal = -10000;
    float lerpSmall = 0; // Find the largest value smaller than t
    float tangSmall = 0;
    float bigVal = 10000;
    float lerpBig = 0; // Find the smallest value bigger than t
    float tangBig = 0;
    
    for (uint i = 0; i < count; i++)
    {
        KeyValuePair p = keys[i];
        if (p.input >= t)
        {
            if (bigVal > p.input)
            {
                bigVal = p.input;
                lerpBig = p.output;
                tangBig = p.derivative;
            }
        }
        else
        {
            if (smallVal < p.input)
            {
                smallVal = p.input;
                lerpSmall = p.output;
                tangSmall = p.derivative;
            }
        }
    }
    
    t = InverseLerp(t, smallVal, bigVal);
    return Bezier(t, float4(lerpSmall, tangSmall + lerpSmall, lerpBig - tangBig, lerpBig));
}

float4 GradientMap(StructuredBuffer<KeyColorPair> keys, uint count, float t)
{
    float smallVal = -10000;
    float4 lerpSmall = float4(0, 0, 0, 0); // Find the largest value smaller than t
    float bigVal = 10000;
    float4 lerpBig = float4(0, 0, 0, 0); // Find the smallest value bigger than t
    
    for (uint i = 0; i < count; i++)
    {
        KeyColorPair p = keys[i];
        if (p.value >= t)
        {
            if (bigVal > p.value)
            {
                bigVal = p.value;
                lerpBig = p.colorVal;
            }
        }
        else
        {
            if (smallVal < p.value)
            {
                smallVal = p.value;
                lerpSmall = p.colorVal;
            }
        }
    }
    return lerp(lerpSmall, lerpBig, InverseLerp(t, smallVal, bigVal));
}

float3 WavelengthToRGB(float wavelength)
{
    float bCone = wavelength * wavelength * 4.4 * exp(-250.0 * (wavelength - 0.43) * (wavelength - 0.43));
    float gCone = wavelength * wavelength * 3.7 * exp(-160.0 * (wavelength - 0.51) * (wavelength - 0.51));
    float rCone = wavelength * wavelength * 2.9 * exp(-180.0 * (wavelength - 0.58) * (wavelength - 0.58));
    return float3(rCone, gCone, bCone);
}