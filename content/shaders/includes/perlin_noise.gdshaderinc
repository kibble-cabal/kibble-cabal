// implementation of MurmurHash (https://sites.google.com/site/murmurhash/) for a 
// single unsigned integer.
uint hash(uint x, uint current_seed) {
    const uint m = 0x5bd1e995U;
    uint hash = current_seed;
    // process input
    uint k = x;
    k *= m;
    k ^= k >> 24U;
    k *= m;
    hash *= m;
    hash ^= k;
    // some final mixing
    hash ^= hash >> 13U;
    hash *= m;
    hash ^= hash >> 15U;
    return hash;
}

// implementation of MurmurHash (https://sites.google.com/site/murmurhash/) for a  
// 3-dimensional unsigned integer input vector.

uint hash_vec3(uvec3 x, uint current_seed){
    const uint m = 0x5bd1e995U;
    uint hash = current_seed;
    // process first vector element
    uint k = x.x; 
    k *= m;
    k ^= k >> 24U;
    k *= m;
    hash *= m;
    hash ^= k;
    // process second vector element
    k = x.y; 
    k *= m;
    k ^= k >> 24U;
    k *= m;
    hash *= m;
    hash ^= k;
    // process third vector element
    k = x.z; 
    k *= m;
    k ^= k >> 24U;
    k *= m;
    hash *= m;
    hash ^= k;
	// some final mixing
    hash ^= hash >> 13U;
    hash *= m;
    hash ^= hash >> 15U;
    return hash;
}


vec3 gradient_direction(uint hash) {
    switch (int(hash) & 15) { // look at the last four bits to pick a gradient direction
    case 0:
        return vec3(1, 1, 0);
    case 1:
        return vec3(-1, 1, 0);
    case 2:
        return vec3(1, -1, 0);
    case 3:
        return vec3(-1, -1, 0);
    case 4:
        return vec3(1, 0, 1);
    case 5:
        return vec3(-1, 0, 1);
    case 6:
        return vec3(1, 0, -1);
    case 7:
        return vec3(-1, 0, -1);
    case 8:
        return vec3(0, 1, 1);
    case 9:
        return vec3(0, -1, 1);
    case 10:
        return vec3(0, 1, -1);
    case 11:
        return vec3(0, -1, -1);
    case 12:
        return vec3(1, 1, 0);
    case 13:
        return vec3(-1, 1, 0);
    case 14:
        return vec3(0, -1, 1);
    case 15:
        return vec3(0, -1, -1);
    }
}

float interpolate(float value1, float value2, float value3, float value4, float value5, float value6, float value7, float value8, vec3 t) {
    return mix(
        mix(mix(value1, value2, t.x), mix(value3, value4, t.x), t.y),
        mix(mix(value5, value6, t.x), mix(value7, value8, t.x), t.y),
        t.z
    );
}

vec3 fade(vec3 t) {
    // 6t^5 - 15t^4 + 10t^3
	return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float simple_perlin_noise(vec3 position, uint current_seed) {
    vec3 floor_position = floor(position);
    vec3 fract_position = position - floor_position;
    uvec3 cell_coords = uvec3(floor_position);
    float value1 = dot(gradient_direction(hash_vec3(cell_coords, current_seed)), fract_position);
    float value2 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(1, 0, 0)), current_seed)), fract_position - vec3(1, 0, 0));
    float value3 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(0, 1, 0)), current_seed)), fract_position - vec3(0, 1, 0));
    float value4 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(1, 1, 0)), current_seed)), fract_position - vec3(1, 1, 0));
    float value5 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(0, 0, 1)), current_seed)), fract_position - vec3(0, 0, 1));
    float value6 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(1, 0, 1)), current_seed)), fract_position - vec3(1, 0, 1));
    float value7 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(0, 1, 1)), current_seed)), fract_position - vec3(0, 1, 1));
    float value8 = dot(gradient_direction(hash_vec3((cell_coords + uvec3(1, 1, 1)), current_seed)), fract_position - vec3(1, 1, 1));
    return interpolate(value1, value2, value3, value4, value5, value6, value7, value8, fade(fract_position));
}

float perlin_noise(vec3 position, int _frequency, int _octave_count, float _persistence, float _lacunarity, int current_seed) {
    float value = 0.0;
    float amplitude = 1.0;
    float current_frequency = float(_frequency);
    uint new_seed = uint(current_seed);
    for (int i = 0; i < _octave_count; i++) {
        new_seed = hash(new_seed, 0x0U); // create a new seed for each octave
        value += simple_perlin_noise(position * current_frequency, new_seed) * amplitude;
        amplitude *= _persistence;
        current_frequency *= _lacunarity;
    }
    return value;
}


float fbm (in vec2 _st, int _octaves, int _seed) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    // Rotate to reduce axial bias
    mat2 rot = mat2(vec2(cos(0.5), sin(0.5)),
                    vec2(-sin(0.5), cos(0.50)));
    for (int i = 0; i < _octaves; ++i) {
        v += a * simple_perlin_noise(vec3(_st, 1.0), uint(_seed + i));
        _st = rot * _st * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}
