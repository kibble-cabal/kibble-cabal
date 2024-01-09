shader_type canvas_item;

uniform vec4 rect_color: source_color = vec4(1.0);

#define DEFAULT_RADIUS 100.0
#include "res://assets/shaders/includes/border_radius.gdshaderinc"

varying float rect_radius;
varying vec2 resolution;
varying vec2 rect_center;
varying vec2 rect_size;

float rounded_rect(vec2 pos) {
	pos /= resolution.y;
    vec2 size = rect_size / resolution.y;
    float radius = rect_radius / resolution.y;
    return 
        smoothstep(size.x / 2.0 + radius, size.x / 2.0 - radius, abs(pos.x)) * 
        smoothstep(size.y / 2.0 + radius, size.y / 2.0 - radius, abs(pos.y));    
}

void fragment() {    
	resolution = 1.0 / SCREEN_PIXEL_SIZE;
	rect_center = resolution / 2.0;
	rect_size = resolution;	
	rect_radius = get_radius(UV);
	
    vec2 pos = (UV * resolution) - rect_center;
    float delta = 0.5 / rect_radius;
    COLOR = rect_color * smoothstep(0.5 - delta, 0.5 + delta, rounded_rect(pos));
}
