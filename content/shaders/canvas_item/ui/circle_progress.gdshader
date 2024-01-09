shader_type canvas_item;

uniform float progress: hint_range(0.0, 1.0) = 0.0;
uniform float thickness: hint_range(0.0, 0.5) = 0.05;
uniform vec4 filled_color: source_color = vec4(1.0);
uniform vec4 empty_color: source_color = vec4(vec3(0.0), 0.25);
uniform bool rounded = true;

varying vec2 resolution;

#include "res://assets/shaders/includes/rotate_uv.gdshaderinc"
#define RADIUS (0.5 - thickness)
#define AA (1.0 / resolution.y)

vec2 rotation_uv(vec2 uv, float d) {
    float s = sin(d);
    float c = cos(d);
    mat2 rot_matrix = mat2(vec2(c, -s), vec2(s, c));
    return uv * rot_matrix;
}

float draw_circle(vec2 uv, vec2 p, float r) {
    uv += p;
    return smoothstep(r + AA, r - AA, length(uv));
}

void fragment() {
	resolution = 1.0 / SCREEN_PIXEL_SIZE;
	
	vec2 uv = flip_uv_y(UV) - 0.5;
    float circle = smoothstep(RADIUS+AA, RADIUS-AA, abs(length(uv) - RADIUS) + RADIUS - thickness);
   	float empty = circle;
	float radial = (atan(uv.x, -uv.y) / PI * 0.5) + 0.5;
    float time = clamp(progress, 0.0, 0.999999);
    radial += time;
    circle *= step(fract(radial), fract(time));
	
	// Only show start & end circles if rounded and progress has begun
	if (rounded && time > 0.00001) {
	    circle = max(circle, draw_circle(uv, vec2(0,-RADIUS), thickness));
	    circle = max(circle, draw_circle(rotation_uv(uv, time * TAU), vec2(0,-RADIUS), thickness));
	}
	
	// Add fade in to start
	if (time < 0.05) circle = circle * time * 25.0;

	COLOR = empty_color * empty;
	COLOR += filled_color * circle;
}