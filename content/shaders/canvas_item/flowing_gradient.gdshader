shader_type canvas_item;

// Copied (with adjustments) from https://www.shadertoy.com/view/wdyczG

uniform vec3 color_1: source_color = vec3(.910, .510, .8);
uniform vec3 color_2: source_color = vec3(0.350, .71, .953);
uniform vec3 color_3: source_color = vec3(.957, .804, .623);
uniform vec3 color_4: source_color = vec3(.192, .384, .933);

uniform float frequency = 5.0;
uniform float amplitude = 30.0;
uniform float speed = 1.0;

#define S(a,b,t) smoothstep(a,b,t)

mat2 Rot(float a) {
    float s = sin(a);
    float c = cos(a);
    return mat2(vec2(c, -s), vec2(s, c));
}


// Created by inigo quilez - iq/2014
// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.
vec2 hash(vec2 p) {
    p = vec2(dot(p, vec2(2127.1,81.17)), dot(p, vec2(1269.5,283.37)));
	return fract(sin(p) * 43758.5453);
}

float noise(in vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
	
	vec2 u = f * f * (3.0-2.0 * f);

    float n = mix(mix(dot(-1.0 + 2.0 * hash(i + vec2(0.0,0.0)), f - vec2(0.0,0.0)), 
                        dot(-1.0 + 2.0 * hash(i + vec2(1.0,0.0)), f - vec2(1.0,0.0)), u.x),
                   mix(dot(-1.0 + 2.0 * hash(i + vec2(0.0,1.0)), f - vec2(0.0,1.0)), 
                        dot(-1.0 + 2.0 * hash(i + vec2(1.0,1.0)), f - vec2(1.0,1.0)), u.x), u.y);
	return 0.5 + 0.5 * n;
}


void fragment() {
    vec2 uv = UV;
	vec2 resolution = 1.0 / SCREEN_PIXEL_SIZE;
    float ratio = resolution.x / resolution.y;

    vec2 tuv = uv;
    tuv -= .5;

    // rotate with Noise
    float degree = noise(vec2(TIME * speed * 0.1, tuv.x * tuv.y));

    tuv.y  *= 1./ratio;
    tuv  *= Rot(radians((degree-0.5) * 720. + 180.));
	tuv.y  *= ratio;

    // Wave warp with sin
    float time = TIME * speed;
    tuv.x += sin(tuv.y * frequency + time) / amplitude;
   	tuv.y += sin(tuv.x * frequency * 1.5 + time) / (amplitude * 0.5);
    
    
    // draw the image
    vec3 layer1 = mix(color_3, color_4, S(-0.3, 0.2, (tuv * Rot(radians(-5.))).x));
    vec3 layer2 = mix(color_1, color_2, S(-0.3, 0.2, (tuv * Rot(radians(-5.))).x));
    vec3 final_comp = mix(layer1, layer2, S(0.5, -0.3, tuv.y));
    vec3 col = final_comp;
    COLOR = vec4(col, 1.0);
}