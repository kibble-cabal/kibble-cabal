// Random Number Generator sourced from Ray Tracing Gems II
// chapter 14 - The Reference Path Tracer
// section 14.3.4 - Random Number Generation

#define RngStateType uint

uint xorshift(inout uint rng_state) {
	rng_state ^= rng_state << 13U;
	rng_state ^= rng_state >> 17U;
	rng_state ^= rng_state << 5U;
	return rng_state;
}

// Jenkins's "one at a time" hash function
const uint jenkins_hash(uint x) {
	x += x << 10U;
	x ^= x >> 6U;
	x += x << 3U;
	x ^= x >> 11U;
	x += x << 15U;
	return x;
}

// Converts unsigned integer into float int range <0; 1) by using 23 most significant bits for mantissa
float uint_to_float(uint x) {
	return uintBitsToFloat(0x3f800000u | (x >> 9U)) - 1.0f;
}

const RngStateType init_rng(vec2 pixel_coords, vec2 resolution, uint frame_number) {
	RngStateType rng_seed = uint(dot(pixel_coords, vec2(1, resolution.x))) ^ jenkins_hash(frame_number);
	return jenkins_hash(rng_seed);
}

// Return random float in (0, 1) range (Xorshift-based version)
float rand(inout RngStateType rng_state) {
	return uint_to_float(xorshift(rng_state));
}

float random (vec2 st) {
    return fract(sin(dot(st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}