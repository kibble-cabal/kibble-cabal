shader_type canvas_item;

uniform vec4 rect_color: source_color = vec4(1.0);
uniform vec2 size = vec2(0.9);
uniform float dampen_distortion: hint_range(0.0, 1.0) = 0.0;

#define USE_REDUCE_MOTION
#include "res://assets/shaders/includes/border_radius.gdshaderinc"
#include "res://assets/shaders/includes/fbm.gdshaderinc"

varying float rect_radius;
varying vec2 resolution;
varying vec2 rect_center;
varying vec2 rect_size;

#define AA (0.1 * TEXTURE_PIXEL_SIZE.x)

float rounded_rect(vec2 pos) {
	pos /= resolution.y;
    vec2 _size = rect_size / resolution.y;
    float radius = rect_radius / resolution.y;
    return 
        smoothstep(_size.x / 2.0 + radius, _size.x / 2.0 - radius, abs(pos.x)) * 
        smoothstep(_size.y / 2.0 + radius, _size.y / 2.0 - radius, abs(pos.y));    
}

void fragment () {
	resolution = 1.0 / SCREEN_PIXEL_SIZE;
	
	rect_center = resolution / 2.0;
	rect_size = resolution * 0.5;
	rect_radius = min(min(get_radius(UV), rect_size.x / 2.0), rect_size.y / 2.0);
	
	vec2 distorted_uv = distort((UV / 2.0) + 0.25, TIME);
	distorted_uv = mix(distorted_uv, (UV / 2.0) + 0.25, dampen_distortion);
	
    vec2 position = (distorted_uv * resolution) - rect_center;
    float delta = 0.5 / rect_radius;
    vec4 rect = rect_color;
	rect.a = smoothstep(
		0.5 - AA, 
		0.5 + AA, 
		rounded_rect(position * 1.0 / size)
	);
	
	COLOR = rect;
}
