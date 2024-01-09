#ifndef FBM

#define FBM 1

group_uniforms Noise;
uniform int seed = 1;
uniform float speed = 1.0;
uniform float octaves = 8.0;
uniform float lacunarity: hint_range(1.0, 2.0) = 1.4;
uniform float power: hint_range(-1.5, 1.5) = 0.0;
group_uniforms;

#ifdef USE_REDUCE_MOTION
global uniform bool reduce_motion;
#endif

float hash(vec2 p) {
    float h = dot(p, vec2(17.1, 311.7));
    return -1.0 + 2.0 * fract(sin(h) * 4358.5453);
}

float noise(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    vec2 u = f * f * (3.0 - 2.0 * f);
    
    return mix(mix(hash(i + vec2(0.0, 0.0)),
                   hash(i + vec2(1.0, 0.0)), u.x),
               mix(hash(i + vec2(0.0, 1.0)),
                   hash(i + vec2(1.0, 1.0)), u.x), u.y);
 
}

vec2 vec_noise(vec2 point) {
    vec2 res;
    res.x = noise(point);
    res.y = noise(point + 0.33);
    return res;
}


float distortion_noise(vec2 point, float distortion) {
    vec2 offset = point + vec2(0.5);
    offset = vec_noise(point);
    offset *= distortion;
    point += offset;
    return noise(point);
}


float dist_fbm_noise(vec2 p) {

  float value = 0.0, k = 0.0;
  for (float i = 0.0; i < octaves; ++ i) {
    value += distortion_noise(p, 1.0) * pow(lacunarity, power * i);
    p *= lacunarity;
    ++k;
  }

  float remainder = fract(octaves);
  if (remainder != 0.0) {
    value += remainder * noise(p) * pow(lacunarity, power * k);
  }
  return value;
}


float fbm_noise(vec2 p) {
  float value = 0.0, k = 0.0;
  for (float i = 0.0; i < octaves; ++ i) {
    value += noise(p) * pow(lacunarity, power * i);
    p *= lacunarity;
    ++k;
  }

  float remainder = fract(octaves);
  if (remainder != 0.0) {
    value += remainder * noise(p) * pow(lacunarity, power * k);
  }
  return value;
}

float offset(vec2 pos) {
    const float km = 1.0;
    float offset = km * fbm_noise(pos);
    return offset;
}

float dist_offset(vec2 pos) {
    const float km = 0.4;
    float offset = km * dist_fbm_noise(pos);
    return offset;
}

vec2 distort(vec2 uv, float time) {
	#ifdef USE_REDUCE_MOTION
	time /= 40.0 / speed;
	if (reduce_motion) time = float(seed);
	#else
	time /= 40.0 / speed;
	#endif
    vec3 normal = vec3(0.0, 1.0, 0.0);
    vec3 pos = vec3(uv.x, 0.0, uv.y);
    
    float offset_value = offset(uv);
    
    float st_offset = dist_offset(uv + (float(seed * 2) + time));
    pos += 0.4 * offset_value * normal;
	
	return (uv + st_offset * 0.008);
}

#endif